using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;

namespace NSoft.NFramework.Web.Mvc
{
    /// <summary>
    /// RSS 형식의 결과를 반환합니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RssActionResult<T> : ActionResult
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public const string RssContentType = "application/rss+xml";

        public RssActionResult(string title, IEnumerable<T> items, Func<T, XElement> formatter)
        {
            Title = title;
            Items = items;
            Formatter = formatter;
        }

        public string Title { get; set; }

        public IEnumerable<T> Items { get; set; }

        public Func<T, XElement> Formatter { get; set; }


        /// <summary>
        /// Enables processing of the result of an action method by a custom type that inherits from the <see cref="T:System.Web.Mvc.ActionResult"/> class.
        /// </summary>
        /// <param name="context">The context in which the result is executed. The context information includes the controller, HTTP content, request context, and route data.</param>
        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;

            response.ContentType = RssContentType;
            var rss = BuildXDocument(response.ContentEncoding.WebName);
            response.Write(rss.ToString());
        }

        private XDocument BuildXDocument(string encoding)
        {
            return
                new XDocument(new XDeclaration("1.0", encoding, "yes"),
                              new XElement("rss", new XAttribute("version", "2.0"),
                                           new XElement("channel", new XElement("title", Title),
                                                        Items.Select(item => Formatter(item)))));
        }
    }
}