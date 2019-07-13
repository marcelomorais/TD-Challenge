using System;
using TalkDeskProject.Interfaces;

namespace TalkDeskProject
{
    public class ConsoleWrapper : IConsoleWrapper
    {
        public void WriteLine(string output)
        {
            Console.WriteLine(output);
        }

        public void Write(string output)
        {
            Console.Write(output);
        }

        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public ConsoleKeyInfo ReadKey()
        {
            return Console.ReadKey();
        }
    }
}
