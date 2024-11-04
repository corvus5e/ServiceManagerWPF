using ServiceManagerWPF.Data;
using ServiceManagerWPF.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace ServiceManagerWPF.ViewModel
{
    public class ServicesViewModel : INotifyPropertyChanged
    {
        private IServicesDataProvider _servicesDataProvider;
        private IConfigDataProvider _configDataProvider;
        private Configs _configs = new Configs();
        private ServiceCollection _services = new ServiceCollection();

        public ServiceCollection Services
        {
            get => _services;
            private set
            {
                _services = value;
                RaisePropertyChanged(nameof(Services));
            }
        }

        public Dictionary<string, IList<string>> Groups
        {
            get => _configs.Groups;
            private set
            {
                _configs.Groups = value;
                RaisePropertyChanged(nameof(Groups));
            }
        }

        public ServicesViewModel(IServicesDataProvider serviceDataProvider, IConfigDataProvider configsDataProvider)
        {
            _servicesDataProvider = serviceDataProvider;
            _configDataProvider = configsDataProvider;
        }

        public void LoadAsync()
        {
            _configs =  _configDataProvider.GetConfigsAsync().Result;
            _configs.Groups.Add("All", new List<string>()); // "All" is a default group

            Groups = _configs.Groups;
            Services =  _servicesDataProvider.GetServicesAsync().Result;
        }

        public ICollectionView GetFilteredCollectionView(string group)
        {
            ICollectionView _defaultView = CollectionViewSource.GetDefaultView(Services);

            if(group != null && Groups.ContainsKey(group))
            {
                var namesToShow = Groups[group];

                if(namesToShow.Count > 0)
                    _defaultView.Filter = s => namesToShow.Contains((s as IService).DisplayName);
                else
                    _defaultView.Filter = s => true;
            }

            return _defaultView;
        }

        //TODO: Possible ways to represent filtered collections:
        // https://stackoverflow.com/questions/15568325/filter-a-datagrid-in-wpf

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
