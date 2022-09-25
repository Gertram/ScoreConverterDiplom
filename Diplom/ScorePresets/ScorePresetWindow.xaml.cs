using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;


namespace ScoreConverter.ScorePresets
{
    /// <summary>
    /// Логика взаимодействия для ScorePresetWindow.xaml
    /// </summary>
    public partial class ScorePresetWindow : Window
    {
        private JsonScorePreset.NumberScorePreset scorePreset;
        private List<IScorePreset> presets;
        internal ScorePresetWindow(List<IScorePreset> presets)
        {
            InitializeComponent();
            this.presets = presets;
        }
        private uint GetValue(TextBox txt)
        {
            if (txt.Text == null || txt.Text.Trim() == "")
            {
                throw new Exception();
            }
            return uint.Parse(txt.Text);
        }
        internal JsonScorePreset.NumberScorePreset ScorePreset
        {
            get => scorePreset;
            set
            {
                MaxTB.Text = value.MAX.ToString();
                MinTB.Text = value.MIN.ToString();
                DirectButton.Content = $" Изменить {value.Translator.GetName()}";
                translator = value.Translator;
                NameTB.Text = value.Name;
                scorePreset = value;
            }
        }
        private ITranslator translator;
        private bool GetMax(out decimal result) => decimal.TryParse(MaxTB.Text, out result);
        private bool GetMin(out decimal result) => decimal.TryParse(MinTB.Text, out result);
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NameTB.Text = NameTB.Text.Trim();
                if (NameTB.Text == "")
                {
                    MessageBox.Show("Name must be not empty");
                    return;
                }
                if (/*presets.Any(x => x.Name == NameTB.Text) &&*/ presets.Find(x => x.Name == NameTB.Text) != ScorePreset)
                {
                    MessageBox.Show("Preset with this name already exists");
                    return;
                }
                var name = NameTB.Text;
                var max = GetValue(MaxTB);
                var min = GetValue(MinTB);
                if (translator == null)
                {
                    MessageBox.Show("Не выбран переводчик системы");
                    return;
                }
                if (scorePreset == null)
                    scorePreset = new JsonScorePreset.NumberScorePreset();
                ScorePreset.Name = name;
                ScorePreset.MAX = max;
                ScorePreset.MIN = min;
                ScorePreset.Translator = translator;
                DialogResult = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Incorrect number format");
            }
        }
        private ITranslator GetTranslator(ITranslator translator, decimal max, decimal min)
        {
            var win = new StepTranslatorWindow(max, min);
            if (translator != null && translator is StepTranslator direct)
                win.Translator = direct;
            if (!(bool)win.ShowDialog())
            {
                return null;
            }
            return win.Translator;
        }
        private void DirectButton_Click(object sender, RoutedEventArgs e)
        {
            if (!GetMax(out var max))
            {
                MessageBox.Show("Не выбрано максимальное значение");
                return;
            }
            if (!GetMin(out var min))
            {
                MessageBox.Show("Не выбрано минимальное значение");
                return;
            }
            translator = GetTranslator(translator, max, min);
            if (translator == null)
            {
                return;
            }
            DirectButton.Content = $"{translator.GetName()} Изменить";
        }

    }
}
