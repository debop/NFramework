using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// 상태 정보 (Persistent인지, Transient인지)를 가진 Entity Class
    /// </summary>
    [Serializable]
    public class StateEntityBase : DataObjectBase, IStateEntity {
        /// <summary>
        /// 생성자
        /// </summary>
        protected StateEntityBase() {
            InitializeProperties();
        }

        /// <summary>
        /// Entity 속성을 초기화 합니다.
        /// </summary>
        protected virtual void InitializeProperties() {
            IsSaved = false;
        }

        /// <summary>
        /// 영구 저장소 (DB) 에 저장된 Entity인지 구분
        /// </summary>
        public virtual bool IsSaved { get; set; }

        /// <summary>
        /// Transient Entity인지 구분 (영구저장소에 저장된 Entity가 아니라, 메모리에서 생성한 임시 Entity라는 뜻)
        /// </summary>
        public virtual bool IsTransient {
            get { return !IsSaved; }
        }

        /// <summary>
        /// 영구 저장소 (DB)에 저장할 때 호출되는 함수. IsSaved를 True로 설정한다.
        /// </summary>
        public virtual void OnSave() {
            IsSaved = true;
        }

        /// <summary>
        /// 영구 저장소 (DB)에서 정보를 읽어올 때 호출되는 함수. IsSaved를 true로 설정한다.
        /// </summary>
        public virtual void OnLoad() {
            IsSaved = true;
        }

        /// <summary>
        /// Persistent Object를 Transient Object로 만든다. (Persistent Object를 복제해서 새로운 Transient Object를 만들 때 사용한다.)
        /// </summary>
        public virtual void ToTransient() {
            IsSaved = false;
        }
    }
}