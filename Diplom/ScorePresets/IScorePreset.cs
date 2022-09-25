namespace ScoreConverter.ScorePresets
{
    internal interface IScorePreset
    {
        IDirectScore Translate(string value);
        IReverseScore ReverseTranslate(decimal value);
        string Average(string[] values);
        string Name { get; }
    }
}
