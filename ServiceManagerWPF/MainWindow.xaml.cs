using System.IO;
using System.ServiceProcess;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace ServiceManagerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /*  ---------  Lets's store data here for now ----------- */
        public string ConfigPath = "Configs.json";
        public Configs Configs;
        public ServiceController[] Services;

        public MainWindow()
        {
            InitializeComponent();

            Services = LoadServices();
            // TODO: Check if config file exists
            Configs = LoadConfigs(ConfigPath);

            FillUIWithData();

            SetupEventHandlers();
        }

        public void SetupEventHandlers()
        {
            _groupsComboBox.SelectionChanged += _groupsComboBox_SelectionChanged;
            _controlPanel.StartClicked += (sender, args) => MessageBox.Show("Start");
            _controlPanel.StopClicked += (sender, args) => MessageBox.Show("Stop");
            _controlPanel.PauseClicked += (sender, args) => MessageBox.Show("Pause");
            _controlPanel.RefreshClicked += (sender, args) => MessageBox.Show("Refresh");
            _controlPanel.ConfigClicked += (sender, args) => MessageBox.Show("Config");
        }

        public void FillUIWithData()
        {
            // Put groups info
            _groupsComboBox.Items.Add("All"); // Put default group to list all services
            foreach(var group in Configs.Groups.Keys)
            {
                _groupsComboBox.Items.Add(group);
            }

            _groupsComboBox.SelectedIndex = 0;

            // Put service info
            foreach(var s in Services)
            {
                _servicesDataGrid.Items.Add(ServiceToDataGridItem(s));
            }
        }

        public ServiceController[] LoadServices()
        {
            return ServiceController.GetServices();
        }

        public Configs LoadConfigs(string fullPath)
        {
            string jsonString = File.ReadAllText(fullPath);
            Configs configs = JsonSerializer.Deserialize<Configs>(jsonString)!;
            return configs;
        }

        public void SaveConfigs(Configs configs, string fullPath)
        {
            using FileStream createStream = File.Create(fullPath);
            JsonSerializer.Serialize(createStream, configs);
        }

        /* -----  Lets put event handlers here for now ----- */

        private void _groupsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var allSelected = _groupsComboBox.SelectedIndex == 0;

            Func<ServiceController, bool> filter = x => true; // Tale all by default

            if (!allSelected)
            {
                var selectedGroup = _groupsComboBox.SelectedValue.ToString();

                if (selectedGroup == null)
                    throw new Exception("Could not retrieve selected group value");

                filter = x => Configs.Groups[selectedGroup].Contains(x.DisplayName);
            }
            
            // TODO: Study usage of ItemsSource
            _servicesDataGrid.Items.Clear();
            var filteredServices = Services.Where(filter); 
            foreach (var s in filteredServices)
            {
                _servicesDataGrid.Items.Add(ServiceToDataGridItem(s));
            }
        }

        private object ServiceToDataGridItem(ServiceController s)
        {
            return new { ServiceName = s.DisplayName, ServiceStatus = s.Status.ToString()};
        }
    }
}