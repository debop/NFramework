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
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using NSoft.NFramework.IO;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;
using NSoft.NFramework.Web.HttpHandlers.RouteHandlers;

namespace RCL.Web.Routing
{
    /// <summary>
    /// RouteTable에 Route 정보 등록처리
    /// </summary>
    public class RouteTableRegister
    {
        #region << logger >>

        private static NLog.Logger log
        {
            get { return NLog.LogManager.GetCurrentClassLogger(); }
        }

        private static bool IsTraceEnabled
        {
            get { return log.IsTraceEnabled; }
        }

        private static bool IsDebugEnabled
        {
            get { return log.IsDebugEnabled; }
        }

        #endregion

        /// <summary>
        /// 무시할 Route 정보
        /// </summary>
        private sealed class IgnoreRouteInternal : Route
        {
            // Methods
            public IgnoreRouteInternal(string url)
                : base(url, new StopRoutingHandler()) {}

            public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary routeValues)
            {
                return null;
            }
        }

        /// <summary>
        /// RouteCollection 에 Route 정보를 등록합니다.
        /// </summary>
        /// <param name="routes">RouteCollection</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            RegisterRoutes(routes, string.Empty);
        }

        /// <summary>
        /// RouteCollection 에 Route 정보를 등록합니다.
        /// </summary>
        /// <param name="routes">RouteCollection</param>
        /// <param name="routeFile">Route Config 파일 경로</param>
        public static void RegisterRoutes(RouteCollection routes, string routeFile)
        {
            lock(routes)
            {
                var routesTableSection = GetRouteTableSection(routeFile);
                if(routesTableSection == null) return;

                //ignore route
                if(routesTableSection.Ignores.Count > 0)
                {
                    foreach(ConfigurationElement item in routesTableSection.Ignores)
                    {
                        var ignore = new IgnoreRouteInternal(((IgnoreRouteElement) item).Url)
                                     {
                                         Constraints = GetRouteValueDictionary(((IgnoreRouteElement) item).Constraints.Attributes)
                                     };

                        routes.Add(ignore);
                    }
                }

                if(routesTableSection.Routes.Count <= 0) return;

                for(int routeIndex = 0; routeIndex < routesTableSection.Routes.Count; routeIndex++)
                {
                    var route = routesTableSection.Routes[routeIndex] as RouteConfigElement;

                    if(IsDebugEnabled)
                        log.Debug("route={0}", route);

                    if(routes[route.Name] != null) continue;

                    if(route.RouteType.IsWhiteSpace())
                    {
                        routes.Add(route.Name, new Route(
                                                   route.Url,
                                                   GetDefaults(route),
                                                   GetConstraints(route),
                                                   GetDataTokens(route),
                                                   GetInstanceOfRouteHandler(route)));
                    }
                    else if(route.RouteType.Equals("System.Web.Mvc.MvcRouteHandler"))
                    {
                        routes.MapRoute(route.Name, route.Url, GetDefaults(route));
                    }
                    else
                    {
                        var customRoute = (RouteBase) Activator.CreateInstance(Type.GetType(route.RouteType),
                                                                               route.Url,
                                                                               GetDefaults(route),
                                                                               GetConstraints(route),
                                                                               GetDataTokens(route),
                                                                               GetInstanceOfRouteHandler(route));

                        routes.Add(route.Name, customRoute);
                    }
                }

                if(IsDebugEnabled)
                    log.Debug("등록된 route collection={0}", routes.CollectionToString());
            }
        }

        /// <summary>
        /// RouteTableSection 정보
        /// </summary>
        /// <param name="file">파일경로</param>
        /// <returns>RouteTableSection</returns>
        private static RouteTableSection GetRouteTableSection(string file)
        {
            if(string.IsNullOrEmpty(file))
            {
                return (RouteTableSection) ConfigurationManager.GetSection("routeTable");
            }
            var section = (RouteTableSection) Activator.CreateInstance(typeof(RouteTableSection));

            section.DeserializeSection(FileTool.ToString(file));

            return (RouteTableSection) section;
        }

        /// <summary>
        /// Gets the instance of route handler.
        /// </summary>
        /// <param name="route">The route.</param>
        /// <returns>IRouteHandler</returns>
        private static IRouteHandler GetInstanceOfRouteHandler(RouteConfigElement route)
        {
            IRouteHandler routeHandler;

            if(IsDebugEnabled)
                log.Debug("route={0}", route);

            if(route.RouteHandlerType.IsWhiteSpace())
                routeHandler = new RouteHandler<Page>(route.VirtualPath, true);
            else
            {
                try
                {
                    Type routeHandlerType = Type.GetType(route.RouteHandlerType);

                    if(IsDebugEnabled)
                        log.Debug("route={0}, routeHandlerType={1}", route, routeHandlerType);

                    routeHandler = Activator.CreateInstance(routeHandlerType) as IRouteHandler;
                }
                catch(Exception e)
                {
                    throw new ApplicationException(string.Format("Can't create an instance of IRouteHandler {0}", route.RouteHandlerType), e);
                }
            }

            if(IsDebugEnabled)
                log.Debug("route={0}, routeHandler={1}", route, routeHandler);

            return routeHandler;
        }

        /// <summary>
        /// Gets the constraints.
        /// </summary>
        /// <param name="route">The route.</param>
        /// <returns>RouteValueDictionary</returns>
        private static RouteValueDictionary GetConstraints(RouteConfigElement route)
        {
            return GetRouteValueDictionary(route.Constraints.Attributes);
        }

        /// <summary>
        /// Gets the defaults.
        /// </summary>
        /// <param name="route">The route.</param>
        /// <returns>RouteValueDictionary</returns>
        private static RouteValueDictionary GetDefaults(RouteConfigElement route)
        {
            return GetRouteValueDictionary(route.Defaults.Attributes);
        }

        /// <summary>
        /// Gets the data tokens.
        /// </summary>
        /// <param name="route">The route.</param>
        /// <returns>RouteValueDictionary</returns>
        private static RouteValueDictionary GetDataTokens(RouteConfigElement route)
        {
            return GetRouteValueDictionary(route.DataTokens.Attributes);
        }

        /// <summary>
        /// Gets the dictionary from attributes.
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        /// <returns>RouteValueDictionary</returns>
        private static RouteValueDictionary GetRouteValueDictionary(Dictionary<string, string> attributes)
        {
            var dataTokensDictionary = new RouteValueDictionary();

            foreach(var dataTokens in attributes)
            {
                //ref : DefaultControllerFactory.GetControllerType
                if(dataTokens.Key == "Namespaces")
                {
                    dataTokensDictionary.Add(dataTokens.Key, dataTokens.Value.Split(','));
                }
                else
                {
                    dataTokensDictionary.Add(dataTokens.Key, dataTokens.Value);
                }
            }

            return dataTokensDictionary;
        }
    }
}