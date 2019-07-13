using System;

namespace TalkDeskProject.Interfaces
{
    public interface IConsoleWrapper
    {
        void WriteLine(string output);
        void Write(string output);
        string ReadLine();
        ConsoleKeyInfo ReadKey();
    }
}
