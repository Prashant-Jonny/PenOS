using System;
using System.Collections.Generic;
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
        //Mover a un nuevo objeto?

        public int newLimit;
        public int readyLimit;
        public int waitingLimit;
        public int quantum;
        public int probability;

        public MainWindow() {
            InitializeComponent();
        }

        private void playButton_Click(object sender, RoutedEventArgs e) {
            if (!errorCheck().Equals("Passed")) {
                MessageBox.Show(errorCheck());
            }
            else {
                MessageBox.Show("Works");
            }
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
    }
}