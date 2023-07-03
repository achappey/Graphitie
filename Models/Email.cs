namespace Graphitie.Models;

public class Email
{

    public string? Html { get; set; }

}

public class Mail
{

    public string Subject { get; set; } = null!;
    public Recipient From { get; set; }  = null!;
    public IEnumerable<Recipient> ToRecipients { get; set; } = null!;
    public IEnumerable<Recipient>? CcRecipients { get; set; }    
    public IEnumerable<Recipient>? BccRecipients { get; set; }
    public ItemBody Body { get; set; } = null!;
}


public class Recipient
{

    public EmailAddress? EmailAddress { get; set; }

}