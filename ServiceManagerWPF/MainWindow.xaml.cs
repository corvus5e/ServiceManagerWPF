using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ServiceProcess;

namespace ServiceManagerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            _groupsComboBox.Items.Add("All");
            _groupsComboBox.SelectedIndex = 0;

            ServiceController[] services = ServiceController.GetServices();
            foreach(var s in services)
            {
                _servicesDataGrid.Items.Add(new { ServiceName = s.DisplayName, ServiceStatus = s.Status.ToString()});
            }
        }
    }
}