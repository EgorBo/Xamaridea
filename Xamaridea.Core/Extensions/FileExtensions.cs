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
            
            if (dirName == dirName.ToLower())
                return; //it's already in lowercase

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

        public static void DirectoryCopy(string sourceDirName, string destDirName)
        {
            var dir = new DirectoryInfo(sourceDirName);
            var dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
            }

            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            var files = dir.GetFiles();
            foreach (var file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            foreach (var subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath);
            }
        }
    }
}
