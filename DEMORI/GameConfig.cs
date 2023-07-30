using System;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace DEMORI
{
    public class GameConfig
    {
        public string ConfigVersion;
        
        public string IdName;
        public string IdPublisher;
        public string IdVersion;

        public string TitleId;
        public string MSAAppId;
        public bool AdvancedUserModel;
        
        public string DefaultDisplayName;
        public string PublisherDisplayName;
        public string Square480x480Logo;
        public string Square150x150Logo;
        public string Square44x44Logo;
        public string Description;
        public string ForegroundText;
        public string BackgroundColor;
        public string SplashScreenImage;
        public string StoreLogo;

        private byte[] _sum = null;
        private string _hash = null;
        
        public GameConfig(string data)
        {
            var doc = new XmlDocument();
            doc.LoadXml(data);

            var game = doc.GetElementsByTagName("Game")[0];
            if (game.Attributes == null) throw new Exception("<Game> has no attributes");
            ConfigVersion = game.Attributes.GetNamedItem("configVersion").Value;
            
            var id = doc.GetElementsByTagName("Identity")[0];
            if (id.Attributes == null) throw new Exception("<Identity> has no attributes");
            IdName = id.Attributes.GetNamedItem("Name").Value;
            IdPublisher = id.Attributes.GetNamedItem("Name").Value;
            IdVersion = id.Attributes.GetNamedItem("Name").Value;

            TitleId = doc.GetElementsByTagName("TitleId")[0].Value;
            MSAAppId = doc.GetElementsByTagName("MSAAppId")[0].Value;
            AdvancedUserModel = doc.GetElementsByTagName("AdvancedUserModel")[0].Value == "true";

            var shellVisuals = doc.GetElementsByTagName("ShellVisuals")[0];
            if (shellVisuals.Attributes == null) throw new Exception("<ShellVisuals> has no attributes");
            DefaultDisplayName = shellVisuals.Attributes.GetNamedItem("DefaultDisplayName").Value;
            PublisherDisplayName = shellVisuals.Attributes.GetNamedItem("PublisherDisplayName").Value;
            Square480x480Logo = shellVisuals.Attributes.GetNamedItem("Square480x480Logo").Value;
            Square150x150Logo = shellVisuals.Attributes.GetNamedItem("Square150x150Logo").Value;
            Square44x44Logo = shellVisuals.Attributes.GetNamedItem("Square44x44Logo").Value;
            Description = shellVisuals.Attributes.GetNamedItem("Description").Value;
            ForegroundText = shellVisuals.Attributes.GetNamedItem("ForegroundText").Value;
            BackgroundColor = shellVisuals.Attributes.GetNamedItem("BackgroundColor").Value;
            SplashScreenImage = shellVisuals.Attributes.GetNamedItem("SplashScreenImage").Value;
            StoreLogo = shellVisuals.Attributes.GetNamedItem("StoreLogo").Value;
        }

        public byte[] Sum()
        {
            if (_sum != null) return _sum;
            var builder = new StringBuilder();
            builder.Append(ConfigVersion);
            builder.Append(IdName);
            builder.Append(IdPublisher);
            builder.Append(TitleId);
            builder.Append(MSAAppId);
            builder.Append(AdvancedUserModel.ToString());
            builder.Append(DefaultDisplayName);
            builder.Append(PublisherDisplayName);
            builder.Append(Square480x480Logo);
            builder.Append(Square150x150Logo);
            builder.Append(Square44x44Logo);
            builder.Append(Description);
            builder.Append(ForegroundText);
            builder.Append(BackgroundColor);
            builder.Append(SplashScreenImage);
            builder.Append(StoreLogo);
            _sum = Encoding.UTF8.GetBytes(builder.ToString());

            return _sum;
        }
        
        public string Hash()
        {
            return _hash ?? (_hash = Decryption.Sha256(Sum()));
        }
    }
}