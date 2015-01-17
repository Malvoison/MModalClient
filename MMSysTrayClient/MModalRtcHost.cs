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
using Microsoft.Win32;
using System.Dynamic;
using System.Windows.Forms;

namespace MMSysTrayClient
{
    public class MModalRtcHost : IDisposable
    {
        private static MModalRtcHost _rtc;

        public bool IsHostConnected { get; set; }
        public bool IsEmrConnected { get; set; }
        public bool IsSpaConnected { get; set; }

        private bool Disposed { get; set; }
        private string RtcURI { get; set; }
        public string RtcGroup { get; set; }
        private IHubProxy HubProxy { get; set; }
        private HubConnection Connection { get; set; }
        private StreamWriter SignalRTrace { get; set; }

        //  Used for disposing handlers of server-called methods
        private IDisposable ClientConnected { get; set; }
        private IDisposable ClientDisconnected { get; set; }
        private IDisposable ClientReconnected { get; set; }
        private IDisposable LaunchSPA { get; set; }
        private IDisposable SetRequestContext { get; set; }
        private IDisposable InstructionsExecute { get; set; }
        private IDisposable RecordExecute { get; set; }
        private IDisposable PlayExecute { get; set; }
        private IDisposable StopExecute { get; set; }
        private IDisposable RewindExecute { get; set; }
        private IDisposable FastForwardExecute { get; set; }
        private IDisposable BeginningExecute { get; set; }
        private IDisposable EndExecute { get; set; }
        private IDisposable SaveDictation { get; set; }
        private IDisposable PendDictation { get; set; }
        private IDisposable CancelDictation { get; set; }
        private IDisposable SpaHostPing { get; set; }
        private IDisposable SetGlobalWaveVolume { get; set; }
        private ExpandoObject PatientContext { get; set; }

        private FormSmHost FormHost { get; set; }


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

                }

