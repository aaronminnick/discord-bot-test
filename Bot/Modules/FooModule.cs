using Discord.Commands;
using System.Threading.Tasks;

namespace Bot.Modules
{
  public class FooModule : ModuleBase<SocketCommandContext>
  {
    // ~say hello world -> hello world
    [Command("say")]
    [Summary("Echoes a message.")]
    public async Task SayAsync([Remainder] [Summary("The text to echo")] string echo)
    {
      await ReplyAsync(echo);
    }
      
    // ReplyAsync is a method on ModuleBase 
  }
}