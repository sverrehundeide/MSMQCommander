namespace MSMQCommander.Utils
{
    public static class ViewContentsFormattingExtensions
    {
         public static string ToYesNo(this bool value)
         {
             return value ? "Yes" :"No";
         }
    }
}