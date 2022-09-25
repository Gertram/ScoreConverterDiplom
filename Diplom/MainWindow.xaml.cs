using ScoreConverter.ScorePresets;
using Microsoft.Win32;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ErrorsTitle = ScoreConverter.Properties.Errors;
using StringResources = ScoreConverter.Properties.Resources;

namespace ScoreConverter
{
    public class NameLabelProvider
    {
        private IScorePreset preset;

        public string SourceName { get; set; } = "";
        public string DestName { get; set; } = "";
        private bool Direction { get; set; } = true;
        internal string Average(string[] strings)
        {
            if (preset == null || strings == null)
            {
                return null;
            }
            if (Direction)
            {
                return preset.Average(strings);
            }
            try
            {
                return strings.Select(x => decimal.Parse(x)).Average().ToString();
            }
            catch
            {
                return null;
            }
        }
        internal IScorePreset Preset
        {
            get => preset;
            set
            {
                preset = value;
                if (Direction)
                {
                    SourceName = preset.Name;
                    DestName = Properties.Resources.RussiaGragingSystemName;
                }
                else
                {
                    DestName = preset.Name;
                    SourceName = Properties.Resources.RussiaGragingSystemName;
                }
            }
        }

        internal string Translate(string value)
        {
            if (preset == null)
            {
                return null;
            }
            if (Direction)
            {
                var score = preset.Translate(value);
                if (score == null)
                {
                    return null;
                }
                var range = score.GetRange();
                if (range == null)
                {
                    return score.GetText();
                }
                return $"{score.GetText()} {score.GetRange().GetStart()} - {score.GetRange().GetEnd()}";
            }
            if (!decimal.TryParse(value, out var res))
            {
                return null;
            }
            var rescore = preset.ReverseTranslate(res);
            return $"{rescore.GetText()} {rescore.GetValue()}";
        }
        internal void Translate(string value, TextBox key, TextBox text)
        {
            if (preset == null)
            {

                return;
            }
            if (Direction)
            {
                var score = preset.Translate(value);
                if (score != null)
                {
                    key.Text = score.GetText();
                    var range = score.GetRange();
                    if (range != null)
                    {
                        text.Text = $"{score.GetRange().GetStart()} - {score.GetRange().GetEnd()}";
                    }
                    else
                    {
                        text.Text = "";
                    }
                }
                else
                {
                    key.Text = "";
                    text.Text = "";
                }
            }
            else
            {
                if (!decimal.TryParse(value, out var input))
                {
                    key.Text = "";
                    text.Text = "";
                    return;
                }
                var score = preset.ReverseTranslate(input);
                if (score != null)
                {
                    key.Text = score.GetText();
                    text.Text = score.GetValue().ToString();
                }
                else
                {

                    key.Text = "";
                    text.Text = "";
                }
            }
        }
        internal void Reverse()
        {
            Direction = !Direction;
            var temp = DestName;
            DestName = SourceName;
            SourceName = temp;
        }
    }    /// <summary>
         /// Логика взаимодействия для MainWindow.xaml
         /// </summary>
    public partial class MainWindow : Window
    {
        private List<IScorePreset> Presets;
        public NameLabelProvider NameLabelProvider { get; set; } = new NameLabelProvider();
        public MainWindow()
        {
            InitializeComponent();
        }
        private void LoadPresets()
        {
            Presets = new List<IScorePreset>();
            var bavarPreset = new BavarScorePreset();
            InsertNewPresetMenu(bavarPreset);
            var extension_presets = Extensions.ExtensionRepository.GetAll();
            if (extension_presets != null)
            {
                foreach (var preset in extension_presets)
                {
                    InsertNewPresetMenu(preset);
                }
            }
            var task = new Task<List<IScorePreset>>(delegate
            {
                return ScorePresetRepository.GetAll();
            });
            task.Start();
            task.GetAwaiter().OnCompleted(delegate
            {
                foreach (var preset in task.Result)
                {
                    AddScorePreset(preset);
                }
            });
        }
        private void AddPresetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var win = new ScorePresetWindow(Presets);
            if (!(bool)win.ShowDialog())
            {
                return;
            }
            AddScorePreset(win.ScorePreset);
        }

