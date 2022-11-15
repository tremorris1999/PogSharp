using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace PogSharp.TwitchClient;

/// <summary>
/// Class representing the OAuth token used by Twitch's API.
/// </summary>
public class Token
{
  private const string TWITCH_BASE_URI = "https://id.twitch.tv/oauth2/token";
  /// <summary>
  /// Access token used in Http request headers.
  /// </summary>
  /// <value></value>
  public string AccessToken { get; }

  /// <summary>
  /// Creates a new token based on client id and secred from Twitch.
  /// </summary>
  /// <param name="clientId">Client ID generated by Twitch.</param>
  /// <param name="clientSecret">Client Secret generated by Twitch.</param>
  public Token(string clientId, string clientSecret)
  {
    HttpClient client = new();
    HttpRequestMessage request = new()
    {
      Method = HttpMethod.Post,
      RequestUri = new($"{TWITCH_BASE_URI}?client_id={clientId}&client_secret={clientSecret}&grant_type=client_credentials")
    };

    HttpResponseMessage response = Task.Run(async () => await client.SendAsync(request)).Result;
    if (!(response?.IsSuccessStatusCode ?? false))
      throw new ApplicationException($"Unable to fetch token: {response?.ReasonPhrase}");

    JsonNode? tokenData = JsonSerializer.Deserialize<JsonNode>(response.Content.ReadAsStream());
    AccessToken = tokenData?["access_token"]?.GetValue<string>()
      ?? throw new ApplicationException("Unable to deserialize/fetch token.");
  }
}