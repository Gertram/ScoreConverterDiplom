using System.Collections.Generic;
namespace ScoreConverter.ScorePresets
{
    internal class BavarScorePreset : IScorePreset
    {
        private readonly JsonScorePreset.NumberScorePreset scorePreset;
        private const decimal MAX = 1;
        private const decimal MIN = 5;
        public string Name => scorePreset.Name;


        public BavarScorePreset()
        {
            var translator = new StepTranslator
            {
                Direct = true,
                Steps = new List<Step>
            {
                new Step(new decimal(1.0),new Range(93,100), new WordExpression(WordExpressionEnum.Excellent)),
                new Step(new decimal(1.3),new Range(86,92), new WordExpression(WordExpressionEnum.Excellent)),
                new Step(new decimal(1.7),new Range(81,85), new WordExpression(WordExpressionEnum.Good)),
                new Step(new decimal(2.0),new Range(76,80), new WordExpression(WordExpressionEnum.Good)),
                new Step(new decimal(2.3),new Range(71,75), new WordExpression(WordExpressionEnum.Good)),
                new Step(new decimal(2.7),new Range(67,70), new WordExpression(WordExpressionEnum.Satisfactorily)),
                new Step(new decimal(3.0),new Range(63,66), new WordExpression(WordExpressionEnum.Satisfactorily)),
                new Step(new decimal(3.3),new Range(59,62), new WordExpression(WordExpressionEnum.Satisfactorily)),
                new Step(new decimal(3.7),new Range(55,58), new WordExpression(WordExpressionEnum.Satisfactorily)),
                new Step(new decimal(4.0),new Range(51,54), new WordExpression(WordExpressionEnum.Satisfactorily)),
                new Step(new decimal(5.0),new Range(0,50), new WordExpression(WordExpressionEnum.NotSatisfactorilly))
            }
            };

            scorePreset = new JsonScorePreset.NumberScorePreset
            {
                MIN = 1,
                MAX = 5,
                Translator = translator,
                Name = "Баварская"
            };
        }
        IDirectScore IScorePreset.Translate(string value) => scorePreset.Translate(value);

        IReverseScore IScorePreset.ReverseTranslate(decimal value) => scorePreset.ReverseTranslate(value);

        public string Average(string[] values) => scorePreset.Average(values);

        //return (NMAX * (4 - value) + NMIN * (value - 1)) / 3;
        /*if (value == MAX)
        {
            return new decimal(5);
        }
        if (value <= new decimal(2.3))
        {
            return new decimal(4);
        }
        if (value <= new decimal(4))
        {
            return new decimal(3);
        }
        return new decimal(-1);*/

    }
}
