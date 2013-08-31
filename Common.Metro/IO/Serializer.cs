using System;
using System.Threading.Tasks;
using System.IO;
using Windows.Storage;
using System.Xml.Linq;
using Windows.Storage.Streams;
using System.Threading;
using System.Diagnostics;

namespace Mono.Framework.Common.IO
{
    public class Serializer : ISerializer
    {
        private static readonly StorageFolder LocalFolder = ApplicationData.Current.LocalFolder;
        private static readonly StorageFolder RoamingFolder = ApplicationData.Current.RoamingFolder;

        public async Task SaveXml(string filename, XElement elem, bool roaming = false)
        {
            var folder = roaming ? RoamingFolder : LocalFolder;
            var file = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            doc.Add(elem);
            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                doc.Save(stream.AsStreamForWrite());
            }
        }

        public async Task<XDocument> LoadXml(string filename, bool roaming = false)
        {
            var folder = roaming ? RoamingFolder : LocalFolder;
            try
            {
                var file = await folder.GetFileAsync(filename);
                using (var stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    return XDocument.Load(stream.AsStreamForRead());
                }
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        public async Task<string> ReadFile(string foldername, string filename, bool roaming = false)
        {
            IStorageFolder folder = null;
            if (foldername == null)
            {
                folder = roaming ? RoamingFolder : LocalFolder;
            }
            else
            {
                folder = await GetOrCreateFolder(foldername, roaming: roaming);   
            }
            try
            {
                var file = await folder.GetFileAsync(filename);
                using (var fs = await file.OpenAsync(FileAccessMode.Read))
                {
                    using (var inStream = fs.GetInputStreamAt(0))
                    {
                        var reader = new DataReader(inStream);
                        await reader.LoadAsync((uint)fs.Size);
                        var data = reader.ReadString((uint)fs.Size);
                        reader.DetachStream();
                        return data;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static readonly SemaphoreSlim _delete = new SemaphoreSlim(initialCount: 1);

        public async Task DeleteFile(string foldername, string filename, bool roaming = false)
        {
            var folder = await GetOrCreateFolder(foldername, roaming: roaming);
            await _delete.WaitAsync();
            try
            {
                var file = await folder.GetFileAsync(filename);
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
            catch (Exception)
            {
                return;
            }
            finally
            {
                _delete.Release();
            }
        }


        public async void WriteFile(string foldername, string filename, string contents, bool roaming = false)
        {
            try
            {

                var folder = await GetOrCreateFolder(foldername, roaming: roaming);
                var file = await folder.CreateFileAsync(filename, Windows.Storage.CreationCollisionOption.ReplaceExisting);
                using (var fs = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    using (var outStream = fs.GetOutputStreamAt(0))
                    {
                        var dataWriter = new DataWriter(outStream);
                        dataWriter.WriteString(contents);
                        await dataWriter.StoreAsync();
                        dataWriter.DetachStream();
                        await outStream.FlushAsync();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private static readonly SemaphoreSlim _sl = new SemaphoreSlim(initialCount: 1);

        public async Task<IStorageFolder> GetOrCreateFolder(string folderName, bool roaming = false)
        {
            var folder = roaming ? RoamingFolder : LocalFolder;
            await _sl.WaitAsync();
            IStorageFolder r = null;
            try
            {
                r = await folder.GetFolderAsync(folderName);
            }
            catch (FileNotFoundException)
            {
            }
            if (r == null)
            {
                r = await folder.CreateFolderAsync(folderName);
            }
            _sl.Release();
            return r;
        }
    }
}
