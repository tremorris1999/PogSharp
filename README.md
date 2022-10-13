# PogSharp
Lightweight .NET 6.0 library for interacting with the Twitch API.

## Overview
This was created to be a simple clip-downloading library for a personal project, but this *could* expand to include other functionality as needed.
Currently, PogSharp is only capable of downloading twitch clips by direct clip id, but [could be expanded](https://dev.twitch.tv/docs/api/clips) to search/discover clips if anyone wants to implement it.

## Quick Start
In order to make use of the library, you will need to [register an application with Twitch](https://dev.twitch.tv/docs/api/get-started) and provide the [Client](https://github.com/tremorris1999/PogSharp/blob/master/TwitchClient/Client.cs) constructor. This could be accomplished with a simple **`config.json`** file to avoid keeping credentials in source.

**`config.json`**:
```json
{
  "client": {
    "id": "<your_client_id>",
    "secret": "<your_client_secret>"
  }
}
```
Continue basic example using the [dotnet-cli](https://learn.microsoft.com/en-us/dotnet/core/tools/):

`dotnet new console --name Example`

`cd Example`

`dotnet add reference <path_to_PogSharp>/PogSharp.csproj`

In **`Program.cs`** place the following code:

```cs
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using PogSharp.TwitchClient;

namespace PogSharp.Tester;

public class Program
{
  public static void Main()
  {
    JsonNode configuration = JsonSerializer.Deserialize<JsonNode>(File.ReadAllText("<path_to_config>/config.json"))
      ?? throw new ApplicationException("Unable to load config file.");
    string clientId = configuration["client"]?["id"]?.GetValue<string>()
      ?? throw new ApplicationException("Unable to load client id.");
    string clientSecret = configuration["client"]?["secret"]?.GetValue<string>()
      ?? throw new ApplicationException("Unable to load client secret.");

    Client client = new(clientId, clientSecret);
    client.DownloadClip("https://clips.twitch.tv/CallousVenomousSoybeanRlyTho-JypQDQpxhf-XU2ly", "test.mp4");
  }
}
```

Running this should download the clip in the first parameter of the [DownloadClip]() call to whatever directory your code executed in.

## Contribution
Feel free to fork the repo and implement whatever features you'd like, I'd be more than happy to accept any well-put-together PR into the main repo.
