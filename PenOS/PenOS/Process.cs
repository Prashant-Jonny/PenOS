using System;

namespace PenOS {

    public class Process {
        public string id { get; set; }

        public int realID { get; set; }

        public int size { get; }
        public int frames { get; set; }
        public int curFrames { get; set; }

        public int cpuUse { get; }
        public int cpuTime { get; }
        public int curTime { get; set; }

        public bool usesIO { get; }
        public int IO_initTime { get; set; }
        public int IO_totalTime { get; set; }
        public int IO_curTime { get; set; }

        public int disk_initTime { get; set; }
        public int disk_totalTime { get; set; }
        public int disk_curTime { get; set; }

        public int arrivalTime { get; set; }
        public int endTime { get; set; }
        public int waitTime { get; set; }
        public int sysEndTime { get; set; }

        public string status { get; set; }
        public Frame[] framesLocation { get; set; }

        public Process(int id, int ioTime, int clock, int frameSize, int diskTime) {
            Random rand = new Random();

            curFrames = 0;
            this.id = "P" + id;
            realID = id;
            arrivalTime = clock;

            size = (rand.Next(0, 60) * 4) + 16;

            frames = (size + frameSize - 1) / frameSize;
            framesLocation = new Frame[frames];

            for (int i = 0; i < framesLocation.Length; i++) {
                framesLocation[i] = new Frame();
                framesLocation[i].parent = this;
                framesLocation[i].frameId = i + 1;
            }

            cpuUse = rand.Next(2, 15);
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
                disk_initTime = rand.Next(1, (cpuTime - 1));
            }
            else {
                IO_initTime = 1;
                disk_initTime = 1;
            }

            IO_totalTime = ioTime;
            IO_curTime = 0;

            disk_totalTime = diskTime;
            disk_curTime = 0;

            status = "Hold";
        }
    }

    public class Frame {
        public Process parent { get; set; }
        public int frameId { get; set; }
        public int timesUsed { get; set; }
        public int timeAssigned { get; set; }
        public string location { get; set; }

        public Frame() {
            location = "Not Loaded";
            timesUsed = 0;
            timeAssigned = 0;
        }

        public Frame(int clock) {
            location = "Not Loaded";
            timesUsed = 0;
            timeAssigned = clock;
        }

        public override string ToString() {
            return location + "/" + timesUsed;
        }
    }
}