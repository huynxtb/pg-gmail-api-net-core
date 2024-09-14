namespace GoogleGmailAPI.Model;

public class MessageResponseListModel
{
    public List<MessageModel> Messages { get; set; }
    public int ResultSizeEstimate { get; set; }
}

