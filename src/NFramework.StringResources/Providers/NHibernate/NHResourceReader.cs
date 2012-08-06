using System;
using System.Collections;
using System.Resources;

namespace NSoft.NFramework.StringResources.NHibernate {
    /// <summary>
    /// NHibernate를 이용한 <see cref="IResourceReader"/>를 구현한 클래스입니다.
    /// </summary>
    [Serializable]
    public sealed class NHResourceReader : ResourceReaderBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public NHResourceReader(Hashtable resources) : base(resources) {}
    }
}