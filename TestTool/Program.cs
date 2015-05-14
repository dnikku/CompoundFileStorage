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
            using (var fs = File.OpenRead("d:\\Test E-mail met unicode chars.msg"))
            {
                //var data = new byte[8092];
                //fs.Read(data, 0, 8092);
                var c = new CompoundFileStorage.CompoundFile(fs);
                var r = c.RootStorage;
            }
        }
    }
}
