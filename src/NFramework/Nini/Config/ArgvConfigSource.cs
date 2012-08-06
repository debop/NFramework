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
using NSoft.NFramework.Nini.Util;

namespace NSoft.NFramework.Nini.Config {
    public class ArgvConfigSource : ConfigSourceBase {
        private readonly ArgvParser _parser;
        private readonly string[] _arguments;

        public ArgvConfigSource(string[] arguments) {
            _parser = new ArgvParser(arguments);
            _arguments = arguments;
        }

        public override void Save() {
            throw new ArgumentException("Source is read only");
        }

        public override void Reload() {
            throw new ArgumentException("Source cannot be reloaded");
        }

        public void AddSwitch(string configName, string longName, string shortName = null) {
            var config = GetConfig(configName);

            if(shortName != null && (shortName.Length < 1 || shortName.Length > 2))
                throw new ArgumentException("Short name may only be 1 or 2 characters");

            // Look for the long name first
            if(_parser[longName] != null) {
                config.Set(longName, _parser[longName]);
            }
            else if(shortName != null && _parser[shortName] != null) {
                config.Set(longName, _parser[shortName]);
            }
        }

        public string[] GetArguments() {
            var result = new string[_arguments.Length];
            Array.Copy(_arguments, 0, result, 0, _arguments.Length);

            return result;
        }

        /// <summary>
        /// Returns an IConfig.  If it does not exist then it is added.
        /// </summary>
        private IConfig GetConfig(string name) {
            IConfig result;

            if(Configs[name] == null) {
                result = new ConfigBase(name, this);
                Configs.Add(result);
            }
            else {
                result = Configs[name];
            }

            return result;
        }
    }
}