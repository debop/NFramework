using System;

namespace NSoft.NFramework.Reflections {
    /// <summary>
    /// 속성 매핑 시에 사용할 옵션들
    /// </summary>
    [Serializable]
    public class MapPropertyOptions : ValueObjectBase {
        /// <summary>
        /// 기본 옵션, 모든 옵션을 False로 한다.
        /// </summary>
        public static readonly MapPropertyOptions Default = new MapPropertyOptions
                                                            {
                                                                SuppressException = false,
                                                                IgnoreCase = false,
                                                                SkipAlreadyExistValue = false
                                                            };

        /// <summary>
        /// 안전하게 매핑을 수행하기 위해 예외 발생을 억제하고, 대소문자 구분을 하지 않습니다.
        /// </summary>
        public static readonly MapPropertyOptions Safety = new MapPropertyOptions
                                                           {
                                                               SuppressException = true,
                                                               IgnoreCase = true,
                                                               SkipAlreadyExistValue = false
                                                           };

        /// <summary>
        /// 예외발생 시 Exception을 발생시키는 것을 억제할 것인가?
        /// </summary>
        public bool SuppressException { get; set; }

        /// <summary>
        /// 속성의 대소문자 구분 여부
        /// </summary>
        public bool IgnoreCase { get; set; }

        /// <summary>
        /// 대상 객체에 이미 값이 존재할 경우에 Skip할 것인가?
        /// </summary>
        public bool SkipAlreadyExistValue { get; set; }

        public override int GetHashCode() {
            return HashTool.Compute(SuppressException, IgnoreCase, SkipAlreadyExistValue);
        }

        public override string ToString() {
            return string.Format("MapPropertyOptions# SuppressException=[{0}], IgnoreCase=[{1}], SkipAlreadyExistValue=[{2}]",
                                 SuppressException, IgnoreCase, SkipAlreadyExistValue);
        }
    }
}