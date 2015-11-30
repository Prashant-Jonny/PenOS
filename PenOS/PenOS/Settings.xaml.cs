using System.Windows;

namespace PenOS {

    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window {
        public int probability;
        public int quantum;
        public int ioTime;
        public int newLimit;
        public int readyLimit;
        public int waitingLimit;
        public int ram;
        public int frames;
        public int diskTime;
        public string algorithm;
        public string delay;
        public string memoryAlg;

        public Settings() {
            InitializeComponent();
        }

        public string errorCheck() {
            bool isNumeric;

            isNumeric = int.TryParse(diskTime_text.Text, out diskTime);
            if (!isNumeric && diskTime > 0) {
                return "Please input positive integers only. (Parameters/Disk Response Time)";
            }

            isNumeric = int.TryParse(probability_text.Text, out probability);
            if (!isNumeric && probability >= 0 && probability <= 100) {
                return "Please input integers in range only. (Parameters/Probability)";
            }

            isNumeric = int.TryParse(quantum_text.Text, out quantum);
            if (!isNumeric && quantum > 0) {
                return "Please input positive integers only. (Parameters/Quantum)";
            }

            isNumeric = int.TryParse(ioUse.Text, out ioTime);
            if (!isNumeric && ioTime > 0) {
                return "Please input positive integers only. (Parameters/IO Response Time)";
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

            if (algSelected.SelectedItem == null) {
                return "Please make a selection. (Parameters/Algorithm)";
            }
            else {
                algorithm = algSelected.Text;
            }

            if (delaySelected.SelectedItem == null) {
                return "Please make a selection. (Parameters/Delay)";
            }
            else {
                delay = delaySelected.Text;
            }

            if (ramSelected.SelectedItem == null) {
                return "Please make a selection. (RAM)";
            }
            else {
                int.TryParse(ramSelected.Text.Replace("kb", ""), out ram);
            }

            if (frameSelected.SelectedItem == null) {
                return "Please make a selection. (Frames)";
            }
            else {
                int.TryParse(frameSelected.Text.Replace("kb", ""), out frames);
            }

            if (map.SelectedItem == null) {
                return "Please make a selection. (MAP)";
            }
            else {
                memoryAlg = map.Text;
            }

            return "Passed";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            MainWindow.settingsOpen = false;
            if (!errorCheck().Equals("Passed")) {
                MessageBox.Show("Error in one of the values");
            }
            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }

        private void frameSelected_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            try {
                int.TryParse(frameSelected.Text.Replace("kb", ""), out frames);
                int.TryParse(ramSelected.Text.Replace("kb", ""), out ram);

                if (frames > ram) {
                    MessageBox.Show("The frames cannot be bigger than the ram");
                    frameSelected.SelectedIndex = 0;
                }
            }
            catch {
            }
        }

        private void ramSelected_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            try {
                int.TryParse(frameSelected.Text.Replace("kb", ""), out frames);
                int.TryParse(ramSelected.Text.Replace("kb", ""), out ram);

                if (frames > ram) {
                    MessageBox.Show("The frames cannot be bigger than the ram");
                    frameSelected.SelectedIndex = 0;
                }
            }
            catch {
            }
        }
    }
}