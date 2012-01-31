using System;
using System.Collections.ObjectModel;
using System.Messaging;
using System.Threading;
using Caliburn.Micro;
using MSMQCommander.Contex;
using MSMQCommander.Dialogs;
using MSMQCommander.Events;
using MSMQCommander.Utils;
using MSMQCommander.Views;
using System.Linq;

namespace MSMQCommander 
{
    public class ShellViewModel : 
        PropertyChangedBase, 
        IShell,
        IHandle<QueueSelectedEvent>,
        IHandle<QueueClosedEvent>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly CurrentSelectedQueueContext _currentSelectedQueueContext;
        private readonly IDialogService _dialogService;
        private readonly ObservableCollection<DetailsView> _detailsViews = new ObservableCollection<DetailsView>();
        private bool _autoRefreshEnabled;
        private Timer _autoRefreshTimer;

        public ShellViewModel(IEventAggregator eventAggregator, CurrentSelectedQueueContext currentSelectedQueueContext, IDialogService dialogService)
        {
            _eventAggregator = eventAggregator;
            _currentSelectedQueueContext = currentSelectedQueueContext;
            _dialogService = dialogService;
            _eventAggregator.Subscribe(this);
        }

        public ObservableCollection<DetailsView> DetailsViews
        {
            get { return _detailsViews; }
        }

        public void Handle(QueueSelectedEvent queueSelectedEvent)
        {
            _currentSelectedQueueContext.CurrentSelectedMessageQueue = queueSelectedEvent.MessageQueue;
            var existingViewForQueue = GetExistingViewForQueue(queueSelectedEvent.MessageQueue);
            if (existingViewForQueue != null)
            {
                existingViewForQueue.Activate();
            }
            else
            {
                var newDetailsView = new DetailsView();
                newDetailsView.CloseAction = OnViewClosed;
                DetailsViews.Add(newDetailsView);
                NotifyOfPropertyChange(() => DetailsViews);
                newDetailsView.Activate();
            }
        }

        private void OnViewClosed(DetailsView view)
        {
            DetailsViews.Remove(view);
        }

        private DetailsView GetExistingViewForQueue(MessageQueue queue)
        {
            var existingViewForQueue = DetailsViews.SingleOrDefault(d => d.Equals(queue));
            return existingViewForQueue;
        }

        public void RefreshQueues()
        {
            _eventAggregator.Publish(new RefreshQueuesEvent());
        }

        public void ToggleAutoRefresh()
        {
            _autoRefreshEnabled = !_autoRefreshEnabled;

            if (_autoRefreshTimer != null)
            {
                _autoRefreshTimer.Dispose();
                _autoRefreshTimer = null;
            }
            if (_autoRefreshEnabled)
            {
                _autoRefreshTimer = new Timer(ToggleAutoRefreshCallBackHandler, null, new TimeSpan(), new TimeSpan(0,0,0,3));
            }
        }

        private void ToggleAutoRefreshCallBackHandler(object state)
        {
            _eventAggregator.Publish(new AutoRefreshEvent());
        }

        public void ConnectToComputer()
        {
            _dialogService.ConnectToComputer();
        }

        public void Handle(QueueClosedEvent queueClosedEvent)
        {
            var existingViewForQueue = GetExistingViewForQueue(queueClosedEvent.MessageQueue);
            if (existingViewForQueue == null)
                return;
            existingViewForQueue.Close();
            DetailsViews.Remove(existingViewForQueue);
        }

        public string Title
        {
            get
            {
                return string.Format("MSMQ Commander (version {0}, {1})", 
                    VersionInformation.GetMajorAndMinorVersion(), 
                    VersionInformation.GetBuildDate().ToShortDateString());
            }
        }
    }
}
