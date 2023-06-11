namespace ooadproject.Models
{
    public class NotificationBuilder : ANotificationBuilder
    {
        private Notification notification = new Notification();

        public NotificationBuilder() { }

        public override NotificationBuilder setMessage(string message)
        {
            notification.Message = message;
            return this;
        }

        public override NotificationBuilder setRecipient(Person recipient)
        {
            notification.Recipient = recipient;
            notification.RecipientID = recipient.Id;
            return this; 
        }

        public override NotificationBuilder setTitle(string title)
        {
            notification.Title = title;
            return this; 
        }

        public override Notification build() => notification;
    }
}
