using System.Collections.Generic;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using Telerik.Web.UI;

namespace NSoft.NFramework.Web.TelerikEx
{
	/// <summary>
	/// RadTreeView에 대한 처리를 도와주는 Adapter입니다.
	/// </summary>
	/// <typeparam name="T"><see cref="ITreeNodeEntity{T}"/></typeparam>
	public interface IRadTreeViewAdapter<T>
	{
		/// <summary>
		/// RadControls for ASP.NET의 TreeView Control
		/// </summary>
		RadTreeView TreeViewCtrl { get; }

		/// <summary>
		/// 선택된 TreeNode의 값 (선택된 값이 없을 때에는 null이다)
		/// </summary>
		string SelectedNodeValue { get; set; }

		/// <summary>
		/// 새로운 RadTreeNode를 생성합니다.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		RadTreeNode CreateNode(T item);

		/// <summary>
		/// 엔티티로부터 TreeNode의 Value 속성을 생성합니다.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		string CreateNodeValue(T item);

		/// <summary>
		/// TreeNode가 추가되고, 후처리를 지정할 수 있습니다.
		/// </summary>
		void NodeAdded(T item, RadTreeNode addedNode);

		/// <summary>
		/// <see cref="ITreeNodeEntity{T}"/> 형식의 정보를 TreeView의 Node로 빌드합니다.
		/// </summary>
		/// <param name="items"></param>
		void AddNodes(IEnumerable<T> items);

		/// <summary>
		/// <paramref name="parent"/>에 Node를 추가합니다.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="items"></param>
		void AddNodes(IRadTreeNodeContainer parent, IEnumerable<T> items);

		/// <summary>
		/// TreeView 의 첫번째 RootNode가 선택되어 Focus를 가지도록 합니다.
		/// </summary>
		void SelectFirstNode();

		/// <summary>
		/// 지정된 정보와 매핑된 TreeNode를 찾습니다.
		/// </summary>
		RadTreeNode FindNodeByItem(T item);
	}
}