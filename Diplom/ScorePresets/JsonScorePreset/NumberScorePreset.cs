using System.IO;
using System.Linq;
using System.Text.Json;

namespace ScoreConverter.ScorePresets.JsonScorePreset
{
    class NumberScorePreset : IScorePreset
    {
        private string Filename { get; set; }
        public string Name { get; set; }
        public decimal MIN { get; set; }
        public decimal MAX { get; set; }
        public ITranslator Translator { get; set; }
        internal static NumberScorePreset LoadFromFile(string filename)
        {
            var option = new JsonSerializerOptions
            {
                MaxDepth = 1
            };
            var data = JsonSerializer.Deserialize<PresetData>(File.OpenRead(filename), option);

            var translator = ParseTranslator(data.TRANSLATOR);

            return new NumberScorePreset()
            {
                MAX = data.MAX,
                MIN = data.MIN,
                Name = data.NAME,
                Filename = Path.GetFileName(filename),
                Translator = translator,
            };
        }

        private static ITranslator ParseTranslator(string str)
        {
            var option = new JsonSerializerOptions
            {
                MaxDepth = 1
            };
            var temp = JsonSerializer.Deserialize<TranslatorData>(str, option);
            return JsonSerializer.Deserialize<StepTranslator>(temp.Translator);
        }
        private static DirectoryInfo Dir => new DirectoryInfo(ScoreConfig.PresetsDirectory);
        private static string FilePath(DirectoryInfo dir, string name)
        {
            return dir.FullName + "\\" + name;
        }
        private string GenerateFileName(DirectoryInfo dir)
        {
            var filePath = FilePath(dir, Name);
            if (!File.Exists(filePath))
            {
                return filePath;
            }
            int i = 1;
            var temp = Filename;
            while (File.Exists(filePath))
            {
                temp = Name + i.ToString() + ".dat";
                filePath = FilePath(dir, temp);
                i++;
            }
            Filename = temp;
            return filePath;
        }
        private TranslatorData GetTranslatorData(ITranslator translator)
        {
            var data = new TranslatorData
            {
                Translator = translator.Sirealize()
            };
            return data;
        }
        internal void Save()
        {
            var dir = Dir;
            if (!dir.Exists)
            {
                dir.Create();
            }
            string path = dir.FullName + "\\" + Filename;
            if (Filename == null)
            {
                path = GenerateFileName(dir);
            }
            else if (Filename != Name + ".dat")
            {
                var temp = GenerateFileName(dir);
                File.Move(path, temp);
                path = temp;
            }
            var data = new PresetData
            {
                NAME = Name,
                MAX = MAX,
                MIN = MIN,
                TRANSLATOR = JsonSerializer.Serialize(GetTranslatorData(Translator))
            };
            File.WriteAllText(path, JsonSerializer.Serialize(data));
        }
        internal void Delete() => File.Delete(Filename);

        public IDirectScore Translate(string value) => Translator.DirectTranslate(value);

        public IReverseScore ReverseTranslate(decimal value) => Translator.ReverseTranslate(value * 20);

        public string Average(string[] values)
        {
            try
            {
                return values.Select(x => decimal.Parse(x)).Average().ToString();
            }
            catch
            {
                return "";
            }
        }
    }
}
