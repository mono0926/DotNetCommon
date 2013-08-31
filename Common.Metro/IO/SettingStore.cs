using System;
using Windows.Storage;

namespace Mono.Framework.Common.IO
{

    public class SettingStore : ISettingStore
    {
        private static readonly ApplicationDataContainer Settings = ApplicationData.Current.LocalSettings;
        private static readonly ApplicationDataContainer RoamingSettings = ApplicationData.Current.RoamingSettings;
        
        public void Save(string key, object value, bool roaming = false)
        {
            var t = roaming ? RoamingSettings : Settings;
            t.Values[key] = value;
        }
        public void SaveAsEnum(string key, object value, bool roaming = false)
        {
            Save(key, (int)value, roaming);
        }

        public T Load<T>(string key, out bool found, bool roaming = false)
        {
            var v = CheckV(key, out found, roaming);
            return v == null ? default(T) : (T)Convert.ChangeType(v, typeof(T));
        }
        public T Load<T>(string key, bool roaming = false)
        {
            var t = roaming ? RoamingSettings : Settings;
            var v = t.Values[key];
            return v == null ? default(T) : (T)Convert.ChangeType(v, typeof(T));
        }

        public T LoadAsEnum<T>(string key, out bool found, bool roaming = false)
        {
            var v = CheckV(key, out found, roaming);
            return v == null ? default(T) : (T)Enum.Parse(typeof(T), Convert.ToString(v));
        }

        private static object CheckV(string key, out bool found, bool roaming = false)
        {
            var t = roaming ? RoamingSettings : Settings;
            found = false;
            var v = t.Values[key];
            if (v != null)
            {
                found = true;
            }
            return v;
        }

    }
}
