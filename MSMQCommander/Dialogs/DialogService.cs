using System.Windows;

namespace MSMQCommander.Dialogs
{
    public interface IDialogService
    {
        MessageBoxResult AskQuestion(string question, string caption, MessageBoxButton button);
    }

    public class DialogService : IDialogService
    {
        public MessageBoxResult AskQuestion(string question, string caption, MessageBoxButton button)
        {
            return MessageBox.Show(question, caption, button, MessageBoxImage.Question);
        }
    }
}