using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GimbalUltra.ModCompiler
{
    class Config : IDictionary<string, object>
    {
        Dictionary<string, object> Data = new Dictionary<string, object>();

        public T Get<T>(string Key, T def)
        {
            if(Data.ContainsKey(Key))
            {
                var v = Data[Key];
                if(v is T)
                {
                    return (T)v;
                }
                return (T)Convert.ChangeType(v, typeof(T));
            }
            return def;
        }

        public Config()
        {
        }

        public Config(Dictionary<string, object> Data)
        {
            this.Data = Data;
        }

        public object this[string key]
        {
            get
            {
                return Data[key];
            }

            set
            {
                Data[key] = value;
            }
        }

        public int Count
        {
            get
            {
                return Data.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return Data.Keys;
            }
        }

        public ICollection<object> Values
        {
            get
            {
                return Data.Values;
            }
        }

        public void Add(KeyValuePair<string, object> item)
        {
            Data.Add(item.Key, item.Value);
        }

        public void Add(string key, object value)
        {
            Data.Add(key, value);
        }

        public void Clear()
        {
            Data.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return Data.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return Data.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return Data.Remove(item.Key);
        }

        public bool Remove(string key)
        {
            return Data.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return Data.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Data.GetEnumerator();
        }
    }
}
