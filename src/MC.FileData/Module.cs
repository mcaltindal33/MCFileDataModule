using System;
using System.Text;
using System.Linq;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Xpo;
using System.Threading.Tasks;

namespace MC.FileData {
    // For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ModuleBase.
    [Description("This module provides the FileSystemFolder and FileSystemObject classes that enable you to store uploaded files in a file system instead of the database.")]
    public sealed partial class FileDataModule : ModuleBase {

        private static GoogleDriveAPI DriveApi;

        public FileDataModule() {
            InitializeComponent();
            BaseObject.OidInitializationMode = OidInitializationMode.AfterConstruction;
        }

        //Sistem Authorized Olur
        public static void Authorize()
        {
            DriveApi = GoogleDriveAPI.GetInstance();
            try
            {
                DriveApi.Authorize();
            }
            catch 
            {
            }
        }

        // Root Klasörünün ID Değeri Döner
        public static string GetRootFolderId()
        {
            return DriveApi.GetRootID();
        }

        public static async Task<string> UploadFile(string file,string folderId,System.IO.Stream stream)
        {
            Google.Apis.Drive.v3.Data.File uploadFile = await DriveApi.UploadFile(file, folderId,stream);
            return uploadFile.Id;
        }

        public static async Task<System.IO.Stream> DownloadFile(string fileId,long fileSize)
        {
            return await DriveApi.DownloadFile(fileId,fileSize);
        }
        public static string CreateFolder(string folderName)
        {
            return DriveApi.CreateFolderAndGetID(folderName);
        }

        public static void DeleteFile(string fileId)
        {
            var result = System.Threading.Tasks.Task.Run(() => DriveApi.DeleteFile(fileId));
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            return new ModuleUpdater[] { updater };
        }
        public override void Setup(XafApplication application) {
            base.Setup(application);
            // Manage various aspects of the application UI and behavior at the module level.
        }
        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            CalculatedPersistentAliasHelper.CustomizeTypesInfo(typesInfo);
        }
        
    }
}
