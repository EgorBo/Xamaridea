using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Xamaridea.Core;
using Xamaridea.Core.Exceptions;

namespace EgorBo.Xamaridea_VisualStudioPlugin
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad("f1536ef8-92ec-443c-9ed7-fdadf150da82")] //UICONTEXT_SolutionExists
    [Guid(GuidList.guidXamaridea_VisualStudioPluginPkgString)]
    public sealed class Xamaridea_VisualStudioPluginPackage : Package
    {
        private static readonly string[] FileExtensions = { ".axml", ".xml" };
        private static readonly string[] FolderNames = { "Resources", "drawable", "layout", "values", "layout", "animator", "anim", "color", "menu", "raw", "xml" };

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public Xamaridea_VisualStudioPluginPackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }

        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Debug.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            var mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null != mcs )
            {
                var menuCommandId = new CommandID(GuidList.guidXamaridea_VisualStudioPluginCmdSet, (int)PkgCmdIDList.cmdidOpenInIdeaCommand);
                var menuItem = new OleMenuCommand(MenuItemCallback, menuCommandId);
                menuItem.BeforeQueryStatus += MenuItem_OnBeforeQueryStatus;
                mcs.AddCommand(menuItem);

                //Add settings command
                var settingsMenuCommandId = new CommandID(GuidList.guidXamaridea_VisualStudioPluginCmdSet, (int)PkgCmdIDList.cmdidOpenInIdeaSettingsCommand);
                var settingsMenuItem = new MenuCommand(SettingsMenuItemCallback, settingsMenuCommandId);
                mcs.AddCommand(settingsMenuItem);
            }
        }

        private void SettingsMenuItemCallback(object sender, EventArgs e)
        {
            var dlg = new ConfigurationDialog();
            dlg.ShowModal();
        }

        #endregion
        
        private void MenuItem_OnBeforeQueryStatus(object sender, EventArgs e)
        {
            bool enable = false;
            var menu = sender as OleMenuCommand;
            var envDte = GetService(typeof(DTE)) as DTE;
            var selectedItems = envDte.SelectedItems.OfType<SelectedItem>().ToArray();
            if (selectedItems.Length == 1)
            {
                var selectedItem = selectedItems[0].ProjectItem;
                var kind = selectedItem.Kind;
                Guid kindId;
                if (Guid.TryParse(kind, out kindId))
                {
                    var isFile = VSConstants.GUID_ItemType_PhysicalFile == kindId;
                    var isFolder = VSConstants.GUID_ItemType_PhysicalFolder == kindId;

                    if (isFile)
                    {
                        var extension = Path.GetExtension(selectedItem.Name);
                        if (DoesContain(extension, FileExtensions))
                        {
                            enable = true;
                        }
                    }
                    else if (isFolder)
                    {
                        if (DoesContain(selectedItem.Name, FolderNames))
                        {
                            enable = true;
                        }
                    }
                }
            }
            menu.Visible = enable;
        }
        private static bool DoesContain(string item, IEnumerable<string> strings)
        {
            return strings.Any(s => string.Equals(item, s, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private async void MenuItemCallback(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Settings.Default.AnidePath))
            {
                var dlg = new ConfigurationDialog();
                dlg.ShowModal();
            }

            if (string.IsNullOrEmpty(Settings.Default.AnidePath))
                return;

            try
            {
                var envDte = GetService(typeof(DTE)) as DTE;
                var selectedItems = envDte.SelectedItems.OfType<SelectedItem>().ToArray();
                var project = selectedItems[0].ProjectItem.ContainingProject; //it should have at least one item
                var fileName = project.FileName;

                var synchronizer = new ProjectsSynchronizer(fileName, Settings.Default.AnidePath);
                await synchronizer.MakeResourcesSubdirectoriesAndFilesLowercase(async () => AskPermissionToChangeCsProj());
                synchronizer.Sync();
                project.Save();
            }
            catch (OperationCanceledException)
            {
            }
            catch (CsprojEditFailedException exc)
            {
                ShowErrorDuringSync(".csproj edit failed: {0} ", exc);
            }
            catch (FileRenameToLowercaseException exc)
            {
                ShowErrorDuringSync("Renaming {0} to lowercase failed: {1}", exc.FileName, exc);
            }
            catch (Exception exc)
            {
                ShowErrorDuringSync("General failure: {0}", exc);
            }
        }

        private bool AskPermissionToChangeCsProj()
        {
            var uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            Guid clsid = Guid.Empty;
            int result;
            ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                       0, ref clsid, "Xamaridea",
                       "Android project requires your files and directories under 'Resources' folder to be in lowercase and have extension \".xml\" instead of \".axml\" - can Xamaridea automaticaly change them in your project? (possibly you will need to fix in code cases like Resources.Layout.Main to Resources.Layout.main).",
                       string.Empty, 0, OLEMSGBUTTON.OLEMSGBUTTON_YESNO,
                       OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                       OLEMSGICON.OLEMSGICON_QUERY, 0, out result));
            return result == 6; //TODO: find defined constant
        }
        
        private void ShowErrorDuringSync(string errorFormat, params object[] args)
        {
            var uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            Guid clsid = Guid.Empty;
            int result;
            ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(0, ref clsid, "Xamaridea", string.Format(errorFormat, args), string.Empty, 0, 
                OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_CRITICAL, 0, out result));
        }
    }
}
