using Discord.WebSocket;
using ikanokaiwa_sharp.Config;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ikanokaiwa_sharp {
    class Program {
        public static async Task Main(string[] args) {
            var secret = SecretConfig.PATH.FromJsonFile(() => new SecretConfig());

            var discords =
                Enumerable.Range(0, 3)
                          .Select(_ => new DiscordSocketClient())
                          .ToArray();
            await discords[0].LoginAsync(Discord.TokenType.Bot, secret.Secret0);
            await discords[1].LoginAsync(Discord.TokenType.Bot, secret.Secret1);
            await discords[2].LoginAsync(Discord.TokenType.Bot, secret.Secret2);
            foreach (var d in discords) {
                await d.StartAsync();
                await d.SetGameAsync("ボイスチャット", null, Discord.ActivityType.Listening);
            }

            Console.WriteLine("press any key to exit...");
            Console.ReadKey();

            secret.ToJsonFile(SecretConfig.PATH);
        }
    }
}
