using Bot.Modules;
using Bot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
// using System.Threading;
using System.Threading.Tasks;

namespace Bot
{
  public class Program
  {
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly IServiceProvider _services;

    public static Task Main(string[] args) => new Program().MainAsync();

    public Program()
    {
      _client = new DiscordSocketClient(); //optionally, may configure
      _commands = new CommandService(); //optionally, may configure

      _client.Log += Log;
      _commands.Log += Log;

      _services = ConfigureServices();
    }

    private static IServiceProvider ConfigureServices()
    {
      var map = new ServiceCollection()
        .AddSingleton<CommandHandler>();

      return map.BuildServiceProvider();
    }

    private static Task Log(LogMessage msg)
    {
      Console.WriteLine(msg.ToString());
      return Task.CompletedTask;
    }

    public async Task MainAsync()
    {
      await _client.LoginAsync(TokenType.Bot, EnvironmentVariables.Token);
      await _client.StartAsync();

      _client.Ready += () => 
      {
        Console.WriteLine("Bot is connected!");
        return Task.CompletedTask;
      };

      //block task - this keeps bot connected indefinitely until program closes
      await Task.Delay(-1);
    }
  }
}
