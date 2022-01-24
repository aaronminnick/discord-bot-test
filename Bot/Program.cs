using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Bot
{
  public class Program
  {
    private DiscordSocketClient _client;

    public static Task Main(string[] args) => new Program().MainAsync();

    public async Task MainAsync()
    {
      _client = new DiscordSocketClient();
      _client.Log += Log;

      //get the token from environment variables
      var token = EnvironmentVariables.Token;

      await _client.LoginAsync(TokenType.Bot, token);
      await _client.StartAsync();

      //block task - this keeps bot connected indefinitely until program closes
      await Task.Delay(-1);
    }

    private Task Log(LogMessage msg)
    {
      Console.WriteLine(msg.ToString());
      return Task.CompletedTask;
    }
  }
}
