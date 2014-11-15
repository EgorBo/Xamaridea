using System;
using System.IO;
using System.Linq;

namespace Xamaridea.Core
{
    public static class AndroidIdeDetector
    {
        public static string TryFindIdeaPath()
        {
            try
            {
                var jetBrainsFolders = Directory.GetDirectories(@"C:\Program Files (x86)\JetBrains");
                var ideaDir = jetBrainsFolders
                    .Where(dir => Path.GetFileName(dir).StartsWith("IntelliJ IDEA") && File.Exists(Path.Combine(dir, @"bin\idea.exe")))
                    .OrderByDescending(i => i)
                    .FirstOrDefault();
                return Path.Combine(ideaDir, @"bin\idea.exe");
            }
            catch (Exception exc)
            {
                return null;
            }
        }
    }
}
