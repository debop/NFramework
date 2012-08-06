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
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace NSoft.NFramework.Nini.Ini {
    [Serializable]
    public class IniException : InvalidOperationException {
        private readonly IniReader iniReader = null;
        private string message = "";

        public int LinePosition {
            get { return (iniReader == null) ? 0 : iniReader.LinePosition; }
        }

        public int LineNumber {
            get { return (iniReader == null) ? 0 : iniReader.LineNumber; }
        }

        public override string Message {
            get {
                if(iniReader == null) {
                    return base.Message;
                }

                return String.Format(CultureInfo.InvariantCulture, "{0} - Line: {1}, Position: {2}.",
                                     message, LineNumber, LinePosition);
            }
        }

        public IniException() : this("An error has occurred") {}
        public IniException(string message, Exception exception) : base(message, exception) {}

        public IniException(string message) : base(message) {
            this.message = message;
        }

        internal IniException(IniReader reader, string message) : this(message) {
            iniReader = reader;
            this.message = message;
        }

        protected IniException(SerializationInfo info, StreamingContext context) : base(info, context) {}

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info,
                                           StreamingContext context) {
            base.GetObjectData(info, context);
            if(iniReader != null) {
                info.AddValue("lineNumber", iniReader.LineNumber);

                info.AddValue("linePosition", iniReader.LinePosition);
            }
        }
    }
}