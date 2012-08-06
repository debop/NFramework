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
using Microsoft.Win32;

namespace NSoft.NFramework.Nini.Config {

    #region RegistryRecurse enumeration

    public enum RegistryRecurse {
        None,
        Flattened,
        Namespacing
    }

    #endregion

    public class RegistryConfigSource : ConfigSourceBase {
        public RegistryConfigSource() {
            DefaultKey = null;
        }

        public RegistryKey DefaultKey { get; set; }

        public override IConfig AddConfig(string name) {
            if(DefaultKey == null)
                throw new ApplicationException("You must set DefaultKey");

            return AddConfig(name, DefaultKey);
        }

        public IConfig AddConfig(string name, RegistryKey key) {
            RegistryConfig result = new RegistryConfig(name, this);
            result.Key = key;
            result.ParentKey = true;

            Configs.Add(result);

            return result;
        }

        public void AddMapping(RegistryKey registryKey, string path) {
            var key = registryKey.OpenSubKey(path, true);

            if(key == null)
                throw new ArgumentException("The specified key does not exist");

            LoadKeyValues(key, ShortKeyName(key));
        }

        public void AddMapping(RegistryKey registryKey, string path, RegistryRecurse recurse) {
            RegistryKey key = registryKey.OpenSubKey(path, true);

            if(key == null)
                throw new ArgumentException("The specified key does not exist");

            if(recurse == RegistryRecurse.Namespacing) {
                LoadKeyValues(key, path);
            }
            else {
                LoadKeyValues(key, ShortKeyName(key));
            }

            var subKeys = key.GetSubKeyNames();

            for(var i = 0; i < subKeys.Length; i++) {
                switch(recurse) {
                    case RegistryRecurse.None:
                        // no recursion
                        break;
                    case RegistryRecurse.Namespacing:
                        AddMapping(registryKey, path + "\\" + subKeys[i], recurse);
                        break;
                    case RegistryRecurse.Flattened:
                        AddMapping(key, subKeys[i], recurse);
                        break;
                }
            }
        }

        public override void Save() {
            MergeConfigsIntoDocument();

            for(var i = 0; i < Configs.Count; i++) {
                // New merged configs are not RegistryConfigs
                if(Configs[i] is RegistryConfig) {
                    var config = (RegistryConfig)Configs[i];
                    var keys = config.GetKeys();

                    for(var j = 0; j < keys.Length; j++) {
                        config.Key.SetValue(keys[j], config.Get(keys[j]));
                    }
                }
            }
        }

        public override void Reload() {
            ReloadKeys();
        }

        /// <summary>
        /// Loads all values from the registry key.
        /// </summary>
        private void LoadKeyValues(RegistryKey key, string keyName) {
            var config = new RegistryConfig(keyName, this) { Key = key };

            var values = key.GetValueNames();
            foreach(var value in values) {
                config.Add(value, key.GetValue(value).ToString());
            }
            Configs.Add(config);
        }

        /// <summary>
        /// Merges all of the configs from the config collection into the 
        /// registry.
        /// </summary>
        private void MergeConfigsIntoDocument() {
            foreach(IConfig config in Configs) {
                if(config is RegistryConfig) {
                    RegistryConfig registryConfig = (RegistryConfig)config;

                    if(registryConfig.ParentKey) {
                        registryConfig.Key =
                            registryConfig.Key.CreateSubKey(registryConfig.Name);
                    }
                    RemoveKeys(registryConfig);

                    string[] keys = config.GetKeys();
                    for(var i = 0; i < keys.Length; i++) {
                        registryConfig.Key.SetValue(keys[i], config.Get(keys[i]));
                    }
                    registryConfig.Key.Flush();
                }
            }
        }

        /// <summary>
        /// Reloads all keys.
        /// </summary>
        private void ReloadKeys() {
            var keys = new RegistryKey[Configs.Count];

            for(int i = 0; i < keys.Length; i++) {
                keys[i] = ((RegistryConfig)Configs[i]).Key;
            }

            Configs.Clear();
            for(var i = 0; i < keys.Length; i++) {
                LoadKeyValues(keys[i], ShortKeyName(keys[i]));
            }
        }

        /// <summary>
        /// Removes all keys not present in the current config.  
        /// </summary>
        private void RemoveKeys(RegistryConfig config) {
            foreach(string valueName in config.Key.GetValueNames()) {
                if(!config.Contains(valueName)) {
                    config.Key.DeleteValue(valueName);
                }
            }
        }

        /// <summary>
        /// Returns the key name without the fully qualified path.
        /// e.g. no HKEY_LOCAL_MACHINE\\MyKey, just MyKey
        /// </summary>
        private string ShortKeyName(RegistryKey key) {
            var index = key.Name.LastIndexOf("\\");

            return (index == -1) ? key.Name : key.Name.Substring(index + 1);
        }

        /// <summary>
        /// Registry Config class.
        /// </summary>
        private class RegistryConfig : ConfigBase {
            /// <summary>
            /// Constructor.
            /// </summary>
            public RegistryConfig(string name, IConfigSource source) : base(name, source) {
                Key = null;
                ParentKey = false;
            }

            /// <summary>
            /// Gets or sets whether the key is a parent key. 
            /// </summary>
            public bool ParentKey { get; set; }

            /// <summary>
            /// Registry key for the Config.
            /// </summary>
            public RegistryKey Key { get; set; }
        }
    }
}