using System;
using System.Collections.Generic;
using System.Text;

namespace ikanokaiwa_sharp.Config {
    public class SecretConfig {
        public static readonly string PATH = "secret.json";

        public string Secret0 { get; set; } = "";
        public string Secret1 { get; set; } = "";
        public string Secret2 { get; set; } = "";

        public ulong VoiceChannelId0 { get; set; } = 0;
        public ulong VoiceChannelId1 { get; set; } = 0;
        public ulong VoiceChannelId2 { get; set; } = 0;
    }
}
