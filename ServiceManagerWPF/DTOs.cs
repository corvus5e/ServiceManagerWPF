
namespace ServiceManagerWPF
{
    class ServiceDTO
    {
        public string ServiceName { get; set; } = string.Empty;

        public string ServiceStatus { get; set; } = string.Empty;

        /// <summary>
        /// Index in the array where all data about each service is stored
        /// </summary>
        public int Index { get; set; } = 0;
    }
}
