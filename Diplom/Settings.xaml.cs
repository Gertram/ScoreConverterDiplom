using System.Windows;
using System.Windows.Forms;

namespace ScoreConverter
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void SelectExtensionsDIrButton_Click(object sender, RoutedEventArgs e)
        {
            var win = new FolderBrowserDialog();
            if (win.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            Config.ExtensionsDir.Value = win.SelectedPath;
        }
    }
}
