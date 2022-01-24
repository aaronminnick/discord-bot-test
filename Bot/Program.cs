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
      var map = new ServiceCollection();
        // .AddSingleton(new SomeServiceClass());

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

    private async Task HandleCommandAsync(SocketMessage arg)
    {
      // Bail out if it's a System Message.
        var msg = arg as SocketUserMessage;
        if (msg == null) return;

        // We don't want the bot to respond to itself or other bots.
        if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) return;
        
        // Create a number to track where the prefix ends and the command begins
        int pos = 0;
        // Replace the '!' with whatever character
        // you want to prefix your commands with.
        // Uncomment the second half if you also want
        // commands to be invoked by mentioning the bot instead.
        if (msg.HasCharPrefix('!', ref pos) /* || msg.HasMentionPrefix(_client.CurrentUser, ref pos) */)
        {
            // Create a Command Context.
            var context = new SocketCommandContext(_client, msg);
            
            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed successfully).
            var result = await _commands.ExecuteAsync(context, pos, _services);

            // Uncomment the following lines if you want the bot
            // to send a message if it failed.
            // This does not catch errors from commands with 'RunMode.Async',
            // subscribe a handler for '_commands.CommandExecuted' to see those.
            //if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
            //    await msg.Channel.SendMessageAsync(result.ErrorReason);
        }
    }
  }
}
