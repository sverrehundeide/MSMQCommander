using Caliburn.Micro;

namespace MSMQCommander.ViewModels
{
    public class MessagesGridViewModel : PropertyChangedBase
    {
        public MessagesGridViewModel()
        {
            Messages = new BindableCollection<MessageGridRowViewModel>();
        }

        public BindableCollection<MessageGridRowViewModel> Messages { get; private set; }
    }
}