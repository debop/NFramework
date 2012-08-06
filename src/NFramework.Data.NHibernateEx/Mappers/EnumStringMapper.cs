namespace NSoft.NFramework.Data.NHibernateEx.Mappers {
    /// <summary>
    /// NHibernate를 이용하여 Enum 타입의 값을 문자열로 직접 DB에 저장하고, Load하기 위해서 사용하는 매퍼입니다.
    /// 참고 : http://orand.blogspot.com/2006/12/generic-nhibernate-enum-string-mapping.html
    /// </summary>
    /// <example>
    ///   // 만약 MyNamespace.MyEnum 값을 특정 hbm에 매핑시킬 때에는 다음과 같이 하면 됩니다.
    /// 
    ///	  // Enum 정의
    ///   public enum MyEnum {
    ///     Unknown,
    ///     Type1,
    ///		Type2
    ///	  }
    /// 
    ///	  // hbm 정의
    ///	 &lt;hibernate-mapping xmlns="urn:nhibernate-mapping-2.2"&gt;
    ///		&lt;class name="MyNamespace.MyClass, MyAssembly" table="TBL_MyClass"&gt;
    ///			&lt;id name="Id" column="MyId"&gt;
    ///				&lt;generator class="native"/&gt;
    ///			&lt;/id&gt;
    /// 
    ///			&lt;property name="MyEnumType" column="MyEnumTypeID"
    ///                   type="NSoft.NFramework.Data.NH.Mappers.EnumStringMapper`1[[MyNamespace.MyEnum, MyAssembly]], NSoft.NFramework.Data"/&gt;
    ///		&lt;/class&gt;
    /// &lt;/hibernate&gt;
    /// </example>
    /// <typeparam name="TEnum"></typeparam>
    public sealed class EnumStringMapper<TEnum> : NHibernate.Type.EnumStringType {
        /// <summary>
        /// default constructor
        /// </summary>
        public EnumStringMapper() : base(typeof(TEnum)) {}
    }
}