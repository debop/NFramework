using System;
using System.Collections;
using NHibernate.Transform;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// NHibernate Query 결과 셋이 object[] 형태로 나온 것을 지정된 형식의 생성자를 호출하여 인스턴스로 빌드한다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TypedResultTransformer<T> : IResultTransformer {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private static readonly Type _entityType = typeof(T);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public virtual IList TransformList(IList collection) {
            return collection;
        }

        /// <summary>
        /// 결과 정보를 가지고, 지정된 형식의 인스턴스를 생성해서 반환한다.
        /// </summary>
        /// <param name="tuple">쿼리 실행 결과 배열</param>
        /// <param name="aliases"></param>
        /// <returns>쿼리 실행결과를 인자로 한 인스턴스</returns>
        public object TransformTuple(object[] tuple, string[] aliases) {
            if(tuple.Length == 1)
                return tuple[0];

            // T 형식의 생성자가 tuple에 해당하는 모든 인자를 받아들이는 생성자가 있어야 한다.
            //
            return ActivatorTool.CreateInstance(_entityType, tuple);
            // return Activator.CreateInstance(typeof(T), tuple);
        }
    }
}