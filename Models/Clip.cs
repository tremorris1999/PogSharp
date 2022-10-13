namespace PogSharp.Models;

public class Clip 
{
  public string Id { get; set; } = "";
  public string Title { get; set; } = "";
  public string Broadcaster { get; set; } = "";
  public int Duration { get; set; }
  public string ThumnailUri { get; set; } = "";
  public string VideoUri { get; set; } = "";
}