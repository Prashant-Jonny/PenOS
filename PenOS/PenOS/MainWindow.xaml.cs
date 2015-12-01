using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PenOS {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        //Values altered by buttons

        public bool paused = false;
        public bool stopped = false;
        public static bool settingsOpen = false;

        public bool simulStarted = false;

        private Simulation simul = new Simulation();
        private Settings settings = new Settings();

        public MainWindow() {
            InitializeComponent();
            Simulation.mWindow = this;
            Simulation.settings = settings;
        }

        private void playButton_Click(object sender, RoutedEventArgs e) {
            if (!settings.errorCheck().Equals("Passed")) {
                MessageBox.Show(settings.errorCheck());
            }
            else if (paused) {
                paused = false;
            }
            else {
                stopped = false;
                paused = false;
                resetValues();

                simul = new Simulation(settings.probability, settings.quantum, settings.newLimit, settings.readyLimit, settings.waitingLimit, settings.algorithm, settings.delay, settings.ioTime, settings.ram, settings.frames, settings.diskTime, settings.memoryAlg);
                simul.Start();
            }
        }

        private void stopButton_Click(object sender, RoutedEventArgs e) {
            stopped = true;
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e) {
            paused = true;
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

        private void help_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("Hover over the elements to see their function");
        }

        private void settings_Click(object sender, RoutedEventArgs e) {
            if (!settingsOpen) {
                try {
                    settings.Show();
                }
                catch (InvalidOperationException ex) {
                    MessageBox.Show(ex.Message);
                }
                settingsOpen = true;
            }
            else {
                MessageBox.Show("A settings window is already running");
            }
        }
    }
}