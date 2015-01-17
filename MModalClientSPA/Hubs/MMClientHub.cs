using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace MModalClientSPA.Hubs
{
    public class DictationContext
    {
        public int DictationContextId { get; set; }
        public DateTime CreateDate { get; set; }
        public int PatientId { get; set; }
        public string PatientLastName { get; set;}
        public string PatientFirstName { get; set; }
        public string PatientSex { get; set; }
        public string PatientDOB { get; set; }
        public string DateOfService { get; set; }
        public int EncounterId { get; set; }
        public int VisitId { get; set; }
        public string WorkType { get; set; }
        public int FFTAuthorId { get; set; }
        public string DictationUniqueId { get; set; }
        public string Base64Wave { get; set; }
    }

    public class ConnectionMapping<T>
    {
        private readonly Dictionary<T, HashSet<string>> _connections =
            new Dictionary<T, HashSet<string>>();

        public int Count
        {
            get { return _connections.Count; }
        }

        public void Add(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    connections = new HashSet<string>();
                    _connections.Add(key, connections);
                }

                lock (connections)
                {
                    connections.Add(connectionId);
                }
            }
        }

        public IEnumerable<string> GetConnections(T key)
        {
            HashSet<string> connections;
            if (_connections.TryGetValue(key, out connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }

        public void Remove(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                    {
                        _connections.Remove(key);
                    }
                }
            }
        }
    }

    public class MMClientHub : Hub
    {
        private readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();

        #region Overrides

        public override Task OnConnected()
        {
            string sClientName = Context.QueryString["client"];
            string sGroupName = Context.QueryString["group"];

            if (sGroupName != null && sGroupName != String.Empty)
            {
                Groups.Add(Context.ConnectionId, sGroupName);

                _connections.Add(sClientName, Context.ConnectionId);

                Clients.Group(sGroupName).ClientConnected(sClientName);
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string sClientName = Context.QueryString["client"];
            string sGroupName = Context.QueryString["group"];

            if (sGroupName != null && sGroupName != String.Empty)
            {
                Groups.Remove(Context.ConnectionId, sGroupName);

                _connections.Remove(sClientName, Context.ConnectionId);

                Clients.Group(sGroupName).ClientDisconnected(sClientName);
            }

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            string sClientName = Context.QueryString["client"];
            string sGroupName = Context.QueryString["group"];

            if (sGroupName != null && sGroupName != String.Empty)
            {
                Groups.Add(Context.ConnectionId, sGroupName);

                Clients.Group(sGroupName).ClientReconnected(sClientName);
            }
            
            return base.OnReconnected();
        }

        #endregion

        //  General Interest Methods
        public bool IsEmrConnected()
        {
            try
            {
                string sConnectionId = _connections.GetConnections("EMR-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

                return (sConnectionId == null) ? false : true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool IsSpaConnected()
        {
            try
            {
                string sConnectionId = _connections.GetConnections("SPA-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

                return (sConnectionId == null) ? false : true;
            }
            catch (Exception ex)
            {
                return true;
            }
        }

        public bool IsSpaHostConnected()
        {            
            string sConnectionId = _connections.GetConnections("SPAHOST-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();
            
            return (sConnectionId == null) ? false : true;
        }

        #region Ping-Pong

        public void SpaHostPing()
        {
            string sConnectionId = _connections.GetConnections("SPAHOST-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).SpaHostPing();
        }

        public void SpaPing()
        {
            string sConnectionId = _connections.GetConnections("SPA-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).SpaPing();
        }

        public void SpaHostPong()
        {
            string sConnectionId = _connections.GetConnections("EMR-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).SpaHostPong();
        }

        public void SpaPong()
        {
            string sConnectionId = _connections.GetConnections("EMR-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).SpaPong();
        }

        #endregion

        #region EMR ---> SPA

        //  Caller: EMR
        //  Target: SPA

        public void SetPatientContext(dynamic context)
        {
            string sConnectionId = _connections.GetConnections("SPA-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).SetPatientContext(context);
        }

        public void SetRequestContext(dynamic context)
        {
            string sConnectionId = _connections.GetConnections("SPA-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
            {
                Clients.Client(sConnectionId).SetRequestContext(context);
            }

            sConnectionId = _connections.GetConnections("SPAHOST-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
            {
                Clients.Client(sConnectionId).SetRequestContext(context);
            }
        }

        public void SetPatientInfo(dynamic context)
        {
            string sConnectionId = _connections.GetConnections("SPA-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).SetPatientInfo(context);
        }

        #endregion

        #region SPA ---> EMR

        public void GetPatientContext()
        {
            string sConnectionId = _connections.GetConnections("EMR-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).GetPatientContext();
        }

        #endregion

        #region EMR ---> SPAHOST

        public void LaunchSPA()
        {
            string sConnectionId = _connections.GetConnections("SPAHOST-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).LaunchSPA();
        }

        #endregion

        #region SPA ---> SPAHOST

        //  Caller: SPA
        //  Target: SPAHOST

        public void SetGlobalWaveVolume(int volume)
        {
            string sConnectionId = _connections.GetConnections("SPAHOST-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).SetGlobalWaveVolume(volume);
        }

        public void InstructionsExecute()
        {
            string sConnectionId = _connections.GetConnections("SPAHOST-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).InstructionsExecute();
        }

        public void RecordExecute()
        {
            string sConnectionId = _connections.GetConnections("SPAHOST-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).RecordExecute();
        }

        public void PlayExecute()
        {
            string sConnectionId = _connections.GetConnections("SPAHOST-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).PlayExecute();
        }

        public void StopExecute()
        {
            string sConnectionId = _connections.GetConnections("SPAHOST-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).StopExecute();
        }

        public void RewindExecute()
        {
            string sConnectionId = _connections.GetConnections("SPAHOST-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).RewindExecute();
        }

        public void FastForwardExecute()
        {
            string sConnectionId = _connections.GetConnections("SPAHOST-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).FastForwardExecute();
        }

        public void BeginningExecute()
        {
            string sConnectionId = _connections.GetConnections("SPAHOST-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).BeginningExecute();
        }

        public void EndExecute()
        {
            string sConnectionId = _connections.GetConnections("SPAHOST-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).EndExecute();
        }

        public void SaveDictation(dynamic job_state)
        {
            string sConnectionId = _connections.GetConnections("SPAHOST-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).SaveDictation(job_state);
        }

        public void PendDictation(dynamic job_state)
        {
            string sConnectionId = _connections.GetConnections("SPAHOST-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).PendDictation(job_state);
        }

        public void CancelDictation(int dictation_job_id)
        {
            string sConnectionId = _connections.GetConnections("SPAHOST-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).CancelDictation(dictation_job_id);
        }

        #endregion

        #region SPAHOST ---> SPA
        //  Caller:  SPAHOST
        //  Target:  SPA
        //  string sConnectionId = _connections.GetConnections("SPA-" + Context.QueryString["group"]).First();

        //  Device Events
        public void SetVolume(int volume)
        {
            string sConnectionId = _connections.GetConnections("SPA-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).SetVolume(volume);
        }

        public void SetSoundLength(int length)
        {
            string sConnectionId = _connections.GetConnections("SPA-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).SetSoundLength(length);
        }

        public void SetRecordingTimeDisplay(string text)
        {
            string sConnectionId = _connections.GetConnections("SPA-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).SetRecordingTimeDisplay(text);
        }

        public void SetSoundPosition(int position)
        {
            string sConnectionId = _connections.GetConnections("SPA-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).SetSoundPosition(position);
        }

        public void SetCurrentDateTime(string timestr)
        {
            string sConnectionId = _connections.GetConnections("SPA-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).SetCurrentDateTime(timestr);
        }

        //  Record/Playback Commands
        public void SetInstructionsCanExecute(bool can)
        {
            string sConnectionId = _connections.GetConnections("SPA-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).SetInstructionsCanExecute(!can);
        }

        public void SetRecordCanExecute(bool can)
        {
            string sConnectionId = _connections.GetConnections("SPA-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).SetRecordCanExecute(!can);
        }

        public void SetPlayCanExecute(bool can)
        {
            string sConnectionId = _connections.GetConnections("SPA-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).SetPlayCanExecute(!can);
        }

        public void SetStopCanExecute(bool can)
        {
            string sConnectionId = _connections.GetConnections("SPA-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).SetStopCanExecute(!can);
        }

        public void SetRewindCanExecute(bool can)
        {
            string sConnectionId = _connections.GetConnections("SPA-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).SetRewindCanExecute(!can);
        }

        public void SetFastForwardCanExecute(bool can)
        {
            string sConnectionId = _connections.GetConnections("SPA-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).SetFastForwardCanExecute(!can);
        }

        public void SetBeginningCanExecute(bool can)
        {
            string sConnectionId = _connections.GetConnections("SPA-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).SetBeginningCanExecute(!can);
        }

        public void SetEndCanExecute(bool can)
        {
            string sConnectionId = _connections.GetConnections("SPA-" + Context.QueryString["group"]).DefaultIfEmpty(null).First();

            if (sConnectionId != null)
                Clients.Client(sConnectionId).SetEndCanExecute(!can);
        }

        #endregion
    }
}