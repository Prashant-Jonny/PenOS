using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Proyect_1 {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        //Values assigned by user

        private int newLimit;
        private int readyLimit;
        private int waitingLimit;
        private int quantum;
        private int probability;

        //Values altered by buttons

        public bool paused = false;
        public bool stopped = false;
        public bool pcbOpen = false;

        public bool simulStarted = false;

        private Simulation simul = new Simulation();
        private PCB pcb = new PCB();

        public MainWindow() {
            InitializeComponent();
            Simulation.mWindow = this;
            Simulation.pcb = pcb;
        }

        private void playButton_Click(object sender, RoutedEventArgs e) {
            if (!errorCheck().Equals("Passed")) {
                MessageBox.Show(errorCheck());
            }
            else if (paused) {
                paused = false;
            }
            else {
                stopped = false;
                paused = false;
                resetValues();

                simul = new Simulation(probability, quantum, newLimit, readyLimit, waitingLimit, algSelected.Text, delaySelected.Text);
                simul.Start();
            }
        }

        private void stopButton_Click(object sender, RoutedEventArgs e) {
            stopped = true;
            /*simul = new Simulation();
            resetValues();*/
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e) {
            paused = true;
        }

        private void pcbButton_Click(object sender, RoutedEventArgs e) {
            simul.pcbUpdate();
            if (!pcbOpen) {
                try {
                    pcb.Show();
                }
                catch (InvalidOperationException ex) {
                    MessageBox.Show(ex.Message);
                }
                pcbOpen = true;
            }
            else {
                MessageBox.Show("A PCB Window is already running");
            }
        }

        public void resetValues() {
            clock.Text = "0";

            newList.ItemsSource = null;
            newList.Items.Clear();
            readyList.ItemsSource = null;
            readyList.Items.Clear();
            waitingList.ItemsSource = null;
            waitingList.Items.Clear();
            terminatedList.ItemsSource = null;
            terminatedList.Items.Clear();

            running.Text = null;
            io.Text = null;
        }

        private string errorCheck() {
            bool isNumeric;

            isNumeric = int.TryParse(probability_text.Text, out probability);
            if (!isNumeric && probability >= 0 && probability <= 100) {
                return "Please input integers in range only. (Parameters/Probability)";
            }

            isNumeric = int.TryParse(quantum_text.Text, out quantum);
            if (!isNumeric && quantum > 0) {
                return "Please input positive integers only. (Parameters/Quantum)";
            }

            isNumeric = int.TryParse(newLimit_text.Text, out newLimit);
            if (!isNumeric && newLimit > 0) {
                return "Please input positive integers only. (List Limit/New)";
            }

            isNumeric = int.TryParse(readyLimit_text.Text, out readyLimit);
            if (!isNumeric && readyLimit > 0) {
                return "Please input positive integers only. (List Limit/Ready)";
            }

            isNumeric = int.TryParse(waitingLimit_text.Text, out waitingLimit);
            if (!isNumeric && waitingLimit > 0) {
                return "Please input positive integers only. (List Limit/Waiting)";
            }

            if (algSelected.SelectedItem.Equals(null)) {
                return "Please make a selection. (Parameters/Algorithm)";
            }

            if (delaySelected.SelectedItem.Equals(null)) {
                return "Please make a selection. (Parameters/Delay)";
            }

            return "Passed";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            using (MemoryStream ms = new MemoryStream()) {
                Properties.Resources.PenOS.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Position = 0;
                BitmapImage bm = new BitmapImage();
                bm.BeginInit();
                bm.StreamSource = ms;
                bm.CacheOption = BitmapCacheOption.OnLoad;
                bm.EndInit();

                image.Source = bm;
            }
        }
    }
}