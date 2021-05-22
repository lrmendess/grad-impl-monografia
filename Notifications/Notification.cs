namespace SCAP.Notifications
{
    public class Notification
    {
        public NotificationType Type { get; }
        public string Key { get; }
        public string Message { get; }

        public Notification(NotificationType type, string key = "", string message = "")
        {
            Type = type;
            Key = key;
            Message = message;
        }
    }
}
