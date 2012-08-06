using System;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Reflections;
using Telerik.Web.UI;

namespace NSoft.NFramework.Web.TelerikEx
{
    public static partial class TelerikTool
    {
        /// <summary>
        /// Depth-First 탐색으로 특정 Node 와 그 자손 Node 들 중에 검사를 통과하는 Node들을 열거합니다.
        /// </summary>
        /// <param name="nodeContainer"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<RadTreeNode> FindNodes(this IRadTreeNodeContainer nodeContainer, Func<RadTreeNode, bool> predicate)
        {
            nodeContainer.ShouldNotBeNull("nodeContainer");
            predicate.ShouldNotBeNull("predicate");

            if(IsDebugEnabled)
                log.Debug("특정 NodeContainer(TreeView or TreeNode)부터 predicate를 만족하는 RadTreeNode 들을 모두 열거합니다. nodeContainer=[{0}]",
                          nodeContainer.AsTextAndValue());

            return nodeContainer.GetDescendentNodes().Where(predicate);
        }

        /// <summary>
        /// 특정 Text를 가진 첫번째 TreeNode를 반환합니다. 없으면 null을 반환합니다.
        /// </summary>
        /// <param name="nodeContainer">RadTreeView</param>
        /// <param name="nodeText">찾을 node text</param>
        /// <param name="ignoreCase">대소문자 무시 여부</param>
        /// <returns></returns>
        public static RadTreeNode FirstOrDefaultByText(this IRadTreeNodeContainer nodeContainer, string nodeText, bool ignoreCase = true)
        {
            if(nodeContainer == null)
                return null;

            if(IsDebugEnabled)
                log.Debug(@"특정 nodeContainer 및 자손 TreeNode 중에 Text 속성 값이 [{0}] 인 TreeNode를 찾습니다... ignoreCase=[{1}]", nodeText, ignoreCase);

            return nodeContainer.FindNodes(n => string.Compare(n.Text, nodeText, ignoreCase) == 0).FirstOrDefault();
        }

        /// <summary>
        /// 특정 노드 Text을 가진 TreeNode를 찾습니다.
        /// </summary>
        /// <param name="nodes">RadTreeNode collection</param>
        /// <param name="nodeText">찾을 node text</param>
        /// <param name="ignoreCase">대소문자 무시 여부</param>
        /// <returns></returns>
        public static RadTreeNode FirstOrDefaultByText(this IEnumerable<RadTreeNode> nodes, string nodeText, bool ignoreCase = true)
        {
            if(nodes == null)
                return null;

            if(IsDebugEnabled)
                log.Debug(@"TreeNode 시퀀스 중에 Text 속성 값이 [{0}] 인 TreeNode를 찾습니다... ignoreCase=[{1}]", nodeText, ignoreCase);

            return nodes.FirstOrDefault(node => string.Compare(node.Text, nodeText, ignoreCase) == 0);
        }

        /// <summary>
        /// 특정 값을 가진 TreeNode를 찾습니다. 없으면 null을 반환합니다.
        /// </summary>
        /// <param name="nodeContainer">RadTreeView or RadTreeNode</param>
        /// <param name="nodeValue">찾을 node value</param>
        /// <param name="ignoreCase">대소문자 무시 여부</param>
        /// <returns></returns>
        public static RadTreeNode FirstOrDefaultByValue(this IRadTreeNodeContainer nodeContainer, string nodeValue, bool ignoreCase=true)
        {
            if(nodeContainer == null)
                return null;

            if(IsDebugEnabled)
                log.Debug(@"특정 nodeContainer 및 자손 TreeNode 중에 Value 속성 값이 [{0}] 인 TreeNode를 찾습니다... ignoreCase=[{1}]", nodeValue, ignoreCase);

            return nodeContainer.FindNodes(n => string.Compare(n.Value, nodeValue, ignoreCase) == 0).FirstOrDefault();
        }

