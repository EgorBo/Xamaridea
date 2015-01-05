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
                const string androidStudioPath = @"C:\Program Files\Android\Android Studio\bin\studio64.exe";
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
