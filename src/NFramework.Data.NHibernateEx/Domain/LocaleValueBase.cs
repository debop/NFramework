using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// 지역화정보를 나타내는 NHibernate Component를 표현하는 추상클래스입니다.
    /// </summary>
    /// <typeparam name="T">Locale 정보를 나타내는 Component의 형식</typeparam>
    [Serializable]
    public abstract class LocaleValueBase<T> : DataObjectBase where T : ILocaleValue {
        //
    }
}