        private MenuItem CreateMenuItem(string name, RoutedEventHandler handler, object parameter)
        {
            var menu = new MenuItem
            {
                Header = name,
                CommandParameter = parameter
            };
            menu.Click += handler;
            return menu;
        }
        private void AddScorePreset(IScorePreset preset)
        {
            var menu = InsertNewPresetMenu(preset);
            menu.Items.Add(CreateMenuItem(StringResources.OpenMenuItemHeader, OpenMenu_Click, preset));
            menu.Items.Add(CreateMenuItem(StringResources.EditMenuItemHeader, EditMenu_Click, preset));
            menu.Items.Add(CreateMenuItem(StringResources.DeleteMenuItemHeader, DeleteMenu_Click, preset));
        }
        private MenuItem InsertNewPresetMenu(IScorePreset preset)
        {
            Presets.Add(preset);
            var menu = new MenuItem
            {
                Header = preset.Name,
                CommandParameter = preset
            };
            menu.Click += OpenMenu_Click;
            ScorePresetMenuItem.Items.Insert(ScorePresetMenuItem.Items.Count - 1, menu);
            return menu;
        }
        private void OpenMenu_Click(object sender, RoutedEventArgs e)
        {
            SetScorePreset((e.Source as MenuItem).CommandParameter as IScorePreset);
        }
        private void SetScorePreset(IScorePreset preset)
        {
            if (preset == null)
            {
                return;
            }
            var id = Presets.IndexOf(preset);

            NameLabelProvider.Preset = preset;
            NameLabelWrap.DataContext = null;
            NameLabelWrap.DataContext = NameLabelProvider;

            TranslateScore();
            Config.LastScorePreset = preset.Name;
            foreach (MenuItem item in ScorePresetMenuItem.Items)
            {
                if (item != ScorePresetMenuItem.Items[id])
                {
                    item.IsChecked = false;
                }
                else
                {
                    item.IsChecked = true;
                }
            }
        }
        private void EditMenu_Click(object sender, RoutedEventArgs e)
        {
            var menu = sender as MenuItem;
            var win = new ScorePresetWindow(Presets)
            {
                ScorePreset = menu.CommandParameter as ScorePresets.JsonScorePreset.NumberScorePreset
            };
            win.ShowDialog();
            if (menu.Parent is MenuItem parent)
            {
                parent.Header = win.ScorePreset.Name;
            }
            win.ScorePreset.Save();
        }
        private MessageBoxResult ShowMessageBox(string message)
        {
            return MessageBox.Show(message, StringResources.Attention, MessageBoxButton.YesNo);
        }
        private MessageBoxResult ShowMessageBox(string message, MessageBoxButton buttons)
        {
            return MessageBox.Show(message, StringResources.Attention, buttons);
        }

        private void DeleteMenu_Click(object sender, RoutedEventArgs e)
        {
            if (ShowMessageBox(Properties.Resources.ScorePresetDeleteAttention, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }
            var preset = (sender as MenuItem).CommandParameter as ScorePresets.JsonScorePreset.NumberScorePreset;
            var ind = Presets.IndexOf(preset);
            ScorePresetMenuItem.Items.RemoveAt(ind);
            Presets.Remove(preset);
            preset.Delete();
        }
        private void TranslateScore()
        {
            NameLabelProvider.Translate(SourceValueTextBox.Text, DestValueWord, DestValueText);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadPresets();
                var presetName = Config.LastScorePreset;
                if (presetName == null)
                {
                    if (Presets.Count == 0)
                    {
                        MessageBox.Show("Не обнаружена ни одна система оценивания");
                        return;
                    }
                    SetScorePreset(Presets[0]);
                    return;
                }
                var preset = Presets.Find(x => x.Name == presetName);
                if (preset == null)
                {
                    Logger.Write(ErrorsTitle.LastScorePresetError);
                    return;
                }
                SetScorePreset(preset);
            }
            catch
            {

            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TranslateScore();
        }

        private void AddNewPresetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var win = new ScorePresetWindow(Presets);
            if (!(bool)win.ShowDialog())
            {
                return;
            }
            win.ScorePreset.Save();
            AddScorePreset(win.ScorePreset);
        }

