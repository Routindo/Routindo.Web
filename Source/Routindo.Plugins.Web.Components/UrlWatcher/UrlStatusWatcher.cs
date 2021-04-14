using System;
using System.Net;
using Routindo.Contract.Arguments;
using Routindo.Contract.Attributes;
using Routindo.Contract.Services;
using Routindo.Contract.Watchers;

namespace Routindo.Plugins.Web.Components.UrlWatcher
{
    [PluginItemInfo(ComponentUniqueId, "Url Status Watcher",
         "Watch a specific URL for status code changing and notifies when server response equals a specific status"),
     ResultArgumentsClass(typeof(UrlStatusWatcherResultArgs))]
    public class UrlStatusWatcher: IWatcher
    {
        public const string ComponentUniqueId = "CA64E1C9-0A38-446D-AAB3-7A94DDBC3CCB";

        public string Id { get; set; }

        [Argument(UrlStatusWatcherArgs.Url)] public string Url { get; set; }

        [Argument(UrlStatusWatcherArgs.WatchAnyStatus)] public bool AnyStatus { get; set; }

        [Argument(UrlStatusWatcherArgs.WatchStatus)] public int StatusCode { get; set; }

        public ILoggingService LoggingService { get; set; }

        private HttpStatusCode? _lastStatusCode; 

        

        public WatcherResult Watch()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Url))
                    throw new Exception("Url not set");

                HttpStatusCode? oldStatusCode = _lastStatusCode;
                HttpStatusCode status;
                try
                {
                    HttpWebRequest request = (HttpWebRequest) WebRequest.Create(Url);
                    using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
                    {
                        status = response.StatusCode;
                    }
                }
                catch (WebException webException)
                {
                    if (webException.Response != null && webException.Response is HttpWebResponse response)
                    {
                        status = response.StatusCode;
                    }
                    else throw;
                }

                bool notify = false;

                if (!_lastStatusCode.HasValue || status != _lastStatusCode.Value)
                {
                    _lastStatusCode = status;
                    if (AnyStatus)
                        notify = true;
                    else if ((int) status == StatusCode)
                    {
                        notify = true;
                    }
                }

                if (!notify) return WatcherResult.NotFound;
                _lastStatusCode = status;
                string resultMessage = oldStatusCode.HasValue
                    ? $"Response from Url {Url} has changed from {oldStatusCode} ({(int) oldStatusCode}) to {status} ({(int) status})"
                    : $"Response from Url {Url} is {status} ({(int) status})";
                return WatcherResult.Succeed(ArgumentCollection.New()
                        .WithArgument(UrlStatusWatcherResultArgs.Url, this.Url)
                        .WithArgument(UrlStatusWatcherResultArgs.StatusCodeName, status.ToString("G")))
                    .WithArgument(UrlStatusWatcherResultArgs.StatusCodeValue, (int) status)
                    .WithArgument(UrlStatusWatcherResultArgs.ResultMessage, resultMessage);
            }
            catch (Exception exception)
            {
                LoggingService.Error(exception);
                return WatcherResult.NotFound;
            }
        }
    }
}
