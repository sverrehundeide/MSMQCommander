using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using MsmqLib;

namespace MSMQCommander.ViewModels.Dialogs
{
    public class CreateNewMessageViewModel : Screen
    {
        private readonly IQueueService _queueService;

        public CreateNewMessageViewModel(IQueueService queueService)
        {
            _queueService = queueService;
        }

        public string Title
        {
            get { return "Create new message"; }
        }
    }
}
