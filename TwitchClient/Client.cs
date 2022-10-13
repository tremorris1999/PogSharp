using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using PogSharp.Models;
using PogSharp.Utilities;

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

  public void DownloadClip(string url, string filePath)
  {
    Clip clip = GetClip(url);
    Stream stream = Task.Run(async ()=> await _httpClient.GetStreamAsync(clip.VideoUri)).Result;
    using FileStream fileStream = File.OpenWrite(filePath);
    Task.Run(async() => await stream.CopyToAsync(fileStream)).Wait();
  }

  public async Task DownloadClipAsync(string url, string filePath)
  {
    Clip clip = await GetClipAsync(url);
    Stream stream = await _httpClient.GetStreamAsync(clip.VideoUri);
    using FileStream fileStream = File.OpenWrite(filePath);
    await stream.CopyToAsync(fileStream);
  }

  public void DownloadClip(Clip clip, string filePath)
  {
    Stream stream = Task.Run(async ()=> await _httpClient.GetStreamAsync(clip.VideoUri)).Result;
    using FileStream fileStream = File.OpenWrite(filePath);
    Task.Run(async() => await stream.CopyToAsync(fileStream)).Wait();
  }
  
  public async Task DownloadClipAsync(Clip clip, string filePath)
  {
    Stream stream = await _httpClient.GetStreamAsync(clip.VideoUri);
    using FileStream fileStream = File.OpenWrite(filePath);
    await stream.CopyToAsync(fileStream);
  }

  public Clip GetClip(string url)
  {
    JsonObject? clipData = GetClipData(url);
    return new()
    {
      Id = clipData.GetIndexValue<string>("id"),
      Title = clipData.GetIndexValue<string>("title"),
      Broadcaster = clipData.GetIndexValue<string>("broadcaster_name"),
      Duration = clipData.GetIndexValue<int>("duration"),
      ThumnailUri = clipData.GetIndexValue<string>("thumbnail_url"),
      VideoUri = clipData.GetIndexValue<string>("thumbnail_url")
        .Replace("-preview-480x272.jpg", ".mp4")
    };
  }

  public async Task<Clip> GetClipAsync(string url)
  {
    JsonObject? clipData = await GetClipDataAsync(url);
    return new()
    {
      Id = clipData.GetIndexValue<string>("id"),
      Title = clipData.GetIndexValue<string>("title"),
      Broadcaster = clipData.GetIndexValue<string>("broadcaster_name"),
      Duration = clipData.GetIndexValue<int>("duration"),
      ThumnailUri = clipData.GetIndexValue<string>("thumbnail_url"),
      VideoUri = clipData.GetIndexValue<string>("thumbnail_url")
        .Replace("-preview-480x272.jpg", ".mp4")
    };
  }

  private JsonObject? GetClipData(string url)
  {
    string id = GetClipId(url);
    HttpResponseMessage response = Task.Run(async () => await RequestClipAsync(id)).Result;
    if(response.StatusCode is System.Net.HttpStatusCode.Unauthorized)
    {
      RefreshToken();
      response = Task.Run(async () => await RequestClipAsync(id)).Result;
    }

    if(!response.IsSuccessStatusCode)
      throw new ApplicationException("Unable to fetch clip.");

    JsonNode? responseData = JsonSerializer.Deserialize<JsonNode>(response.Content.ReadAsStream());
    return responseData?["data"]?[0]?.AsObject();
  }

  private async Task<JsonObject?> GetClipDataAsync(string url)
  {
    string id = GetClipId(url);
    HttpResponseMessage response = await RequestClipAsync(id);
    if(response.StatusCode is System.Net.HttpStatusCode.Unauthorized)
    {
      RefreshToken();
      response = await RequestClipAsync(id);
    }

    if(!response.IsSuccessStatusCode)
      throw new ApplicationException("Unable to fetch clip.");

    JsonNode? responseData = JsonSerializer.Deserialize<JsonNode>(response.Content.ReadAsStream());
    return responseData?["data"]?[0]?.AsObject();
  }

  private static string GetClipId(string url)
  {
    Regex idGrabber = new(@"(?<=https:\/\/clips.twitch.tv\/).*");
    return idGrabber.Match(url).Value;
  }

  private async Task<HttpResponseMessage> RequestClipAsync(string id)
  {
    HttpRequestMessage request = new()
    {
      Method = HttpMethod.Get,
      RequestUri = new($"https://api.twitch.tv/helix/clips?id={id}")
    };

    request.Headers.Authorization = new("Bearer", _token.AccessToken);
    return await _httpClient.SendAsync(request);
  }

  private void RefreshToken()
  {
    _token = new(_clientId, _clientSecret);
  }
}