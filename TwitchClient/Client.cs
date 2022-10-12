using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PogSharp.TwitchClient;

public class Client
{
  private readonly HttpClient _httpClient;
  private readonly string _clientId;
  private readonly string _clientSecret;
  private Token _token;
  public Token Token { get => _token; }

  public Client(string clientId, string clientSecret)
  {
    _clientId = clientId;
    _clientSecret = clientSecret;
    _token = new(_clientId, _clientSecret);
    _httpClient = new();
    _httpClient.DefaultRequestHeaders.Add("Client-Id", _clientId);
    _httpClient.DefaultRequestHeaders.Accept.Add(new("application/json"));
  }

  public void GetClip(string url)
  {
    string downloadUri = GetClipDownloadUri(url);
  }

  private string GetClipDownloadUri(string url)
  {
    Regex idGrabber = new(@"(?<=https:\/\/clips.twitch.tv\/).*");
    string id = idGrabber.Match(url).Value;
    HttpRequestMessage request = new()
    {
      Method = HttpMethod.Get,
      RequestUri = new($"https://api.twitch.tv/helix/clips?id={id}")
    };

    request.Headers.Authorization = new("Bearer", _token.AccessToken);
    HttpResponseMessage response = Task.Run(async () => await _httpClient.SendAsync(request)).Result;
    if(response.StatusCode is System.Net.HttpStatusCode.Unauthorized)
    {
      RefreshToken();
      request = new()
      {
        Method = HttpMethod.Get,
        RequestUri = new($"https://api.twitch.tv/helix/clips?id={id}")
      };

      request.Headers.Authorization = new("Bearer", _token.AccessToken);
      response = Task.Run(async () => await _httpClient.SendAsync(request)).Result;
    }

    if(!response.IsSuccessStatusCode)
      throw new ApplicationException("Unable to fetch clip.");

    JsonNode? responseData = JsonSerializer.Deserialize<JsonNode>(response.Content.ReadAsStream());
    string? downloadUri = responseData?["data"]?[0]?["thumbnail_url"]?
      .GetValue<string>()?
      .Replace("-preview-480x272.jpg", ".mp4");

      return downloadUri ?? throw new ApplicationException("Unable to deserialize clip object.");
  }

  private void RefreshToken()
  {
    _token = new(_clientId, _clientSecret);
  }
}