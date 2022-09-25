namespace ScoreConverter.ScorePresets
{
    internal static partial class ScoreConfig
    {
        private const string PRESETS_DIRECTORY_KEY = "PRESETS_DIRECTORY";

        internal static string PresetsDirectory
        {
            get
            {
                var value = Config.Get(PRESETS_DIRECTORY_KEY);
                if (value == null)
                {
                    value = Properties.Resources.DefaultScorePresetsDir;
                }
                return value;
            }

            set => Config.Set(PRESETS_DIRECTORY_KEY, value);
        }
    }
}
