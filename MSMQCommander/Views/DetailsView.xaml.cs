using System.Messaging;
using MSMQCommander.ViewModels;

namespace MSMQCommander.Views
{
    public partial class DetailsView
    {
        public DetailsView()
        {
            InitializeComponent();
        }

        public bool Equals(MessageQueue queue)
        {
            return ((DetailsViewModel) DataContext).Equals(queue);
        }
    }
}