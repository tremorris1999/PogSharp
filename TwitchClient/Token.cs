using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace PogSharp.TwitchClient;

public class Token
{
  private const string TWITCH_BASE_URI = "https://id.twitch.tv/oauth2/token";
  public string AccessToken { get; }

  public Token(string clientId, string clientSecret)
  {
    HttpClient client = new();
    HttpRequestMessage request = new()
    {
      Method = HttpMethod.Post,
      RequestUri = new($"{TWITCH_BASE_URI}?client_id={clientId}&client_secret={clientSecret}&grant_type=client_credentials")
    };

    HttpResponseMessage response = Task.Run(async () => await client.SendAsync(request)).Result;
    if (!response.IsSuccessStatusCode)
      throw new ApplicationException($"Unable to fetch token: {response.ReasonPhrase}");

    JsonNode tokenData = JsonSerializer.Deserialize<JsonNode>(response.Content.ReadAsStream()) ?? throw new ApplicationException("Unable to deserialize response data."); 
    AccessToken = tokenData["access_token"]?.GetValue<string>() ?? throw new ApplicationException("Unable to fetch token.");
  }
}