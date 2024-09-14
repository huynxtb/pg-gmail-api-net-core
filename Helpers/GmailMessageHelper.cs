using System.Text;
using System.Text.RegularExpressions;

namespace GoogleGmailAPI.Helpers;

public static class GmailMessageHelper
{
    public static string BuildMessage(string subject, string senderName, string receiverName, string from, string to, string htmlContent)
    {
        // First, base64 encode the HTML content
        var base64HtmlContent = Convert.ToBase64String(Encoding.UTF8.GetBytes(htmlContent));
        var inputBytes = Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(subject)));
        var base64Subject = Convert.ToBase64String(inputBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .Replace("=", "");

        // Then, create the MIME message
        var mimeMessage = string.Join("\n",
            new string[]
            {
                "MIME-Version: 1.0", "Content-type: text/html; charset=utf-8", "Content-Transfer-Encoding: base64",
                $"From: {senderName} <{from}>", $"To: {receiverName} <{to}>",
                $"Subject: =?UTF-8?B?{base64Subject}?=", "", base64HtmlContent
            });

        // Finally, base64url encode the entire MIME message
        var base64MimeMessage = Convert.ToBase64String(Encoding.UTF8.GetBytes(mimeMessage));
        var base64UrlMimeMessage = Regex.Replace(base64MimeMessage, @"\+", "-");
        base64UrlMimeMessage = Regex.Replace(base64UrlMimeMessage, @"\/", "_");
        base64UrlMimeMessage = Regex.Replace(base64UrlMimeMessage, @"=+$", "");

        return base64UrlMimeMessage;
    }
}