        /// <summary>
        /// 특정 노드 값을 가진 TreeNode를 찾습니다.
        /// </summary>
        /// <param name="nodes">RadTreeNode collection</param>
        /// <param name="nodeValue">찾을 node value</param>
        /// <param name="ignoreCase">대소문자 무시 여부</param>
        /// <returns></returns>
        public static RadTreeNode FirstOrDefaultByValue(this IEnumerable<RadTreeNode> nodes, string nodeValue, bool ignoreCase = true)
        {
            if(nodes == null)
                return null;

            if(IsDebugEnabled)
                log.Debug(@"TreeNode 시퀀스 중에 Value 속성 값이 [{0}] 인 TreeNode를 찾습니다... ignoreCase={1}", nodeValue, ignoreCase);

            return nodes.FirstOrDefault(node => string.Compare(node.Value, nodeValue, ignoreCase) == 0);
        }

        /// <summary>
        /// 특정 NodeContainer가 TreeView라면 자식 Nodes들을 열거하고, TreeNode라면, TreeNode가 속한 TreeView의 Root Nodes 들을 반환합니다.
        /// </summary>
        /// <param name="nodeContainer"></param>
        /// <returns></returns>
        public static IEnumerable<RadTreeNode> FindRootNodes(this IRadTreeNodeContainer nodeContainer)
        {
            if(IsDebugEnabled)
                log.Debug("특정 NodeContainer가 TreeView라면 자식 Nodes들을 열거하고, TreeNode라면, TreeNode가 속한 TreeView의 Root Nodes 들을 반환합니다... " +
                          "nodeContainer=[{0}]", nodeContainer.AsTextAndValue());

            RadTreeView radTreeView = null;

            if(nodeContainer is RadTreeView)
                radTreeView = (RadTreeView)nodeContainer;
            else if(nodeContainer is RadTreeNode)
                radTreeView = ((RadTreeNode)nodeContainer).TreeView;

            if(radTreeView != null)
                return radTreeView.Nodes.Cast<RadTreeNode>();

            return Enumerable.Empty<RadTreeNode>();
        }

        /// <summary>
        /// 지정한 Node Container가 TreeView라면 Nodes 들을 열거하고, TreeNode라면 자신을 반환한다.
        /// </summary>
        /// <param name="nodeContainer"></param>
        /// <returns></returns>
        public static IEnumerable<RadTreeNode> GetRootNodesOrSelfNode(this IRadTreeNodeContainer nodeContainer)
        {
            if(IsDebugEnabled)
                log.Debug(@"지정한 Node Container가 TreeView라면 Nodes 들을 열거하고, TreeNode라면 자신을 반환한다. nodeContainer=[{0}]", nodeContainer.AsTextAndValue());

            if(nodeContainer == null)
                yield break;

            if(nodeContainer is RadTreeNode)
                yield return (RadTreeNode)nodeContainer;

            else if(nodeContainer is RadTreeView)
            {
                var rootNodes = ((RadTreeView)nodeContainer).Nodes;
                foreach(RadTreeNode node in rootNodes)
                    yield return node;
            }
        }

        /// <summary>
        /// 특정 NodeContainer의 자손 중 자식 노드가 없는 TreeNode들 (나무 잎) 을 열거합니다.
        /// </summary>
        /// <param name="nodeContainer"></param>
        /// <returns></returns>
        public static IEnumerable<RadTreeNode> FindLeafNodes(this IRadTreeNodeContainer nodeContainer)
        {
            nodeContainer.ShouldNotBeNull("nodeContainer");

            if(IsDebugEnabled)
                log.Debug(@"특정 NodeContainer 하위의 모든 Leaf Node (자식이 없는 노드) 들을 Depth First 방식으로 찾습니다. nodeContainer=[{0}]", nodeContainer.AsTextAndValue());

            return nodeContainer.GetDescendentNodes().Where(n => n.Nodes.Count == 0);
        }

        /// <summary>
        /// nodeContainer의 자신과 자손 TreeNode를 깊이 우선 탐색 방식으로 열거합니다.
        /// </summary>
        /// <param name="nodeContainer">기준 노드 또는 TreeView</param>
        /// <returns></returns>
        public static IEnumerable<RadTreeNode> GetNodesByDepthFirst(this IRadTreeNodeContainer nodeContainer)
        {
            if(nodeContainer == null)
                yield break;

            if(IsDebugEnabled)
                log.Debug(@"특정 NodeContainer를 깊이 우선 탐색을 수행합니다. nodeContainer=[{0}]", nodeContainer.AsTextAndValue());

            foreach(RadTreeNode root in GetRootNodesOrSelfNode(nodeContainer))
            {
                yield return root;

                foreach(var node in root.GraphDepthFirstScan(n => n.Nodes.Cast<RadTreeNode>()))
                    yield return node;
            }
        }

