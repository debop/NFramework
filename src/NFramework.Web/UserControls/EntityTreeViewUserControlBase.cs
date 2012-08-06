using System;
using System.Collections.Generic;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NSoft.NFramework.Web.TelerikEx;
using NSoft.NFramework.Web.Tools;
using Telerik.Web.UI;

namespace NSoft.NFramework.Web.UserControls
{
    /// <summary>
    /// TreeView 표현 컨트롤
    /// </summary>
    /// <typeparam name="T">ITreeNodeEntity</typeparam>
    public abstract class EntityTreeViewUserControlBase<T> : UserControlBase, IEntityTreeViewUserControl<T> where T : class, ITreeNodeEntity<T>
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly Type _concreteType = typeof(T);

        /// <summary>
        /// 엔티티의 형식
        /// </summary>
        public static Type ConcreteType
        {
            get { return _concreteType; }
        }

        /// <summary>
        /// TreeViewControl의 NodeClick 이벤트를 외부로 Bubbling하기 만든 이벤트입니다.
        /// </summary>
        public event RadTreeViewEventHandler TreeNodeClick = delegate { };

        /// <summary>
        /// TreeView에 표현해주는 Adapter입니다. Initialize시에 생성되어야 합니다.
        /// </summary>
        public virtual IRadTreeViewAdapter<T> TreeViewAdapter { get; set; }

        /// <summary>
        /// 트리 뷰 컨트롤
        /// </summary>
        public abstract RadTreeView EntityTreeView { get; }

        /// <summary>
        /// TreeView에 바인딩할 엔티티 컬렉션
        /// </summary>
        public virtual IEnumerable<T> Entities { get; set; }

        /// <summary>
        /// 선택된 Node
        /// </summary>
        public RadTreeNode SelectedNode
        {
            get { return EntityTreeView.SelectedNode; }
        }

        /// <summary>
        /// 선택된 Node 의 Parent
        /// </summary>
        public RadTreeNode SelectedParentNode
        {
            get { return SelectedNode != null ? SelectedNode.ParentNode : null; }
        }

        /// <summary>
        /// OnInit
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            TreeViewAdapter = CreateTreeViewAdapter();

            // node 선택 시 Control외부로 Event를 전파하기 위해 사용합니다.
            if(TreeNodeClick != null)
                EntityTreeView.NodeClick += TreeNodeClick;
            EntityTreeView.NodeExpand += new RadTreeViewEventHandler(EntityTreeView_NodeExpand);
        }

        /// <summary>
        /// TreeView 확장시 이벤트
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">RadTreeNodeEventArgs</param>
        private void EntityTreeView_NodeExpand(object sender, RadTreeNodeEventArgs e)
        {
            if(IsDebugEnabled)
                log.Debug(@"==>S TreeView의 NodeExpand 이벤트가 발생하였습니다.");

            LoadAndAddEntity(e.Node, null);
        }

        /// <summary>
        /// Page Load 시 수행할 작업
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if(IsPostBack == false)
                DisplayDefault();
        }

        /// <summary>
        /// OnUnload
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected override void OnUnload(EventArgs e)
        {
            if(TreeNodeClick != null)
                EntityTreeView.NodeClick -= TreeNodeClick;

            base.OnUnload(e);
        }

        /// <summary>
        /// 초기 화면 표시
        /// </summary>
        protected virtual void DisplayDefault()
        {
            LoadAndBindEntity();
        }

        /// <summary>
        ///  엔티티를 로드하고, UI Control에 바인딩 합니다. 
        /// </summary>
        protected void LoadAndBindEntity()
        {
            LoadAndBindEntity(null);
        }

        /// <summary>
        ///  엔티티를 로드하고, UI Control에 바인딩 합니다. 
        /// </summary>
        /// <param name="exceptionAction">예외발생 시 수행할 Action</param>
        protected virtual void LoadAndBindEntity(Action<Exception> exceptionAction)
        {
            if(IsDebugEnabled)
                log.Debug(@"==>S 엔티티[{0}] 목록을 로드하여, TreeView에 바인딩 시킵니다...", ConcreteType.Name);
            try
            {
                LoadEntity();
                BindEntity();
            }
            catch(Exception ex)
            {
                var errorMsg = string.Format(@"엔티티[{0}]를 로드하고, Binding 시에 예외가 발생했습니다.", ConcreteType.Name);

                if(log.IsErrorEnabled)
                    log.ErrorException(errorMsg, ex);

                if(exceptionAction != null)
                    exceptionAction(ex);
                else
                    WebAppTool.MessageBox(errorMsg, this);
            }
        }

        /// <summary>
        ///  엔티티를 로드하고, IRadTreeNodeContainer에 자식노드로 추가한다.
        /// </summary>
        /// <param name="node">선택된 IRadTreeNodeContainer</param>
        /// <param name="exceptionAction">예외발생 시 수행할 Action</param>
        protected virtual void LoadAndAddEntity(IRadTreeNodeContainer node, Action<Exception> exceptionAction)
        {
            if(IsDebugEnabled)
                log.Debug(@"==>S 엔티티[{0}] 목록을 로드하여, TreeView에 바인딩 시킵니다...", ConcreteType.Name);

            node.ShouldNotBeNull("선택된 IRadTreeNodeContainer정보가 없습니다.");

            try
            {
                LoadEntity(node);
                AddEntity(node);
            }
            catch(Exception ex)
            {
                var errorMsg = string.Format(@"엔티티[{0}]를 로드하고, Binding 시에 예외가 발생했습니다.", ConcreteType.Name);

                if(log.IsErrorEnabled)
                    log.ErrorException(errorMsg, ex);

                if(exceptionAction != null)
                    exceptionAction(ex);
                else
                    WebAppTool.MessageBox(errorMsg, this);
            }
        }

        /// <summary>
        /// IRadTreeViewAdapter 생성자
        /// </summary>
        /// <returns>IRadTreeViewAdapter</returns>
        protected abstract IRadTreeViewAdapter<T> CreateTreeViewAdapter();

        /// <summary>
        /// 엔티티 목록을 로드하여 <see cref="Entities"/>에 할당합니다.
        /// </summary>
        protected abstract void LoadEntity();

        /// <summary>
        /// 현재 노드에 대한 자식 엔티티 목록을 로드하여 <see cref="Entities"/>에 할당합니다.
        /// </summary>
        /// <param name="node"></param>
        protected abstract void LoadEntity(IRadTreeNodeContainer node);

        /// <summary>
        /// 로딩된 엔티티를 UI Control에 바인딩합니다.
        /// </summary>
        protected virtual void BindEntity()
        {
            if(IsDebugEnabled)
                log.Debug(@"==>S EntityGrid에 로딩된 Entity[{0}] 컬렉션을 Binding합니다...", ConcreteType.Name);

            AddEntity(EntityTreeView);
        }

        /// <summary>
        /// parent Container에 <see cref="Entities"/>값을 자식으로 추가한다.
        /// </summary>
        protected virtual void AddEntity(IRadTreeNodeContainer parent)
        {
            if(IsDebugEnabled)
                log.Debug(@"==>S EntityGrid에 로딩된 Entity[{0}] 컬렉션을 Binding합니다...", ConcreteType.Name);

            TreeViewAdapter.AddNodes(parent, Entities);
        }
    }
}