using System.Collections.Generic;

namespace SCAP.Notifications
{
    public interface INotificator
    {
        void Handle(Notification notification);
        bool HasNotifications();
        bool HasSuccesses();
        bool HasWarnings();
        bool HasErrors();
        IEnumerable<Notification> GetNotifications();
        IEnumerable<Notification> GetSuccesses();
        IEnumerable<Notification> GetWarnings();
        IEnumerable<Notification> GetErrors();
    }
}
