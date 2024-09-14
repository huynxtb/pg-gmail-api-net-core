namespace GoogleGmailAPI.Model;

public class MessageModel
{
    public string Id { get; set; }
    public string ThreadId { get; set; }
    public string Subject { get; set; }
    public string ReceiverName { get; set; }
    public string SendTo { get; set; }
    public string Content { get; set; }
}