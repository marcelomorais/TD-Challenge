using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TalkDeskProject.Interfaces;

namespace TalkDeskProject
{
    public class FileWrapper : IFileWrapper
    {
        public List<string> ReadFileParallel(string input, Encoding encoding, int threadQuantity)
        {
            List<string> allLines = new List<string>();

            File.ReadLines(input, encoding)
                    .AsParallel()
                    .WithDegreeOfParallelism(threadQuantity)
                    .ForAll(x => allLines.Add(x));

            return allLines;
        }
    }
}
