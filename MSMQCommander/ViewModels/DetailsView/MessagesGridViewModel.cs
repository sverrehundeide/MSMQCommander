using System;
using System.Collections.Specialized;
using System.Linq;
using System.Messaging;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using MSMQCommander.Contex;
using MSMQCommander.Dialogs;
using MSMQCommander.Events;
using MsmqLib;

namespace MSMQCommander.ViewModels
{
    public class MessagesGridViewModel : PropertyChangedBase, IHandle<RefreshQueuesEvent>
    {
        private readonly IQueueService _queueService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogService _dialogService;
        private readonly MessageQueue _messageQueue;
        private MessageGridRowViewModel _lastSelectedItem;

        public BindableCollection<MessageGridRowViewModel> Messages { get; private set; }

        public MessagesGridViewModel(IQueueService queueService, IEventAggregator eventAggregator,
            CurrentSelectedQueueContext selectedQueueContext, IDialogService dialogService)
        {
            _queueService = queueService;
            _eventAggregator = eventAggregator;
            _dialogService = dialogService;
            _messageQueue = selectedQueueContext.CurrentSelectedMessageQueue;
            Messages = new BindableCollection<MessageGridRowViewModel>();
            RefreshMessages();
            _eventAggregator.Subscribe(this);

            Messages.CollectionChanged += MessagesOnCollectionChanged;
        }

        private void RefreshMessages()
        {
            var messageInfos = _queueService.GetMessageInfos(_messageQueue);
            Messages.Clear();
            Messages.AddRange(messageInfos.Select(info => new MessageGridRowViewModel(info)));
        }

        public MessageGridRowViewModel SelectedItem
        {
            set
            {
                _lastSelectedItem = value;
                NotifyOfPropertyChange(() => IsExportMessageBodyEnabled);
                NotifyOfPropertyChange(() => IsDeleteMessageEnabled);

                if (value == null || !Messages.Any())
                    return;

                _eventAggregator.Publish(new MessageSelectedEvent(_messageQueue, value.Id));
            }
        }

        public void Handle(RefreshQueuesEvent message)
        {
            RefreshMessages();
        }

        public bool IsExportMessageBodyEnabled
        {
            get { return _lastSelectedItem != null; }
        }

        public void ExportMessageBody()
        {
            _dialogService.ExportMessageBody(_messageQueue, _lastSelectedItem.Id);
        }

        public void ImportMessageBody()
        {
            if (_dialogService.ImportMessageBody(_messageQueue))
            {
                _eventAggregator.Publish(new RefreshQueuesEvent());
            }
        }

        public void CreateNewMessage()
        {
            if (_dialogService.CreateNewMessage(_messageQueue))
            {
                _eventAggregator.Publish(new RefreshQueuesEvent());
            }
        }

        public bool IsDeleteMessageEnabled
        {
            get { return _lastSelectedItem != null; }
        }

        public void DeleteMessage()
        {
            var messageId = _lastSelectedItem.Id;
            if (_dialogService.DeleteMessage(_messageQueue, messageId))
            {
                _eventAggregator.Publish(new MessageDeletedEvent(messageId));
                _eventAggregator.Publish(new RefreshQueuesEvent());
            }
        }

        public void HandlePreviewKeyDown(KeyEventArgs args)
        {
            if (args.Key == Key.Delete)
            {
                if (_dialogService.AskQuestion("Delete selected row(s)?", "Delete", MessageBoxButton.YesNoCancel) != MessageBoxResult.Yes)
                {
                    args.Handled = true;
                }
            }
        }

        private void MessagesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (MessageGridRowViewModel deletedItem in notifyCollectionChangedEventArgs.OldItems)
                {
                    string errorMessage;
                    if (false == _queueService.DeleteMessage(_messageQueue, deletedItem.Id, out errorMessage))
                    {
                        _dialogService.ShowError(errorMessage);
                    }
                    if (_lastSelectedItem.Id == deletedItem.Id)
                    {
                        _eventAggregator.Publish(new MessageDeletedEvent(deletedItem.Id));
                    }
                }
                _eventAggregator.Publish(new QueueMessageCountChangedEvent(_messageQueue));
            }
        }
    }
}