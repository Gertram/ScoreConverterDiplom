using ScoreConverter.ScorePresets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ScoreConverter.Extensions
{
    class Types
    {
        internal Type Main { get; set; }
        internal Type ScorePreset { get; set; }
        internal Type Range { get; set; }
        internal Type DirectScore { get; set; }
        internal Type ReverseScore { get; set; }
    }
    partial class Extension
    {
        private object obj;
        private Types types;
        private string name;

        internal static Extension Load(string path)
        {
            try
            {
                var dll = Assembly.LoadFile(path);
                var types = LoadTypes(dll);
                if (types == null)
                {
                    return null;
                }
                var obj = dll.CreateInstance(types.Main.FullName);

                return new Extension(obj, types, Path.GetFileNameWithoutExtension(path));
            }
            catch (Exception exp)
            {
                Logger.Write(exp.Message);
            }

            return null;
        }
        private static Types LoadTypes(Assembly dll)
        {
            var temp = new Types
            {
                Main = FindType(dll, "Main"),
                ScorePreset = FindType(dll, "IScorePreset"),
                DirectScore = FindType(dll, "IDirectScore"),
                ReverseScore = FindType(dll, "IReverseScore"),
                Range = FindType(dll, "IRange")
            };
            if (temp.Main == null || temp.ScorePreset == null || temp.DirectScore == null || temp.ReverseScore == null || temp.Range == null)
            {
                return null;
            }
            return temp;
        }
        private static Type FindType(Assembly dll, string name)
        {
            foreach (var item in dll.GetExportedTypes())
            {
                if (item.Name == name)
                {
                    return item;
                }
            }
            return null;
        }
        private Extension(object obj, Types types, string name)
        {
            this.obj = obj;
            this.types = types;
            this.name = name;
        }

        public string Name => name;//(string)name.Invoke(obj, null);
        public List<IScorePreset> Presets
        {
            get
            {
                var method = types.Main.GetMethod("GetPresets");
                var temp = method.Invoke(obj, null);
                if (temp is IEnumerable<object> presets)
                {
                    var list = new List<IScorePreset>();
                    foreach (var preset in presets)
                    {
                        if (!types.ScorePreset.IsInstanceOfType(preset))
                        {
                            return null;
                        }
                        var buff = ImportedScorePreset.Load(preset, types);
                        if (buff != null)
                        {
                            list.Add(buff);
                        }
                    }

                    return list;
                }
                return null;
            }
        }
    }
}
