using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyect_1
{
    internal class Process
    {
        public string id { get; set; }
        public int cpuUse { get; }
        public int cpuTime { get; }
        public int IO_initTime { get; set; }
        public int IO_totalTime { get; set; }
        public int endTime { get; set; }
        public int sysEndTime { get; set; }

        public Process(int id)
        {
            Random rand = new Random();

            this.id = "P" + id;

            cpuUse = rand.Next(2, 10);
            cpuTime = rand.Next(1, cpuUse - 1);
        }
    }
}