                Disposed = true;
            }
        }

        #endregion

        private async void Initialize()
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

                //  Connect to the RTC server
                await ConnectToServer();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                throw;
            }
        }

        public delegate void SaveCancelPendDelegate(int id);
        public delegate void SavePendDelegate(ExpandoObject jobState);
        public delegate void SetRequestContextDelegate(ExpandoObject reqcontext);
        public delegate void ShowAppDelegate(string uri);
        public delegate void SetGlobalWaveVolumeDelegate(int volume);

        private async Task ConnectToServer()
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
                Connection.Error += Connection_Error;

                HubProxy = Connection.CreateHubProxy("MMClientHub");

                #region Connection Info

                ClientConnected = HubProxy.On<string>("ClientConnected", message =>
                    {
                        Trace.WriteLine("ClientConnected: " + message);
                        if (message.StartsWith("SPAHOST-")) IsHostConnected = true;

                        if (message.StartsWith("SPA-")) IsSpaConnected = true;

                        if (message.StartsWith("EMR-")) IsEmrConnected = true;
                    });

                ClientDisconnected = HubProxy.On<string>("ClientDisconnected", message =>
                    {
                        Trace.WriteLine("ClientDisconnected: " + message);
                        if (message.StartsWith("SPAHOST-")) IsHostConnected = false;
                        if (message.StartsWith("SPA-")) IsSpaConnected = false;
                        if (message.StartsWith("EMR-")) IsEmrConnected = false;
                    });

                ClientReconnected = HubProxy.On<string>("ClientReconnected", message =>
                    {
                        Trace.WriteLine("ClientReconnected: " + message);
                        if (message.StartsWith("SPAHOST-")) IsHostConnected = true;
                        if (message.StartsWith("SPA-")) IsSpaConnected = true;
                        if (message.StartsWith("EMR-")) IsEmrConnected = true;
                    });

                #endregion

                #region Application Commands

                LaunchSPA = HubProxy.On("LaunchSPA", ( ) =>
                    {
                        string sUri = 
                            String.Format("{0}?group={1}", ConfigurationManager.AppSettings["appUrl"], MModalRtcHost.GetInstance().RtcGroup);
                        

                        ShowAppDelegate del = new ShowAppDelegate(Program._formSmHost.ShowApp);
                        Program._formSmHost.BeginInvoke(del, new Object[] { sUri });

                    });

                SetRequestContext = HubProxy.On<ExpandoObject>("SetRequestContext", (context) =>
                    {
                        SetRequestContextDelegate del = new SetRequestContextDelegate(Program._formSmHost.SetApiRequestContext);
                        Program._formSmHost.BeginInvoke(del, new Object[] { context });
    
                    });

                SpaHostPing = HubProxy.On("SpaHostPing", () =>
                {
                    HubProxy.Invoke("SpaHostPong");
                });

                SetGlobalWaveVolume = HubProxy.On("SetGlobalWaveVolume", (volume) =>
                    {
                        SetGlobalWaveVolumeDelegate del = new SetGlobalWaveVolumeDelegate(Program._formSmHost.SetGlobalWaveVolume);
                        Program._formSmHost.Invoke(del, new Object[] { Convert.ToInt32(volume) });
                    });

                #endregion

                #region SM Commands

                InstructionsExecute = HubProxy.On("InstructionsExecute", () =>
                    {
                        Program._formSmHost.Invoke((Action)(() =>
                            {
                                Program._formSmHost.DoInstructions();
                            }));
                    });

                RecordExecute = HubProxy.On("RecordExecute", () =>
                {
                    Program._formSmHost.Invoke((Action)(() =>
                    {
                        Program._formSmHost.DoRecord();
                    }));
                });

                PlayExecute = HubProxy.On("PlayExecute", () =>
                {
                    Program._formSmHost.Invoke((Action)(() =>
                    {
                        Program._formSmHost.DoPlay();
                    }));
                });

                StopExecute = HubProxy.On("StopExecute", () =>
                {
                    Program._formSmHost.Invoke((Action)(() =>
                    {
                        Program._formSmHost.DoStop();
                    }));
                });

                RewindExecute = HubProxy.On("RewindExecute", () =>
                {
                    Program._formSmHost.Invoke((Action)(() =>
                    {
                        Program._formSmHost.DoRewind();
                    }));
                });

                FastForwardExecute = HubProxy.On("FastForwardExecute", () =>
                {
                    Program._formSmHost.Invoke((Action)(() =>
                    {
                        Program._formSmHost.DoFastForward();
                    }));
                });

                BeginningExecute = HubProxy.On("BeginningExecute", () =>
                {
                    Program._formSmHost.Invoke((Action)(() =>
                    {
                        Program._formSmHost.DoBeginning();
                    }));
                });

                EndExecute = HubProxy.On("EndExecute", () =>
                {
                    Program._formSmHost.Invoke((Action)(() =>
                    {
                        Program._formSmHost.DoEnd();
                    }));
                });

                SaveDictation = HubProxy.On<ExpandoObject>("SaveDictation", (job_state) =>
                    {
                        SavePendDelegate del = new SavePendDelegate(Program._formSmHost.DoSave);
                        Program._formSmHost.Invoke(del, new Object[] { job_state });
                    });

                PendDictation = HubProxy.On<ExpandoObject>("PendDictation", (job_state) =>
                    {
                        SavePendDelegate del = new SavePendDelegate(Program._formSmHost.DoPend);
                        Program._formSmHost.Invoke(del, new Object[] { job_state });
                    });

                CancelDictation = HubProxy.On<int>("CancelDictation", (job_id) =>
                    {
                        SaveCancelPendDelegate del = new SaveCancelPendDelegate(Program._formSmHost.DoCancel);
                        Program._formSmHost.Invoke(del, new Object[] { job_id });
                    });
                    

                #endregion

                await Connection.Start().ContinueWith(startTask => 
                {
                    HubProxy.Invoke<bool>("IsSpaConnected").ContinueWith(spaTask =>
                    {
                        IsSpaConnected = spaTask.Result;
                        Trace.WriteLine("IsSpaConnected == " + IsSpaConnected.ToString());
                    });

                    HubProxy.Invoke<bool>("IsEmrConnected").ContinueWith(emrTask =>
                    {
                        IsEmrConnected = emrTask.Result;
                        Trace.WriteLine("IsEmrConnected == " + IsEmrConnected.ToString());                        
                    });
                });
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        public void DisconnectFromServer()
        {
            //  Release event handlers
            Connection.Closed -= Connection_Closed;
            Connection.Received -= Connection_Received;
            Connection.ConnectionSlow -= Connection_ConnectionSlow;
            Connection.Reconnecting -= Connection_Reconnecting;
            Connection.Reconnected -= Connection_Reconnected;
            Connection.StateChanged -= Connection_StateChanged;
            Connection.Error -= Connection_Error;

            //  Release server-called methods
            ClientConnected.Dispose();
            ClientDisconnected.Dispose();
            ClientReconnected.Dispose();
            LaunchSPA.Dispose();
            SetRequestContext.Dispose();
            InstructionsExecute.Dispose();
            RecordExecute.Dispose();
            PlayExecute.Dispose();
            StopExecute.Dispose();
            RewindExecute.Dispose();
            FastForwardExecute.Dispose();
            BeginningExecute.Dispose();
            EndExecute.Dispose();
            SaveDictation.Dispose();
            PendDictation.Dispose();
            CancelDictation.Dispose();
            SpaHostPing.Dispose();

            Connection.Stop();
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
            //Trace.WriteLine("MMClientHub: Received ==> " + obj);
        }

        void Connection_Closed()
        {
            Trace.WriteLine("MMClientHub: Closed");
        }

        void Connection_Error(Exception obj)
        {
            Trace.WriteLine(obj.Message);
        }


        #endregion

        #region Hub Method Wrappers

        public void SetVolume(int volume)
        {
            if (Connection.State == ConnectionState.Connected)
                HubProxy.Invoke("SetVolume", volume);
        }

        public void SetSoundLength(int length)
        {
            if (Connection.State == ConnectionState.Connected)
                HubProxy.Invoke("SetSoundLength", length);
        }

        public void SetRecordingTimeDisplay(string text)
        {
            if (Connection.State == ConnectionState.Connected)
                HubProxy.Invoke("SetRecordingTimeDisplay", text);
        }

        public void SetSoundPosition(int position)
        {
            if (Connection.State == ConnectionState.Connected)
                HubProxy.Invoke("SetSoundPosition", position);
        }

        public void SetInstructionsCanExecute(bool can)
        {
            if (Connection.State == ConnectionState.Connected)
                HubProxy.Invoke("SetInstructionsCanExecute", can);
        }

        public void SetRecordCanExecute(bool can)
        {
            if (Connection.State == ConnectionState.Connected)
                HubProxy.Invoke("SetRecordCanExecute", can);
        }

        public void SetPlayCanExecuute(bool can)
        {
            if (Connection.State == ConnectionState.Connected)
                HubProxy.Invoke("SetPlayCanExecute", can);
        }

        public void SetStopCanExecute(bool can)
        {
            if (Connection.State == ConnectionState.Connected)
                HubProxy.Invoke("SetStopCanExecute", can);
        }

        public void SetRewindCanExecute(bool can)
        {
            if (Connection.State == ConnectionState.Connected)
                HubProxy.Invoke("SetRewindCanExecute", can);
        }

        public void SetFastForwardCanExecute(bool can)
        {
            if (Connection.State == ConnectionState.Connected)
                HubProxy.Invoke("SetFastForwardCanExecute", can);
        }

        public void SetBeginningCanExecute(bool can)
        {
            if (Connection.State == ConnectionState.Connected)
                HubProxy.Invoke("SetBeginningCanExecute", can);
        }

        public void SetEndCanExecute(bool can)
        {
            if (Connection.State == ConnectionState.Connected)
                HubProxy.Invoke("SetEndCanExecutge", can);
        }

        public void SetCurrentDateTime()
        {
            if (Connection.State == ConnectionState.Connected)
                HubProxy.Invoke("SetCurrentDateTime", "your daddy");
        }

        //  For unit testing purposes
        public void SetRequestContextTest(dynamic reqObject)
        {
            if (Connection.State == ConnectionState.Connected)
                HubProxy.Invoke("SetRequestContext", reqObject);
        }

        public void LaunchSPATest()
        {
            if (Connection.State == ConnectionState.Connected)
                HubProxy.Invoke("LaunchSPA");
        }


        #endregion

    }
}
