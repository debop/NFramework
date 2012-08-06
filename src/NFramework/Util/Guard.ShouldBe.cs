using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework {
    public static partial class Guard {
        /// <summary>
        /// <paramref name="value"/> 이 <paramref name="targetValue"/>과 같아야 합니다. 두 값이 다르면 예외가 발생합니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="targetValue">비교 대상 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException">value와 targetValue가 값이 다르면 발생합니다.</exception>
        public static void ShouldBeEquals(this object value, object targetValue, string valueName) {
            if(Equals(value, targetValue))
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldBeEquals, valueName, value, targetValue));
        }

        /// <summary>
        /// <paramref name="value"/> 이 <paramref name="targetValue"/>과 같지 않아야 합니다. 만약 같다면 예외를 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="targetValue">비교 대상 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException">value와 targetValue가 같으면 발생합니다.</exception>
        public static void ShouldNotBeEquals(this object value, object targetValue, string valueName) {
            if(Equals(value, targetValue) == false)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldNotBeEquals, valueName, value, targetValue));
        }

        /// <summary>
        /// 지정된 <paramref name="value"/>가 Null이면 <see cref="InvalidOperationException"/>을 발생시킵니다. 값이 null 이면 예외가 발생합니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="ArgumentNullException">value가 null인 경우</exception>
        public static void ShouldNotBeNull(this object value, string valueName) {
            if(ReferenceEquals(value, null) == false)
                return;

            throw new ArgumentNullException(string.Format(SR.ErrorShouldNotBeNull, valueName));
        }

        /// <summary>
        /// <paramref name="value"/>가 default(T)와 같다면, <see cref="InvalidOperationException"/>을 발생시킵니다. default(T)와 같으면 예외를 발생시킵니다.
        /// </summary>
        /// <typeparam name="T">값의 수형</typeparam>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="ArgumentNullException">value가 default(T)인 경우</exception>
        public static void ShouldNotBeDefault<T>(this T value, string valueName) {
            if(Equals(value, default(T)) == false)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldNotBeDefault, valueName, typeof(T).FullName));
        }

        /// <summary>
        /// 지정한 문자열이 null, 빈 문자열, 공백만 있는 문자열이라면 예외를 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">검사할 값의 명</param>
        /// <exception cref="InvalidOperationException"> <paramref name="value"/>가 NULL, 빈문자열일 경우</exception>
        public static void ShouldNotBeEmpty(this string value, string valueName) {
            ShouldNotBeEmpty(value, valueName, false);
        }

        /// <summary>
        /// 지정한 문자열이 null 이거나, 빈 문자열이라면 예외를 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <param name="doTrim">Trim 후 검사</param>
        /// <exception cref="InvalidOperationException"> <paramref name="value"/>가 NULL, 빈문자열, 공백만 있는 문자열일때 발생한다.</exception>
        public static void ShouldNotBeEmpty(this string value, string valueName, bool doTrim) {
            if(value.IsEmpty(doTrim))
                throw new InvalidOperationException(string.Format(SR.ErrorShouldNotEmptyString, valueName));
        }

        /// <summary>
        /// <paramref name="value"/>가 NULL, 빈문자열, 공백만 있는 문자열이라면 예외를 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"> <paramref name="value"/>가 NULL, 빈문자열, 공백만 있는 문자열일때 발생한다.</exception>
        public static void ShouldNotBeWhiteSpace(this string value, string valueName) {
            if(value.IsNotWhiteSpace())
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldNotWhiteSpaceString, valueName));
        }

        /// <summary>
        /// 지정한 시퀀스가 null 이거나 빈 컬렉션이면 예외를 발생시킵니다.
        /// </summary>
        /// <param name="sequence">검사할 시퀀스</param>
        /// <param name="sequenceName">시퀀스 명</param>
        /// <exception cref="InvalidOperationException">지정한 시퀀스가 null이거나 빈 컬렉션인 경우</exception>
        public static void ShouldNotBeEmpty(this IEnumerable sequence, string sequenceName) {
            if(sequence != null && sequence.GetEnumerator().MoveNext())
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorCollectionIsEmpty, sequenceName));
        }

        /// <summary>
        /// 지정한 시퀀스가 null 이거나 빈 컬렉션이면 예외를 발생시킵니다.
        /// </summary>
        /// <param name="sequence">검사할 시퀀스</param>
        /// <param name="sequenceName">시퀀스 명</param>
        /// <exception cref="InvalidOperationException">지정한 시퀀스가 null이거나 빈 컬렉션인 경우</exception>
        public static void ShouldNotBeEmpty<T>(this IEnumerable<T> sequence, string sequenceName) {
            if(sequence != null && sequence.Any())
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorCollectionIsEmpty, sequenceName));
        }

        /// <summary>
        /// 지정한 시퀀스에 요소가 있어야 합니다. NULL이거나 요소가 없다면 예외를 발생시킵니다.
        /// </summary>
        /// <param name="sequence">검사할 시퀀스</param>
        /// <param name="sequenceName">시퀀스 명</param>
        /// <exception cref="InvalidOperationException">지정한 시퀀스가 null이거나 빈 컬렉션인 경우</exception>
        public static void ShouldBeExists<T>(this IEnumerable<T> sequence, string sequenceName) {
            if(sequence != null && sequence.Any())
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorCollectionIsEmpty, sequenceName));
        }

        /// <summary>
        /// 지정한 객체가 Numeric 수형이여야 합니다. Numeric 수형이 아니면 예외를 발생시킨다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException">값이 Numeric 수형이 아닌 경우</exception>
        public static void ShouldBeNumeric(this object value, string valueName) {
            if(value != null && TypeTool.IsNumeric(value))
                return;

            throw new InvalidOperationException(string.Format(SR.IsNotNumeric, valueName, value));
        }

        /// <summary>
        /// 지정한 수형이 Numeric 수형이어야 합니다.
        /// </summary>
        /// <param name="type">검사할 수형</param>
        /// <exception cref="InvalidOperationException">값이 Numeric 수형이 아닌 경우</exception>
        public static void ShouldBeNumericType(this Type type) {
            type.ShouldNotBeNull("type");

            if(TypeTool.IsNumericType(type))
                return;

            throw new InvalidOperationException(string.Format(SR.IsNotNumeric, type.Name, type));
        }

        /// <summary>
        /// <paramref name="instance"/> 가 <typeparamref name="TTarget"/> 수형의 인스턴스가 아니라면 <see cref="InvalidOperationException"/> 예외를 발생시킵니다.
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="instance">검사할 객체</param>
        /// <param name="instanceName">객체명</param>
        /// <exception cref="InvalidOperationException"><paramref name="instance"/> 가 <typeparamref name="TTarget"/> 수형의 인스턴스가 아닐 경우</exception>
        public static void ShouldBeInstanceOf<TTarget>(this object instance, string instanceName) {
            instance.ShouldNotBeNull(instanceName);

            if(instance.GetType() == typeof(TTarget))
                return;

            var errorMsg = string.Format("[{0}][{1}]는 [{2}] 수형이 아닙니다.", instanceName, instance.GetType().FullName,
                                         typeof(TTarget).FullName);
            throw new InvalidOperationException(errorMsg);
        }

        /// <summary>
        /// <paramref name="value"/>이 0이면 예외를 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 0 인 경우</exception>
        public static void ShouldNotBeZero(this int value, string valueName) {
            if(value != 0)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldNotBeZero, valueName, value));
        }

        /// <summary>
        /// 지정한 값이 0이면 예외를 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 0 인 경우</exception>
        public static void ShouldNotBeZero(this long value, string valueName) {
            if(value != 0L)

                throw new InvalidOperationException(string.Format(SR.ErrorShouldNotBeZero, valueName, value));
        }

        /// <summary>
        /// 지정한 값이 0이면 예외를 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 0 인 경우</exception>
        public static void ShouldNotBeZero(this float value, string valueName) {
            value.ShouldNotBeNaN("value");
            if(Math.Abs(value - 0.0f) > float.Epsilon)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldNotBeZero, valueName, value));
        }

        /// <summary>
        /// 지정한 값이 0이면 예외를 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 0 인 경우</exception>
        public static void ShouldNotBeZero(this double value, string valueName) {
            value.ShouldNotBeNaN("value");
            if(Math.Abs(value - 0.0d) > double.Epsilon)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldNotBeZero, valueName, value));
        }

        /// <summary>
        /// 지정한 값이 0이면 예외를 발생시킵니다.
        /// </summary>
        /// <exception cref="InvalidOperationException">값이 0일때</exception>
        public static void ShouldNotBeZero(this short value, string valueName) {
            if(value != 0)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldNotBeZero, valueName, value));
        }

        /// <summary>
        /// 지정한 값이 0이면 예외를 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 0 인 경우</exception>
        public static void ShouldNotBeZero(this decimal value, string valueName) {
            if(value != 0m)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldNotBeZero, valueName, value));
        }

        /// <summary>
        /// <paramref name="value"/>이 양수 (0 초과의 값) 이어야 합니다. 그렇지 않으면 <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 0 보다 큰 값이 아닐 경우</exception>
        public static void ShouldBePositive(this int value, string valueName) {
            if(value > 0)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldBePositive, valueName, value));
        }

        /// <summary>
        /// <paramref name="value"/>이 양수 (0 초과의 값) 이어야 합니다. 그렇지 않으면 <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 0 보다 큰 값이 아닐 경우</exception>
        public static void ShouldBePositive(this long value, string valueName) {
            if(value > 0L)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldBePositive, valueName, value));
        }

        /// <summary>
        /// <paramref name="value"/>이 양수 (0 초과의 값) 이어야 합니다. 그렇지 않으면 <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 0 보다 큰 값이 아닐 경우</exception>
        public static void ShouldBePositive(this float value, string valueName) {
            value.ShouldNotBeNaN("value");
            if(value > 0.0f)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldBePositive, valueName, value));
        }

        /// <summary>
        /// <paramref name="value"/>이 양수 (0 초과) 이어야 합니다. 그렇지 않으면 <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 0 보다 큰 값이 아닐 경우</exception>
        public static void ShouldBePositive(this double value, string valueName) {
            value.ShouldNotBeNaN("value");
            if(value > 0.0d)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldBePositive, valueName, value));
        }

        /// <summary>
        /// <paramref name="value"/>이 양수 (0 초과) 이어야 합니다. 그렇지 않으면 <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 0 보다 큰 값이 아닐 경우</exception>
        public static void ShouldBePositive(this short value, string valueName) {
            if(value > 0)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldBePositive, valueName, value));
        }

        /// <summary>
        /// <paramref name="value"/>이 양수 (0 초과) 이어야 합니다. 그렇지 않으면 <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 0 보다 큰 값이 아닐 경우</exception>
        public static void ShouldBePositive(this decimal value, string valueName) {
            if(value > 0m)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldBePositive, valueName, value));
        }

        /// <summary>
        /// <paramref name="value"/>이 0이거나 양수 이어야 합니다. 그렇지 않으면 <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 0 이거나 양수가 아닐 경우</exception>
        public static void ShouldBePositiveOrZero(this int value, string valueName) {
            if(value >= 0)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldBePositiveOrZero, valueName, value));
        }

        /// <summary>
        /// <paramref name="value"/>이 0이거나 양수 이어야 합니다. 그렇지 않으면 <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 0 이거나 양수가 아닐 경우</exception>
        public static void ShouldBePositiveOrZero(this long value, string valueName) {
            if(value >= 0L)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldBePositiveOrZero, valueName, value));
        }

        /// <summary>
        /// <paramref name="value"/>이 0이거나 양수 이어야 합니다. 그렇지 않으면 <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 0 이거나 양수가 아닐 경우</exception>
        public static void ShouldBePositiveOrZero(this float value, string valueName) {
            value.ShouldNotBeNaN("value");
            if(value >= 0.0F)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldBePositiveOrZero, valueName, value));
        }

        /// <summary>
        /// <paramref name="value"/>이 0이거나 양수 이어야 합니다. 그렇지 않으면 <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 0 이거나 양수가 아닐 경우</exception>
        public static void ShouldBePositiveOrZero(this double value, string valueName) {
            value.ShouldNotBeNaN("value");
            if(value >= 0.0d)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldBePositiveOrZero, valueName, value));
        }

        /// <summary>
        /// <paramref name="value"/>이 0이거나 양수 이어야 합니다. 그렇지 않으면 <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 0 이거나 양수가 아닐 경우</exception>
        public static void ShouldBePositiveOrZero(this short value, string valueName) {
            if(value >= 0)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldBePositiveOrZero, valueName, value));
        }

        /// <summary>
        /// <paramref name="value"/>이 0이거나 양수 이어야 합니다. 그렇지 않으면 <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 0 이거나 양수가 아닐 경우</exception>
        public static void ShouldBePositiveOrZero(this decimal value, string valueName) {
            if(value >= 0m)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldBePositiveOrZero, valueName, value));
        }

        /// <summary>
        /// <paramref name="value"/>이 음수 (0 미만) 이어야 합니다. 그렇지 않으면 <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 0 미만의 음수가 아닐 경우</exception>
        public static void ShouldBeNegative(this int value, string valueName) {
            if(value < 0)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldBeNegative, valueName, value));
        }

        /// <summary>
        /// <paramref name="value"/>이 음수 (0 미만) 이어야 합니다. 그렇지 않으면 <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 0 미만의 음수가 아닐 경우</exception>
        public static void ShouldBeNegative(this long value, string valueName) {
            if(value < 0L)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldBeNegative, valueName, value));
        }

        /// <summary>
        /// <paramref name="value"/>이 음수 (0 미만) 이어야 합니다. 그렇지 않으면 <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 0 미만의 음수가 아닐 경우</exception>
        public static void ShouldBeNegative(this float value, string valueName) {
            value.ShouldNotBeNaN("value");

            if(value < 0.0F)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldBeNegative, valueName, value));
        }

        /// <summary>
        /// <paramref name="value"/>이 음수 (0 미만) 이어야 합니다. 그렇지 않으면 <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 0 미만의 음수가 아닐 경우</exception>
        public static void ShouldBeNegative(this double value, string valueName) {
            value.ShouldNotBeNaN("value");

            if(value < 0.0d)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldBeNegative, valueName, value));
        }

        /// <summary>
        /// <paramref name="value"/>이 음수 (0 미만) 이어야 합니다. 그렇지 않으면 <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 0 미만의 음수가 아닐 경우</exception>
        public static void ShouldBeNegative(this short value, string valueName) {
            if(value < 0)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldBeNegative, valueName, value));
        }

        /// <summary>
        /// <paramref name="value"/>이 음수 (0 미만) 이어야 합니다. 그렇지 않으면 <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 0 미만의 음수가 아닐 경우</exception>
        public static void ShouldBeNegative(this decimal value, string valueName) {
            if(value < 0m)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldBeNegative, valueName, value));
        }

        /// <summary>
        /// <paramref name="value"/>이 0이거나 음수이어야 합니다. 그렇지 않으면 <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 0이거나 음수가 아닐 경우</exception>
        public static void ShouldBeNegativeOrZero(this int value, string valueName) {
            if(value <= 0)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldBeNegativeOrZero, valueName, value));
        }

        /// <summary>
        /// <paramref name="value"/>이 0이거나 음수이어야 합니다. 그렇지 않으면 <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 0이거나 음수가 아닐 경우</exception>
        public static void ShouldBeNegativeOrZero(this long value, string valueName) {
            if(value <= 0L)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldBeNegativeOrZero, valueName, value));
        }

        /// <summary>
        /// <paramref name="value"/>이 0이거나 음수이어야 합니다. 그렇지 않으면 <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 0이거나 음수가 아닐 경우</exception>
        public static void ShouldBeNegativeOrZero(this float value, string valueName) {
            value.ShouldNotBeNaN("value");

            if(value <= 0.0f)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldBeNegativeOrZero, valueName, value));
        }

        /// <summary>
        /// <paramref name="value"/>이 0이거나 음수이어야 합니다. 그렇지 않으면 <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 0이거나 음수가 아닐 경우</exception>
        public static void ShouldBeNegativeOrZero(this double value, string valueName) {
            value.ShouldNotBeNaN("value");

            if(value <= 0.0d)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldBeNegativeOrZero, valueName, value));
        }

        /// <summary>
        /// <paramref name="value"/>이 0이거나 음수이어야 합니다. 그렇지 않으면 <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 0이거나 음수가 아닐 경우</exception>
        public static void ShouldBeNegativeOrZero(this short value, string valueName) {
            if(value <= 0)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldBeNegativeOrZero, valueName, value));
        }

        /// <summary>
        /// <paramref name="value"/>이 0이거나 음수이어야 합니다. 그렇지 않으면 <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 0이거나 음수가 아닐 경우</exception>
        public static void ShouldBeNegativeOrZero(this decimal value, string valueName) {
            if(value <= 0m)
                return;

            throw new InvalidOperationException(string.Format(SR.ErrorShouldBeNegativeOrZero, valueName, value));
        }

        /// <summary>
        /// <paramref name="value"/>이 NaN 이면 <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 NaN인 경우</exception>
        public static void ShouldNotBeNaN(this double value, string valueName) {
            if(double.IsNaN(value) == false)
                return;

            throw new InvalidOperationException("값이 double.NaN 이면 안됩니다.");
        }

        /// <summary>
        /// <paramref name="value"/>이 NaN 이면 <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 NaN인 경우</exception>
        public static void ShouldNotBeNaN(this float value, string valueName) {
            if(float.IsNaN(value) == false)
                return;

            throw new InvalidOperationException("값이 float.NaN 이면 안됩니다.");
        }

        /// <summary>
        /// <paramref name="value"/>이 <paramref name="target"/> 보다 커야 합니다. 같거나 작으면 <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="target">대상 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 <paramref name="target"/>보다 작거나 같은 경우</exception>
        public static void ShouldBeGreaterThan<T>(this T value, T target, string valueName) where T : IComparable {
            if(value.CompareTo(target) > 0)
                return;

            throw new InvalidOperationException(string.Format("[{0}[{1}] > target[{2}] 이어야 합니다.", valueName, value, target));
        }

        /// <summary>
        /// <paramref name="value"/>이 <paramref name="target"/> 보다 크거나 같아야 합니다. 작으면 <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="target">대상 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 <paramref name="target"/>보다 작은 경우</exception>
        public static void ShouldBeGreaterOrEqual<T>(this T value, T target, string valueName) where T : IComparable {
            if(value.CompareTo(target) >= 0)
                return;

            throw new InvalidOperationException(string.Format("[{0}[{1}] >= target[{2}] 이어야 합니다.", valueName, value, target));
        }

        /// <summary>
        /// <paramref name="value"/>이 <paramref name="target"/> 보다 작아야 합니다. 크거나 같으면 <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="target">대상 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 <paramref name="target"/>보다 크거나 같은 경우</exception>
        public static void ShouldBeLessThan<T>(this T value, T target, string valueName) where T : IComparable {
            if(value.CompareTo(target) < 0)
                return;

            throw new InvalidOperationException(string.Format("[{0}[{1}] < target[{2}] 이어야 합니다.", valueName, value, target));
        }

        /// <summary>
        /// <paramref name="value"/>이 <paramref name="target"/> 보다 작거나 같아야 합니다. 크면 <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="target">대상 값</param>
        /// <param name="valueName">명칭</param>
        /// <exception cref="InvalidOperationException"><paramref name="value"/>가 <paramref name="target"/>보다 큰 경우</exception>
        public static void ShouldBeLessOrEqual<T>(this T value, T target, string valueName) where T : IComparable {
            if(value.CompareTo(target) <= 0)
                return;

            throw new InvalidOperationException(string.Format("[{0}[{1}] <= target[{2}] 이어야 합니다.", valueName, value, target));
        }

        /// <summary>
        /// <paramref name="value"/>가 <paramref name="fromInclude"/> 이상이고, <paramref name="toExclude"/> 미만이어야 합니다. 아니면 예외를 발생시킵니다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <param name="fromInclude">하한 값 (범위에 포함됨)</param>
        /// <param name="toExclude">상한 값 (범위에 포함 안됨)</param>
        /// <param name="valueName">검사할 값의 명칭</param>
        /// <exception cref="InvalidOperationException">값이 범위를 벗어났을 때</exception>
        public static void ShouldBeInRange<T>(this T value, T fromInclude, T toExclude, string valueName) where T : IComparable<T> {
            if(value.CompareTo(fromInclude) >= 0 && value.CompareTo(toExclude) < 0)
                return;

            var errorMsg = string.Format("{0}[{1}] 이 범위 [{2},{3})을 벗어났습니다.", valueName, value, fromInclude, toExclude);
            throw new InvalidOperationException(errorMsg);
        }

        /// <summary>
        /// 두 값 사이 (경계 포함)에 있어야 한다. 그렇지 않으면, <see cref="InvalidOperationException"/>을 발생시킨다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">검사할 값</param>
        /// <param name="lowerInclusive">하한 값 (범위에 포함)</param>
        /// <param name="upperInclusive">상한 값 (범위에 포함)</param>
        /// <param name="valueName">검사할 값의 명칭</param>
        /// <exception cref="InvalidOperationException">값이 범위 내에 있지 않을 경우</exception>
        public static void ShouldBeBetween<T>(this T value, T lowerInclusive, T upperInclusive, string valueName)
            where T : IComparable<T> {
            if(value.CompareTo(lowerInclusive) >= 0 && value.CompareTo(upperInclusive) <= 0)
                return;

            var errorMsg = string.Format("{0}[{1}]이 범위[{2},{3}]을 벗어났습니다.", valueName, value, lowerInclusive, upperInclusive);
            throw new InvalidOperationException(errorMsg);
        }

        /// <summary>
        /// <paramref name="actual"/> 값이 <paramref name="expected"/>값이 같아야 합니다. 다르면 예외를 발생시킵니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual">실제 값</param>
        /// <param name="expected">예상 값</param>
        public static void ShouldBeEqualTo<T>(this T actual, T expected) where T : IEquatable<T> {
            if(!actual.Equals(expected))
                throw new InvalidOperationException(string.Format("두 값이 같아야 합니다. actual=[{0}] == expected=[{1}]", actual, expected));
        }

        /// <summary>
        /// <paramref name="actual"/> 값이 <paramref name="expected"/>값이 달라야 합니다. 같으면 예외를 발생시킵니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        public static void ShouldNotBeEqualTo<T>(this T actual, T expected) where T : IEquatable<T> {
            if(actual.Equals(expected))
                throw new InvalidOperationException(string.Format("두 값이 달라야 합니다. actual=[{0}] != expected=[{1}]", actual, expected));
        }

        /// <summary>
        /// <paramref name="actual"/> 값이 <paramref name="expected"/>값보다 커야 합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        public static void ShouldBeGreatThan<T>(this T actual, T expected) where T : IComparable<T> {
            if(actual.CompareTo(expected) <= 0)
                throw new InvalidOperationException(string.Format("actual 값이 더 커야 합니다. actual=[{0}] > expected=[{1}]", actual, expected));
        }

        /// <summary>
        /// <paramref name="actual"/> 값이 <paramref name="expected"/>값보다 커서는 안됩니다. (이하여야 합니다)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        public static void ShouldNotBeGreatThan<T>(this T actual, T expected) where T : IComparable<T> {
            if(actual.CompareTo(expected) > 0)
                throw new InvalidOperationException(string.Format("두 값이 달라야 합니다. actual=[{0}] <= expected=[{1}]", actual, expected));
        }

        /// <summary>
        /// <paramref name="conditionExpr"/>을 수행해서 True가 나와야 합니다. False가 반환되면 예외를 발생시킵니다.
        /// </summary>
        /// <param name="conditionExpr"></param>
        /// <param name="message"></param>
        public static void ShouldBe(this Expression<Func<bool>> conditionExpr, string message = SR.ShouldBe) {
            conditionExpr.ShouldNotBeNull("condition expression");

            if(conditionExpr.Compile().Invoke() == false)
                throw new InvalidOperationException(message ?? string.Format(SR.ShouldBe, conditionExpr.Body));
        }

        /// <summary>
        /// <paramref name="conditionExpr"/>을 수행해서 False가 나와야 합니다. True가 반환되면 예외를 발생시킵니다.
        /// </summary>
        /// <param name="conditionExpr"></param>
        /// <param name="message"></param>
        public static void ShouldNotBe(this Expression<Func<bool>> conditionExpr, string message = SR.ShouldNotBe) {
            conditionExpr.ShouldNotBeNull("condition expression");

            if(conditionExpr.Compile().Invoke())
                throw new InvalidOperationException(message ?? string.Format(SR.ShouldNotBe, conditionExpr.Body));
        }
    }
}