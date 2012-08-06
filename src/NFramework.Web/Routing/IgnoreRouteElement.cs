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

using System.Configuration;
using NSoft.NFramework;

namespace RCL.Web.Routing
{
    /// <summary>
    /// RouteConfig 구성파일에서 무시될 Element 정보
    /// </summary>
    public class IgnoreRouteElement : ConfigurationElement
    {
        /// <summary>
        /// 생성자
        /// </summary>
        public IgnoreRouteElement() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="elementName">경로를 식별하는 값</param>
        public IgnoreRouteElement(string elementName)
        {
            Name = elementName;
        }

        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return this["name"].AsText(); }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("url", IsRequired = true)]
        public string Url
        {
            get { return this["url"].AsText(); }
            set { this["url"] = value; }
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
    }
}