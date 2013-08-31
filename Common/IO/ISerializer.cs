using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
namespace Mono.Framework.Common.IO
{
    public interface ISerializer
    {
        Task SaveXml(string filename, XElement elem, bool roaming = false);
        Task<XDocument> LoadXml(string filename, bool roaming = false);
        Task<string> ReadFile(string foldername, string filename, bool roaming = false);
        Task DeleteFile(string foldername, string filename, bool roaming = false);
        void WriteFile(string foldername, string filename, string contents, bool roaming = false);
    }
}
