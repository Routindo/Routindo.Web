using System.Net.NetworkInformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Routindo.Contract;
using Routindo.Contract.Services;
using Routindo.Contract.Watchers;
using Routindo.Plugins.Web.Components.PingWatcher;
using Routindo.Plugins.Web.Tests.Mock;

namespace Routindo.Plugins.Web.Tests
{
    [TestClass]
    public class PingStatusWatcherTests
    {
        private IWatcher GetPingWatcher(bool anyStatus, string targetHost, bool targetStatus = true)
        {
            return anyStatus
                ? new PingStatusWatcher()
                {
                    Id = PluginUtilities.GetUniqueId(),
                    AnyStatus = anyStatus,
                    Host = targetHost
                }
                : new PingStatusWatcher()
                {
                    Id = PluginUtilities.GetUniqueId(),
                    AnyStatus = anyStatus,
                    Host = targetHost,
                    Reachable = targetStatus
                };
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public void PingUnreachableFakeHostTest() 
        {
            const bool expectedStatus = false;
            const string targetHost = "fakehost";
            const bool anyStatus = true;

            IWatcher pingWatcher = GetPingWatcher(anyStatus, targetHost);

            pingWatcher.LoggingService =
                ServicesContainer.ServicesProvider.GetLoggingService(nameof(PingStatusWatcher));

            var watcherResult = pingWatcher.Watch();
            Assert.IsNotNull(watcherResult);
            Assert.IsTrue(watcherResult.Result);
            Assert.IsTrue(watcherResult.WatchingArguments.HasArgument(PingStatusWatcherResultArgs.IsReachable));
            Assert.IsInstanceOfType(watcherResult.WatchingArguments[PingStatusWatcherResultArgs.IsReachable], typeof(bool));
            var status = watcherResult.WatchingArguments.GetValue<bool>(PingStatusWatcherResultArgs.IsReachable);
            Assert.AreEqual(expectedStatus, status);
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public void PingReachableHostTest() 
        {
            const bool expectedStatus = true;
            const string targetHost = "google.com";
            const bool anyStatus = true;

            IWatcher pingWatcher = GetPingWatcher(anyStatus, targetHost);

            pingWatcher.LoggingService =
                ServicesContainer.ServicesProvider.GetLoggingService(nameof(PingStatusWatcher));

            var watcherResult = pingWatcher.Watch();
            Assert.IsNotNull(watcherResult);
            Assert.IsTrue(watcherResult.Result);
            Assert.IsTrue(watcherResult.WatchingArguments.HasArgument(PingStatusWatcherResultArgs.IsReachable));
            Assert.IsInstanceOfType(watcherResult.WatchingArguments[PingStatusWatcherResultArgs.IsReachable], typeof(bool));
            var status = watcherResult.WatchingArguments.GetValue<bool>(PingStatusWatcherResultArgs.IsReachable);
            Assert.AreEqual(expectedStatus, status);
        }
    }
}
