
namespace ServiceManagerWPF
{
    public class Configs
    {
        /// <summary>
        /// Service group name -> list of service, which belong to this group 
        /// </summary>
        public Dictionary<string, IList<string>> Groups { get; set; } = new Dictionary<string, IList<string>>();

        public int? SelectedGroup { get; set; }
    }
}
