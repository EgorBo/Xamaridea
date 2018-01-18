using Xamaridea.Core;

namespace Xamaridea.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            TestAsync();
            System.Console.ReadKey();
        }

        private static async void TestAsync()
        {
            //Just for test without integration to VS:
            string androidStudio = @"C:\Program Files\Android\Android Studio\bin\studio64.exe";
            string testXamarinProject = @"C:\Users\Ghalani 1\Documents\Projects\MESTKitchen\MESTKitchenDroid\MESTKitchenDroid.csproj";

            var projectsSynchronizer = new ProjectsSynchronizer(testXamarinProject, androidStudio);
            await projectsSynchronizer.MakeResourcesSubdirectoriesAndFilesLowercase(async () =>
                {
                    System.Console.WriteLine("Permissions to change original project has been requested. Granted.");
                    return true;
                });
            projectsSynchronizer.Sync();
        }
    }
}