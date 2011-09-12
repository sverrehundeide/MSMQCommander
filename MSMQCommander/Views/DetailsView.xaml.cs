using System;
using System.Messaging;
using MSMQCommander.ViewModels;

namespace MSMQCommander.Views
{
    public partial class DetailsView
    {
        public DetailsView()
        {
            InitializeComponent();
            CloseAction = null;
        }

        public bool Equals(MessageQueue queue)
        {
            return ((DetailsViewModel) DataContext).Equals(queue);
        }

        public Action<DetailsView> CloseAction { get; set; }

        public override bool Close()
        {
            if (CloseAction == null)
                throw new InvalidOperationException("CloseAction must be set");

            CloseAction(this);
            return true;
        }
    }
}