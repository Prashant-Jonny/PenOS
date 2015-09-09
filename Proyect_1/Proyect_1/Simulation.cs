using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Proyect_1
{
    internal class Simulation
    {
        private int probability;
        private int quantum;
        private int newLimit;
        private int readyLimit;
        private int waitingLimit;

        private int clock = 0;

        private int id = 0;

        private string algorithm;
        private string delay;

        private List<Process> newList;
        private List<Process> readyList;
        private List<Process> waitingList;
        private List<Process> terminatedList;

        public static MainWindow mainWindow;

        public Simulation()
        {
        }

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

        public async void Start()
        {
            mainWindow.clock.Text = 0.ToString();
            clock = 0;
            Random rand = new Random();

            while (!MainWindow.stopped || !MainWindow.paused)
            {
                mainWindow.clock.Text = clock.ToString();

                if (rand.Next(0, 100) <= probability)
                {
                    Add(new Process(id));
                    id++;
                }
                /*if (algorithm.Equals("Round Robin"))
                {
                }
                else if (algorithm.Equals("Priority"))
                {
                }*/
                switch (delay)
                {
                    case "Slow":
                        await Task.Delay(2000);
                        break;

                    case "Medium":
                        await Task.Delay(1000);
                        break;

                    case "Fast":
                        await Task.Delay(500);
                        break;

                    default:
                        MessageBox.Show("Error in delay input: " + delay);
                        break;
                }
                clock++;
            }
        }
    }
}