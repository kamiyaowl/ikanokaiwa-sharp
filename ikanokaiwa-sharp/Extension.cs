using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ikanokaiwa_sharp {
    public static class Extension {
        public static void ToJsonFile<T>(this T src, string path) {
            File.WriteAllText(path, JsonConvert.SerializeObject(src, Formatting.Indented));
        }
        public static T FromJsonFile<T>(this string path, Func<T> generator = null) {
            if (generator == null) {
                generator = () => default(T);
            }
            if (!File.Exists(path)) {
                return generator();
            }
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }
    }
}
