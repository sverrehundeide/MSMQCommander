using Caliburn.Micro;

namespace MSMQCommander.ViewModels
{
    public class QueueTypeTreeNodeViewModel : PropertyChangedBase
    {
        public string Name { get; set; }

        public QueueTypeTreeNodeViewModel(string queueTypeName)
        {
            Name = queueTypeName;
        }
    }
}