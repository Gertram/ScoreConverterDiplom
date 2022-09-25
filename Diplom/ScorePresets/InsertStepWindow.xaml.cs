using System.Windows;

namespace ScoreConverter.ScorePresets
{
    /// <summary>
    /// Логика взаимодействия для InsertStepWindow.xaml
    /// </summary>
    public partial class InsertStepWindow : Window
    {
        private decimal min, max;
        public InsertStepWindow(decimal min, decimal max)
        {
            this.min = min;
            this.max = max;
            InitializeComponent();
        }
        internal Step Step { get; private set; }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(KeyTextBox.Text, out var key)
                || !decimal.TryParse(RangeStartTextBox.Text, out var start)
                || !decimal.TryParse(RangeEndTextBox.Text, out var end)
                || start > end
                || key < min || key > max || start < 0 || end > 100)
            {
                return;
            }
            try
            {
                Step = new Step(key, new Range(start, end), new WordExpression((WordExpressionEnum)WordExpressionComboBox.SelectedIndex));
                DialogResult = true;
            }
            catch
            {

            }
        }
    }
}
