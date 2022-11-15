namespace PogSharp.Models;

public class Clip 
{
  public string Id { get; set; } = "";
  public string Title { get; set; } = "";
  public string Broadcaster { get; set; } = "";
  public int Duration { get; set; } = 0;
  public string ThumbnailUri { get; set; } = "";
  public string VideoUri { get; set; } = "";
}