        /// <summary>
        /// nodeContainer의 자신과 자손 TreeNode를 폭 우선 탐색 방식으로 열거합니다.
        /// </summary>
        /// <param name="nodeContainer"></param>
        /// <returns></returns>
        public static IEnumerable<RadTreeNode> GetNodesByBreadthFirst(this IRadTreeNodeContainer nodeContainer)
        {
            if(nodeContainer == null)
                yield break;

            if(IsDebugEnabled)
                log.Debug(@"특정 NodeContainer를 폭 우선 탐색을 수행합니다. nodeContainer=[{0}]", nodeContainer.AsTextAndValue());

            // CollectionExtensions.GraphBreadthFirstScan() 을 사용할 수 있지만, TreeView의 RootNode가 여러 개라면, 폭 우선 탐색이 되지 않는다. 
            // 그래서 여기서 전체를 다 구현했다.

            var toScan = new Queue<RadTreeNode>(GetRootNodesOrSelfNode(nodeContainer));
            var scanned = new HashSet<RadTreeNode>();

            while(toScan.Count > 0)
            {
                RadTreeNode current = toScan.Dequeue();

                yield return current;
                scanned.Add(current);

                foreach(RadTreeNode item in current.Nodes.Cast<RadTreeNode>())
                {
                    if(scanned.Contains(item) == false)
                        toScan.Enqueue(item);
                }
            }
        }

        /// <summary>
        /// 특정 TreeNode부터 조상들의 노드들을 차례로 열거합니다.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static IEnumerable<RadTreeNode> GetAncestorNodes(this RadTreeNode node)
        {
            if(node == null)
                yield break;

            if(IsDebugEnabled)
                log.Debug(@"특정 nodeContainer의 자신과 모든 조상 노드들을 열거합니다... nodeContainer=[{0}]", node.AsTextAndValue());

            var current = node;
            while(current != null)
            {
                yield return current;
                current = current.ParentNode;
            }
        }

        /// <summary>
        /// 최상위 조상이 Stack의 가장 위에 있고, 지정한 node가 가장 아래에 있도록 Stack을 만들어 반환합니다. 
        /// Stack에서 하나씩 꺼내서 Expand 시키거나 Navigation할 때 필요합니다.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static Stack<RadTreeNode> GetAncestorNodeAsStack(this RadTreeNode node)
        {
            var stack = new Stack<RadTreeNode>();

            if(IsDebugEnabled)
                log.Debug(@"특정 nodeContainer의 자신과 모든 조상 노드들을 Stack에 담습니다. 최상위 조상이 Stack의 최상위에 위치하도록 합니다. node=[{0}]", node.AsTextAndValue());

            foreach(var ancestorNode in node.GetAncestorNodes())
                stack.Push(ancestorNode);

            return stack;
        }

        /// <summary>
        /// 깊이 우선 탐색을 통해, 현재 Node와 Node의 자손 Node들을 열거합니다. 열거 순서에 상관없으려면 node.GetAllNodes()를 사용하세요
        /// </summary>
        /// <param name="nodeContainer"></param>
        /// <returns></returns>
        public static IEnumerable<RadTreeNode> GetDescendentNodes(this IRadTreeNodeContainer nodeContainer)
        {
            if(nodeContainer == null)
                yield break;

            if(IsDebugEnabled)
                log.Debug(@"특정 nodeContainer의 자신과 모든 자손 노드들을 열거합니다... nodeContainer=[{0}]", nodeContainer.AsTextAndValue());

            foreach(RadTreeNode rootNode in GetRootNodesOrSelfNode(nodeContainer))
                foreach(var childNode in rootNode.GraphDepthFirstScan(n => n.Nodes.Cast<RadTreeNode>()))
                    yield return childNode;
        }

        /// <summary>
        /// 특정 TreeNode를 Expand 시켜서, 노드의 자식들을 보이도록 합니다.
        /// </summary>
        public static void ExpandNode(this RadTreeNode node)
        {
            ExpandNode(node, false);
        }

