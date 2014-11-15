using Xamaridea.Core;

namespace Xamaridea.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            TestAsync();
            System.Console.ReadKey();
        }

        private static async void TestAsync()
        {
            //Just for test without integration to VS:
            string idea = @"C:\Program Files (x86)\JetBrains\IntelliJ IDEA Community Edition 13.1.5\bin\idea.exe";
            string androidStudio = @"C:\Users\Egorbo\Downloads\android-studio-ide-135.1538390-windows\android-studio\bin\studio64.exe";
            string testXamarinProject = @"C:\Users\Egorbo\Documents\Visual Studio 2013\Projects\App17\App17\App17.csproj";

            var projectsSynchronizer = new ProjectsSynchronizer(testXamarinProject, androidStudio);
            await projectsSynchronizer.MakeResourcesSubdirectoriesAndFilesLowercase(async () =>
                {
                    System.Console.WriteLine("Permissions to change original projec has been requested. Granted.");
                    return true;
                });
            projectsSynchronizer.Sync();
        }
    }
}
