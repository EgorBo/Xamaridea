using System;
using System.IO;
using System.Linq;
using Xamaridea.Core.Exceptions;

namespace Xamaridea.Core.Extensions
{
    public static class FileExtensions
    {
        public static bool HasUppercaseChars(this string str)
        {
            return str.Any(char.IsUpper);
        }

        public static void RenameFileExtensionAndMakeLowercase(string path, string extension)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            try
            {
                var parentDir = Path.GetDirectoryName(path);
                var file = Path.GetFileName(path);
                var newPath = Path.Combine(parentDir, Path.ChangeExtension(file.ToLower(), extension));
                File.Move(path, newPath);
            }
            catch (Exception exc)
            {
                throw new FileRenameToLowercaseException(path, exc);
            }
        }

        public static void RenameFileOrFolderToLowercase(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            var parentDir = Path.GetDirectoryName(path);
            var dirName = Path.GetFileName(path);
            var pathToLowercaseDir = Path.Combine(parentDir, dirName.ToLower());
            var tempPath = path + "_temp"; //since NTFS is case insensitive

            try
            {
                if (IsFolder(path))
                {
                    Directory.Move(path, tempPath);
                    Directory.Move(tempPath, pathToLowercaseDir);
                }
                else
                {
                    File.Move(path, tempPath);
                    File.Move(tempPath, pathToLowercaseDir);
                }
            }
            catch (Exception exc)
            {
                throw new FileRenameToLowercaseException(path, exc);
            }
        }

        public static bool IsFolder(string path)
        {
            FileAttributes attr = File.GetAttributes(path);
            return (attr & FileAttributes.Directory) == FileAttributes.Directory;
        }
    }
}
