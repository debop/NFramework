#region Copyright

//
// Nini Configuration Project.
// Copyright (C) 2006 Brent R. Matzelle.  All rights reserved.
//
// This software is published under the terms of the MIT X11 license, a copy of 
// which has been included with this distribution in the LICENSE.txt file.
// 

#endregion

using System;
using System.Globalization;
using NSoft.NFramework.Nini.Util;

namespace NSoft.NFramework.Nini.Config {
    public delegate void ConfigKeyEventHandler(object sender, ConfigKeyEventArgs e);

    public class ConfigKeyEventArgs : EventArgs {
        public ConfigKeyEventArgs(string keyName, string keyValue) {
            KeyName = keyName;
            KeyValue = keyValue;
        }

        public string KeyName { get; private set; }

        public string KeyValue { get; private set; }
    }

    public class ConfigBase : IConfig {
        private string _configName = null;
        private readonly IFormatProvider _format = NumberFormatInfo.CurrentInfo;

        protected OrderedList keys = new OrderedList();

        public ConfigBase(string name, IConfigSource source) {
            _configName = name;
            ConfigSource = source;

            Alias = new AliasText();
        }

        public string Name {
            get { return _configName; }
            set {
                if(_configName != value) {
                    Rename(value);
                }
            }
        }

        public IConfigSource ConfigSource { get; private set; }

        public AliasText Alias { get; private set; }

        public bool Contains(string key) {
            return (Get(key) != null);
        }

        public virtual string Get(string key) {
            string result = null;

            if(keys.Contains(key)) {
                result = keys[key].ToString();
            }

            return result;
        }

        public string Get(string key, string defaultValue) {
            string result = Get(key);

            return result ?? defaultValue;
        }

        public string GetExpanded(string key) {
            return ConfigSource.GetExpanded(this, key);
        }

        public string GetString(string key) {
            return Get(key);
        }

        public string GetString(string key, string defaultValue) {
            return Get(key, defaultValue);
        }

        public int GetInt(string key) {
            var text = Get(key);

            if(text == null)
                throw new ArgumentException("Value not found: " + key);

            return Convert.ToInt32(text, _format);
        }

        public int GetInt(string key, bool fromAlias) {
            if(!fromAlias) {
                return GetInt(key);
            }

            string result = Get(key);

            if(result == null) {
                throw new ArgumentException("Value not found: " + key);
            }

            return GetIntAlias(key, result);
        }

        public int GetInt(string key, int defaultValue) {
            string result = Get(key);

            return (result == null)
                       ? defaultValue
                       : Convert.ToInt32(result, _format);
        }

        public int GetInt(string key, int defaultValue, bool fromAlias) {
            if(!fromAlias) {
                return GetInt(key, defaultValue);
            }

            string result = Get(key);

            return (result == null) ? defaultValue : GetIntAlias(key, result);
        }

        public long GetLong(string key) {
            string text = Get(key);

            if(text == null) {
                throw new ArgumentException("Value not found: " + key);
            }

            return Convert.ToInt64(text, _format);
        }

        public long GetLong(string key, long defaultValue) {
            string result = Get(key);

            return (result == null)
                       ? defaultValue
                       : Convert.ToInt64(result, _format);
        }

        public bool GetBoolean(string key) {
            string text = Get(key);

            if(text == null)
                throw new ArgumentException("Value not found: " + key);

            return GetBooleanAlias(text);
        }

        public bool GetBoolean(string key, bool defaultValue) {
            string text = Get(key);

            return (text == null) ? defaultValue : GetBooleanAlias(text);
        }

        public float GetFloat(string key) {
            string text = Get(key);

            if(text == null)
                throw new ArgumentException("Value not found: " + key);

            return Convert.ToSingle(text, _format);
        }

        public float GetFloat(string key, float defaultValue) {
            string result = Get(key);

            return (result == null)
                       ? defaultValue
                       : Convert.ToSingle(result, _format);
        }

        public double GetDouble(string key) {
            string text = Get(key);

            if(text == null)
                throw new ArgumentException("Value not found: " + key);

            return Convert.ToDouble(text, _format);
        }

        public double GetDouble(string key, double defaultValue) {
            string result = Get(key);

            return (result == null)
                       ? defaultValue
                       : Convert.ToDouble(result, _format);
        }

        public string[] GetKeys() {
            string[] result = new string[keys.Keys.Count];

            keys.Keys.CopyTo(result, 0);

            return result;
        }

        public string[] GetValues() {
            string[] result = new string[keys.Values.Count];

            keys.Values.CopyTo(result, 0);

            return result;
        }

        public void Add(string key, string value) {
            keys.Add(key, value);
        }

        public virtual void Set(string key, object value) {
            value.ShouldNotBeNull("value");

            if(Get(key) == null) {
                Add(key, value.ToString());
            }
            else {
                keys[key] = value.ToString();
            }

            if(ConfigSource.AutoSave) {
                ConfigSource.Save();
            }

            OnKeySet(new ConfigKeyEventArgs(key, value.ToString()));
        }

        public virtual void Remove(string key) {
            key.ShouldNotBeNull("key");

            if(Get(key) != null) {
                string keyValue = null;
                if(KeySet != null) {
                    keyValue = Get(key);
                }
                keys.Remove(key);

                OnKeyRemoved(new ConfigKeyEventArgs(key, keyValue));
            }
        }

        public event ConfigKeyEventHandler KeySet;
        public event ConfigKeyEventHandler KeyRemoved;

        protected void OnKeySet(ConfigKeyEventArgs e) {
            if(KeySet != null) {
                KeySet(this, e);
            }
        }

        protected void OnKeyRemoved(ConfigKeyEventArgs e) {
            if(KeyRemoved != null) {
                KeyRemoved(this, e);
            }
        }

        /// <summary>
        /// Renames the config to the new name. 
        /// </summary>
        private void Rename(string name) {
            ConfigSource.Configs.Remove(this);
            _configName = name;
            ConfigSource.Configs.Add(this);
        }

        /// <summary>
        /// Returns the integer alias first from this IConfig then 
        /// the parent if there is none.
        /// </summary>
        private int GetIntAlias(string key, string alias) {
            return Alias.ContainsInt(key, alias)
                       ? Alias.GetInt(key, alias)
                       : ConfigSource.Alias.GetInt(key, alias);
        }

        /// <summary>
        /// Returns the boolean alias first from this IConfig then 
        /// the parent if there is none.
        /// </summary>
        private bool GetBooleanAlias(string key) {
            bool result = false;

            if(Alias.ContainsBoolean(key)) {
                result = Alias.GetBoolean(key);
            }
            else {
                if(ConfigSource.Alias.ContainsBoolean(key)) {
                    result = ConfigSource.Alias.GetBoolean(key);
                }
                else {
                    throw new ArgumentException("Alias value not found: " + key + ". Add it to the Alias property.");
                }
            }

            return result;
        }
    }
}