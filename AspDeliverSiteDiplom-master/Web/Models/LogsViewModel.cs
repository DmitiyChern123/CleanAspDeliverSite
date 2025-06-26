using Newtonsoft.Json.Linq;

namespace DiplomApplication.Web.Models
{
    public class LogsViewModel
    {public List<JObject> Logs { get; set; } = new List<JObject>();
        public string ErrorMessage { get; set; }
        public DateTime SelectedDate { get; set; } = DateTime.Today;
        public string SelectedLevel { get; set; }
    }
}
