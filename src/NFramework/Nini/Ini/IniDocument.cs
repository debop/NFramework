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
using System.IO;

namespace NSoft.NFramework.Nini.Ini {
    public enum IniFileType {
        Standard,
        PythonStyle,
        SambaStyle,
        MysqlStyle,
        WindowsStyle
    }

    public class IniDocument {
        private readonly IniSectionCollection sections = new IniSectionCollection();
        private readonly ArrayList initialComment = new ArrayList();
        private IniFileType fileType = IniFileType.Standard;

        public IniFileType FileType {
            get { return fileType; }
            set { fileType = value; }
        }

        public IniDocument(string filePath) {
            fileType = IniFileType.Standard;
            Load(filePath);
        }

        public IniDocument(string filePath, IniFileType type) {
            fileType = type;
            Load(filePath);
        }

        public IniDocument(TextReader reader) {
            fileType = IniFileType.Standard;
            Load(reader);
        }

        public IniDocument(TextReader reader, IniFileType type) {
            fileType = type;
            Load(reader);
        }

        public IniDocument(Stream stream) {
            fileType = IniFileType.Standard;
            Load(stream);
        }

        public IniDocument(Stream stream, IniFileType type) {
            fileType = type;
            Load(stream);
        }

        public IniDocument(IniReader reader) {
            fileType = IniFileType.Standard;
            Load(reader);
        }

        public IniDocument() {}

        public void Load(string filePath) {
            Load(new StreamReader(filePath));
        }

        public void Load(TextReader reader) {
            Load(GetIniReader(reader, fileType));
        }

        public void Load(Stream stream) {
            Load(new StreamReader(stream));
        }

        public void Load(IniReader reader) {
            LoadReader(reader);
        }

        public IniSectionCollection Sections {
            get { return sections; }
        }

        public void Save(TextWriter textWriter) {
            var writer = GetIniWriter(textWriter, fileType);

            foreach(string comment in initialComment) {
                writer.WriteEmpty(comment);
            }

            for(int j = 0; j < sections.Count; j++) {
                var section = sections[j];
                writer.WriteSection(section.Name, section.Comment);
                for(var i = 0; i < section.ItemCount; i++) {
                    var item = section.GetItem(i);
                    switch(item.Type) {
                        case IniType.Key:
                            writer.WriteKey(item.Name, item.Value, item.Comment);
                            break;
                        case IniType.Empty:
                            writer.WriteEmpty(item.Comment);
                            break;
                    }
                }
            }

            writer.Close();
        }

        public void Save(string filePath) {
            var writer = new StreamWriter(filePath);
            Save(writer);
            writer.Close();
        }

        public void Save(Stream stream) {
            Save(new StreamWriter(stream));
        }

        /// <summary>
        /// Loads the file not saving comments.
        /// </summary>
        private void LoadReader(IniReader reader) {
            reader.IgnoreComments = false;
            var sectionFound = false;
            IniSection section = null;

            try {
                while(reader.Read()) {
                    switch(reader.Type) {
                        case IniType.Empty:
                            if(!sectionFound) {
                                initialComment.Add(reader.Comment);
                            }
                            else {
                                section.Set(reader.Comment);
                            }

                            break;
                        case IniType.Section:
                            sectionFound = true;
                            // If section already exists then overwrite it
                            if(sections[reader.Name] != null) {
                                sections.Remove(reader.Name);
                            }
                            section = new IniSection(reader.Name, reader.Comment);
                            sections.Add(section);

                            break;
                        case IniType.Key:
                            if(section.GetValue(reader.Name) == null) {
                                section.Set(reader.Name, reader.Value, reader.Comment);
                            }
                            break;
                    }
                }
            }
            catch(Exception ex) {
                throw;
            }
            finally {
                // Always close the file
                reader.Close();
            }
        }

        /// <summary>
        /// Returns a proper INI reader depending upon the type parameter.
        /// </summary>
        private IniReader GetIniReader(TextReader reader, IniFileType type) {
            IniReader result = new IniReader(reader);

            switch(type) {
                case IniFileType.Standard:
                    // do nothing
                    break;
                case IniFileType.PythonStyle:
                    result.AcceptCommentAfterKey = false;
                    result.SetCommentDelimiters(new char[] { ';', '#' });
                    result.SetAssignDelimiters(new char[] { ':' });
                    break;
                case IniFileType.SambaStyle:
                    result.AcceptCommentAfterKey = false;
                    result.SetCommentDelimiters(new char[] { ';', '#' });
                    result.LineContinuation = true;
                    break;
                case IniFileType.MysqlStyle:
                    result.AcceptCommentAfterKey = false;
                    result.AcceptNoAssignmentOperator = true;
                    result.SetCommentDelimiters(new char[] { '#' });
                    result.SetAssignDelimiters(new char[] { ':', '=' });
                    break;
                case IniFileType.WindowsStyle:
                    result.ConsumeAllKeyText = true;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Returns a proper IniWriter depending upon the type parameter.
        /// </summary>
        private IniWriter GetIniWriter(TextWriter reader, IniFileType type) {
            IniWriter result = new IniWriter(reader);

            switch(type) {
                case IniFileType.Standard:
                case IniFileType.WindowsStyle:
                    // do nothing
                    break;
                case IniFileType.PythonStyle:
                    result.AssignDelimiter = ':';
                    result.CommentDelimiter = '#';
                    break;
                case IniFileType.SambaStyle:
                case IniFileType.MysqlStyle:
                    result.AssignDelimiter = '=';
                    result.CommentDelimiter = '#';
                    break;
            }

            return result;
        }
    }
}