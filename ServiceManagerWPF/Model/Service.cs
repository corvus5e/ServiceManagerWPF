using System.ServiceProcess;

namespace ServiceManagerWPF.Model
{
    public enum ServiceStatus
    {
        Stopped = 1,
        StartPending = 2,
        StopPending = 3,
        Running = 4,
        ContinuePending = 5,
        PausePending = 6,
        Paused = 7,
        NotInstalled = 8
    }

    public interface IService
    {
        public string Name { get; }
        public string DisplayName { get; }
        public ServiceStatus Status { get; }

        public void Start();
        public void Stop();
        public void Pause();
        public void Refresh();
        public void WaitForStatus(ServiceStatus desiredStatus);
    }

    public class Service : IService
    {
        private ServiceController _nativeService;

        public string Name { get { return _nativeService.ServiceName; } }

        public string DisplayName { get { return _nativeService.DisplayName; } }

        public ServiceStatus Status { get { return (ServiceStatus)_nativeService.Status; } }

        public Service(ServiceController s)
        {
            _nativeService = s;
        }
        public void Start()
        {
            _nativeService.Start();
        }
        public void Stop()
        {
            _nativeService.Stop();
        }

        public void Pause()
        {
            _nativeService.Pause();
        }

        public void Refresh()
        {
            _nativeService.Refresh();
        }

        public void WaitForStatus(ServiceStatus desiredStatus)
        {
            _nativeService.WaitForStatus((ServiceControllerStatus)desiredStatus);
        }
    }

    public class NonExistentService : IService
    {
        string _name;
        string _displayName;

        public string Name { get { return _name; } }

        public string DisplayName { get { return _displayName; } }

        public ServiceStatus Status { get { return ServiceStatus.NotInstalled; } }       

        public NonExistentService(string name, string displayName)
        {
            _name = name;
            _displayName = displayName;
        }

        public void Start()
        { }
        public void Stop()
        { }
        public void Pause()
        { }
        public void Refresh()
        { }
        public void WaitForStatus(ServiceStatus desiredStatus)
        { }
    }
}
