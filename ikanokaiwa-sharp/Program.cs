using Discord.WebSocket;
using ikanokaiwa_sharp.Config;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ikanokaiwa_sharp {
    class Program {
        private static SecretConfig secret;
        private static string[] tokens;
        private static ulong[] channels;

        public static async Task Main(string[] args) {
            secret = SecretConfig.PATH.FromJsonFile(() => new SecretConfig());
            tokens = new string[] {
                secret.Secret0,
                secret.Secret1,
                secret.Secret2,
            };
            channels = new ulong[] {
                secret.VoiceChannelId0,
                secret.VoiceChannelId1,
                secret.VoiceChannelId2,
            };

            var discords =
                Enumerable.Range(0, 3)
                          .Select(_ => new DiscordSocketClient())
                          .ToArray();
            foreach (var (d, index) in discords.Select((d, i) => (d, i))) {
                await d.LoginAsync(Discord.TokenType.Bot, tokens[index]);
                await d.StartAsync();
                await d.SetGameAsync("ボイスチャット", null, Discord.ActivityType.Listening);
                d.Log += d_Log;
                d.MessageReceived += discord_MessageReceived;
                d.GuildAvailable += discord_GuildAvailable;
                // TODO: もっとまともな実装があるはず
                Console.WriteLine($"[{DateTime.Now}] wait for Guild {secret.GuildId}");
                SocketGuild g = null;
                SocketVoiceChannel vc = null;
                while (g == null || vc == null) { // TODO: timeout
                    await Task.Delay(1000);
                    g = d.GetGuild(secret.GuildId);
                    vc = g?.VoiceChannels?.FirstOrDefault(x => x.Id == channels[index]) ?? null;
                }
                // ボイチャ参加
                var audioClient = await vc.ConnectAsync();
                audioClient.StreamCreated += discordAudioClient_StreamCreated;

            }

            Console.WriteLine($"[{DateTime.Now}] press any key to exit...");
            Console.ReadKey();

            foreach (var d in discords) {
                await d.StopAsync();
            }
            secret.ToJsonFile(SecretConfig.PATH);
        }

        private static Task discordAudioClient_StreamCreated(ulong arg1, Discord.Audio.AudioInStream arg2) {
            Console.WriteLine($"[{DateTime.Now}] StreamCreated");
            return Task.CompletedTask;
        }

        private static Task d_Log(Discord.LogMessage message) {
            Console.WriteLine($"[{DateTime.Now}] {message.Message}");
            return Task.CompletedTask;
        }

        private static Task discord_GuildAvailable(SocketGuild g) {
            Console.WriteLine($"[{DateTime.Now}] Guild Available: {g.Name}({g.Id})");
            return Task.CompletedTask;
        }

        private static Task discord_MessageReceived(SocketMessage socketMessage) {
            var message = socketMessage as SocketUserMessage;
            Console.WriteLine($"[{DateTime.Now}] #{message.Channel.Name} - {message.Author.Username}#{message.Author.Discriminator}: {message}");
            return Task.CompletedTask;
        }
    }
}
