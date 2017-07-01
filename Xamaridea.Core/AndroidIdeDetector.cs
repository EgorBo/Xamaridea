using System;
using System.IO;
using System.Linq;

namespace Xamaridea.Core
{
    public static class AndroidIdeDetector
    {
        public static string TryFindIdePath()
        {
            try
            {
                var progFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles).Replace("(x86)", "").TrimEnd();
                var progFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

                string[] androidStudioPaths = new string[] 
                    {
                        Path.Combine(progFiles, @"Android\Android Studio\bin\studio64.exe"),
                        Path.Combine(progFiles, @"Android\Android Studio\bin\studio.exe"),
                        Path.Combine(progFilesX86, @"Android\Android Studio\bin\studio.exe"),
                        Path.Combine(progFilesX86, @"Android\Android Studio\bin\studio64.exe"),
                    };

                var androidStudioPath = androidStudioPaths.FirstOrDefault(p => File.Exists(p));
                if (File.Exists(androidStudioPath))
                    return androidStudioPath;

                var jetBrainsFolders = Directory.GetDirectories(@"C:\Program Files (x86)\JetBrains");
                var ideaDir = jetBrainsFolders
                    .Where(dir => Path.GetFileName(dir).StartsWith("IntelliJ IDEA") && File.Exists(Path.Combine(dir, @"bin\idea.exe")))
                    .OrderByDescending(i => i)
                    .FirstOrDefault();

                if (ideaDir != null)
                    return Path.Combine(ideaDir, @"bin\idea.exe");

                return null;
            }
            catch (Exception exc)
            {
                return null;
            }
        }
    }
}
