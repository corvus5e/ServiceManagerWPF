using ServiceManagerWPF.Data;
using ServiceManagerWPF.Model;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
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

        private readonly string DefaultGroup = "All";

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

        public string? SelectedGroup { get; set; }

        public IList? SelectedServices { get; set; }

        public ServicesViewModel(IServicesDataProvider serviceDataProvider, IConfigDataProvider configsDataProvider)
        {
            _servicesDataProvider = serviceDataProvider;
            _configDataProvider = configsDataProvider;
        }

        public async Task LoadAsync()
        {
            if (!_configDataProvider.IsResourceAccessible())
            {
                // Write default configs
                _configs = new Configs();
                await _configDataProvider.SaveConfigAsync(_configs);
            }
            else
            {
                _configs = await _configDataProvider.GetConfigsAsync();
            }

            _configs.Groups.Add(DefaultGroup, new List<string>());

            SelectedGroup = DefaultGroup;
            Groups = _configs.Groups;
            Services = await _servicesDataProvider.GetServicesAsync();

            RaisePropertyChanged($"{nameof(SelectedGroup)}");
        }

        public ICollectionView GetFilteredCollectionView(string? group)
        {
            ICollectionView _defaultView = CollectionViewSource.GetDefaultView(Services);
            _defaultView.Filter = s => true;

            if(group != null && Groups.ContainsKey(group))
            {
                var namesToShow = Groups[group];

                if (namesToShow.Count > 0)
                    _defaultView.Filter = s =>
                                         s != null && (s is IService) && namesToShow.Contains(((IService)s).DisplayName);
            }

            return _defaultView;
        }

        public async Task ApplyCommandToSelectedServicesAsync(ServiceCommand command)
        {
            await Task.Run(() =>
            {
#if RELEASE 
                try
                {
#endif
                    if (SelectedServices == null)
                        return;

                    foreach (var s in SelectedServices.Cast<IService>())
                    {
                        var desiredStatus = ServiceStatus.NotInstalled;
                        switch (command)
                        {
                            case ServiceCommand.Start: s.Start(); desiredStatus = ServiceStatus.Running; break;
                            case ServiceCommand.Stop: s.Stop(); desiredStatus = ServiceStatus.Stopped; break;
                            case ServiceCommand.Pause: s.Pause(); desiredStatus = ServiceStatus.Stopped; break;
                            case ServiceCommand.Refresh: s.Refresh(); desiredStatus = s.Status; break;
                            default:
                                throw new Exception("Unknow service command");
                        }

                        s.WaitForStatus(desiredStatus);
                        RaisePropertyChanged(nameof(Services));
                    }
#if RELEASE
                }
                catch(InvalidOperationException e)
                {
                    MessageBox.Show(e.ToString()); //TODO: Probably this should not be here
                }
#endif
            });
        }

        public async Task Refresh()
        {
            if(SelectedServices != null && SelectedServices.Count > 0)
            {
                await ApplyCommandToSelectedServicesAsync(ServiceCommand.Refresh);
            }
            else
            {
                await LoadAsync();
            }
        }

        public async Task OpenConfigFileViaDefaultAppAsync(string fullPath)
        {
            await Task.Run(() =>
            {
                //TODO: Get the default app for a json file format
                //      For now use notepad
                using Process myProcess = new Process();
                myProcess.StartInfo.FileName = "notepad.exe";
                myProcess.StartInfo.Arguments = fullPath;
                myProcess.Start();
            });

        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
