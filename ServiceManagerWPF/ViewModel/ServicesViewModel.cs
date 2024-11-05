using ServiceManagerWPF.Data;
using ServiceManagerWPF.Model;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace ServiceManagerWPF.ViewModel
{

    public enum ServiceCommand
    {
        Start, Stop, Pause, Refresh
    }

    public class ServicesViewModel : INotifyPropertyChanged
    {
        private IServicesDataProvider _servicesDataProvider;
        private IConfigDataProvider _configDataProvider;
        private Configs _configs = new Configs();
        private ServiceCollection _services = new ServiceCollection();

        public event PropertyChangedEventHandler? PropertyChanged;

        public ServiceCollection Services
        {
            get => _services;
            private set
            {
                _services = value;
                RaisePropertyChanged(nameof(Services));
            }
        }

        public Dictionary<string, IList<string>> Groups // TODO: Make an "observable" collection instead of Dictionary?
        {
            get => _configs.Groups;
            private set
            {
                _configs.Groups = value;
                RaisePropertyChanged(nameof(Groups));
            }
        }
    
        public string SelectedGroup { get; set; }

        public IList SelectedServices { get; set; }

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
            _defaultView.Filter = s => true;

            if(group != null && Groups.ContainsKey(group))
            {
                var namesToShow = Groups[group];

                if(namesToShow.Count > 0)
                    _defaultView.Filter = s => namesToShow.Contains((s as IService).DisplayName);
            }

            return _defaultView;
        }

        public void ApplyCommandToSelectedServices(ServiceCommand command)
        {
            foreach (var s in SelectedServices.Cast<IService>())
            {
                var desiredStatus = ServiceStatus.NotInstalled;
                switch (command)
                {
                    case ServiceCommand.Start: s.Start(); desiredStatus = ServiceStatus.Running; break;
                    case ServiceCommand.Stop: s.Stop(); desiredStatus = ServiceStatus.Stopped; break;
                    case ServiceCommand.Pause: s.Pause();desiredStatus = ServiceStatus.Stopped; break;
                    case ServiceCommand.Refresh: s.Refresh(); desiredStatus = s.Status; break;
                    default:
                        throw new Exception("Unknow service command");
                }

                s.WaitForStatus(desiredStatus);
                RaisePropertyChanged(nameof(Services));

            }
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
