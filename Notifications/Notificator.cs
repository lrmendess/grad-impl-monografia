using System.Collections.Generic;
using System.Linq;

namespace SCAP.Notifications
{
    public class Notificator : INotificator
    {
        private readonly List<Notification> _notifications;

        public Notificator()
        {
            _notifications = new List<Notification>();
        }

        public void Handle(Notification notificacao)
        {
            _notifications.Add(notificacao);
        }

        public IEnumerable<Notification> GetNotifications()
        {
            return _notifications;
        }

        public bool HasNotifications()
        {
            return _notifications.Any();
        }

        public IEnumerable<Notification> GetSuccesses()
        {
            return _notifications.Where(n => n.Type == NotificationType.SUCCESS);
        }

        public bool HasSuccesses()
        {
            return _notifications.Any(n => n.Type == NotificationType.SUCCESS);
        }

        public IEnumerable<Notification> GetWarnings()
        {
            return _notifications.Where(n => n.Type == NotificationType.WARNING);
        }

        public bool HasWarnings()
        {
            return _notifications.Any(n => n.Type == NotificationType.WARNING);
        }

        public IEnumerable<Notification> GetErrors()
        {
            return _notifications.Where(n => n.Type == NotificationType.ERROR);
        }

        public bool HasErrors()
        {
            return _notifications.Any(n => n.Type == NotificationType.ERROR);
        }
    }
}
