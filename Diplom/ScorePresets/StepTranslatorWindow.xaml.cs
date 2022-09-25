using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ScoreConverter.ScorePresets
{
    /// <summary>
    /// Логика взаимодействия для StepTranslatorWindow.xaml
    /// </summary>
    public partial class StepTranslatorWindow : Window
    {
        private BindingList<Step> Steps { get; set; } = new BindingList<Step>();
        private StepTranslator translator = new StepTranslator();
        private readonly decimal min, max;

        public StepTranslatorWindow(decimal min, decimal max)
        {
            this.min = min;
            this.max = max;
            InitializeComponent();
        }
        internal StepTranslator Translator
        {
            get => translator; set
            {

                StepsList.ItemsSource = null;
                StepsList.DataContext = null;
                Steps.Clear();
                foreach (var step in value.Steps)
                {
                    Steps.Add(step.Clone());
                }
                translator = value;
                StepsList.ItemsSource = Steps;
                StepsList.DataContext = Steps;
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            bool direction = DirectionComboBox.SelectedIndex == 1;
            if (translator == null)
            {
                translator = new StepTranslator();
            }
            translator.Steps = Steps.ToList();
            translator.Direct = direction;
            DialogResult = true;
        }
        private void DeleteStep()
        {
            if (StepsList.SelectedItem is Step step)
            {
                StepsList.ItemsSource = null;
                StepsList.DataContext = null;
                Steps.Remove(step);
                StepsList.ItemsSource = Steps;
                StepsList.DataContext = Steps;
            }
        }
        private void ListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeleteStep();
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            DeleteStep();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var win = new InsertStepWindow(min, max);
            if (!(bool)win.ShowDialog())
            {
                return;
            }
            StepsList.ItemsSource = null;
            StepsList.DataContext = null;
            Steps.Add(win.Step);
            Steps = new BindingList<Step>(Steps.OrderBy(x => x.Value).ToList());
            StepsList.ItemsSource = Steps;
            StepsList.DataContext = Steps;
        }
    }
}
