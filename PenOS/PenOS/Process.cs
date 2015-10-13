using System;

namespace PenOS {

    public class Process {
        public string id { get; set; }

        public int sysEndTime { get; set; }
        public int waitTime { get; set; }
        public int realID { get; set; }
        public int cpuUse { get; }
        public int cpuTime { get; }
        public int curTime { get; set; }
        public int IO_initTime { get; set; }
        public int IO_totalTime { get; set; }
        public int IO_curTime { get; set; }
        public int arrivalTime { get; set; }
        public int endTime { get; set; }

        public bool usesIO { get; }

        public Process(int id, int ioTime, int clock) {
            Random rand = new Random();

            this.id = "P" + id;
            realID = id;
            arrivalTime = clock;

            cpuUse = rand.Next(2, 10);
            cpuTime = rand.Next(1, cpuUse - 1);
            curTime = 0;

            if (rand.NextDouble() > 0.5) {
                usesIO = true;
            }
            else {
                usesIO = false;
            }

            if (cpuTime != 1) {
                IO_initTime = rand.Next(1, (cpuTime - 1));
            }
            else {
                IO_initTime = 1;
            }

            IO_totalTime = ioTime;
            IO_curTime = 0;
        }
    }
}