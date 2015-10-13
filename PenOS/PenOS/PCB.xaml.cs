using System.Windows;

namespace PenOS {

    /// <summary>
    /// Interaction logic for PCB.xaml
    /// </summary>
    public partial class PCB : Window {

        public PCB() {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            Simulation.mWindow.pcbOpen = false;
            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }
    }
}