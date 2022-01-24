// using Bot.Modules;
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
      await InitCommands();
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

     private async Task InitCommands()
    {
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        _client.MessageReceived += HandleCommandAsync;
    }

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        // Don't process the command if it was a system message
        var message = messageParam as SocketUserMessage;
        if (message == null) return;

        // Create a number to track where the prefix ends and the command begins
        int argPos = 0;

        // Determine if the message is a command based on the prefix and make sure no bots trigger commands
        if (!(message.HasCharPrefix('!', ref argPos) || 
            message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
            message.Author.IsBot)
            return;

        // Create a WebSocket-based command context based on the message
        var context = new SocketCommandContext(_client, message);

        // Execute the command with the command context we just
        // created, along with the service provider for precondition checks.
        await _commands.ExecuteAsync(
            context: context, 
            argPos: argPos,
            _services);
    }
  }
}
