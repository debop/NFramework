namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// <see cref="IUnitOfWork"/>를 중첩사용하기 위한 옵션
    /// </summary>
    public enum UnitOfWorkNestingOptions {
        /// <summary>
        /// 현재 활성화된 <see cref="IUnitOfWork"/>를 사용하던가, 새로 생성한다.
        /// </summary>
        ReturnExistingOrCreateUnitOfWork,

        /// <summary>
        /// 현재 활성화된 <see cref="IUnitOfWork"/>의 인스턴스가 아닌, 새로운 인스턴스를 생성한다.
        /// </summary>
        CreateNewOrNestUnitOfWork
    }
}