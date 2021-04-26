using System;
using System.Net.NetworkInformation;
using Routindo.Contract.Arguments;
using Routindo.Contract.Attributes;
using Routindo.Contract.Services;
using Routindo.Contract.Watchers;

namespace Routindo.Plugins.Web.Components.PingWatcher
{
    [PluginItemInfo(ComponentUniqueId, "Ping a specific host",
         "Ping a specific host and reports status changing or watch a specific status"),
     ResultArgumentsClass(typeof(PingStatusWatcherResultArgs))]
    public class PingStatusWatcher: IWatcher
    {
        public const string ComponentUniqueId = "D5B793D6-92D2-4E58-82CA-CC862D8FF709";

        public string Id { get; set; }

        [Argument(PingStatusWatcherArgs.Host)] public string Host { get; set; } 

        [Argument(PingStatusWatcherArgs.WatchAnyStatus)] public bool AnyStatus { get; set; }

        [Argument(PingStatusWatcherArgs.WatchStatus)] public bool Reachable { get; set; } 

        public ILoggingService LoggingService { get; set; }

        private bool? _lastStatus; 



        public WatcherResult Watch()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Host))
                    throw new Exception($"{nameof(Host)} not set");

                bool? oldStatus = _lastStatus;
                bool status;
                try
                {
                    using (var ping = new Ping())
                    {
                        PingReply pingReply = ping.Send(Host);
                        status = (pingReply != null && pingReply.Status == IPStatus.Success);
                    }
                }
                catch (PingException pingException)
                {
                    LoggingService.Error(pingException);
                    LoggingService.Error($"Exception HResult = {pingException.HResult}");
                    status = false;
                }

                bool notify = false;

                if (!_lastStatus.HasValue || status != _lastStatus.Value)
                {
                    _lastStatus = status;
                    if (AnyStatus)
                        notify = true;
                    else if (status == Reachable)
                    {
                        notify = true;
                    }
                }

                if (!notify) return WatcherResult.NotFound;
                _lastStatus = status;
                var statusString = status ? "Reachable" : "Unreachable";
                return WatcherResult.Succeed(ArgumentCollection.New()
                        .WithArgument(PingStatusWatcherResultArgs.Host, this.Host)
                        .WithArgument(PingStatusWatcherResultArgs.IsReachable, status))
                    .WithArgument(PingStatusWatcherResultArgs.ResultMessage, $"Host {Host} is {statusString}");
            }
            catch (Exception exception)
            {
                LoggingService.Error(exception);
                return WatcherResult.NotFound;
            }
        }
    }
}
