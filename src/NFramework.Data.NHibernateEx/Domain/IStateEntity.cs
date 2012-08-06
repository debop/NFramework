namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// 상태를 가지는 Entity를 표현하는 Interface
    /// </summary>
    public interface IStateEntity : IDataObject {
        /// <summary>
        /// 영구 저장소 (DB) 에 저장된 Entity인지 구분
        /// </summary>
        bool IsSaved { get; set; }

        /// <summary>
        /// Transient Entity인지 구분 (영구저장소에 저장된 Entity가 아니라, 메모리에서 생성한 임시 Entity라는 뜻)
        /// </summary>
        bool IsTransient { get; }

        /// <summary>
        /// 영구 저장소 (DB)에 저장할 때 호출되는 함수
        /// </summary>
        void OnSave();

        /// <summary>
        /// 영구 저장소 (DB)에서 정보를 읽어올 때 호출되는 함수
        /// </summary>
        void OnLoad();

        /// <summary>
        /// Persistent Object를 Transient Object로 만든다. (Persistent Object를 복제해서 새로운 Transient Object를 만들 때 사용한다.)
        /// </summary>
        void ToTransient();
    }
}