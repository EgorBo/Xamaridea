using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using Xamaridea.Core.Extensions;

namespace Xamaridea.Core
{
    public class AndroidProjectTemplateManager
    {
        public const string AndroidTemplateProjectResourceName = "Xamaridea.Core.AndroidProjectTemplate.zip";
        public const string AppDataFolderName = "Xamaridea";
        public const string TemplateFolderName = "Template_v.1.0.1";
        public const string ProjectsFolderName = "Projects";
        public const string XamarinResourcesFolderVariable = "%XAMARIN_RESOURCES_FOLDER%";

        public void Reset()
        {
            var templateDir = TemplateDirectory;
            if (Directory.Exists(templateDir))
                Directory.Delete(templateDir, true);
            ExtractTemplate();
        }

        public void ExtractTemplateIfNotExtracted()
        {
            var templateDir = TemplateDirectory;
            if (!Directory.Exists(templateDir))
            {
                ExtractTemplate();
            }
        }

        public void OpenTempateFolder()
        {
            Process.Start("explorer.exe", TemplateDirectory);
        }

        public string CreateProjectFromTemplate(string xamarinResourcesDir, string xamarinProjectPath)
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var projectName = ExtractProjectName(xamarinProjectPath);
            var tempNewProjectDir = Path.Combine(appData, AppDataFolderName, ProjectsFolderName, projectName);
            FileExtensions.DirectoryCopy(TemplateDirectory, tempNewProjectDir);
            var gradleConfig = Path.Combine(tempNewProjectDir, @"app\build.gradle");
            var configContent = File.ReadAllText(gradleConfig);
            configContent = configContent
                .Replace(XamarinResourcesFolderVariable, xamarinResourcesDir) //gradle is awesome, it allows us to specify any folder as resource container
                .Replace(@"\", "/"); //change backslashes to common ones

            File.WriteAllText(gradleConfig, configContent);
            return tempNewProjectDir;
        }

        private string ExtractProjectName(string xamarinProjectPath)
        {
            var startIndex = xamarinProjectPath.LastIndexOf('\\') + 1;
            return xamarinProjectPath.Substring(startIndex, xamarinProjectPath.Length - startIndex);
        }

        private string TemplateDirectory
        {
            get
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                return Path.Combine(appData, AppDataFolderName, TemplateFolderName);
            }
        }

        private void ExtractTemplate()
        {
            using (var embeddedStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(AndroidTemplateProjectResourceName))
            {
                if (embeddedStream == null)
                    throw new InvalidOperationException(AndroidTemplateProjectResourceName + " was not found");
                //let's generate new project each time the plugin is calledm (to avoid file locking)
                //TODO: clean up
                using (var archive = new ZipArchive(embeddedStream, ZipArchiveMode.Read))
                {
                    archive.ExtractToDirectory(TemplateDirectory);
                }
            }
        }
    }
}