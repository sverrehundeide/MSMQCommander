using Caliburn.Micro;
using MSMQCommander.Contex;
using MSMQCommander.Dialogs;
using MsmqLib;

namespace MSMQCommander.ViewModels.Dialogs
{
    public class ConfigureConnectionViewModel : Screen
    {
        private readonly QueueConnectionContext _queueConnectionContext;
        private readonly IQueueService _queueService;
        private readonly IDialogService _dialogService;
        private string _computerName;
        public string ComputerName
        {
            get { return _computerName; }
            set
            {
                _computerName = value;
                NotifyOfPropertyChange(() => CanOk);
            }
        }

        public ConfigureConnectionViewModel(QueueConnectionContext queueConnectionContext, IQueueService queueService, IDialogService dialogService)
        {
            _queueConnectionContext = queueConnectionContext;
            _queueService = queueService;
            _dialogService = dialogService;
            ComputerName = _queueConnectionContext.ComputerName;
        }

        public string Title
        {
            get { return "Enter host name or IP address for connection"; }
        }

        public bool CanOk
        {
            get {return !string.IsNullOrWhiteSpace(ComputerName);}
        }

        public void Ok()
        {
            string errorMessage;
            if(false == _queueService.TryConnect(ComputerName, out errorMessage))
            {
                _dialogService.ShowError("Failed to connect to '{0}': {1}", ComputerName, errorMessage);
                return;
            }

            TryClose(true);
        }
    }
}