        private void SourceValueText_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TranslateScore();
            }
        }

        private void ReverseButton_Click(object sender, RoutedEventArgs e)
        {
            NameLabelProvider.Reverse();
            NameLabelWrap.DataContext = null;
            NameLabelWrap.DataContext = NameLabelProvider;
            SourceValueTextBox.Text = DestValueText.Text;
            SourceValueTextBox.Text = "";
        }
        private void ShowError(string message)
        {
            Logger.Write(message);
            MessageBox.Show(message, ErrorsTitle.Error);
        }
        class ScoreFormatException : Exception
        {
            public override string Message => "Ошибка в формате вводимого числа";
        }
        class ScoreCountException : Exception
        {
            public override string Message => "В файле не обнаружено искомых значений";
        }
        class FileFormatException : Exception
        {
            public override string Message => "Неподдерживаемый формат файла";
        }
        class CalcAverageException : Exception
        {
            public override string Message => "Ошибка при подсчете среднего значения";
        }

        private string[] ValuesFromTextFile(string path)
        {
            using (var reader = new StreamReader(path))
            {
                //return Regex.Split(reader.ReadToEnd().Trim(), @"\D+").ToArray();
                try
                {
                    var str = reader.ReadToEnd().Trim();
                    var values = Regex.Split(str, @"\W+").ToArray();
                    return values;
                }
                catch(ArgumentException)
                {
                    throw new ScoreFormatException();
                }
            }
        }
        private string[] ValuesFromExelFile(string path)
        {
            using (var book = new Workbook())
            {
                book.LoadFromFile(path);
                var sheet = book.Worksheets[0];
                var table = sheet.ExportDataTable();
                var values = new List<string>();
                foreach (System.Data.DataRow row in table.Rows)
                {
                    var obj = (string)row.ItemArray[0];
                    values.Add(obj);
                }
                return values.ToArray();
            }
        }
        private string GetAverageFromFile(string path)
        {
            var values = GetValuesFromFile(path);
            if(values.Length == 0)
            {
                throw new ScoreCountException();
                
            }
            return NameLabelProvider.Average(values);
        }

        private string[] GetValuesFromFile(string path)
        {
            switch (System.IO.Path.GetExtension(path))
            {
                case ".txt":
                    return ValuesFromTextFile(path);
                case ".xlsx":
                    return ValuesFromExelFile(path);
                default:
                    throw new FileFormatException();
            }
        }
      
        private void TranslateFiles(string[] files)
        {
            if (files.Length == 0)
            {
                MessageBox.Show("Передан пустой список");
                return;
            }
            if (files.Length == 1)
            {
                var path = files[0];
                try
                {
                    var value = GetAverageFromFile(path);
                    if(value != null)
                    {
                        throw new CalcAverageException();
                    }
                    SourceValueTextBox.Text = value;
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
                return;
            }
            var scores = new Dictionary<string, string>();
           
            foreach (var file in files)
            {
                var filename = Path.GetFileNameWithoutExtension(file);
                if (scores.ContainsKey(filename))
                {
                    filename = $"{filename} ({Path.GetFileName(file)})";
                }
                var value = "";
                try
                {
                    var average = GetAverageFromFile(file);
                    if(average == null)
                    {
                        throw new CalcAverageException();
                    }
                    value = NameLabelProvider.Translate(average);
                }
                catch (Exception exception)
                {
                    value = exception.Message;
                }
                scores.Add(filename, value);
            }
            Notepad.NotepadHelper.ShowMessage(string.Join("\n", scores.Select(x => $"{x.Key}: {x.Value}")), "Результаты");
        }
        private void OpenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "Текстовый файл (*.txt)|*.txt|Документ excel|*.xlsx|Все файлы (*.*)|*.*",
                Title = "Выберите файл содержащий значения",
                InitialDirectory = Environment.CurrentDirectory,
                Multiselect = true
            };
            if (!(bool)ofd.ShowDialog())
            {
                return;
            }
            TranslateFiles(ofd.FileNames);
            e.Handled = true;
        }
        private void Window_Drop(object sender, DragEventArgs e)
        {
            base.OnDrop(e);
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var obj = e.Data.GetData(DataFormats.FileDrop);
                var files = obj as string[];

                TranslateFiles(files);
            }
            e.Handled = true;
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var win = new Settings();
            win.ShowDialog();
        }
    }
}
