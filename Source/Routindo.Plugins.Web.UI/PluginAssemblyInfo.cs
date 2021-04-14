using Routindo.Contract.Attributes;
using Routindo.Plugins.Web.Components.PingWatcher;
using Routindo.Plugins.Web.Components.UrlWatcher;
using Routindo.Plugins.Web.UI.Views;

[assembly: ComponentConfigurator(typeof(UrlStatusWatcherView), UrlStatusWatcher.ComponentUniqueId, "Configurator for URL Status Watcher")]
[assembly: ComponentConfigurator(typeof(PingStatusWatcherView), PingStatusWatcher.ComponentUniqueId, "Configurator for Ping Status Watcher")]