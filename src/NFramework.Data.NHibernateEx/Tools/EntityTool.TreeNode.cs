using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NSoft.NFramework.LinqEx;

namespace NSoft.NFramework.Data.NHibernateEx {
    public static partial class EntityTool {
        /// <summary>
        /// Node Position을 갱신합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        public static void UpdateNodePosition<T>(this T node) where T : ITreeNodeEntity<T> {
            node.ShouldNotBeDefault("node");

            if(IsDebugEnabled)
                log.Debug("트리 노드 위치를 갱신합니다... node=[{0}]", node);

            if(ReferenceEquals(node.Parent, null) == false)
                // if(node.Parent != null)
            {
                node.NodePosition.Level = node.Parent.NodePosition.Level + 1;
                var index = node.Parent.Children.IndexOf(node);
                node.NodePosition.Order = (index > -1) ? index : 0;
            }
            else {
                node.NodePosition.Level = 0;
                node.NodePosition.Order = 0;
            }
        }

        /// <summary>
        /// TreeNode의 자식의 숫자를 구한다. (TreeView 구성 시 Populating을 구현할 때 필요하다)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        public static int GetChildCount<T>(this T node) where T : class, ITreeNodeEntity<T> {
            if(IsDebugEnabled)
                log.Debug("지정한 노드의 자식 노드의 갯수를 구합니다... node=[{0}]", node);

            var childCount =
                UnitOfWork.CurrentSession
                    .QueryOver<T>()
                    .Where(n => n.Parent == node)
                    .RowCount();

            if(IsDebugEnabled)
                log.Debug("지정한 노드의 자식 노드의 갯수를 구했습니다. node=[{0}], childCount=[{1}]", node, childCount);

