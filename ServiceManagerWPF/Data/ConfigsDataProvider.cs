using ServiceManagerWPF.Model;
using System.IO;
using System.Text.Json;

namespace ServiceManagerWPF.Data
{
    public interface IConfigDataProvider
    {
        Task<Configs> GetConfigsAsync();
    }

    public class JsonConfigsDataProvider : IConfigDataProvider
    {
        private string _configFileFullPath;

        public static string DefaultConfigFilePath { get => "Configs.json"; }

        //TODO: Do it real async if possible
        //https://stackoverflow.com/questions/14526377/why-does-this-async-action-hang-when-i-try-and-access-the-result-property-of-my

        public async Task<Configs> GetConfigsAsync(){
            string jsonString = File.ReadAllText(_configFileFullPath);
            Configs configs = JsonSerializer.Deserialize<Configs>(jsonString)!;
            return configs;
        }

        public JsonConfigsDataProvider(string jsonConfigFileFullPath)
        {
            _configFileFullPath = jsonConfigFileFullPath;
        }
    }
}
