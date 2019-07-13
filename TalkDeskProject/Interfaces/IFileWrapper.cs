using System;
using System.Collections.Generic;
using System.Text;

namespace TalkDeskProject.Interfaces
{
    public interface IFileWrapper
    {
        List<string> ReadFileParallel(string path, Encoding enconding, int threadQuantity);
    }
}
