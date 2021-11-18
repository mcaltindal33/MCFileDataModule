using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace MC.FileData.Module.BusinessObjects
{
    [DefaultProperty(nameof(FileName))]
    public class FileSystemObject : BaseObject, IFileData,IEmptyCheckable
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public FileSystemObject(Session session) : base(session) { }
        public override string ToString()
        {
            return FileName;
        }

        public void LoadFromStream(string fileName, Stream stream)
        {
            //FileDataModule.Authorize();

            Guard.ArgumentNotNull(stream, "stream");
            FileName = fileName;
            Size = Convert.ToInt32(stream.Length);
            FileSize = stream.Length;
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            Folder = DateTime.Today.Date.ToString();

            FileId =  System.Threading.Tasks.Task.Run(() => FileDataModule.UploadFile(FileName, DateTime.Today.Date.ToString(), stream)).Result;
        }

        public void SaveToStream(Stream stream)
        {
            //FileDataModule.Authorize();

            var result = System.Threading.Tasks.Task.Run(() => FileDataModule.DownloadFile(FileId, FileSize)).Result;
            result.Position = 0;
            result.Seek(0, SeekOrigin.Begin);
            result.CopyTo(stream);
        }

        public void Clear()
        {
            FileName = string.Empty;
            FileId = string.Empty;
            FileDataModule.DeleteFile(FileId);
        }

        long fFileSize;
        string fFolder;
        string fFileId;
        string fFileName;
        int fSize;

        [Size(260)]
        public string FileName
        {
            get => fFileName;
            set => SetPropertyValue(nameof(FileName), ref fFileName, value);
        }

        [Size(250)]
        [Browsable(false)]
        public string FileId
        {
            get => fFileId;
            set => SetPropertyValue(nameof(FileId), ref fFileId, value);
        }
        
        public long FileSize
        {
            get => fFileSize;
            set => SetPropertyValue(nameof(FileSize), ref fFileSize, value);
        }

        public string Folder
        {
            get => fFolder;
            set => SetPropertyValue(nameof(Folder), ref fFolder, value);
        }
        
        public int Size
        {
            get => fSize;
            set => SetPropertyValue(nameof(Size), ref fSize, value);
        }

        [NonPersistent,MemberDesignTimeVisibility(false)]
        public bool IsEmpty => string.IsNullOrEmpty(FileName);
    }
}