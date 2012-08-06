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
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;

namespace NSoft.NFramework.Nini.Config {
    public class DotNetConfigSource : ConfigSourceBase {
        private readonly string[] _sections;
        private XmlDocument _configDoc;
        private string _savePath;

        public DotNetConfigSource(string[] sections) {
            _sections = sections;
            Load();
        }

        public DotNetConfigSource() {
            _configDoc = new XmlDocument();
            _configDoc.LoadXml("<configuration><configSections/></configuration>");
            PerformLoad(_configDoc);
        }

        public DotNetConfigSource(string path) {
            Load(path);
        }

        public DotNetConfigSource(XmlReader reader) {
            Load(reader);
        }

        public string SavePath {
            get { return _savePath; }
        }

        public void Load(string path) {
            _savePath = path;
            _configDoc = new XmlDocument();
            _configDoc.Load(_savePath);
            PerformLoad(_configDoc);
        }

        public void Load(XmlReader reader) {
            _configDoc = new XmlDocument();
            _configDoc.Load(reader);
            PerformLoad(_configDoc);
        }

        public override void Save() {
            if(!IsSavable())
                throw new ArgumentException("Source cannot be saved in this state");

            MergeConfigsIntoDocument();

            _configDoc.Save(_savePath);
            base.Save();
        }

        public void Save(string path) {
            if(!IsSavable())
                throw new ArgumentException("Source cannot be saved in this state");

            _savePath = path;
            Save();
        }

        public void Save(TextWriter writer) {
            if(!IsSavable())
                throw new ArgumentException("Source cannot be saved in this state");

            MergeConfigsIntoDocument();
            _configDoc.Save(writer);
            _savePath = null;
            OnSaved(new EventArgs());
        }

        public void Save(Stream stream) {
            if(!IsSavable())
                throw new ArgumentException("Source cannot be saved in this state");

            MergeConfigsIntoDocument();
            _configDoc.Save(stream);
            _savePath = null;
            OnSaved(new EventArgs());
        }

        public override void Reload() {
            if(_savePath == null)
                throw new ArgumentException("Error reloading: You must have the loaded the source from a file");

            _configDoc = new XmlDocument();
            _configDoc.Load(_savePath);
            MergeDocumentIntoConfigs();
            base.Reload();
        }

        public override string ToString() {
            MergeConfigsIntoDocument();

            using(var writer = new StringWriter()) {
                _configDoc.Save(writer);
                return writer.ToString();
            }
        }

        public static string GetFullConfigPath() {
            return (Assembly.GetCallingAssembly().Location + ".config");
        }

        /// <summary>
        /// Merges all of the configs from the config collection into the 
        /// XmlDocument.
        /// </summary>
        private void MergeConfigsIntoDocument() {
            RemoveSections();
            foreach(IConfig config in Configs) {
                string[] keys = config.GetKeys();

                RemoveKeys(config.Name);
                XmlNode node = GetChildElement(config.Name);
                if(node == null) {
                    node = SectionNode(config.Name);
                }

                for(int i = 0; i < keys.Length; i++) {
                    SetKey(node, keys[i], config.Get(keys[i]));
                }
            }
        }

        /// <summary>
        /// Loads all collection classes.
        /// </summary>
        private void Load() {
            Merge(this); // required for SaveAll

            for(var i = 0; i < _sections.Length; i++) {
                LoadCollection(_sections[i], (NameValueCollection)ConfigurationManager.GetSection(_sections[i]));
                // LoadCollection(sections[i], (NameValueCollection)ConfigurationSettings.GetConfig(sections[i]));
            }
        }

        /// <summary>
        /// Loads all sections and keys.
        /// </summary>
        private void PerformLoad(XmlDocument document) {
            Configs.Clear();

            Merge(this); // required for SaveAll

            if(document.DocumentElement.Name != "configuration")
                throw new ArgumentException("Did not find configuration node");

            LoadSections(document.DocumentElement);
        }

