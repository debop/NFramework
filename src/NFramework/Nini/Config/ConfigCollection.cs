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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Nini.Config {
    public delegate void ConfigEventHandler(object sender, ConfigEventArgs e);

    public class ConfigEventArgs : EventArgs {
        public ConfigEventArgs(IConfig config) {
            Config = config;
        }

        public IConfig Config { get; private set; }
    }

    public class ConfigCollection : IList {
        private readonly List<IConfig> configList = new List<IConfig>();
        private readonly ConfigSourceBase owner = null;

        public ConfigCollection(ConfigSourceBase owner) {
            this.owner = owner;
        }

        public int Count {
            get { return configList.Count; }
        }

        public bool IsSynchronized {
            get { return false; }
        }

        public object SyncRoot {
            get { return this; }
        }

        public IConfig this[int index] {
            get { return configList[index]; }
        }

        object IList.this[int index] {
            get { return configList[index]; }
            set { }
        }

        public IConfig this[string configName] {
            get { return configList.FirstOrDefault(config => config.Name == configName); }
        }

        public bool IsFixedSize {
            get { return false; }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        public void Add(IConfig config) {
            if(configList.Contains(config))
                throw new ArgumentException("IConfig already exists");

            var existingConfig = this[config.Name];

            if(existingConfig != null) {
                // Set all new keys
                var keys = config.GetKeys();
                for(var i = 0; i < keys.Length; i++) {
                    existingConfig.Set(keys[i], config.Get(keys[i]));
                }
            }
            else {
                configList.Add(config);
                OnConfigAdded(new ConfigEventArgs(config));
            }
        }

        int IList.Add(object config) {
            var newConfig = config as IConfig;

            Guard.Assert(() => newConfig != null, "Must be an IConfig");

            Add(newConfig);
            return IndexOf(newConfig);
        }

        public IConfig Add(string name) {
            ConfigBase result = null;

            if(this[name] == null) {
                result = new ConfigBase(name, owner);
                configList.Add(result);
                OnConfigAdded(new ConfigEventArgs(result));
            }
            else {
                throw new ArgumentException("An IConfig of that name already exists");
            }

            return result;
        }

        public void Remove(IConfig config) {
            configList.Remove(config);
            OnConfigRemoved(new ConfigEventArgs(config));
        }

        public void Remove(object config) {
            configList.Remove((IConfig)config);
            OnConfigRemoved(new ConfigEventArgs((IConfig)config));
        }

        public void RemoveAt(int index) {
            var config = configList[index];
            configList.RemoveAt(index);
            OnConfigRemoved(new ConfigEventArgs(config));
        }

        public void Clear() {
            configList.Clear();
        }

        public IEnumerator GetEnumerator() {
            return configList.GetEnumerator();
        }

        public void CopyTo(Array array, int index) {
            configList.CopyTo((IConfig[])array, index);
        }

        public void CopyTo(IConfig[] array, int index) {
            ((ICollection)configList).CopyTo(array, index);
        }

        public bool Contains(object config) {
            return configList.Contains((IConfig)config);
        }

        public int IndexOf(object config) {
            return configList.IndexOf((IConfig)config);
        }

        public void Insert(int index, object config) {
            configList.Insert(index, (IConfig)config);
        }

        public event ConfigEventHandler ConfigAdded;

        public event ConfigEventHandler ConfigRemoved;

        protected void OnConfigAdded(ConfigEventArgs e) {
            if(ConfigAdded != null)
                ConfigAdded(this, e);
        }

        protected void OnConfigRemoved(ConfigEventArgs e) {
            if(ConfigRemoved != null)
                ConfigRemoved(this, e);
        }
    }
}