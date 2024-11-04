using ServiceManagerWPF.Data;
using ServiceManagerWPF.Model; //TODO: Remove this reference
using ServiceManagerWPF.ViewModel;
using System.IO;
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
        private ServicesViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new ServicesViewModel(new WindowsLocalServiceDataProvider(),
                new JsonConfigsDataProvider(JsonConfigsDataProvider.DefaultConfigFilePath));

            DataContext = _viewModel;

            Loaded += (s, a) => _viewModel.LoadAsync(); 

            SetupEventHandlers();
        }

        public void SetupEventHandlers()
        {

            _controlPanel.StartClicked += (sender, args) => ApplyCommandToSelectedServices(s => s.Start());
            _controlPanel.StopClicked += (sender, args) => ApplyCommandToSelectedServices(s => s.Stop());
            _controlPanel.PauseClicked += (sender, args) => ApplyCommandToSelectedServices(s => s.Pause());
            _controlPanel.RefreshClicked += (sender, args) => ApplyCommandToSelectedServices(s => s.Refresh());
            _controlPanel.ConfigClicked += (sender, args) => MessageBox.Show("Config");
            _groupsBox.SelectionChanged += GroupSelected;
        }

        /*public void SaveConfigs(Configs configs, string fullPath)
        {
            using FileStream createStream = File.Create(fullPath);
            JsonSerializer.Serialize(createStream, configs);
        }*/

        public void GroupSelected(object sender, SelectionChangedEventArgs e)
        {
            var selectedGroup = _groupsBox.SelectedItem.ToString();

            var filteredView = _viewModel.GetFilteredCollectionView(selectedGroup);

            _serviceTable.FilterView(filteredView);
        }

        public void ApplyCommandToSelectedServices(Action<IService> command)
        {
            /*var serviceNames = GetSelectedServices();

            foreach(var n in serviceNames)
            {
                command(_viewModel.Services.Where(x => x.Name == n).Select(x => x).First());
            }*/
        }
    }
}