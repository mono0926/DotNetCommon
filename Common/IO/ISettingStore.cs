using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mono.Framework.Common.IO
{
    public interface ISettingStore
    {
        void Save(string key, object value, bool roaming = false);
        void SaveAsEnum(string key, object value, bool roaming = false);
        T Load<T>(string key, out bool found, bool roaming = false);
        T Load<T>(string key, bool roaming = false);
        T LoadAsEnum<T>(string key, out bool found, bool roaming = false);
    }
}
