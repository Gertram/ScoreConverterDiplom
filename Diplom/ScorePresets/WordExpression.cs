namespace ScoreConverter.ScorePresets
{
    enum WordExpressionEnum
    {
        Excellent, Good, Satisfactorily, NotSatisfactorilly
    }
    internal class WordExpression
    {
        private readonly WordExpressionEnum expressionEnum;

        internal WordExpression(WordExpressionEnum expressionEnum)
        {
            this.expressionEnum = expressionEnum;
        }

        public override string ToString()
        {
            if (expressionEnum == WordExpressionEnum.Excellent)
            {
                return "Отлично";
            }
            if (expressionEnum == WordExpressionEnum.Good)
            {
                return "Хорошо";
            }
            if (expressionEnum == WordExpressionEnum.Satisfactorily)
            {
                return "Удовлетворительно";
            }
            return "Неудовлетворительно";
        }
    }
}
