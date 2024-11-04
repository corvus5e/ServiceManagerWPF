using ServiceManagerWPF.Model;
using System.ServiceProcess;

namespace ServiceManagerWPF.Data
{
    public interface IServicesDataProvider
    {
        Task<ServiceCollection> GetServicesAsync(); 
    }

    /// <summary>
    /// This gets Windows services from local PC
    /// </summary>
    public class WindowsLocalServiceDataProvider : IServicesDataProvider
    {
        public async Task<ServiceCollection> GetServicesAsync() //TODO: The same as with Configs. Look at the same methon in the ConfigDataProvider class
        {
            var services = new ServiceCollection();
            var nativeServices = ServiceController.GetServices();
            foreach (var s in nativeServices)
            {
                services.Add(new Service(s));
            }

            return services;
        }
    }
}
