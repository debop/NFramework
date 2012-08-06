/*
Kooboo is a content management system based on ASP.NET MVC framework. Copyright 2009 Yardi Technology Limited.

This program is free software: you can redistribute it and/or modify it under the terms of the
GNU General Public License version 3 as published by the Free Software Foundation.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program.
If not, see http://www.kooboo.com/gpl3/.
*/

using System;
using System.Configuration;
using NSoft.NFramework;

namespace RCL.Web.Routing
{
    /// <summary>
    /// RouteConfig 구성파일의 Element 정보
    /// </summary>
    public class RouteConfigElement : ConfigurationElement
    {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="newName">경로를 식별하는 값</param>
        /// <param name="newUrl">경로의 URL 패턴</param>
        /// <param name="routeHandlerType">RouteHandlerType</param>
        public RouteConfigElement(String newName, String newUrl, String routeHandlerType)
        {
            Name = newName;
            Url = newUrl;
            RouteHandlerType = routeHandlerType;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        public RouteConfigElement() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="elementName">경로를 식별하는 값</param>
        public RouteConfigElement(string elementName)
        {
            Name = elementName;
        }

        /// <summary>
        /// 경로를 식별하는 값입니다. 값은 null 또는 빈 문자열일 수 있습니다. 
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return this["name"].AsText(); }
            set { this["name"] = value; }
        }

        /// <summary>
        /// 경로의 URL 패턴을 가져오거나 설정합니다.
        /// </summary>
        [ConfigurationProperty("url", IsRequired = true)]
        public string Url
        {
            get { return this["url"].AsText(); }
            set { this["url"] = value; }
        }

        /// <summary>
        /// 실제 실행된 Handler의 경로
        /// </summary>
        [ConfigurationProperty("virtualPath", IsRequired = false)]
        public string VirtualPath
        {
            get { return this["virtualPath"].AsText(); }
            set { this["virtualPath"] = value; }
        }

        /// <summary>
        /// RouteHandler Type
        /// </summary>
        [ConfigurationProperty("routeHandlerType", IsRequired = false)]
        public string RouteHandlerType
        {
            get { return this["routeHandlerType"].AsText(); }
            set { this["routeHandlerType"] = value; }
        }

        /// <summary>
        /// RoteCollection 에 추가되어지는 Route의 타입
        /// </summary>
        [ConfigurationProperty("routeType", IsRequired = false)]
        public string RouteType
        {
            get { return this["routeType"].AsText(); }
            set { this["routeType"] = value; }
        }

        /// <summary>
        /// URL에 모든 매개 변수가 포함되지 않을 경우 사용할 값을 가져오거나 설정합니다.
        /// </summary>
        [ConfigurationProperty("defaults", IsRequired = false)]
        public RouteChildElement Defaults
        {
            get { return (RouteChildElement) this["defaults"]; }
            set { this["defaults"] = value; }
        }

        /// <summary>
        /// URL 매개 변수에 대해 유효한 값을 지정하는 식 사전을 가져오거나 설정합니다.
        /// </summary>
        [ConfigurationProperty("constraints", IsRequired = false)]
        public RouteChildElement Constraints
        {
            get { return (RouteChildElement) this["constraints"]; }
            set { this["constraints"] = value; }
        }

        /// <summary>
        /// 경로 처리기에 전달되지만 경로가 URL 패턴과 일치하는지 여부를 확인하는 데 사용되지는 않는 사용자 지정 값을 가져오거나 설정합니다.
        /// </summary>
        [ConfigurationProperty("dataTokens", IsRequired = false)]
        public RouteChildElement DataTokens
        {
            get { return (RouteChildElement) this["dataTokens"]; }
            set { this["dataTokens"] = value; }
        }

        protected override void DeserializeElement(System.Xml.XmlReader reader, bool serializeCollectionKey)
        {
            base.DeserializeElement(reader, serializeCollectionKey);
        }

        protected override bool SerializeElement(System.Xml.XmlWriter writer, bool serializeCollectionKey)
        {
            return base.SerializeElement(writer, serializeCollectionKey);
        }

        public override string ToString()
        {
            return string.Format("RouteConfigElement# Name={0}, Url={1}, VirtualPath={2}, RouteHandlerType={3}, RouteType={4}", Name, Url, VirtualPath, RouteHandlerType, RouteType);
        }
    }
}