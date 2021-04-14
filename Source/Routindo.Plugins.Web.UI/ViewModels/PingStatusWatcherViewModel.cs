using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Routindo.Contract.Arguments;
using Routindo.Contract.UI;
using Routindo.Plugins.Web.Components.PingWatcher;
using Routindo.Plugins.Web.UI.Models;

namespace Routindo.Plugins.Web.UI.ViewModels
{
    public class PingStatusWatcherViewModel: PluginConfiguratorViewModelBase
    { 
        private string _host;   
        private PingStatusCodeModel _watchStatus;
        private bool _anyStatus = true;

        public PingStatusWatcherViewModel()
        {
            var statuses = new List<PingStatusCodeModel>()
            {
                new PingStatusCodeModel(true), new PingStatusCodeModel(false)
            };
            this.Statuses = new ObservableCollection<PingStatusCodeModel>(statuses);
        }


        public string Host
        { 
            get => _host;
            set 
            {
                _host = value;
                ClearPropertyErrors();
                ValidateNonNullOrEmptyString(Host);
                OnPropertyChanged();
            }
        }

        public PingStatusCodeModel WatchStatus
        {
            get => _watchStatus;
            set
            {
                _watchStatus = value;
                ClearPropertyErrors();
                ValidateWatchingStatus();
                OnPropertyChanged();
            }
        }

        public bool AnyStatus
        {
            get => _anyStatus;
            set
            {
                _anyStatus = value;
                OnPropertyChanged();
                WatchStatus = null;
            }
        }

        public ObservableCollection<PingStatusCodeModel> Statuses { get; set; }

        public override void Configure()
        {
            this.InstanceArguments = ArgumentCollection.New()
                .WithArgument(PingStatusWatcherArgs.Host, Host)
                .WithArgument(PingStatusWatcherArgs.WatchAnyStatus, AnyStatus);
            if (WatchStatus != null)
                InstanceArguments.Add(PingStatusWatcherArgs.WatchStatus, WatchStatus.StatusCodeValue);
        }

        public override void SetArguments(ArgumentCollection arguments)
        {
            if (arguments == null || !arguments.Any())
                return;

            if (arguments.HasArgument(PingStatusWatcherArgs.Host))
                Host = arguments.GetValue<string>(PingStatusWatcherArgs.Host);

            if (arguments.HasArgument(PingStatusWatcherArgs.WatchAnyStatus)) 
                AnyStatus = arguments.GetValue<bool>(PingStatusWatcherArgs.WatchAnyStatus);

            if (arguments.HasArgument(PingStatusWatcherArgs.WatchStatus))
            {
                var watchStatusValue = arguments.GetValue<bool>(PingStatusWatcherArgs.WatchStatus);
                WatchStatus = Statuses.SingleOrDefault(s => s.StatusCodeValue == watchStatusValue)?? new PingStatusCodeModel(watchStatusValue);
            }
        }

        private void ValidateWatchingStatus()
        { 
            if (WatchStatus == null && !AnyStatus)
            {
                AddPropertyError(nameof(WatchStatus), "If option 'Any Status' is not selected, It's mandatory to choose a status from the list");
            }
        }

        protected override void ValidateProperties()
        {
            base.ValidateProperties();

            // Url
            ClearPropertyErrors(nameof(Host));
            ValidateNonNullOrEmptyString(Host, nameof(Host));
            OnPropertyChanged(nameof(Host));

            // Watch Status
            ClearPropertyErrors(nameof(WatchStatus));
            ValidateWatchingStatus();
            OnPropertyChanged(nameof(WatchStatus));
        }
    }
}
