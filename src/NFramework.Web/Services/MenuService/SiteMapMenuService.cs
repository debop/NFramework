using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Web.Access;

namespace NSoft.NFramework.Web.Services.MenuService
{
    /// <summary>
    /// Provider가 SiteMap으로 구현된 메뉴서비스
    /// </summary>
    public class SiteMapMenuService : DefaultMenuService
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 메뉴중 사용자가 권한이 있는 루트메뉴목록
        /// </summary>
        /// <param name="productId">제품Id</param>
        /// <param name="identity">요청자 정보</param>
        /// <returns>메뉴목록</returns>
        public override IEnumerable<MenuItem> GetRootMenu(string productId, IAccessIdentity identity)
        {
            if(IsDebugEnabled)
                log.Debug("==>> S productId={0}, identity={1}", productId, identity);

            productId.ShouldNotBeWhiteSpace("값이 없습니다.");
            //identity.ShouldNotBeNull("사용자 정보가 없습니다.");

            if(identity == null)
                return null;

            var provider = SiteMap.Providers[productId];

            if(IsDebugEnabled)
                log.Debug("productId={0}, identity={1}, provider={2}", productId, identity, provider);

            var menus = new List<MenuItem>();

            if(provider != null)
            {
                if(IsDebugEnabled)
                    log.Debug("provider.RootNode={0}", provider.RootNode);

                if(provider.RootNode != null)
                {
                    var nodes = GetAuthMenus(provider.RootNode.ChildNodes, identity);

                    foreach(SiteMapNode node in nodes)
                    {
                        var menu = new MenuItem(node.Title, node.Title, node.Url);
                        menu.NodePosition.Level = 0;
                        menu.NodePosition.Order = 0;
                        menu.Parent = null;
                        AddNode(node, menu, 1);
                        menus.Add(menu);
                    }
                }
            }
            if(IsDebugEnabled)
                log.Debug("==>> E menus={0}", menus.CollectionToString());

            return menus;
        }

        /// <summary>
        /// 제품메뉴중 사용자가 권한이 있는 메뉴목록
        /// </summary>
        /// <param name="productId">제품Id</param>
        /// <param name="identity">요청자 정보</param>
        /// <returns>메뉴목록</returns>
        public override IEnumerable<MenuItem> FindAllMenuByProduct(string productId, IAccessIdentity identity)
        {
            if(IsDebugEnabled)
                log.Debug("==>> S productId={0}, identity={1}", productId, identity);

            productId.ShouldNotBeWhiteSpace("값이 없습니다.");
            //identity.ShouldNotBeNull("사용자 정보가 없습니다.");

            if(identity == null)
                return null;

            var provider = SiteMap.Providers[productId];

            if(IsDebugEnabled)
                log.Debug("productId={0}, identity={1}, provider={2}", productId, identity, provider);

            var menus = new List<MenuItem>();

            if(provider != null)
            {
                if(IsDebugEnabled)
                    log.Debug("provider.RootNode={0}", provider.RootNode);

                if(provider.RootNode != null)
                {
                    var nodes = GetAuthMenus(provider.RootNode.GetAllNodes(), identity);

                    foreach(SiteMapNode node in nodes)
                    {
                        var menu = new MenuItem(node.Title, node.Title, node.Url);
                        menu.NodePosition.Level = 0;
                        menu.NodePosition.Order = 0;
                        menu.Parent = null;
                        //AddNode(node, menu, 1);
                        menus.Add(menu);
                    }
                }
            }
            if(IsDebugEnabled)
                log.Debug("==>> E menus={0}", menus.CollectionToString());

            return menus;
        }

        /// <summary>
        /// 메뉴정보를 반환한다.
        /// </summary>
        /// <param name="productId">제품Id</param>
        /// <param name="identity">요청자 정보</param>
        /// <param name="menuId">메뉴Id</param>
        /// <returns>메뉴</returns>
        public override MenuItem FindOneMenu(string productId, IAccessIdentity identity, string menuId)
        {
            if(IsDebugEnabled)
                log.Debug("==>> S productId={0}, identity={1}, menuId={2}", productId, identity, menuId);

            productId.ShouldNotBeWhiteSpace("값이 없습니다.");
            //identity.ShouldNotBeNull("사용자 정보가 없습니다.");
            menuId.ShouldNotBeWhiteSpace("값이 없습니다.");

            if(identity == null)
                return null;

            var provider = SiteMap.Providers[productId];

            if(provider != null)
            {
                if(IsDebugEnabled)
                    log.Debug("provider.RootNode={0}", provider.RootNode);

                if(provider.RootNode != null)
                {
                    var nodes = GetAuthMenus(provider.RootNode.GetAllNodes(), identity);
                    var node = nodes.ToList<SiteMapNode>().FirstOrDefault(nd => nd.Title == menuId);
                    if(node != null)
                    {
                        var menu = new MenuItem(node.Title, node.Title, node.Url)
                                   {
                                       NodePosition =
                                           {
                                               Level = 0,
                                               Order = 0
                                           }
                                   };
                        SetParent(node, menu);

                        return menu;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 상위노드를 추가한다.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="childMenu"></param>
        private static void SetParent(SiteMapNode node, MenuItem childMenu)
        {
            if(node == null)
                return;

            if(node.ParentNode != null)
            {
                var menu = new MenuItem(node.ParentNode.Title, node.ParentNode.Title, node.ParentNode.Url)
                           {
                               NodePosition =
                                   {
                                       Level = 0,
                                       Order = 0
                                   }
                           };
                childMenu.Parent = menu;

                SetParent(node.ParentNode, menu);
            }
        }

        /// <summary>
        /// 메뉴를 추가한다.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent"></param>
        /// <param name="level"></param>
        private static void AddNode(SiteMapNode node, MenuItem parent, int level)
        {
            if(IsDebugEnabled)
                log.Debug("==>> S node={0}, parent={1}, level={2}", node, parent, level);

            foreach(SiteMapNode childNode in node.ChildNodes)
            {
                var menu = new MenuItem(childNode.Title, childNode.Title, childNode.Url)
                           {
                               NodePosition =
                                   {
                                       Level = level,
                                       Order = 0
                                   },
                               Parent = parent
                           };

                if(IsDebugEnabled)
                    log.Debug("parent={0}, menu={1}, childNode.ChildNodes.Count={2}", parent, menu, childNode.ChildNodes.Count);

                if(childNode.ChildNodes.Count > 0)
                    AddNode(childNode, menu, level + 1);

                parent.Children.Add(menu);
            }
        }

        /// <summary>
        /// Role에 해당하는 메뉴만 필터링하여 반환한다.
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="identity"></param>
        /// <returns></returns>
        private static IEnumerable<SiteMapNode> GetAuthMenus(SiteMapNodeCollection nodes, IAccessIdentity identity)
        {
            if(identity == null)
                return null;

            var roles = WebAppContext.Services.RoleService.GetRoles(identity).ToArray();

            if(roles.Length == 0)
                return Enumerable.Empty<SiteMapNode>();

            return nodes.Cast<SiteMapNode>().Where(node => roles.In(node.Roles.Cast<string>()));
        }
    }
}