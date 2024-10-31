using System.IO;
using System.ServiceProcess;
using System.Text;
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

            _controlPanel.StartClicked += (sender, args) => ApplyCommandToSelectedServices(s => s.Start());
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
            for(int i = 0; i < Services.Count(); i++)
            {
                _servicesDataGrid.Items.Add(ServiceToDataGridItem(Services[i], i));
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
            var selectedGroup = _groupsComboBox.SelectedValue.ToString();

            if (selectedGroup == null)
                throw new Exception("Could not retrieve selected group value");

            var filteredIndices = new List<int>(); 

            if(!Configs.Groups.ContainsKey(selectedGroup))
            {   
                // Select all
                filteredIndices = Enumerable.Range(0, Services.Count()).ToList();
            }
            else
            {
                var servicesInGroup = Configs.Groups[selectedGroup];

                foreach(var serviceName in servicesInGroup)
                {
                    int i = Array.FindIndex(Services, 0, Services.Count(), x => x.DisplayName == serviceName);

                    if(i < 0)
                    {
                        // Service not exists ot not installed
                        //TODO: List him as gray
                    }
                    else
                    {
                        filteredIndices.Add(i);
                    }
                }
            }
            
            _servicesDataGrid.Items.Clear();

            foreach(var i in filteredIndices)
            {
                _servicesDataGrid.Items.Add(ServiceToDataGridItem(Services[i], i));
            }
        }

        public void ApplyCommandToSelectedServices(Action<ServiceController> command)
        {
            var serviceIndices = GetSelectedServices();

            foreach(var i in serviceIndices)
            {
                command(Services[i]);
            }
        }

        private object ServiceToDataGridItem(ServiceController s, int index)
        {
            return new ServiceDTO{ServiceName = s.DisplayName, ServiceStatus = s.Status.ToString(), Index = index,};
        }

        private IList<int> GetSelectedServices()
        {
            var selectedServiceIndices = new List<int>();

            foreach (var s in _servicesDataGrid.SelectedItems)
            {
                var serviceDTO = s as ServiceDTO;

                if(serviceDTO != null)
                    selectedServiceIndices.Add(serviceDTO.Index);
            }

            return selectedServiceIndices;
        }
    }
}