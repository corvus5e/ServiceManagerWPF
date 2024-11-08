using ServiceManagerWPF.Data;
using ServiceManagerWPF.ViewModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ServiceManagerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ServicesViewModel _viewModel;
        private string _configFile = string.Empty;

        public MainWindow()
        {
            InitializeComponent();

            _configFile = JsonConfigsDataProvider.DefaultConfigFilePath;
            _viewModel = new ServicesViewModel(new WindowsLocalServiceDataProvider(),
                new JsonConfigsDataProvider(_configFile));

            DataContext = _viewModel;

            Loaded += async (s, a) => await _viewModel.LoadAsync();

            SetupEventHandlers();
        }

        public void SetupEventHandlers()
        {
            _controlPanel.StartClicked += async (sender, args) => await _viewModel.ApplyCommandToSelectedServicesAsync(ServiceCommand.Start);
            _controlPanel.StopClicked += async (sender, args) => await _viewModel.ApplyCommandToSelectedServicesAsync(ServiceCommand.Stop);
            _controlPanel.PauseClicked += async (sender, args) => await _viewModel.ApplyCommandToSelectedServicesAsync(ServiceCommand.Pause);
            _controlPanel.RefreshClicked += async (sender, args) => await _viewModel.Refresh();
            _controlPanel.ConfigClicked += async (sender, args) => await _viewModel.OpenConfigFileViaDefaultAppAsync(_configFile);
            _groupsBox.SelectionChanged += GroupSelected;
            _viewModel.PropertyChanged += (s, a) => {
                /*When this handler is invoked from another thread, need to use Dispatcher*/
                this.Dispatcher.Invoke(() => {
                     var foo = (_servicesDataGrid.ItemsSource as ICollectionView);
                     foo?.Refresh();
                });
            };
        }
        public void GroupSelected(object sender, SelectionChangedEventArgs e)
        {
            var view = _viewModel.GetFilteredCollectionView(_viewModel.SelectedGroup);
            _servicesDataGrid.ItemsSource = view;
            view.Refresh();
        }

        private void _servicesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.SelectedServices = _servicesDataGrid.SelectedItems;
        }
    }
}