        /// <summary>
        /// 특정 TreeNode를 Expand 시켜서, 노드의 자식들을 보이도록 합니다.
        /// </summary>
        public static void ExpandNode(this RadTreeNode node, bool expandSelf)
        {
            if(node == null)
                return;

            node.ExpandParentNodes();
            node.Expanded = expandSelf;
        }

        /// <summary>
        /// 특정 노드에 Focus를 줍니다. Focus를 주려면, 현재 노드가 보여야 하므로, 부모 노드들을 모두 Expand 시킵니다.
        /// </summary>
        /// <param name="node"></param>
        public static void FocusNode(this RadTreeNode node)
        {
            if(node == null)
                return;

            if(node.ParentNode != null && node.ParentNode.Expanded == false)
                node.ExpandParentNodes();

            node.Focus();
        }

        /// <summary>
        /// 특정 노드를 선택 상태로 설정합니다.
        /// </summary>
        /// <param name="node"></param>
        public static void SelectNode(this RadTreeNode node)
        {
            if(node == null)
                return;

            if(IsDebugEnabled)
                log.Debug(@"지정한 TreeNode를 선택된 노드로 설정합니다. node=[{0}]", node.AsTextAndValue());

            node.FocusNode();

            node.Selected = true;
            node.Expanded = true;
        }

        public static void SelectNodeByValue(this RadTreeView treeview, string nodeValue)
        {
            if(IsDebugEnabled)
                log.Debug(@"TreeView에 특정 NodeValue를 가지는 TreeNode를 찾아서, Selected Node로 설정합니다. nodeValue=[{0}]", nodeValue);

            var node = treeview.GetRootNodesOrSelfNode().FirstOrDefaultByValue(nodeValue);

            if(node != null)
                node.SelectNode();
        }

        /// <summary>
        /// 특정 TreeNode의 자식으로 노드를 추가합니다.	NFramework.Data.NHibernateEx.Domain.ITreeNodeEntity를 구현한 클래스에 대해서는 BuildTreeNodes 함수를 사용하세요.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="nodeFactory"></param>
        /// <param name="items"></param>
        public static void AddNodes<T>(this IRadTreeNodeContainer parent, Func<T, RadTreeNode> nodeFactory, IEnumerable<T> items)
        {
            AddNodes<T>(parent, nodeFactory, null, items);
        }

        /// <summary>
        /// 특정 TreeNode의 자식으로 노드를 추가합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="nodeFactory">노드 생성 메소드</param>
        /// <param name="nodeAddedAction">노드 추가마다 호출되는 메소드</param>
        /// <param name="items"></param>
        public static void AddNodes<T>(this IRadTreeNodeContainer parent,
                                       Func<T, RadTreeNode> nodeFactory,
                                       Action<T, RadTreeNode> nodeAddedAction,
                                       IEnumerable<T> items)
        {
            if(items.IsEmptySequence())
                return;

            parent.ShouldNotBeNull("parent");
            nodeFactory.ShouldNotBeNull("nodeFactory");

            if(IsDebugEnabled)
                log.Debug(@"TreeNode의 자식 노드들을 추가합니다...");

            foreach(T item in items)
            {
                if(IsDebugEnabled)
                    log.Debug(@"엔티티를 RadTreeNode로 빌드합니다. item=[{0}]", item);

                var childNode = nodeFactory(item);

                if(childNode != null)
                {
                    parent.Nodes.Add(childNode);

                    if(IsDebugEnabled)
                        log.Debug(@"TreeNode에 자식 노드를 추가했습니다. parent=[{0}], childNode=[{1}]", parent, childNode.Text);

                    if(nodeAddedAction != null)
                        nodeAddedAction(item, childNode);
                }
            }
        }

        /// <summary>
        /// <see cref="ITreeNodeEntity{T}"/>를 구현한 엔티티들을 표현할 TreeNode를 생성하여 TreeView에 추가합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent">부모 노드 또는 TreeView</param>
        /// <param name="nodeFactory">TreeNode를 생성하는 Factory 함수</param>
        /// <param name="findNodeFunc">기존 TreeView에서 엔티티와 관련된 Node를 찾는 함수</param>
        /// <param name="nodeAddedAction">TreeNode 하나를 추가할 때마다 수행하는 Action</param>
        /// <param name="items">엔티티</param>
        public static void BuildTreeNodes<T>(this IRadTreeNodeContainer parent,
                                             Func<T, RadTreeNode> nodeFactory,
                                             Func<T, RadTreeNode> findNodeFunc,
                                             Action<T, RadTreeNode> nodeAddedAction,
                                             IEnumerable<T> items) where T : class, ITreeNodeEntity<T>
        {
            BuildTreeNodes(parent, nodeFactory, findNodeFunc, nodeAddedAction, items, false);
        }

