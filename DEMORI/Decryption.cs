using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DEMORI
{
    public static class Decryption
    {
        private static string[] _requiredFiles =
        {
            "MicrosoftGame.Config",
            "resources.pri",
            "GraphicsLogo.png",
            "LargeLogo.png",
            "SmallLogo.png",
            "SplashScreen.png",
            "StoreLogo.png"
        };

        private static Dictionary<string, string> _staticHashes = new Dictionary<string, string>()
        {
            {"resources.pri", "74F64242242A19704A667921AE6E2C3D8DE1DE0EA914036F6C3AC366EA77BA15"},
            {"GraphicsLogo.png", "D814A18F0371E45C5FDDEBA067193772DBB9B7668061CF5525671A88E95EDBDF"},
            {"LargeLogo.png", "FFB1B9619FFBC58C61A3ADA9DEAD7BF884D543E9C5D671EF89EBAF358D21F388"},
            {"SmallLogo.png", "2CB75925A6FE8A9B8AB012B7D44957C6CD04A36957236D5E70FE4C66FA8F045E"},
            {"SplashScreen.png", "FB61573961F412FC81C454A8945961DBA848E47CC3947E90C2B1AF4DC9B7A254"},
            {"StoreLogo.png", "8BD7F5398F1552527668889040194735E5866690BF7855A9BF76478C27A6546E"},
        };

        private static string MicrosoftGameConfigHash =
            "D83A7838B883416E3B4A332F04507484FB87CA771D54A0771CE535735FC9AB3C";

        private static void Xor(byte[] dst, byte[] key)
        {
            for (var i = 0; i < dst.Length; i++)
            {
                dst[i] ^= key[i % key.Length];
            }
        }

        public static Tuple<Dictionary<string, string>, GameConfig> Validate(string dir)
        {
            var status = new Tuple<Dictionary<string, string>, GameConfig>(new Dictionary<string, string>(), null);
            var exit = false;

            foreach (var file in _requiredFiles)
            {
                if (File.Exists(Path.Combine(dir, file)))
                {
                    status.Item1[file] = "Pass";
                    continue;
                }
                
                exit = true;
                status.Item1[file] = "Missing";
            }
            if (exit) return status;

            foreach (var hashedFile in _staticHashes)
            {
                var file = hashedFile.Key;
                var reference = hashedFile.Value;
                var hash = Sha256(Path.Combine(dir, file));

                if (reference == hash)
                {
                    status.Item1[file] = "Pass";
                    continue;
                }
                
                status.Item1[file] = $"{hash} != {reference}";
                exit = true;
            }
            if (exit) return status;

            var rawConfig = File.ReadAllText(Path.Combine(dir, "MicrosoftGame.Config"));
            GameConfig config;
            try
            {
                config = new GameConfig(rawConfig);
            }
            catch (Exception ex)
            {
                status.Item1["MicrosoftGame.Config"] = ex + "\n" + rawConfig;
                return status;
            }

            if (config.Hash() != MicrosoftGameConfigHash)
            {
                status.Item1["MicrosoftGame.Config"] =
                    $"MicrosoftGame.Config(XML) {config.Hash()} != {MicrosoftGameConfigHash}";
                return status;
            }
            
            status = new Tuple<Dictionary<string, string>, GameConfig>(
                status.Item1, config);
            return status;
        }

        // NOTE: This method assumes that Validate has already been run and was successful
        // a use *could* intentionally cause a race condition, override the files
        // used for encryption with fake ones and cause the encryption to fail
        // but at that point, that's the user's fault
        public static void Decrypt(string dir, byte[] data)
        {
            Xor(data, File.ReadAllBytes(Path.Combine(dir, "resources.pri")));
            Xor(data, File.ReadAllBytes(Path.Combine(dir, "GraphicsLogo.png")));
            Xor(data, File.ReadAllBytes(Path.Combine(dir, "LargeLogo.png")));
            Xor(data, File.ReadAllBytes(Path.Combine(dir, "SmallLogo.png")));
            Xor(data, File.ReadAllBytes(Path.Combine(dir, "SplashScreen.png")));
            Xor(data, File.ReadAllBytes(Path.Combine(dir, "StoreLogo.png")));
            
            var config = new GameConfig(File.ReadAllText(Path.Combine(dir, "MicrosoftGame.Config")));
            Xor(data, config.Sum());
        }
        
        private static string BytesToHex(byte[] bytes)
        {
            var sb = new StringBuilder();

            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString();
        }
        
        public static string Sha256(byte[] data)
        {
            using (var sha256 = SHA256.Create())
            {
                return BytesToHex(sha256.ComputeHash(data));
            }
        }
        
        public static string Sha256(string filePath)
        {
            using (var sha256 = SHA256.Create())
            {
                using (var fs = File.OpenRead(filePath))
                    return BytesToHex(sha256.ComputeHash(fs));
            }
        }
    }
}