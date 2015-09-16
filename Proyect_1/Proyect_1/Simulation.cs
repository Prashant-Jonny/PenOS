using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Proyect_1
{
    public class Simulation
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

        private List<Process> newList = new List<Process>();
        private List<Process> readyList = new List<Process>();
        private List<Process> waitingList = new List<Process>();
        private List<Process> terminatedList = new List<Process>();

        public static MainWindow mWindow;
        public static PCB pcb;

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
            mWindow.clock.Text = 0.ToString();
            clock = 0;
            Random rand = new Random();

            while (!mWindow.stopped || !mWindow.paused)
            {
                mWindow.clock.Text = clock.ToString();

                if (rand.Next(0, 100) <= probability)
                {
                    Add(new Process(id));
                    id++;
                    MessageBox.Show("Got here");
                    pcbUpdate();
                }
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
                listUpdate();
                clock++;
            }
        }

        private void pcbUpdate()
        {
            pcb.dataGrid.ItemsSource = newList;
        }

        private void listUpdate()
        {
            mWindow.newList.ItemsSource = newList.Select(Process => Process.id).ToList();
            mWindow.readyList.ItemsSource = readyList.Select(Process => Process.id).ToList();
            mWindow.waitingList.ItemsSource = waitingList.Select(Process => Process.id).ToList();
            mWindow.terminatedList.ItemsSource = terminatedList.Select(Process => Process.id).ToList();
        }
    }
}