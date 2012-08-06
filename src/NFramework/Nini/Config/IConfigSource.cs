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

namespace NSoft.NFramework.Nini.Config {
    public interface IConfigSource {
        ConfigCollection Configs { get; }

        bool AutoSave { get; set; }

        AliasText Alias { get; }

        void Merge(IConfigSource source);

        void Save();

        void Reload();

        IConfig AddConfig(string name);

        string GetExpanded(IConfig config, string key);

        void ExpandKeyValues();

        void ReplaceKeyValues();

        event EventHandler Reloaded;

        event EventHandler Saved;
    }
}