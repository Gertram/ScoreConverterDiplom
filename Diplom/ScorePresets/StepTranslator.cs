using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace ScoreConverter.ScorePresets
{

    class Step : IDirectScore, IReverseScore
    {
        internal Step(decimal value, Range range, WordExpression wordExpression)
        {
            Range = range;
            Value = value;
            WordExpression = wordExpression;
        }

        public string GetText() => WordExpression.ToString();
        public IRange GetRange() => Range;
        public Range Range { get; private set; }
        public string GetValue() => Value.ToString();
        public decimal Value { get; private set; }
        public WordExpression WordExpression { get; private set; }

        internal Step Clone() => new Step(Value, Range, WordExpression);

    }
    internal class StepTranslator : ITranslator
    {
        public List<Step> Steps { get; set; }
        public bool Direct { get; set; }

        public string GetName()
        {
            return "Ступенчатый";
        }

        public string Sirealize()
        {
            return JsonSerializer.Serialize(this);
        }

        public IDirectScore DirectTranslate(string value)
        {
            if (!decimal.TryParse(value, out var input))
            {
                return null;
            }
            if (Direct)
            {
                foreach (var step in Steps)
                {
                    if (input <= step.Value)
                    {
                        return step;
                    }
                }
            }
            else
            {
                foreach (var step in Steps.Reverse<Step>())
                {
                    if (input >= step.Value)
                    {
                        return step;
                    }
                }
            }
            return null;
        }
        public IReverseScore ReverseTranslate(decimal value)
        {
            if (Direct)
            {
                foreach (var step in Steps)
                {
                    if (step.Range.InRange(value))
                    {
                        return step;
                    }
                }
            }
            else
            {
                foreach (var step in Steps.Reverse<Step>())
                {
                    if (step.Range.InRange(value))
                    {
                        return step;
                    }
                }
            }
            return null;
        }
    }
}
