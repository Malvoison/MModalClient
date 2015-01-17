using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Http;
using System.Configuration;

namespace MMSysTrayClient
{
    public class HostErrorHandler
    {
        private static HostErrorHandler _instance;

        public string ApplicationName { get; set; }
        public string Detail { get; set; }
        public string HostName { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string StatusCode { get; set; }
        public DateTime Time { get; set; }
        public string Type { get; set; }
        public string User { get; set; }
        public string WebHostHtmlMessage { get; set; }

        private string BaseAddress { get; set; }

        public static HostErrorHandler GetInstance()
        {
            if (_instance == null)
            {
                _instance = new HostErrorHandler();

                _instance.BaseAddress = ConfigurationManager.AppSettings["errorUrl"];
                
                _instance.ApplicationName = "MMSysTrayClient";
                _instance.HostName = Environment.MachineName;
                _instance.Source = MModalRtcHost.GetInstance().RtcGroup;
                _instance.Type = "Host Error";
                _instance.User = "TESTUSER";
            }

            return _instance;
        }

        public async void LogHostError(string type, string message, string detail)
        {
            try
            {
                this.Type = type;
                this.Message = message;
                this.Detail = detail;
                this.Time = DateTime.Now;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseAddress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("LogHostError", this);
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }
    }
}
