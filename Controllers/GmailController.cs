using System.Net;
using GoogleGmailAPI.Helpers;
using GoogleGmailAPI.Model;
using GoogleGmailAPI.OptionModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace GoogleGmailAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class GmailController : ControllerBase
{
    private readonly IOptions<GmailAPIOption> _gmailOption;

    public GmailController(IOptions<GmailAPIOption> gmailOption)
    {
        _gmailOption = gmailOption;
    }

    [HttpGet("Get")]
    public async Task<string> Get()
    {
        var accessToken = await GetTokenAsync();
        var client = new RestClient(_gmailOption.Value.GmailAPIUrl + "/users/me/messages");
        var request = new RestRequest() { Method = Method.Get };

        request.AddHeader("Authorization", "Bearer " + accessToken);

        var response = await client.ExecuteAsync(request);

        var json = JObject.Parse(response.Content);

        return json.ToString();
    }

    [HttpGet("Read")]
    public async Task<string> Read(string messageId)
    {
        var accessToken = await GetTokenAsync();
        var client = new RestClient(_gmailOption.Value.GmailAPIUrl + $"/users/me/messages/{messageId}");
        var request = new RestRequest() { Method = Method.Get };

        request.AddHeader("Authorization", "Bearer " + accessToken);

        var response = await client.ExecuteAsync(request);

        var json = JObject.Parse(response.Content);

        return json.ToString();
    }

    [HttpPost("Send")]
    public async Task<string> Send(MessageModel model)
    {
        var accessToken = await GetTokenAsync();
        var client = new RestClient(_gmailOption.Value.GmailAPIUrl + "/users/me/messages/send");
        var request = new RestRequest() { Method = Method.Post };
        var message = GmailMessageHelper.BuildMessage(model.Subject, _gmailOption.Value.SenderName, model.ReceiverName,
            _gmailOption.Value.From, model.SendTo, model.Content);
        
        request.AddHeader("Authorization", "Bearer " + accessToken);
        request.AddJsonBody(new
        {
            raw = message
        });

        var response = await client.ExecuteAsync(request);

        var json = JObject.Parse(response.Content);

        return json.ToString();
    }

    private async Task<string> GetTokenAsync()
    {
        var client = new RestClient(_gmailOption.Value.Oauth2 + "/token");
        var request = new RestRequest() { Method = Method.Post };

        request.AddParameter("client_id", _gmailOption.Value.ClientID);
        request.AddParameter("client_secret", _gmailOption.Value.ClientSecret);
        request.AddParameter("refresh_token", _gmailOption.Value.RefreshToken);
        request.AddParameter("grant_type", _gmailOption.Value.GrandType);

        var response = await client.ExecuteAsync(request);

        var jObject = JObject.Parse(response.Content ?? string.Empty);

        return jObject.ContainsKey("access_token") ? (string)jObject["access_token"]! : "";
    }
}