        /// <summary>
        /// Loads all configuration sections.
        /// </summary>
        private void LoadSections(XmlNode rootNode) {
            LoadOtherSection(rootNode, "appSettings");

            var sections = GetChildElement(rootNode, "configSections");

            if(sections == null) {
                // There is no configSections node so exit
                return;
            }

            foreach(XmlNode node in sections.ChildNodes) {
                if(node.NodeType == XmlNodeType.Element && node.Name == "section") {
                    var config = new ConfigBase(node.Attributes["name"].Value, this);

                    Configs.Add(config);
                    LoadKeys(rootNode, config);
                }
            }
        }

        /// <summary>
        /// Loads special sections that are not loaded in the configSections
        /// node.  This includes such sections such as appSettings.
        /// </summary>
        private void LoadOtherSection(XmlNode rootNode, string nodeName) {
            var section = GetChildElement(rootNode, nodeName);

            if(section != null) {
                var config = new ConfigBase(section.Name, this);

                Configs.Add(config);
                LoadKeys(rootNode, config);
            }
        }

        /// <summary>
        /// Loads all keys for a config.
        /// </summary>
        private void LoadKeys(XmlNode rootNode, ConfigBase config) {
            var section = GetChildElement(rootNode, config.Name);

            foreach(XmlNode node in section.ChildNodes) {
                if(node.NodeType == XmlNodeType.Element && node.Name == "add") {
                    config.Add(node.Attributes["key"].Value, node.Attributes["value"].Value);
                }
            }
        }

        /// <summary>
        /// Removes all XML sections that were removed as configs.
        /// </summary>
        private void RemoveSections() {
            var sections = GetChildElement("configSections");

            if(sections == null) {
                // There is no configSections node so exit
                return;
            }

            foreach(XmlNode node in sections.ChildNodes) {
                if(node.NodeType == XmlNodeType.Element && node.Name == "section") {
                    var attr = node.Attributes["name"];
                    if(attr != null) {
                        if(Configs[attr.Value] == null) {
                            // Removes the configSections section
                            node.ParentNode.RemoveChild(node);

                            // Removes the <SectionName> section
                            var dataNode = GetChildElement(attr.Value);
                            if(dataNode != null)
                                _configDoc.DocumentElement.RemoveChild(dataNode);
                        }
                    }
                    else {
                        throw new ArgumentException("Section name attribute not found");
                    }
                }
            }
        }

        /// <summary>
        /// Removes all XML keys that were removed as config keys.
        /// </summary>
        private void RemoveKeys(string sectionName) {
            var node = GetChildElement(sectionName);

            if(node == null)
                return;

            foreach(XmlNode key in node.ChildNodes) {
                if(key.NodeType == XmlNodeType.Element && key.Name == "add") {
                    var keyName = key.Attributes["key"];
                    if(keyName != null) {
                        if(Configs[sectionName].Get(keyName.Value) == null) {
                            node.RemoveChild(key);
                        }
                    }
                    else {
                        throw new ArgumentException("Key attribute not found in node");
                    }
                }
            }
        }

        /// <summary>
        /// Sets an XML key.  If it does not exist then it is created.
        /// </summary>
        private void SetKey(XmlNode sectionNode, string key, string value) {
            var keyNode = GetKey(sectionNode, key);

            if(keyNode == null) {
                CreateKey(sectionNode, key, value);
            }
            else {
                keyNode.Attributes["value"].Value = value;
            }
        }

