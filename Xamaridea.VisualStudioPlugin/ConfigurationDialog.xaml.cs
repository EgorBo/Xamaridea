using System.Windows;
using System.Windows.Documents;
using Microsoft.VisualStudio.PlatformUI;
using Xamaridea.Core;

namespace EgorBo.Xamaridea_VisualStudioPlugin
{
    /// <summary>
    /// Interaction logic for ConfigurationDialog.xaml
    /// </summary>
    public partial class ConfigurationDialog : DialogWindow
    {
        private readonly AndroidProjectTemplateManager _androidProjectTemplateManager = new AndroidProjectTemplateManager();

        public ConfigurationDialog(string helpTopic) : base(helpTopic)
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        public ConfigurationDialog()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _androidProjectTemplateManager.ExtractTemplateIfNotExtracted();
            BrowseTextBox.Text = Settings.Default.AnidePath;
            if (string.IsNullOrWhiteSpace(BrowseTextBox.Text))
            {
                var ideaPath = AndroidIdeDetector.TryFindIdeaPath();
                if (!string.IsNullOrEmpty(ideaPath))
                {
                    BrowseTextBox.Text = ideaPath;
                    Settings.Default.AnidePath = BrowseTextBox.Text;
                }
            }
            UpdateOkButtonState();
        }
        
        private void BrowseButton_OnClick(object sender, RoutedEventArgs e)
        {    
            var dlg = new Microsoft.Win32.OpenFileDialog { DefaultExt = ".exe", Filter = "Exe Files (*.exe)|*.exe" };
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                BrowseTextBox.Text = filename;
                UpdateOkButtonState();
            }
        }

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(((Hyperlink) sender).NavigateUri.ToString());
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            Settings.Default.AnidePath = BrowseTextBox.Text;
            Close();
        }

        private void UpdateOkButtonState()
        {
            OkButton.IsEnabled = !string.IsNullOrEmpty(BrowseTextBox.Text);
        }

        private void OnOpenAndroidTemplateDirectory(object sender, RoutedEventArgs e)
        {
            _androidProjectTemplateManager.OpenTempateFolder();
        }

        private void OnResetTemplateToDefault(object sender, RoutedEventArgs e)
        {
            _androidProjectTemplateManager.Reset();
        }
    }
}
