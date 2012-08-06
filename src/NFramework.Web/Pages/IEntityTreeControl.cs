using NSoft.NFramework.Data.NHibernateEx.Domain;
using NSoft.NFramework.Web.TelerikEx;
using Telerik.Web.UI;

namespace RCL.Web.Pages
{
    /// <summary>
    /// 트리 바인딩
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEntityTreeControl<T> where T : class, ITreeNodeEntity<T>
    {
        /// <summary>
        /// TreeViewControl의 NodeClick 이벤트를 외부로 Bubbling하기 만든 이벤트입니다.
        /// </summary>
        event RadTreeViewEventHandler TreeNodeClick;

        /// <summary>
        /// TreeView에 표현해주는 Adapter입니다. Initialize시에 생성되어야 합니다.
        /// </summary>
        IRadTreeViewAdapter<T> TreeViewAdapter { get; }

        /// <summary>
        /// 트리 뷰 컨트롤
        /// </summary>
        RadTreeView EntityTreeView { get; }

        /// <summary>
        /// 선택된 Node
        /// </summary>
        RadTreeNode SelectedNode { get; }

        /// <summary>
        /// 선택된 Node 의 Parent
        /// </summary>
        RadTreeNode SelectedParentNode { get; }
    }
}