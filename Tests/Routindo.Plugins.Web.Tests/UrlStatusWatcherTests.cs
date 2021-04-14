using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Routindo.Contract;
using Routindo.Contract.Services;
using Routindo.Plugins.Web.Components.UrlWatcher;

namespace Routindo.Plugins.Web.Tests
{
    [TestClass]
    public class UrlStatusWatcherTests
    {
        [TestMethod]
        [TestCategory("Integration Test")]
        public void CheckStatusOfAvailableUrl()
        {
            var watcher = new UrlStatusWatcher()
            {
                AnyStatus = true,
                Url = "http://routindo.com/contact",
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(nameof(UrlStatusWatcher))
            };

            var watcherResult = watcher.Watch();
            Assert.IsNotNull(watcherResult);
            Assert.IsTrue(watcherResult.Result);
            Assert.IsTrue(watcherResult.WatchingArguments.HasArgument(UrlStatusWatcherResultArgs.StatusCodeName));
            Assert.IsInstanceOfType(watcherResult.WatchingArguments[UrlStatusWatcherResultArgs.StatusCodeName], typeof(string));
            Assert.IsTrue(watcherResult.WatchingArguments.HasArgument(UrlStatusWatcherResultArgs.StatusCodeValue));
            Assert.IsInstanceOfType(watcherResult.WatchingArguments[UrlStatusWatcherResultArgs.StatusCodeValue], typeof(int));
            var statusCodeValue = watcherResult.WatchingArguments.GetValue<int>(UrlStatusWatcherResultArgs.StatusCodeValue);
            var statusCode = (HttpStatusCode)statusCodeValue;
            Assert.AreEqual(HttpStatusCode.OK, statusCode);
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public void CheckStatusOfUnavailableUrl()
        { 
            var watcher = new UrlStatusWatcher()
            {
                AnyStatus = true,
                Url = "http://routindo.com/error404notfound",
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(nameof(UrlStatusWatcher))
            };

            var watcherResult = watcher.Watch();
            Assert.IsNotNull(watcherResult);
            Assert.IsTrue(watcherResult.Result);
            Assert.IsTrue(watcherResult.WatchingArguments.HasArgument(UrlStatusWatcherResultArgs.StatusCodeName));
            Assert.IsInstanceOfType(watcherResult.WatchingArguments[UrlStatusWatcherResultArgs.StatusCodeName], typeof(string));
            Assert.IsTrue(watcherResult.WatchingArguments.HasArgument(UrlStatusWatcherResultArgs.StatusCodeValue));
            Assert.IsInstanceOfType(watcherResult.WatchingArguments[UrlStatusWatcherResultArgs.StatusCodeValue], typeof(int));
            var statusCodeValue = watcherResult.WatchingArguments.GetValue<int>(UrlStatusWatcherResultArgs.StatusCodeValue);
            var statusCode = (HttpStatusCode)statusCodeValue;
            Assert.AreEqual(HttpStatusCode.NotFound, statusCode);
        }
    }
}
