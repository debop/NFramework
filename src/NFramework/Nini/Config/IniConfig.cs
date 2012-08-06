#region Copyright

//
// Nini Configuration Project.
// Copyright (C) 2006 Brent R. Matzelle.  All rights reserved.
//
// This software is published under the terms of the MIT X11 license, a copy of 
// which has been included with this distribution in the LICENSE.txt file.
// 

#endregion

namespace NSoft.NFramework.Nini.Config {
    public class IniConfig : ConfigBase {
        private readonly IniConfigSource _parent;

        public IniConfig(string name, IConfigSource source) : base(name, source) {
            _parent = (IniConfigSource)source;
        }

        public override string Get(string key) {
            if(!_parent.CaseSensitive) {
                key = CaseInsensitiveKeyName(key);
            }

            return base.Get(key);
        }

        public override void Set(string key, object value) {
            if(!_parent.CaseSensitive) {
                key = CaseInsensitiveKeyName(key);
            }

            base.Set(key, value);
        }

        public override void Remove(string key) {
            if(!_parent.CaseSensitive) {
                key = CaseInsensitiveKeyName(key);
            }

            base.Remove(key);
        }

        /// <summary>
        /// Returns the key name if the case insensitivity is turned on.  
        /// </summary>
        private string CaseInsensitiveKeyName(string key) {
            string result = null;

            var lowerKey = key.ToLower();

            foreach(string currentKey in keys.Keys) {
                if(currentKey.ToLower() == lowerKey) {
                    result = currentKey;
                    break;
                }
            }

            return result ?? key;
        }
    }
}