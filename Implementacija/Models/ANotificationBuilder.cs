namespace ooadproject.Models
{
    public abstract class ANotificationBuilder
    {
        public abstract ANotificationBuilder setRecipient(Person recipient);
        public abstract ANotificationBuilder setTitle(string title);
        public abstract ANotificationBuilder setMessage(string message);
        public abstract Notification build();
    }
}
