using System;
using System.ComponentModel;
using System.Configuration;
namespace ScoreConverter
{
    public class StringValue : INotifyPropertyChanged
    {
        private readonly string name;
        private readonly string @default;
        internal StringValue(string name, string @default = null)
        {
            this.name = name;
            this.@default = @default;
        }
        public string Value
        {

            get
            {
                var temp = Config.Get(name);
                if (temp == null)
                {
                    temp = @default;
                }
                return temp;
            }

            set
            {
                Config.Set(name, value);
                if (PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
    internal static partial class Config
    {
        internal static string LastScorePreset
        {
            get => Get("LastScorePreset");
            set => Set("LastScorePreset", value);
        }
        public static StringValue ExtensionsDir { get; set; } = new StringValue("ExtensionsDir", "Extensions");
        internal static string Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
        internal static void Set(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }

    }
}
