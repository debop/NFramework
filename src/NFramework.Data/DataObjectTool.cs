using System;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data {
    public static class DataObjectTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public static readonly string[] ExcludePropertiesForMap = new[] { "Id", "IsSaved", "IsTransient" };

        /// <summary>
        /// TTarget 형식의 인스턴스를 생성하고, 현재 인스턴스의 속성 정보로 설정하여 반환한다.
        /// </summary>
        /// <typeparam name="TTarget">복사될 인스턴스의 수형</typeparam>
        /// <param name="source">원본 객체</param>
        /// <param name="target">대상 객체</param>
        /// <param name="additionalMapping">추가 매핑 작업</param>
        /// <returns></returns>
        /// <seealso cref="ObjectMapper"/>
        /// <seealso cref="AdoTool"/>
        /// <seealso cref="ObjectMapper.MapProperty{TTarget}(object,System.Func{TTarget},System.Linq.Expressions.Expression{System.Func{TTarget,object}}[])"/>
        public static void MapDataObject<TTarget>(this IDataObject source, TTarget target,
                                                  Action<IDataObject, TTarget> additionalMapping = null) where TTarget : class {
            if(typeof(TTarget).HasInterface(typeof(IDataObject)))
                ObjectMapper.MapObject<IDataObject, TTarget>(source, target, additionalMapping, ExcludePropertiesForMap);

            ObjectMapper.MapObject<IDataObject, TTarget>(source, target, additionalMapping);
        }

        /// <summary>
        /// TTarget 형식의 인스턴스를 생성하고, 현재 인스턴스의 속성 정보로 설정하여 반환한다.
        /// </summary>
        /// <typeparam name="TTarget">복사될 인스턴스의 수형</typeparam>
        /// <param name="source">원본 객체</param>
        /// <param name="additionalMapping">추가 매핑 작업</param>
        /// <returns></returns>
        /// <seealso cref="ObjectMapper"/>
        /// <seealso cref="AdoTool"/>
        /// <seealso cref="ObjectMapper.MapProperty{TTarget}(object,System.Func{TTarget},System.Linq.Expressions.Expression{System.Func{TTarget,object}}[])"/>
        public static TTarget MapDataObject<TTarget>(this IDataObject source, Action<IDataObject, TTarget> additionalMapping = null)
            where TTarget : class {
            return MapDataObject<TTarget>(source, ActivatorTool.CreateInstance<TTarget>, additionalMapping);
        }

        /// <summary>
        /// TTarget 형식의 인스턴스를 생성하고, 현재 인스턴스의 속성 정보로 설정하여 반환한다.
        /// </summary>
        /// <typeparam name="TTarget">복사될 인스턴스의 수형</typeparam>
        /// <param name="source">원본 객체</param>
        /// <param name="resultFactory">복사될 인스턴스를 생성해주는 함수</param>
        /// <param name="additionalMapping">추가 매핑 작업</param>
        /// <returns></returns>
        /// <seealso cref="ObjectMapper"/>
        /// <seealso cref="AdoTool"/>
        /// <seealso cref="ObjectMapper.MapProperty{TTarget}(object,System.Func{TTarget},System.Linq.Expressions.Expression{System.Func{TTarget,object}}[])"/>
        public static TTarget MapDataObject<TTarget>(this IDataObject source, Func<TTarget> resultFactory,
                                                     Action<IDataObject, TTarget> additionalMapping = null) where TTarget : class {
            if(typeof(TTarget).HasInterface(typeof(IDataObject)))
                return ObjectMapper.MapObject<IDataObject, TTarget>(source, resultFactory, MapPropertyOptions.Safety, additionalMapping,
                                                                    ExcludePropertiesForMap);

            return ObjectMapper.MapObject<IDataObject, TTarget>(source, resultFactory, additionalMapping);
        }
    }
}