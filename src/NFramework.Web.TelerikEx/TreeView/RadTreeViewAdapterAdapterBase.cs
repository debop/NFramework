using System;
using System.Collections.Generic;
using NSoft.NFramework.Reflections;
using Telerik.Web.UI;

namespace NSoft.NFramework.Web.TelerikEx.TreeView
{
	[Serializable]
	public class RadTreeViewAdapterAdapterBase<T> : IRadTreeViewAdapter<T>
	{
		#region << logger >>

		private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
		private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

		#endregion

		/// <summary>
		/// 생성자
		/// </summary>
		/// <param name="treeViewCtrl"><c>RadTreeView</c></param>
		protected RadTreeViewAdapterAdapterBase(RadTreeView treeViewCtrl)
		{
			treeViewCtrl.ShouldNotBeNull("treeViewCtrl");
			TreeViewCtrl = treeViewCtrl;
		}

		/// <summary>
		/// TreeView Control
		/// </summary>
		public RadTreeView TreeViewCtrl { get; private set; }

		/// <summary>
		/// 선택된 TreeNode의 값 (선택된 값이 없을 때에는 null이다)
		/// </summary>
		public virtual string SelectedNodeValue
		{
			get { return TreeViewCtrl.SelectedValue; }
			set
			{
				if(SelectedNodeValue != value)
				{
					var node = TreeViewCtrl.FindNodeByValue(value, true);
					if(node != null)
						node.SelectNode();
				}
			}
		}

		/// <summary>
		/// TreeNode가 추가되고, 후처리를 지정할 수 있습니다.
		/// </summary>
		public virtual void NodeAdded(T item, RadTreeNode addedNode)
		{
			if(IsDebugEnabled)
				log.Debug(@"새로운 TreeNode가 추가되었습니다. item={0}, addedNode={1}", item, addedNode);
		}

		/// <summary>
		/// 새로운 RadTreeNode를 생성합니다.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public virtual RadTreeNode CreateNode(T item)
		{
			item.ShouldNotBeDefault<T>("item");

			if(IsDebugEnabled)
				log.Debug(@"새로운 TreeNode를 생성합니다. item=[{0}]", item);

			return
				new RadTreeNode
				{
					Text = item.ToString(),
					Value = CreateNodeValue(item)
				};
		}

		/// <summary>
		/// 엔티티로부터 TreeNode의 Value 속성을 생성합니다.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public virtual string CreateNodeValue(T item)
		{
			var nodeValue = item.ToString();

			if(IsDebugEnabled)
				log.Debug(@"TreeNode의 Value를 생성했습니다. nodeValue={0} from item={1}", nodeValue, item);

			return nodeValue;
		}

		/// <summary>
		/// <see cref="TreeViewCtrl"/>에 컬렉션을 바인딩합니다.
		/// </summary>
		/// <param name="items"></param>
		public void AddNodes(IEnumerable<T> items)
		{
			if(IsDebugEnabled)
				log.Debug(@"RadTreeView의 현재 모든 정보를 삭제하고, 지정된 정보를 Node로 추가합니다...");

			TreeViewCtrl.Nodes.Clear();
			AddNodes(TreeViewCtrl, items);
		}

		/// <summary>
		/// 지정된 노드에 Tree를 추가합니다.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="items"></param>
		public virtual void AddNodes(IRadTreeNodeContainer parent, IEnumerable<T> items)
		{
			if(IsDebugEnabled)
				log.Debug(@"지정된 엔티티들을 TreeView의 노드로 추가합니다... parent={0}, items={1}",
						  parent, items.CollectionToString());

			TelerikTool.AddNodes(parent ?? TreeViewCtrl, CreateNode, NodeAdded, items);
		}

		/// <summary>
		/// TreeView 의 첫번째 RootNode가 선택되어 Focus를 가지도록 합니다.
		/// </summary>
		public virtual void SelectFirstNode()
		{
			if(TreeViewCtrl.Nodes.Count > 0)
				TreeViewCtrl.Nodes[0].SelectNode();
		}
		/// <summary>
		/// 지정된 정보와 매핑된 TreeNode를 찾습니다.
		/// </summary>
		public virtual RadTreeNode FindNodeByItem(T item)
		{
			if(IsDebugEnabled)
				log.Debug(@"지정된 요소와 매핑된 TreeNode를 찾습니다. item=[{0}]", item);

			var node = TreeViewCtrl.FindNodeByValue(CreateNodeValue(item), true);

			if(IsDebugEnabled)
			{
				if(node != null)
					log.Debug(@"검색된 TreeNode Text=[{0}], Value=[{1}]", node.Text, node.Value);
				else
					log.Debug(@"일치하는 노드를 찾지 못했습니다. null을 반환합니다.");
			}

			return node;
		}
	}
}
