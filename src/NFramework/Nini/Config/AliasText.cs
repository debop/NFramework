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

namespace NSoft.NFramework.Nini.Config {
    public class AliasText {
        private readonly Hashtable _intAlias = InsensitiveHashtable();
        private readonly Hashtable _booleanAlias = InsensitiveHashtable();

        public AliasText() {
            DefaultAliasLoad();
        }

        public void AddAlias(string key, string alias, int value) {
            if(_intAlias.Contains(key)) {
                Hashtable keys = (Hashtable)_intAlias[key];

                keys[alias] = value;
            }
            else {
                Hashtable keys = InsensitiveHashtable();
                keys[alias] = value;
                _intAlias.Add(key, keys);
            }
        }

        public void AddAlias(string alias, bool value) {
            _booleanAlias[alias] = value;
        }

        public void AddAlias(string key, Enum enumAlias) {
            SetAliasTypes(key, enumAlias);
        }

        public bool ContainsBoolean(string key) {
            return _booleanAlias.Contains(key);
        }

        public bool ContainsInt(string key, string alias) {
            var result = false;

            if(_intAlias.Contains(key)) {
                Hashtable keys = (Hashtable)_intAlias[key];
                result = (keys.Contains(alias));
            }

            return result;
        }

        public bool GetBoolean(string key) {
            if(!_booleanAlias.Contains(key))
                throw new ArgumentException("Alias does not exist for text");

            return (bool)_booleanAlias[key];
        }

        public int GetInt(string key, string alias) {
            if(!_intAlias.Contains(key))
                throw new ArgumentException("Alias does not exist for key");

            Hashtable keys = (Hashtable)_intAlias[key];

            if(!keys.Contains(alias))
                throw new ArgumentException("Config value does not match a supplied alias");

            return (int)keys[alias];
        }

        /// <summary>
        /// Loads the default alias values.
        /// </summary>
        private void DefaultAliasLoad() {
            AddAlias("true", true);
            AddAlias("false", false);
        }

        /// <summary>
        /// Extracts and sets the alias types from an enumeration.
        /// </summary>
        private void SetAliasTypes(string key, Enum enumAlias) {
            var names = Enum.GetNames(enumAlias.GetType());
            var values = (int[])Enum.GetValues(enumAlias.GetType());

            for(var i = 0; i < names.Length; i++) {
                AddAlias(key, names[i], values[i]);
            }
        }

        /// <summary>
        /// Returns a case insensitive hashtable.
        /// </summary>
        private static Hashtable InsensitiveHashtable() {
            return new Hashtable(StringComparer.InvariantCultureIgnoreCase);
        }
    }
}