        /// <summary>
        /// <see cref="ITreeNodeEntity{T}"/>를 구현한 엔티티들을 표현할 TreeNode를 생성하여 TreeView에 추가합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent">부모 노드 또는 TreeView</param>
        /// <param name="nodeFactory">TreeNode를 생성하는 Factory 함수</param>
        /// <param name="findNodeFunc">기존 TreeView에서 엔티티와 관련된 Node를 찾는 함수</param>
        /// <param name="nodeAddedAction">TreeNode 하나를 추가할 때마다 수행하는 Action</param>
        /// <param name="items">엔티티</param>
        /// <param name="includeChildren">엔티티의 자손들도 Binding 할 것인가?</param>
        public static void BuildTreeNodes<T>(this IRadTreeNodeContainer parent,
                                             Func<T, RadTreeNode> nodeFactory,
                                             Func<T, RadTreeNode> findNodeFunc,
                                             Action<T, RadTreeNode> nodeAddedAction,
                                             IEnumerable<T> items,
                                             bool includeChildren) where T : class, ITreeNodeEntity<T>
        {
            if(items.IsEmptySequence())
                return;

            parent.ShouldNotBeNull("parent");
            nodeFactory.ShouldNotBeNull("nodeFactory");
            findNodeFunc.ShouldNotBeNull("findNodeFunc");

            RadTreeNode parentNode = null;

            foreach(T item in items)
            {
                // 이미 관련 노드가 있다면 추가하지 않습니다.
                if(findNodeFunc(item) != null)
                    continue;

                if(IsDebugEnabled)
                    log.Debug(@"엔티티를 TreeNode로 빌드하여, TreeView에 추가합니다... item=[{0}]", item);

                var childNode = nodeFactory(item);

                if(item.Parent != null)
                    parentNode = findNodeFunc(item.Parent);

                if(parentNode != null)
                {
                    // 부모 노드가 이미 TreeView에 추가되어 있다면, 부모 노드에 자식으로 추가합니다.
                    parentNode.Nodes.Add(childNode);
                }
                else
                {
                    // 부모 노드가 없다면, parent에 추가한다.
                    parent.Nodes.Add(childNode);
                }

                if(IsDebugEnabled)
                    log.Debug(@"엔티티를 TreeNode로 빌드하여, TreeView에 추가했습니다!!!. childNode=[{0}]", childNode);

                if(nodeAddedAction != null)
                    nodeAddedAction(item, childNode);

                // 자식 노드들이 있고, 추가해야 한다면 DepthFirst 방식으로 추가합니다.
                //
                if(includeChildren && item.GetChildCount() > 0)
                    BuildTreeNodes(parentNode ?? parent, nodeFactory, findNodeFunc, nodeAddedAction, item.Children, true);
            }
        }

        /// <summary>
        /// <see cref="nodeContainer"/>의 모든 Node에 대한 정보를 문자열로 표현하도록 합니다. 디버그 시에만 사용하세요.
        /// </summary>
        /// <param name="nodeContainer"></param>
        /// <returns></returns>
        public static string AsTextAndValue(this IRadTreeNodeContainer nodeContainer)
        {
            if(nodeContainer == null)
                return @"NULL";

            if(nodeContainer is RadTreeNode)
            {
                var node = (RadTreeNode)nodeContainer;
                return string.Format(@"RadTreeNode#Text={0},Value={1}", node.Text, node.Value);
            }

            if(nodeContainer is RadTreeView)
            {
                var treeview = (RadTreeView)nodeContainer;
                return string.Format(@"RadTreeView#ID={0},ClientID={1},SelectedValue={2}", treeview.ID, treeview.ClientID, treeview.SelectedValue);
            }

            return nodeContainer.ObjectToString();
        }
    }
}
