using ServiceManagerWPF.Data;
using ServiceManagerWPF.ViewModel;
using System.ComponentModel;
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
            _controlPanel.StartClicked += (sender, args) => _viewModel.ApplyCommandToSelectedServices(ServiceCommand.Start);
            _controlPanel.StopClicked += (sender, args) => _viewModel.ApplyCommandToSelectedServices(ServiceCommand.Stop);
            _controlPanel.PauseClicked += (sender, args) => _viewModel.ApplyCommandToSelectedServices(ServiceCommand.Pause);
            _controlPanel.RefreshClicked += (sender, args) => _viewModel.ApplyCommandToSelectedServices(ServiceCommand.Refresh);
            _controlPanel.ConfigClicked += (sender, args) => MessageBox.Show("Config");
            _groupsBox.SelectionChanged += GroupSelected;
            _viewModel.PropertyChanged += (s, a) => (_servicesDataGrid.ItemsSource as ICollectionView)?.Refresh();
        }

        /*public void SaveConfigs(Configs configs, string fullPath)
        {
            using FileStream createStream = File.Create(fullPath);
            JsonSerializer.Serialize(createStream, configs);
        }*/

        public void GroupSelected(object sender, SelectionChangedEventArgs e)
        {
            var filteredView = _viewModel.GetFilteredCollectionView(_viewModel.SelectedGroup);

            _servicesDataGrid.ItemsSource = filteredView;

            filteredView.Refresh();
        }

        private void _servicesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.SelectedServices = _servicesDataGrid.SelectedItems;
        }
    }
}