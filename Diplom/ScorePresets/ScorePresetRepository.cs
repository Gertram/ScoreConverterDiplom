using System;
using System.Collections.Generic;
using System.IO;


namespace ScoreConverter.ScorePresets
{
    internal class ScorePresetRepository
    {
        internal static List<IScorePreset> GetAll()
        {
            var dir = new DirectoryInfo(ScoreConfig.PresetsDirectory);
            var list = new List<IScorePreset>();
            if (!dir.Exists)
            {
                return list;
            }
            foreach (var fileInfo in dir.GetFiles())
            {
                if (fileInfo.Extension != ".dat")
                {
                    continue;
                }
                try
                {
                    var preset = JsonScorePreset.NumberScorePreset.LoadFromFile(fileInfo.FullName);
                    if (preset == null)
                    {
                        Logger.Write($"Preset in \"fileInfo.FullName\" wasn't load");
                    }
                    else
                    {
                        list.Add(preset);
                    }
                }
                catch (Exception exp)
                {
                    Logger.Write($"Preset in \"fileInfo.FullName\" wasn't load by {exp.Message}");
                }
            }
            return list;

        }
    }
}
