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
using System.IO;
using NSoft.NFramework.Nini.Ini;

namespace NSoft.NFramework.Nini.Config {
    public class IniConfigSource : ConfigSourceBase {
        private IniDocument iniDocument;

        public IniConfigSource() {
            CaseSensitive = true;
            iniDocument = new IniDocument();
        }

        public IniConfigSource(string filePath) {
            CaseSensitive = true;
            Load(filePath);
        }

        public IniConfigSource(TextReader reader) {
            CaseSensitive = true;
            Load(reader);
        }

        public IniConfigSource(IniDocument document) {
            CaseSensitive = true;
            Load(document);
        }

        public IniConfigSource(Stream stream) {
            CaseSensitive = true;
            Load(stream);
        }

        public bool CaseSensitive { get; set; }

        public string SavePath { get; private set; }

        public void Load(string filePath) {
            Load(new StreamReader(filePath));
            SavePath = filePath;
        }

        public void Load(TextReader reader) {
            Load(new IniDocument(reader));
        }

        public void Load(IniDocument document) {
            Configs.Clear();

            Merge(this); // required for SaveAll
            iniDocument = document;
            Load();
        }

        public void Load(Stream stream) {
            Load(new StreamReader(stream));
        }

        public override void Save() {
            if(!IsSavable())
                throw new ArgumentException("Source cannot be saved in this state");

            MergeConfigsIntoDocument();

            iniDocument.Save(SavePath);
            base.Save();
        }

        public void Save(string path) {
            SavePath = path;
            Save();
        }

        public void Save(TextWriter writer) {
            MergeConfigsIntoDocument();
            iniDocument.Save(writer);
            SavePath = null;
            OnSaved(new EventArgs());
        }

        public void Save(Stream stream) {
            MergeConfigsIntoDocument();
            iniDocument.Save(stream);
            SavePath = null;
            OnSaved(new EventArgs());
        }

        public override void Reload() {
            if(SavePath == null)
                throw new ArgumentException("Error reloading: You must have the loaded the source from a file");

            iniDocument = new IniDocument(SavePath);
            MergeDocumentIntoConfigs();
            base.Reload();
        }

        public override string ToString() {
            MergeConfigsIntoDocument();

            using(var writer = new StringWriter()) {
                iniDocument.Save(writer);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Merges all of the configs from the config collection into the 
        /// IniDocument before it is saved.  
        /// </summary>
        private void MergeConfigsIntoDocument() {
            RemoveSections();
            foreach(IConfig config in Configs) {
                var keys = config.GetKeys();

                // Create a new section if one doesn't exist
                if(iniDocument.Sections[config.Name] == null) {
                    IniSection section = new IniSection(config.Name);
                    iniDocument.Sections.Add(section);
                }
                RemoveKeys(config.Name);

                for(int i = 0; i < keys.Length; i++) {
                    iniDocument.Sections[config.Name].Set(keys[i], config.Get(keys[i]));
                }
            }
        }

        /// <summary>
        /// Removes all INI sections that were removed as configs.
        /// </summary>
        private void RemoveSections() {
            for(var i = 0; i < iniDocument.Sections.Count; i++) {
                var section = iniDocument.Sections[i];

                if(Configs[section.Name] == null)
                    iniDocument.Sections.Remove(section.Name);
            }
        }

        /// <summary>
        /// Removes all INI keys that were removed as config keys.
        /// </summary>
        private void RemoveKeys(string sectionName) {
            IniSection section = iniDocument.Sections[sectionName];

            if(section != null) {
                foreach(string key in section.GetKeys()) {
                    if(Configs[sectionName].Get(key) == null) {
                        section.Remove(key);
                    }
                }
            }
        }

        /// <summary>
        /// Loads the configuration file.
        /// </summary>
        private void Load() {
            IniConfig config = null;
            IniSection section = null;
            IniItem item = null;

            for(int j = 0; j < iniDocument.Sections.Count; j++) {
                section = iniDocument.Sections[j];
                config = new IniConfig(section.Name, this);

                for(int i = 0; i < section.ItemCount; i++) {
                    item = section.GetItem(i);

                    if(item.Type == IniType.Key) {
                        config.Add(item.Name, item.Value);
                    }
                }

                Configs.Add(config);
            }
        }

        /// <summary>
        /// Merges the IniDocument into the Configs when the document is 
        /// reloaded.  
        /// </summary>
        private void MergeDocumentIntoConfigs() {
            // Remove all missing configs first
            RemoveConfigs();

            IniSection section = null;
            for(int i = 0; i < iniDocument.Sections.Count; i++) {
                section = iniDocument.Sections[i];

                IConfig config = Configs[section.Name];
                if(config == null) {
                    // The section is new so add it
                    config = new ConfigBase(section.Name, this);
                    Configs.Add(config);
                }
                RemoveConfigKeys(config);
            }
        }

        /// <summary>
        /// Removes all configs that are not in the newly loaded INI doc.  
        /// </summary>
        private void RemoveConfigs() {
            IConfig config = null;
            for(int i = Configs.Count - 1; i > -1; i--) {
                config = Configs[i];
                // If the section is not present in the INI doc
                if(iniDocument.Sections[config.Name] == null) {
                    Configs.Remove(config);
                }
            }
        }

        /// <summary>
        /// Removes all INI keys that were removed as config keys.
        /// </summary>
        private void RemoveConfigKeys(IConfig config) {
            IniSection section = iniDocument.Sections[config.Name];

            // Remove old keys
            string[] configKeys = config.GetKeys();
            foreach(string configKey in configKeys) {
                if(!section.Contains(configKey)) {
                    // Key doesn't exist, remove
                    config.Remove(configKey);
                }
            }

            // Add or set all new keys
            string[] keys = section.GetKeys();
            for(int i = 0; i < keys.Length; i++) {
                string key = keys[i];
                config.Set(key, section.GetItem(i).Value);
            }
        }

        /// <summary>
        /// Returns true if this instance is savable.
        /// </summary>
        private bool IsSavable() {
            return (SavePath != null);
        }
    }
}