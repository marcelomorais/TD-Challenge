using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkDeskProject.Constants;
using TalkDeskProject.Interfaces;

namespace TalkDeskProject.Services
{
    public abstract class AbstractEngine : IEngine
    {
        private IConsoleWrapper _console;
        private IFileWrapper _fileWrapper;

        public AbstractEngine(IConsoleWrapper console,
            IFileWrapper fileWrapper)
        {
            _console = console;
            _fileWrapper = fileWrapper;
        }

        public virtual async Task Initialise()
        {
            try
            {
                _console.WriteLine(Message.InsertFile);
                string input = _console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    throw new FileNotFoundException();
                }

                List<string> alllines = new List<string>();

                alllines = _fileWrapper.ReadFileParallel(input, Encoding.UTF8, 10);

                if (!alllines.Any())
                {
                    _console.WriteLine(Message.EmptyFile);
                    await Initialise();
                }

                _console.WriteLine(Message.Calculating);

                await ProcessLines(alllines);
            }
            catch (FileNotFoundException)
            {
                _console.WriteLine(Message.FileDoNotExist);
            }
            catch (DirectoryNotFoundException)
            {
                _console.WriteLine(Message.DirectoryDoNotExist);
            }
            catch (IOException)
            {
                _console.WriteLine(Message.Error);
            }

            _console.WriteLine(Message.PressEscKey);

            if (_console.ReadKey().Key == ConsoleKey.Escape)
            {
                return;
            }

            await Initialise();
        }

        public abstract Task ProcessLines(List<string> lines);
    }
}