        /// <summary>
        /// Gets an XML key by it's name. Returns null if it does not exist.
        /// </summary>
        private XmlNode GetKey(XmlNode sectionNode, string keyName) {
            XmlNode result = null;

            foreach(XmlNode node in sectionNode.ChildNodes) {
                if(node.NodeType == XmlNodeType.Element
                   && node.Name == "add"
                   && node.Attributes["key"].Value == keyName) {
                    result = node;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Creates a key node and adds it to the collection at the end.
        /// </summary>
        private void CreateKey(XmlNode sectionNode, string key, string value) {
            var node = _configDoc.CreateElement("add");
            var keyAttr = _configDoc.CreateAttribute("key");
            var valueAttr = _configDoc.CreateAttribute("value");

            keyAttr.Value = key;
            valueAttr.Value = value;

            node.Attributes.Append(keyAttr);
            node.Attributes.Append(valueAttr);

            sectionNode.AppendChild(node);
        }

        /// <summary>
        /// Loads a collection class.
        /// </summary>
        private void LoadCollection(string name, NameValueCollection collection) {
            var config = new ConfigBase(name, this);

            if(collection == null)
                throw new ArgumentException("Section was not found");

            for(var i = 0; i < collection.Count; i++) {
                config.Add(collection.Keys[i], collection[i]);
            }

            Configs.Add(config);
        }

        /// <summary>
        /// Returns a new section node.
        /// </summary>
        private XmlNode SectionNode(string name) {
            // Add node for configSections node
            var node = _configDoc.CreateElement("section");
            var attr = _configDoc.CreateAttribute("name");
            attr.Value = name;
            node.Attributes.Append(attr);

            attr = _configDoc.CreateAttribute("type");
            attr.Value = "System.Configuration.NameValueSectionHandler";
            node.Attributes.Append(attr);

            var section = GetChildElement("configSections");
            section.AppendChild(node);

            // Add node for configuration node
            var result = _configDoc.CreateElement(name);
            _configDoc.DocumentElement.AppendChild(result);

            return result;
        }

        /// <summary>
        /// Returns true if this instance is savable.
        /// </summary>
        private bool IsSavable() {
            return (_savePath != null || _configDoc != null);
        }

        /// <summary>
        /// Returns the single named child element.
        /// </summary>
        private XmlNode GetChildElement(XmlNode parentNode, string name) {
            XmlNode result = null;

            foreach(XmlNode node in parentNode.ChildNodes) {
                if(node.NodeType == XmlNodeType.Element && node.Name == name) {
                    result = node;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a child element from the XmlDocument.DocumentElement.
        /// </summary>
        private XmlNode GetChildElement(string name) {
            return GetChildElement(_configDoc.DocumentElement, name);
        }

        /// <summary>
        /// Merges the XmlDocument into the Configs when the document is 
        /// reloaded.  
        /// </summary>
        private void MergeDocumentIntoConfigs() {
            // Remove all missing configs first
            RemoveConfigs();

            var sections = GetChildElement("configSections");

            if(sections == null) {
                // There is no configSections node so exit
                return;
            }

            foreach(XmlNode node in sections.ChildNodes) {
                // Find all section nodes
                if(node.NodeType == XmlNodeType.Element && node.Name == "section") {
                    var sectionName = node.Attributes["name"].Value;
                    var config = Configs[sectionName];

                    if(config == null) {
                        // The section is new so add it
                        config = new ConfigBase(sectionName, this);
                        Configs.Add(config);
                    }
                    RemoveConfigKeys(config);
                }
            }
        }

        /// <summary>
        /// Removes all configs that are not in the newly loaded XmlDocument.  
        /// </summary>
        private void RemoveConfigs() {
            for(var i = Configs.Count - 1; i > -1; i--) {
                var config = Configs[i];
                // If the section is not present in the XmlDocument
                if(GetChildElement(config.Name) == null) {
                    Configs.Remove(config);
                }
            }
        }

        /// <summary>
        /// Removes all XML keys that were removed as config keys.
        /// </summary>
        private void RemoveConfigKeys(IConfig config) {
            var section = GetChildElement(config.Name);

            // Remove old keys
            var configKeys = config.GetKeys();

            foreach(var configKey in configKeys) {
                if(GetKey(section, configKey) == null) {
                    // Key doesn't exist, remove
                    config.Remove(configKey);
                }
            }

            // Add or set all new keys
            foreach(XmlNode node in section.ChildNodes) {
                if(node.NodeType == XmlNodeType.Element && node.Name == "add") {
                    config.Set(node.Attributes["key"].Value, node.Attributes["value"].Value);
                }
            }
        }
    }
}