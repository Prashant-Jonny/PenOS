using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyect_1
{
    internal class Simulation
    {
        private int probability;
        private int quantum;
        private int newLimit;
        private int readyLimit;
        private int waitingLimit;

        private string algorithm;
        private string delay;

        private List<Process> newList;
        private List<Process> readyList;
        private List<Process> waitingList;
        private List<Process> terminatedList;

        public Simulation(int probability, int quantum, int newLimit, int readyLimit, int waitingLimit, string algSelected, string delaySelected)
        {
            this.probability = probability;
            this.quantum = quantum;
            this.newLimit = newLimit;
            this.readyLimit = readyLimit;
            this.waitingLimit = waitingLimit;

            algorithm = algSelected;
            delay = delaySelected;
        }

        public void Add(Process process)
        {
            newList.Add(process);
        }

        public void Start()
        {
            Random rand = new Random();

            if (rand.Next(0, 100) <= probability)
            {
            }
            /*if (algorithm.Equals("Round Robin"))
            {
            }
            else if (algorithm.Equals("Priority"))
            {
            }*/
        }
    }
}