using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using Routindo.Contract.Arguments;
using Routindo.Contract.UI;
using Routindo.Plugins.Web.Components.UrlWatcher;
using Routindo.Plugins.Web.UI.Models;

namespace Routindo.Plugins.Web.UI.ViewModels
{
    public class UrlStatusWatcherViewModel: PluginConfiguratorViewModelBase
    {
        private string _url; 
        private StatusCodeModel _watchStatus;
        private bool _anyStatus = true;

        public UrlStatusWatcherViewModel()
        {
            var statuses = Enum.GetValues<HttpStatusCode>().ToList().Select(s => new StatusCodeModel(s));
            this.Statuses = new ObservableCollection<StatusCodeModel>(statuses);
        }


        public string Url
        {
            get => _url;
            set 
            {
                _url = value;
                ClearPropertyErrors();
                ValidateNonNullOrEmptyString(Url);
                OnPropertyChanged();
            }
        }

        public StatusCodeModel WatchStatus
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

        public ObservableCollection<StatusCodeModel> Statuses { get; set; }

        public override void Configure()
        {
            this.InstanceArguments = ArgumentCollection.New()
                .WithArgument(UrlStatusWatcherArgs.Url, Url)
                .WithArgument(UrlStatusWatcherArgs.WatchAnyStatus, AnyStatus);
            if (WatchStatus != null)
                InstanceArguments.Add(UrlStatusWatcherArgs.WatchStatus, WatchStatus.StatusCodeValue);
        }

        public override void SetArguments(ArgumentCollection arguments)
        {
            if (arguments == null || !arguments.Any())
                return;

            if (arguments.HasArgument(UrlStatusWatcherArgs.Url))
                Url = arguments.GetValue<string>(UrlStatusWatcherArgs.Url);

            if (arguments.HasArgument(UrlStatusWatcherArgs.WatchAnyStatus))
                AnyStatus = arguments.GetValue<bool>(UrlStatusWatcherArgs.WatchAnyStatus);

            if (arguments.HasArgument(UrlStatusWatcherArgs.WatchStatus))
            {
                int statusId = arguments.GetValue<int>(UrlStatusWatcherArgs.WatchStatus);
                if (Enum.IsDefined(typeof(HttpStatusCode), statusId))
                {
                    WatchStatus = new StatusCodeModel((HttpStatusCode)statusId);
                }
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
            ClearPropertyErrors(nameof(Url));
            ValidateNonNullOrEmptyString(Url, nameof(Url));
            OnPropertyChanged(nameof(Url));

            // Watch Status
            ClearPropertyErrors(nameof(WatchStatus));
            ValidateWatchingStatus();
            OnPropertyChanged(nameof(WatchStatus));
        }
    }
}
