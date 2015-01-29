using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TestTool
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var fs = File.OpenRead("d:\\test.msg"))
            {
                var data = new byte[2048];
                fs.Read(data, 0, 2048);
                var c = new CompoundFileStorage.CompoundFile(new MemoryStream(data));
            }
        }
    }
}
