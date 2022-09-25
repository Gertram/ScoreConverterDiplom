using ScoreConverter.ScorePresets;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace ScoreConverter.Extensions
{
    class NameWrap : IScorePreset
    {
        private readonly IScorePreset preset;
        private readonly string extensionName;

        public NameWrap(IScorePreset preset, string extensionName)
        {
            this.preset = preset;
            this.extensionName = extensionName;
        }

        public string Name => $"{preset.Name} ({extensionName})";

        public string Average(string[] values) => preset.Average(values);

        public IReverseScore ReverseTranslate(decimal value) => preset.ReverseTranslate(value);

        public IDirectScore Translate(string value) => preset.Translate(value);
    }
    internal class ExtensionRepository
    {
        internal static string ExtensionDir => Config.ExtensionsDir.Value;
        internal static List<IScorePreset> GetAll()
        {
            var extensionDir = new DirectoryInfo(ExtensionDir);
            if (!extensionDir.Exists)
            {
                extensionDir.Create();
                return null;
            }
            var presets = new Dictionary<string, IScorePreset>();
            foreach (var dir in extensionDir.GetDirectories())
            {
                var extension = LoadExtension(dir);
                if (extension == null)
                {
                    continue;
                }
                foreach (var preset in extension.Presets)
                {
                    var temp = new NameWrap(preset, extension.Name);
                    presets.Add(temp.Name, temp);
                }
            }

            return presets.Values.ToList();
        }
        private static Extension LoadExtension(DirectoryInfo dir)
        {
            var dllName = dir.Name + ".dll";
            foreach (var file in dir.GetFiles())
            {
                if (file.Name == dllName)
                {
                    return Extension.Load(file.FullName);
                }
            }
            return null;
        }
    }
}
