using System.Windows;
using System.Windows.Documents;
using Microsoft.VisualStudio.PlatformUI;

namespace EgorBo.Xamaridea_VisualStudioPlugin
{
    /// <summary>
    /// Interaction logic for ConfigurationDialog.xaml
    /// </summary>
    public partial class ConfigurationDialog : DialogWindow
    {
        public ConfigurationDialog(string helpTopic) : base(helpTopic)
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        public ConfigurationDialog()
        {
            this.HasMaximizeButton = true;
            this.HasMinimizeButton = true;
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            BrowseTextBox.Text = Settings.Default.AnidePath;
        }
        
        private void BrowseButton_OnClick(object sender, RoutedEventArgs e)
        {    
            var dlg = new Microsoft.Win32.OpenFileDialog { DefaultExt = ".exe", Filter = "Exe Files (*.exe)|*.exe" };
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                BrowseTextBox.Text = filename;
                Settings.Default.AnidePath = BrowseTextBox.Text;
            }
        }

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(((Hyperlink) sender).NavigateUri.ToString());
        }
    }
}
