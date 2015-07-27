using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TestTool
{
    class Program
    {
        private static void Main(string[] args)
        {
            //var data = new byte[8092];
            //fs.Read(data, 0, 8092);
            var c = new CompoundFileStorage.CompoundFile();
            var r = c.RootStorage;
            var s = r.AddStream("teststream");
            s.SetData(new byte[0]);
            c.Save("d:\\test.msg");
        }
    }
}
