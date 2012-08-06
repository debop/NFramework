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
    public interface IConfig {
        IConfigSource ConfigSource { get; }

        string Name { get; set; }

        AliasText Alias { get; }

        bool Contains(string key);

        string Get(string key);

        string Get(string key, string defaultValue);

        string GetExpanded(string key);

        string GetString(string key);

        string GetString(string key, string defaultValue);

        int GetInt(string key);

        int GetInt(string key, bool fromAlias);

        int GetInt(string key, int defaultValue);

        int GetInt(string key, int defaultValue, bool fromAlias);

        long GetLong(string key);

        long GetLong(string key, long defaultValue);

        bool GetBoolean(string key);

        bool GetBoolean(string key, bool defaultValue);

        float GetFloat(string key);

        float GetFloat(string key, float defaultValue);

        double GetDouble(string key);

        double GetDouble(string key, double defaultValue);

        string[] GetKeys();

        string[] GetValues();

        void Set(string key, object value);

        void Remove(string key);

        event ConfigKeyEventHandler KeySet;

        event ConfigKeyEventHandler KeyRemoved;
    }
}