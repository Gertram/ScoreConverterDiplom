namespace ScoreConverter.ScorePresets
{
    internal interface ITranslator
    {
        IDirectScore DirectTranslate(string value);
        IReverseScore ReverseTranslate(decimal value);
        string GetName();
        string Sirealize();
    }
}
