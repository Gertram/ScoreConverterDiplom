using ScoreConverter.ScorePresets;
using System;
using System.Reflection;

namespace ScoreConverter.Extensions
{
    partial class ImportedScorePreset : IScorePreset
    {
        private readonly object obj;
        private readonly Types types;
        private readonly MethodInfo name;
        private readonly MethodInfo translate;
        private readonly MethodInfo reverseTranslate;
        public static ImportedScorePreset Load(object obj, Types types)
        {

            try
            {
                return new ImportedScorePreset(obj, types);
            }
            catch (Exception exp)
            {
                Logger.Write(exp.Message);
            }

            return null;
        }
        internal ImportedScorePreset(object obj, Types types)
        {
            this.obj = obj;
            this.types = types;
            name = types.ScorePreset.GetMethod("GetName");
            translate = types.ScorePreset.GetMethod("Translate");
            reverseTranslate = types.ScorePreset.GetMethod("ReverseTranslate");
        }


        public string Name => (string)name.Invoke(obj, null);
        public IReverseScore ReverseTranslate(decimal value)
        {
            var temp = reverseTranslate.Invoke(obj, new object[] { value });
            if (temp == null)
            {
                return null;
            }
            if (!types.ReverseScore.IsInstanceOfType(temp))
            {
                return null;
            }

            var text_method = types.ReverseScore.GetMethod("GetText");
            var value_method = types.ReverseScore.GetMethod("GetValue");
            var text = (string)text_method.Invoke(temp, null);
            var val = (string)value_method.Invoke(temp, null);
            return new ImportedReverseScore(text, val);
        }


        public IDirectScore Translate(string value)
        {
            var temp = translate.Invoke(obj, new object[] { value });
            if (temp == null)
            {
                return null;
            }
            if (!types.DirectScore.IsInstanceOfType(temp))
            {
                return null;
            }

            var text_method = types.DirectScore.GetMethod("GetText");
            var range_method = types.DirectScore.GetMethod("GetRange");
            var text = (string)text_method.Invoke(temp, null);
            var range = range_method.Invoke(temp, null);
            if (!types.Range.IsInstanceOfType(range))
            {
                return new ImportedDirectScore(text, null);
            }
            var start_method = types.Range.GetMethod("GetStart");
            var end_method = types.Range.GetMethod("GetEnd");
            var start = (decimal)start_method.Invoke(range, null);
            var end = (decimal)end_method.Invoke(range, null);
            return new ImportedDirectScore(text, new Range(start, end));
        }

        public string Average(string[] values)
        {
            var average_method = types.ScorePreset.GetMethod("Average");
            var res = average_method.Invoke(obj, new object[] { values });
            if (res is string str)
            {
                return str;
            }
            return null;
        }
    }
    class ImportedReverseScore : IReverseScore
    {
        private string text;
        private string value;
        internal ImportedReverseScore(string text, string value)
        {
            this.text = text;
            this.value = value;
        }

        public string GetText() => text;

        public string GetValue() => value;
    }
    class ImportedDirectScore : IDirectScore
    {
        private readonly string text;
        private readonly Range range;
        internal ImportedDirectScore(string text, Range range)
        {
            this.text = text;
            this.range = range;
        }

        public string GetText() => text;

        public IRange GetRange() => range;
    }
}
