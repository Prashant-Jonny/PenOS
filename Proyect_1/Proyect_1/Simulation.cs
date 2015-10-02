using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Proyect_1 {

    public class Simulation {
        private int probability;
        private int quantum;
        private int newLimit;
        private int readyLimit;
        private int waitingLimit;

        //Main Window clock counter
        private int clock = 0;

        //Process ID
        private int id = 0;

        //Quantum timer
        private int quantumTimer = 0;

        private string algorithm;
        private string delay;

        private Process running = null;
        private Process waiting = null;

        private ObservableCollection<Process> newList = new ObservableCollection<Process>();
        private ObservableCollection<Process> readyList = new ObservableCollection<Process>();
        private ObservableCollection<Process> waitingList = new ObservableCollection<Process>();
        private ObservableCollection<Process> terminatedList = new ObservableCollection<Process>();

        public static MainWindow mWindow;
        public static PCB pcb;

        public Simulation() {
        }

        public Simulation(int probability, int quantum, int newLimit, int readyLimit, int waitingLimit, string algSelected, string delaySelected) {
            this.probability = probability;
            this.quantum = quantum;
            this.newLimit = newLimit;
            this.readyLimit = readyLimit;
            this.waitingLimit = waitingLimit;

            algorithm = algSelected;
            delay = delaySelected;
        }

        public void Add(Process process) {
            newList.Add(process);
        }

        public async void Start() {
            mWindow.clock.Text = "0";
            clock = 0;
            Random rand = new Random();

            while (true) {
                if (mWindow.stopped || mWindow.paused) {
                    await Task.Delay(1000);
                    if (mWindow.stopped) {
                        Stop();
                        break;
                    }
                    else if (mWindow.paused) {
                        continue;
                    }
                }

                mWindow.clock.Text = clock.ToString();

                if (running != null) {
                    mWindow.running.Text = running.id;
                }
                else {
                    mWindow.running.Text = "";
                }
                if (waiting != null) {
                    mWindow.io.Text = waiting.id;
                }
                else {
                    mWindow.io.Text = "";
                }

                if (algorithm == "RR") {
                    roundRobin();
                }
                else if (algorithm == "FCFS") {
                    fcfs();
                }
                else {
                    MessageBox.Show("Algorithm error");
                }

                if (rand.Next(1, 100) <= probability && newList.Count() < newLimit) {
                    Add(new Process(id));
                    id++;
                }

                switch (delay) {
                    case "Slow (2s)":
                        await Task.Delay(2000);
                        break;

                    case "Medium (1s)":
                        await Task.Delay(1000);
                        break;

                    case "Fast (0.5s)":
                        await Task.Delay(500);
                        break;

                    default:
                        MessageBox.Show("Error in delay input: " + delay);
                        break;
                }
                pcb.dataGrid.ItemsSource = terminatedList;
                listUpdate();
                clock++;
            }
        }

        private void Stop() {
            mWindow.resetValues();
        }

        private void fcfs() {
            //If nothing is running, then send something from the readylist
            if (running == null) {
                if (readyList.Count > 0) {
                    running = readyList.ElementAt(0);
                    readyList.RemoveAt(0);
                }
            }
            //If somethign is running, add a 1 to current time running, if its over then send it home
            else {
                if (running.cpuTime < running.curTime) {
                    running.curTime++;
                }
                else {
                    running.IO_initTime = 0;
                    running.IO_totalTime = 0;
                    running.endTime = clock;
                    running.sysEndTime = DateTime.Now.ToString("h:mm:ss tt");
                    terminatedList.Add(running);
                    running = null;
                }
            }
            if (newList.Count > 0 && readyList.Count < readyLimit) {
                readyList.Add(newList.ElementAt(0));
                newList.RemoveAt(0);
            }
        }

        private void roundRobin() {
            //If nothing is running and there is something ready, send it over.
            if (running == null) {
                if (readyList.Count > 0) {
                    running = readyList.ElementAt(0);
                    readyList.RemoveAt(0);
                }
            }
            //If somethign is running for the first time and it uses the IO, send it to the using IO
            else if (running.curTime == running.IO_initTime && running.usesIO) {
                running.curTime++;
                waitingList.Add(running);
                running = null;
            }
            //If the timer is still not the quantum, keep adding some cycles until its finished.
            else {
                if (quantumTimer < quantum) {
                    if (running.curTime < running.cpuTime) {
                        running.curTime++;
                    }
                    else {
                        running.endTime = clock;
                        running.sysEndTime = DateTime.Now.ToString("h:mm:ss tt");
                        terminatedList.Add(running);
                        running = null;
                    }
                }
                else {
                    readyList.Add(running);
                    running = null;
                }
            }
            //FCFS from new to ready
            if (newList.Count > 0 && readyList.Count < readyLimit) {
                readyList.Add(newList.ElementAt(0));
                newList.RemoveAt(0);
            }
            //If something is not using the IO, and there is a waiting list, send it.
            if (waiting == null) {
                if (waitingList.Count > 0) {
                    waiting = waitingList.ElementAt(0);
                    waitingList.RemoveAt(0);
                }
            }
            //Keep waiting until IO_curTime = IO_totalTime
            else {
                if (waiting.IO_curTime == waiting.IO_totalTime) {
                    readyList.Add(waiting);
                    waiting = null;
                }
                else {
                    waiting.IO_curTime++;
                }
            }
        }

        private void listUpdate() {
            mWindow.newList.ItemsSource = newList.Select(Process => Process.id);
            mWindow.readyList.ItemsSource = readyList.Select(Process => Process.id);
            mWindow.waitingList.ItemsSource = waitingList.Select(Process => Process.id);
            mWindow.terminatedList.ItemsSource = terminatedList.Select(Process => Process.id);
        }

        public void pcbUpdate() {
        }
    }
}