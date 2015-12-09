using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PenOS {

    public class Simulation {
        private int probability;
        private int quantum;
        private int newLimit;
        private int readyLimit;
        private int waitingLimit;

        private int ram;
        private int frames;

        //Main Window clock counter
        private int clock = 0;

        //Process vars
        private int id = 0;

        private int ioTime = 0;

        //Quantum timer
        private int quantumTimer = 0;

        private string algorithm;
        private string delay;
        private string memoryAlg;

        private Process running = null;
        private Process waiting = null;
        private Process usingDisk = null;

        private ObservableCollection<Process> newList = new ObservableCollection<Process>();
        private ObservableCollection<Process> readyList = new ObservableCollection<Process>();
        private ObservableCollection<Process> waitingList = new ObservableCollection<Process>();
        private ObservableCollection<Process> waitingDiskList = new ObservableCollection<Process>();
        private ObservableCollection<Process> terminatedList = new ObservableCollection<Process>();
        private ObservableCollection<Process> fullList = new ObservableCollection<Process>();
        private ObservableCollection<Process> tapList = new ObservableCollection<Process>();
        private ObservableCollection<Page> pageList = new ObservableCollection<Page>();

        private ObservableCollection<Frame> swapped = new ObservableCollection<Frame>();

        private Page[] Pages;

        public static MainWindow mWindow;
        public static Settings settings;

        //Module II
        private int diskTime = 0;

        private int sysMemory = 0;

        public Simulation() {
        }

        public Simulation(int probability, int quantum, int newLimit, int readyLimit, int waitingLimit, string algSelected, string delaySelected, int ioTime, int ram, int frames, int diskTime, string memoryAlg) {
            this.probability = probability;
            this.quantum = quantum;
            this.newLimit = newLimit;
            this.readyLimit = readyLimit;
            this.waitingLimit = waitingLimit;

            this.ioTime = ioTime;
            this.diskTime = diskTime;

            algorithm = algSelected;
            delay = delaySelected;
            this.memoryAlg = memoryAlg;

            this.ram = ram;
            this.frames = frames;

            sysMemory = (ram + 3) / 4;

            Pages = new Page[(ram - sysMemory) / frames];

            for (int i = 0; i < Pages.Length; i++) {
                Pages[i] = new Page();
                Pages[i].pageID = i;
            }
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
                        changeValues();
                        continue;
                    }
                }

                checkPages();

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
                if (usingDisk != null) {
                    mWindow.usingDisk.Text = usingDisk.id;
                }
                else {
                    mWindow.usingDisk.Text = "";
                }

                if (algorithm == "RR") {
                    roundRobin();
                }
                else if (algorithm == "FCFS") {
                    fcfs();
                }
                else {
                    MessageBox.Show("Algorithm error" + algorithm);
                    break;
                }

                if (rand.Next(1, 100) <= probability && newList.Count() < newLimit) {
                    Add(new Process(id, ioTime, clock, frames, diskTime));
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
                drawCanvas();
                listUpdate();
                mWindow.dataGrid.ItemsSource = fullList;
                mWindow.tap.ItemsSource = tapList;
                mWindow.swapping.ItemsSource = swapped;
                mWindow.pagesList.ItemsSource = pageList;
                clock++;
            }
        }

        //Change to use algorithm
        private void checkPages() {
            if (memoryAlg.Equals("Least Used")) {
                List<int> leastUsed = new List<int>(0);

                foreach (Page item in Pages) {
                    if (item.state == 0) {
                        leastUsed.Add(item.timesUsed);
                    }
                }

                if (leastUsed.Count > 0) {
                    leastUsed.Sort();
                    int min = leastUsed[0];

                    foreach (Page item in Pages) {
                        if (item.timesUsed == min && item.state == 0) {
                            item.state = 1;
                            break;
                        }
                    }

                    if (leastUsed.Count > 1) {
                        min = leastUsed[1];

                        foreach (Page item in Pages) {
                            if (item.timesUsed == min) {
                                mWindow.nextPage.Text = "Page " + item.pageID;
                                break;
                            }
                        }
                    }
                    else {
                        foreach (Page item in Pages) {
                            if (item.state == 0) {
                                mWindow.nextPage.Text = "Page " + item.pageID;
                                break;
                            }
                        }
                    }
                }
            }
            else if (memoryAlg.Equals("Oldest")) {
                List<int> oldest = new List<int>();

                foreach (Page item in Pages) {
                    if (item.state == 0) {
                        oldest.Add(item.timeAssigned);
                    }
                }

                if (oldest.Count > 0) {
                    oldest.Sort();
                    int old = oldest[0];

                    foreach (Page item in Pages) {
                        if (item.timeAssigned == old && item.state == 0) {
                            item.state = 1;
                            break;
                        }
                    }
                    if (oldest.Count > 1) {
                        old = oldest[1];

                        foreach (Page item in Pages) {
                            if (item.timeAssigned == old) {
                                mWindow.nextPage.Text = "Page " + item.pageID;
                                break;
                            }
                        }
                    }
                    else {
                        foreach (Page item in Pages) {
                            if (item.state == 0) {
                                mWindow.nextPage.Text = "Page " + item.pageID;
                                break;
                            }
                        }
                    }
                }
            }
            else {
                MessageBox.Show("Memory Algorithm error" + memoryAlg);
            }

            for (int i = 0; i < Pages.Length; i++) {
                if (Pages[i].state == 1) {
                    Pages[i].state = 2;
                    break;
                }
            }
        }

        private void changeValues() {
            if (settings.errorCheck().Equals("Passed")) {
                probability = settings.probability;
                quantum = settings.quantum;
                newLimit = settings.newLimit;
                readyLimit = settings.readyLimit;
                waitingLimit = settings.waitingLimit;
                ioTime = settings.ioTime;
                algorithm = settings.algorithm;
                delay = settings.delay;
                ram = settings.ram;
                frames = settings.frames;

                sysMemory = (ram + 3) / 4;
            }
            else {
                MessageBox.Show(settings.errorCheck());
            }
        }

        private void Stop() {
            mWindow.resetValues();
        }

        private void fcfs() {
            //Waiting Disk
            if (usingDisk == null) {
                if (waitingDiskList.Count > 0) {
                    usingDisk = waitingDiskList.ElementAt(0);
                    usingDisk.status = "Using Disk";
                    waitingDiskList.RemoveAt(0);
                }
            }
            else {
                for (int i = 0; i < Pages.Length; i++) {
                    if (Pages[i].frame == null) {
                        try {
                            Pages[i].frame = usingDisk.framesLocation[usingDisk.curFrames];
                            Pages[i].frame.frameId = usingDisk.curFrames;
                            Pages[i].state = 0;
                            Pages[i].frame.location = "RAM";
                            Pages[i].timesUsed++;
                            Pages[i].timeAssigned = clock;
                            usingDisk.curFrames++;
                        }
                        catch { }
                    }
                    else if (Pages[i].state == 2) {
                        Pages[i].frame.location = "Memory";
                        Pages[i].frame.parent.framesDone++;
                        swapped.Add(Pages[i].frame);

                        Pages[i].frame = null;
                        Pages[i].state = 0;

                        Pages[i].frame = usingDisk.framesLocation[usingDisk.curFrames];
                        Pages[i].frame.frameId = usingDisk.curFrames;

                        Pages[i].frame.location = "RAM";
                        Pages[i].timesUsed++;
                        Pages[i].timeAssigned = clock;
                        usingDisk.curFrames++;
                    }
                }
                usingDisk.status = "Ready";
                readyList.Add(usingDisk);
                usingDisk = null;
            }

            //If nothing is running, then send something from the readylist
            if (running == null) {
                if (readyList.Count > 0) {
                    running = readyList.ElementAt(0);
                    running.status = "Running";
                    readyList.RemoveAt(0);
                }
            }
            //If something is running, add a 1 to current time running, if its over then send it home
            else {
                if (running.cpuUse > running.curTime) {
                    running.curTime++;
                }
                else if (running.framesDone >= running.frames) {
                    running.endTime = clock;
                    running.IO_initTime = 0;
                    running.IO_totalTime = 0;
                    running.sysEndTime = running.endTime - running.arrivalTime + 1;
                    running.waitTime = running.sysEndTime - running.cpuUse - running.IO_totalTime + 1;
                    running.status = "Terminated";
                    terminatedList.Add(running);
                    running = null;
                }
                else {
                    running.status = "Waiting Disk";
                    waitingDiskList.Add(running);
                    running = null;
                }
            }
            if (newList.Count > 0 && readyList.Count < readyLimit) {
                newList.ElementAt(0).status = "Ready";
                readyList.Add(newList.ElementAt(0));
                newList.RemoveAt(0);
            }
        }

        private void roundRobin() {
            //If something is not using the IO, and there is a waiting list, send it.
            if (usingDisk == null) {
                if (waitingDiskList.Count > 0) {
                    usingDisk = waitingDiskList.ElementAt(0);
                    usingDisk.status = "Using Disk";
                    waitingDiskList.RemoveAt(0);
                }
            }
            else {
                for (int i = 0; i < Pages.Length; i++) {
                    if (Pages[i].frame == null) {
                        try {
                            Pages[i].frame = usingDisk.framesLocation[usingDisk.curFrames];
                            Pages[i].frame.frameId = usingDisk.curFrames;
                            Pages[i].state = 0;
                            Pages[i].frame.location = "RAM";
                            Pages[i].timesUsed++;
                            Pages[i].timeAssigned = clock;
                            usingDisk.curFrames++;
                        }
                        catch { }
                    }
                    else if (Pages[i].state == 2) {
                        Pages[i].frame.location = "Memory";
                        Pages[i].frame.parent.framesDone++;
                        swapped.Add(Pages[i].frame);

                        Pages[i].state = 0;
                        Pages[i].frame = null;

                        try {
                            Pages[i].frame = usingDisk.framesLocation[usingDisk.curFrames];
                            Pages[i].frame.frameId = usingDisk.curFrames;

                            Pages[i].frame.location = "RAM";
                            Pages[i].timesUsed++;
                            Pages[i].timeAssigned = clock;
                            usingDisk.curFrames++;
                        }
                        catch { }
                    }
                }
                usingDisk.status = "Ready";
                readyList.Add(usingDisk);
                usingDisk = null;
            }

            if (waiting == null) {
                if (waitingList.Count > 0) {
                    waiting = waitingList.ElementAt(0);
                    waiting.status = "Using I/O";
                    waitingList.RemoveAt(0);
                }
            }

            //Keep waiting until IO_curTime = IO_totalTime
            else {
                if (waiting.IO_curTime >= waiting.IO_totalTime) {
                    if (waiting.cpuUse == waiting.curTime) {
                        waiting.status = "Terminated";
                        terminatedList.Add(waiting);
                    }
                    else {
                        waiting.status = "Ready";
                        readyList.Add(waiting);
                    }
                    waiting = null;
                }
                else {
                    waiting.IO_curTime++;
                }
            }
            //If nothing is running and there is something ready, send it over.
            if (running == null) {
                if (readyList.Count > 0) {
                    running = readyList.ElementAt(0);
                    running.status = "Running";
                    readyList.RemoveAt(0);
                }
            }
            //If somethign is running for the first time and it uses the IO, send it to the using IO
            else if (running.curTime == running.IO_initTime && running.usesIO) {
                running.curTime++;
                running.status = "Waiting for I/O";
                waitingList.Add(running);
                running = null;
            }
            //If the timer is still not the quantum, keep adding some cycles until its finished.
            else {
                if (quantumTimer < quantum) {
                    if (running.curTime < running.cpuUse) {
                        running.curTime++;
                    }
                    else if (running.framesDone >= running.frames) {
                        running.endTime = clock;
                        running.sysEndTime = running.endTime - running.arrivalTime + 1;
                        running.waitTime = running.sysEndTime - running.cpuUse - running.IO_totalTime + 1;
                        running.status = "Terminated";
                        terminatedList.Add(running);
                        running = null;
                    }
                    else {
                        running.status = "Waiting Disk";
                        waitingDiskList.Add(running);
                        running = null;
                    }
                }
                else {
                    running.status = "Ready";
                    readyList.Add(running);
                    running = null;
                }
            }
            //FCFS from new to ready
            if (newList.Count > 0 && readyList.Count < readyLimit) {
                newList.ElementAt(0).status = "Ready";
                readyList.Add(newList.ElementAt(0));
                newList.RemoveAt(0);
            }
        }

        private void listUpdate() {
            mWindow.newList.ItemsSource = newList.Select(Process => Process.id);
            mWindow.readyList.ItemsSource = readyList.Select(Process => Process.id);
            mWindow.waitingList.ItemsSource = waitingList.Select(Process => Process.id);
            mWindow.waitingDiskList.ItemsSource = waitingDiskList.Select(Process => Process.id);
            mWindow.terminatedList.ItemsSource = terminatedList.Select(Process => Process.id);

            fullList = new ObservableCollection<Process>();
            fullList = new ObservableCollection<Process>(fullList.Concat(newList));
            fullList = new ObservableCollection<Process>(fullList.Concat(readyList));
            fullList = new ObservableCollection<Process>(fullList.Concat(waitingList));
            fullList = new ObservableCollection<Process>(fullList.Concat(terminatedList));
            fullList = new ObservableCollection<Process>(fullList.Concat(waitingDiskList));

            tapList = new ObservableCollection<Process>();
            tapList = new ObservableCollection<Process>(tapList.Concat(readyList));
            tapList = new ObservableCollection<Process>(tapList.Concat(waitingList));

            pageList = new ObservableCollection<Page>();
            pageList = new ObservableCollection<Page>(pageList.Concat(Pages));

            if (running != null) {
                fullList.Add(running);
                tapList.Add(running);
            }
            if (waiting != null) {
                fullList.Add(waiting);
                tapList.Add(waiting);
            }

            if (usingDisk != null) {
                fullList.Add(usingDisk);
                tapList.Add(usingDisk);
            }

            try {
                fullList = new ObservableCollection<Process>(fullList.OrderBy(Process => Process.realID));
                tapList = new ObservableCollection<Process>(tapList.OrderBy(Process => Process.realID));
            }
            catch {
                MessageBox.Show("Wow you managed to fuck it up");
            }

            mWindow.tap.Columns.Clear();

            ObservableCollection<DataGridTextColumn> _defTap = new ObservableCollection<DataGridTextColumn>();

            DataGridTextColumn id = new DataGridTextColumn() {
                Header = "ID",
                Binding = new Binding("id")
            };

            DataGridTextColumn size = new DataGridTextColumn() {
                Header = "Size",
                Binding = new Binding("size")
            };

            DataGridTextColumn frames = new DataGridTextColumn() {
                Header = "Frames",
                Binding = new Binding("frames")
            };

            DataGridTextColumn status = new DataGridTextColumn() {
                Header = "Status",
                Binding = new Binding("status")
            };

            _defTap.Add(id);
            _defTap.Add(size);
            _defTap.Add(frames);
            _defTap.Add(status);

            foreach (DataGridTextColumn item in _defTap) {
                mWindow.tap.Columns.Add(item);
            }

            int maxFrames;
            try {
                maxFrames = tapList.Max(Process => Process.framesLocation.Length);
            }
            catch {
                maxFrames = 0;
            }

            for (int i = 1; i <= maxFrames; i++) {
                DataGridTextColumn column = new DataGridTextColumn();
                column.Header = "Frame " + i;
                string _temp = "framesLocation[" + i + "]";
                column.Binding = new Binding(_temp);
                mWindow.tap.Columns.Add(column);
            }
        }

        private void drawCanvas() {
            mWindow.memoryMap.Children.Clear();

            Canvas canvas = mWindow.memoryMap;
            Line line = new Line();

            line.Stroke = Brushes.Black;
            line.SnapsToDevicePixels = true;
            line.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);

            line.X1 = 0;
            line.X2 = canvas.Width;
            line.Y1 = canvas.Height / 4;
            line.Y2 = line.Y1;

            line.StrokeThickness = 1;

            TextBlock text = new TextBlock();
            text.FontSize = 14;

            text.Text = "Operating System";
            Canvas.SetTop(text, canvas.Height / 8);
            Canvas.SetLeft(text, canvas.Width / 8);

            canvas.Children.Add(text);
            canvas.Children.Add(line);

            int toDraw = (ram - sysMemory) / frames;

            for (int i = 0; i < toDraw; i++) {
                line = new Line();
                line.Stroke = Brushes.Black;
                line.SnapsToDevicePixels = true;
                line.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);

                line.X1 = 0;
                line.X2 = canvas.Width;
                line.Y1 = (canvas.Height / 4) + ((canvas.Height * 0.75) * i / toDraw);
                line.Y2 = line.Y1;

                canvas.Children.Add(line);
            }

            for (int i = 0; i < Pages.Length; i++) {
                text = new TextBlock();
                text.FontSize = 10;
                try {
                    text.Text = Pages[i].frame.parent.id + " - Frame " + Pages[i].frame.frameId;
                    //state 0 = not used yet, blue
                    //state 1 = being used, green
                    //state 2 = used, red
                    if (Pages[i].state == 0) {
                        Polygon square = new Polygon();
                        square.Stroke = Brushes.Yellow;
                        square.Fill = Brushes.Yellow;

                        double height = ((canvas.Height / 4) + ((canvas.Height * 0.75) * (i + 1) / toDraw)) - ((canvas.Height / 4) + ((canvas.Height * 0.75) * i / toDraw));

                        square.Points = new PointCollection() {
                            new Point(0, 0), new Point(150, 0), new Point(150, height), new Point(0, height)
                        };

                        Canvas.SetTop(square, (canvas.Height / 4) + ((canvas.Height * 0.75) * i / toDraw));
                        Canvas.SetLeft(square, 0);

                        canvas.Children.Add(square);
                    }
                    else if (Pages[i].state == 1) {
                        Polygon square = new Polygon();
                        square.Stroke = Brushes.Green;
                        square.Fill = Brushes.Green;

                        double height = ((canvas.Height / 4) + ((canvas.Height * 0.75) * (i + 1) / toDraw)) - ((canvas.Height / 4) + ((canvas.Height * 0.75) * i / toDraw));

                        square.Points = new PointCollection() {
                            new Point(0, 0), new Point(150, 0), new Point(150, height), new Point(0, height)
                        };

                        Canvas.SetTop(square, (canvas.Height / 4) + ((canvas.Height * 0.75) * i / toDraw));
                        Canvas.SetLeft(square, 0);

                        canvas.Children.Add(square);
                    }
                    else {
                        Polygon square = new Polygon();
                        square.Stroke = Brushes.Red;
                        square.Fill = Brushes.Red;

                        double height = ((canvas.Height / 4) + ((canvas.Height * 0.75) * (i + 1) / toDraw)) - ((canvas.Height / 4) + ((canvas.Height * 0.75) * i / toDraw));

                        square.Points = new PointCollection() {
                            new Point(0, 0), new Point(150, 0), new Point(150, height), new Point(0, height)
                        };

                        Canvas.SetTop(square, (canvas.Height / 4) + ((canvas.Height * 0.75) * i / toDraw));
                        Canvas.SetLeft(square, 0);

                        canvas.Children.Add(square);
                    }
                }
                catch {
                }
                Canvas.SetTop(text, (canvas.Height / 4) + ((canvas.Height * 0.75) * i / toDraw));
                Canvas.SetLeft(text, canvas.Width / 4);

                canvas.Children.Add(text);
            }

            toDraw = ram / frames;

            for (int i = 0; i < toDraw; i++) {
                text = new TextBlock();
                text.FontSize = 8;
                text.Text = frames * i + "kb";
                Canvas.SetTop(text, (canvas.Height * i + 1) / toDraw);
                Canvas.SetLeft(text, 0);

                canvas.Children.Add(text);

                text = new TextBlock();
                text.FontSize = 8;
                text.Text = i.ToString();
                Canvas.SetTop(text, (canvas.Height * i + 1) / toDraw);
                Canvas.SetRight(text, 0);

                canvas.Children.Add(text);
            }

            mWindow.memoryMap = canvas;
        }

        private Frame lru() {
            Frame next = new Frame();

            return next;
        }
    }
}