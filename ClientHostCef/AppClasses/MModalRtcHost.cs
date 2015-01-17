using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using Microsoft.AspNet.SignalR.Client;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;
using System.Configuration;

namespace ClientHostCef.AppClasses
{
    public class MModalRtcHost : IDisposable
    {
        private static MModalRtcHost _rtc;

        private bool Disposed { get; set; }
        private string RtcURI { get; set; }
        public string RtcGroup { get; set; }
        private IHubProxy HubProxy { get; set; }
        private HubConnection Connection { get; set; }
        private StreamWriter SignalRTrace { get; set; }

        #region Externs


        #endregion

        static MModalRtcHost() { }

        public static MModalRtcHost GetInstance()
        {
            try
            {
                if (_rtc != null)
                {
                    return _rtc;
                }
                else
                {
                    _rtc = new MModalRtcHost();
                    _rtc.Initialize();
                    return _rtc;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                throw;
            }
        }

        private MModalRtcHost() { }

        ~MModalRtcHost()
        {
            //  dispose of anything in need of disposition
        }

        #region IDisposable Implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    SignalRTrace.Close();
                    SignalRTrace.Dispose();
                }

                Disposed = true;
            }
        }

        #endregion

        private void Initialize()
        {
            try
            {
                RtcURI = ConfigurationManager.AppSettings["rtcUrl"];
                RtcGroup = Environment.MachineName;

                IPHostEntry host;
                string sIpAddress = String.Empty;
                host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        sIpAddress = ip.ToString();
                        break;
                    }
                }
                RtcGroup += "@";
                RtcGroup += sIpAddress;

                //  setup log file
                SignalRTrace = File.CreateText("SignalRTraceLog");

                //  Connect to the RTC server
                ConnectToServer();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                throw;
            }
        }

        private async void ConnectToServer()
        {
            try
            {
                Connection = new HubConnection(RtcURI, String.Format("client=SPAHOST-{0}&group={1}", RtcGroup, RtcGroup));
                Connection.Closed += Connection_Closed;
                Connection.Received += Connection_Received;
                Connection.ConnectionSlow += Connection_ConnectionSlow;
                Connection.Reconnecting += Connection_Reconnecting;
                Connection.Reconnected += Connection_Reconnected;
                Connection.StateChanged += Connection_StateChanged;

                Connection.TraceLevel = TraceLevels.All;
                Connection.TraceWriter = SignalRTrace;

                Connection.Error += ex => Trace.WriteLine("SignalR Error: " + ex.Message);

                HubProxy = Connection.CreateHubProxy("MMClientHub");

                HubProxy.On("Hello", () => Trace.WriteLine("MMClientHub: Hello"));

                HubProxy.On<string>("GroupMessage", message =>
                    Trace.WriteLine("GroupMessage: " + message));

                HubProxy.On<string>("ClientConnected", message =>
                    Trace.WriteLine("ClientConnected: " + message));

                HubProxy.On<string>("ClientDisconnected", message =>
                    Trace.WriteLine("ClientDisconnected: " + message));

                HubProxy.On<string>("ClientReconnected", message =>
                    Trace.WriteLine("ClientReconnected: " + message));

                await Connection.Start();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        #region Hub Connection State Events

        void Connection_StateChanged(StateChange obj)
        {
            Trace.WriteLine("MMClientHub: StateChanged ==> " + obj.OldState.ToString() + " to " + obj.NewState.ToString());
        }

        void Connection_Reconnected()
        {
            Trace.WriteLine("MMClientHub: Reconnected");
        }

        void Connection_Reconnecting()
        {
            Trace.WriteLine("MMClientHub: Reconnecting");
        }

        void Connection_ConnectionSlow()
        {
            Trace.WriteLine("MMClientHub: ConnectionSlow");
        }

        void Connection_Received(string obj)
        {
            Trace.WriteLine("MMClientHub: Received ==> " + obj);
        }

        void Connection_Closed()
        {
            Trace.WriteLine("MMClientHub: Closed");
        }

        #endregion

        #region Hub Method Wrappers

        public void SendGroupMessage(string message)
        {
            HubProxy.Invoke("SendGroupMessage", RtcGroup, message);
        }

        #endregion

        public void DisconnectFromServer()
        {
            Connection.Stop();
        }
    }
}
