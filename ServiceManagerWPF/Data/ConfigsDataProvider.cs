using ServiceManagerWPF.Model;
using System.IO;
using System.Text.Json;

namespace ServiceManagerWPF.Data
{
    public interface IConfigDataProvider
    {
        Task<Configs> GetConfigsAsync();

        Task SaveConfigAsync(Configs c);

        bool IsResourceAccessible();
    }

    public class JsonConfigsDataProvider : IConfigDataProvider
    {
        private string _configFileFullPath;

        public static string DefaultConfigFilePath { get => "Configs.json"; }

        //TODO: Do it real async if possible
        //https://stackoverflow.com/questions/14526377/why-does-this-async-action-hang-when-i-try-and-access-the-result-property-of-my

        public async Task<Configs> GetConfigsAsync(){
            return await Task.Run(() => {
                string jsonString = File.ReadAllText(_configFileFullPath); 
                Configs configs = JsonSerializer.Deserialize<Configs>(jsonString)!;
                return configs;
            });
        }
        public async Task SaveConfigAsync(Configs config)
        {
            await Task.Run(() => {
                using FileStream createStream = File.Create(_configFileFullPath);
                JsonSerializer.Serialize(createStream, config, new JsonSerializerOptions { WriteIndented = true });
            });
        }
        public bool IsResourceAccessible()
        {
            return File.Exists(_configFileFullPath);
        }

        public JsonConfigsDataProvider(string jsonConfigFileFullPath)
        {
            _configFileFullPath = jsonConfigFileFullPath;
        }
    }
}
