using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NSoft.NFramework.Reflections;
using Telerik.Web.UI;

namespace NSoft.NFramework.Web.TelerikEx.TreeView
{
	/// <summary>
	/// <see cref="ITreeNodeEntity{T}"/> 형태의 엔티티를 TreeView에 표현할 수 있도록 하는 Adapter 클래스입니다.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class TreeNodeEntityTreeViewAdapterAdapterBase<T> : RadTreeViewAdapterAdapterBase<T> where T : class, ITreeNodeEntity<T>
	{
		#region << logger >>

		private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
		private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

		#endregion

		/// <summary>
		/// 생성자
		/// </summary>
		/// <param name="treeViewCtrl"><c>RadTreeView</c></param>
		protected TreeNodeEntityTreeViewAdapterAdapterBase(RadTreeView treeViewCtrl) : base(treeViewCtrl) { }

		/// <summary>
		/// 지정된 노드에 Tree를 추가합니다.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="items"></param>
		public override void AddNodes(IRadTreeNodeContainer parent, IEnumerable<T> items)
		{
			if(IsDebugEnabled)
				log.Debug(@"지정된 엔티티들을 TreeView의 노드로 추가합니다... parent={0}, items={1}",
						  parent, items.CollectionToString());

			if(items != null)
				(parent ?? TreeViewCtrl).BuildTreeNodes(CreateNode, FindNodeByItem, NodeAdded, items);
		}
	}
}