            return childCount;
        }

        /// <summary>
        /// TreeNode의 자식이 존재하는지를 알아본다. 자식의 수를 세는 것보다 빠르다. (TreeView 구성 시 Populating을 구현할 때 필요하다)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        public static bool HasChildren<T>(this T node) where T : class, ITreeNodeEntity<T> {
            return GetChildCount<T>(node) > 0;
        }

        /// <summary>
        /// 노드의 Order를 지정된 값으로 설정한다. 
        /// </summary>
        public static void SetNodeOrder<T>(this T node, int order) where T : ITreeNodeEntity<T> {
            node.ShouldNotBeDefault("node");
            order.ShouldBePositiveOrZero("order");

            if(IsDebugEnabled)
                log.Debug("노드의 Order 를 설정합니다... node=[{0}], order=[{1}]", node, order);

            if(ReferenceEquals(node.Parent, null) == false) {
                var oldOrder = node.NodePosition.Order.GetValueOrDefault(order);

                // Sibling node중에 새로운 view order보다 같거나 큰 놈들은 1씩 증가시킨다.
                var nexts =
                    node.Parent.Children.Where(child => child.NodePosition.Order.HasValue && child.NodePosition.Order.Value >= oldOrder);
                nexts.RunEach(child => child.NodePosition.Order++);
            }

            node.NodePosition.Order = order;
        }

        /// <summary>
        /// 노드의 자식 노드들의 순서를 재정렬 합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        public static void AdjustChildOrders<T>(this T parent) where T : ITreeNodeEntity<T> {
            parent.ShouldNotBeDefault("parent");

            if(IsDebugEnabled)
                log.Debug("노드의 자식 노드들의 순서를 재정렬 합니다...");

            var order = 0;
            foreach(T node in parent.Children.OrderBy(child => child.NodePosition.Order))
                node.NodePosition.Order = order++;
        }

        /// <summary>
        /// Node의 부모를 변경합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node">자식 노드</param>
        /// <param name="oldParent">기존 부모</param>
        /// <param name="newParent">새로운 부모</param>
        public static void ChangeParent<T>(this T node, T oldParent, T newParent) where T : ITreeNodeEntity<T> {
            node.ShouldNotBeDefault<T>("node");

            if(IsDebugEnabled)
                log.Debug("노드의 부모를 변경합니다... node=[{0}], oldParent=[{1}], newParent=[{2}]", node, oldParent, newParent);

            if(Equals(oldParent, default(T)) == false) {
                oldParent.Children.Remove(node);
                UnitOfWork.CurrentSession.SaveOrUpdate(oldParent);
            }

            if(Equals(newParent, default(T)) == false) {
                newParent.Children.Add(node);
                UnitOfWork.CurrentSession.SaveOrUpdate(newParent);
            }

            node.Parent = newParent;
            node.UpdateNodePosition();
        }

        /// <summary>
        /// 자식노드의 부모를 지정한 부모 노드로 설정한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node">자식 노드</param>
        /// <param name="parent">부모가 될 노드</param>
        public static void SetParent<T>(this T node, T parent) where T : ITreeNodeEntity<T> {
            node.ShouldNotBeDefault<T>("node");

            if(IsDebugEnabled)
                log.Debug("지정된 노드의 부모를 설정합니다. node=[{0}], parent=[{1}]", node, parent);

            node.ChangeParent(node.Parent, parent);
        }

        /// <summary>
        /// 부모 Node에게 자식노드를 추가한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <param name="order"></param>
        public static void InsertChildNode<T>(this T parent, T child, int order) where T : class, ITreeNodeEntity<T> {
            parent.ShouldNotBeDefault<T>("parent");
            child.ShouldNotBeDefault<T>("child");

            if(IsDebugEnabled)
                log.Debug("부모 노드에 자식 노드를 지정된 순서에 삽입합니다... parent=[{0}], child=[{1}], order=[{2}]",
                          parent, child, order);

            order = Math.Max(0, Math.Min(order, parent.Children.Count - 1));

            child.Parent = parent;
            parent.Children.Add(child);

            child.SetNodeOrder(order);
        }

        /// <summary>
        /// 지정된 <see cref="ITreeNodePosition"/> 속성 값을 원본 객체의 속성값으로 설정한다.
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="src"></param>
        public static void CopyFrom(this ITreeNodePosition dest, ITreeNodePosition src) {
            if(src != null) {
                dest.Level = src.Level;
                dest.Order = src.Order;
            }
            else {
                dest.Level = 0;
                dest.Order = 0;
            }
        }

        /// <summary>
        /// 지정한 TreeNode Entity와 모든 조상을 가져온다.
        /// </summary>
        /// <typeparam name="T">TreeNode 엔티티의 수형</typeparam>
        /// <param name="current">기준이 되는 ITreeNodeEntity{T}</param>
        /// <returns>지정한 TreeNode의 모든 조상 노드들 (자신은 제외)</returns>
        public static IEnumerable<T> GetAncestors<T>(this T current) where T : class, ITreeNodeEntity<T> {
            if(IsDebugEnabled)
                log.Debug("지정된 노드의 모든 조상 노드를 조회한다... current=[{0}]", current);

            if(current == null)
                yield break;

            var parent = current;

            while(parent != null) {
                yield return parent;
                parent = parent.Parent;
            }
        }

        /// <summary>
        /// 지정된 TreeNode Entity와 모든 자손들을 가져온다.
        /// </summary>
        /// <typeparam name="T">TreeNode 엔티티의 수형</typeparam>
        /// <param name="current">기준이 되는 ITreeNodeEntity{T}</param>
        /// <returns>지정한 TreeNode의 모든 자손 노드들 (자신은 제외)</returns>
        public static IEnumerable<T> GetDescendents<T>(this T current) where T : ITreeNodeEntity<T> {
            current.ShouldNotBeDefault("current");

            if(IsDebugEnabled)
                log.Debug("지정된 노드의 모든 자손 노드를 깊이 우선 탐색으로 조회한다... current=[{0}]", current);

            return current.GraphDepthFirstScan(node => node.Children).Where(child => !Equals(child, null));
        }

        /// <summary>
        /// 지정된 TreeNode의 최상위 부모를 반환합니다.
        /// </summary>
        /// <typeparam name="T">TreeNode 엔티티의 수형</typeparam>
        /// <param name="current">기준이 되는 ITreeNodeEntity{T}</param>
        /// <returns>최상위 Root Node</returns>
        public static T GetRoot<T>(this T current) where T : ITreeNodeEntity<T> {
            if(Equals(current, null))
                return default(T);

            if(IsDebugEnabled)
                log.Debug("노드의 최상위 조상 (RootNode)를 조회합니다. current=[{0}]", current);

            var root = current;
            var parent = current.Parent;

            while(Equals(parent, null) == false) {
                root = parent;
                parent = parent.Parent;
            }

            return root;
        }

        /// <summary>
        /// 트리의 Root Node들을 구합니다. (부모가 null 인 TreeNode가 RootNode가 됩니다)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IList<T> GetRoots<T>() where T : class, ITreeNodeEntity<T> {
            if(IsDebugEnabled)
                log.Debug("최상위 부모 노드들을 조회합니다...");

            return
                Repository<T>.Session
                    .QueryOver<T>()
                    .Where(node => node.Parent == null)
                    .List();
        }

        /// <summary>
        /// 지정된 TreeNode의 최상위 부모들의 갯수를 구합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int GetRootCount<T>() where T : class, ITreeNodeEntity<T> {
            if(IsDebugEnabled)
                log.Debug("최상위 부모 노드의 갯수를 조회합니다.");

            return
                Repository<T>.Session
                    .QueryOver<T>()
                    .Where(node => node.Parent == null)
                    .RowCount();
        }

        /// <summary>
        /// 트리의 모든 끝 노드(자식이 없는 노드)들 구합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IList<T> GetLeafs<T>() where T : class, ITreeNodeEntity<T> {
            if(IsDebugEnabled)
                log.Debug("자식이 없는 끝 노드 (잎새 노드)들을 조회합니다...");

            return
                Repository<T>.Session
                    .Query<T>()
                    .Where(node => node.Children.Any() == false)
                    .ToList();
        }

        /// <summary>
        /// 트리의 모든 끝 노드(자식이 없는 노드)의 갯수를 구합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int GetLeafCount<T>() where T : class, ITreeNodeEntity<T> {
            if(IsDebugEnabled)
                log.Debug("자식이 없는 끝 노드 (잎새 노드)들을 갯수를 조회합니다.");

            return
                Repository<T>.Session
                    .Query<T>()
                    .Count(node => node.Children.Any() == false);
        }
    }
}