namespace NSoft.NFramework {
    /// <summary>
    /// 내부 String Resources
    /// </summary>
    internal static class SR {
        internal const string LibraryName = "NSoft.NFramework";

        internal const string ShouldNotBeNull = "[{0}] should not be null.";
        internal const string ShouldNotBeEmpty = "[{0}] should not be null or empty string.";
        internal const string ShouldNotBeWhiteSpace = "[{0}] should not be WhiteSpace string.";

        internal const string ShouldBe = "해당 조건을 만족하지 않습니다. 조건=[{0}]";
        internal const string ShouldNotBe = "해당 조건을 만족하지 않아야 합니다. 조건=[{0}]";

        /// <summary>
        /// 대칭형 암호화 알고리즘의 기본 비밀번호입니다.
        /// </summary>
        public const string DefaultSymmetricKey = "NFamework@BobSoft.com";

        /// <summary>
        /// 같은 종류의 타입이 아닙니다.
        /// </summary>
        public const string ErrNotSameType = "같은 종류의 형식(Type)이 아닙니다!!!";

        /// <summary>
        /// 지정한 값이 주어진 영역을 벗어났습니다.
        /// </summary>
        public const string ErrValueIsOutOfRange = "지정된 값[{0}]이 주어진 영역([{1}]~[{2}]) 를 벗어났습니다.";

        /// <summary>
        /// 지정한 파일을 읽을 수 없습니다.
        /// </summary>
        public const string ErrFileRead = "지정된 파일을 읽을 수 없습니다!!!";

        public const string ErrFileWrite = "지정된 파일을 생성할 수 없습니다.";
        public const string ErrInvalidFilename = "지정된 파일명이 유효하지 않습니다.";
        public const string ErrInvalidProvider = "적절치 않은 암호화 프로바이더 입니다.";
        public const string ErrInvalidTextLength = "암호화된 문장의 길이가 짝수가 아닙니다.";
        public const string ErrNoAlgorithm = "암호화 프로바이더 종류를 지정하지 않았습니다.";
        public const string ErrNoContent = "암호/복호화할 내용이 없습니다.";
        public const string ErrNoFile = "지정된 파일을 찾을 수 없습니다.";
        public const string ErrNoKeyForSymmetric = "대칭형 암호화를 위한 키 값이 제공되지 않았습니다.";

        public const string ErrorArgumentIsNull = "[{0}] 이(가) null 입니다!!!";
        public const string ErrorArrayIsEmpty = "배열 [{0}] 이(가) 비어있습니다!!!";
        public const string ErrorCollectionIsEmpty = "컬렉션 [{0}] 이(가) null이거나 빈 컬렉션입니다!!!";
        public const string ErrorInvalidHexDecimalString = "[{0}] 는 16진수 표현 문자열이 아닙니다.";
        public const string ErrorInvalidOffetLength = "지정된 배열의 offset 의 크기가 배열의 실제크기보다 큽니다.	";
        public const string ErrorNotExpectedType = "[{0}] 타입은 예상했던 [{1}] 타입이 아닙니다!!!";
        public const string ErrorNotSameLengthArray = "두 배열의 크기가 같아야 합니다.";
        public const string ErrorShouldBeNegative = "[{0}]의 값은 0보다 작은 음수여야 합니다. [{0}]=[{1}]";
        public const string ErrorShouldBePositive = "[{0}]의 값은 0보다 큰 양수여야 합니다. [{0}]=[{1}]";

        public const string ErrorShouldNotBeNull = "[{0}]는 null이면 안됩니다!!!";
        public const string ErrorShouldNotBeDefault = "[{0}]는 default(T)이면 안됩니다!!! type=[{1}]";
        public const string ErrorShouldNotBeZero = "[{0}] 값은 0이 아니어야 합니다!!! [{0}]=[{1}]";
        public const string ErrorShouldNotEmptyString = "[{0}]는 빈 문자열(empty string)이면 안됩니다.";
        public const string ErrorShouldNotWhiteSpaceString = "[{0}]는 빈 문자열(empty string)이거나 공백만 있는 문자열이면 안됩니다.";
        public const string ErrorShouldBePositiveOrZero = "[{0}]의 값은 0보다 크거나 같아야 합니다. [{0}]=[{1}]";
        public const string ErrorShouldBeNegativeOrZero = "[{0}]의 값은 0보다 작거나 같아야 합니다. [{0}]=[{1}]";

        public const string ErrorTypeMismatchDestTypeAndDefType = "대상 타입과 기본값 타입의 형식이 일치하지 않습니다.";
        public const string ErrorValueIsNotDefinedInEnum = "[{0}] 값은 Enum [{1}]에 정의되어 있지 않습니다.";

        /// <summary>
        /// 지정한 인자는 숫자 형식이 아닙니다.
        /// </summary>
        public const string IsNotNumeric = "[{0}] 은 숫자 형식이 아닙니다. [{0}]=[{1}]";

        public const string PropertyCopyFailed = "속성 값 복사에 실패했습니다.";

        public const string ErrorShouldBeEquals = "변수 [{0}] 값 [{1}] == [{2}] 이여야 합니다.";
        public const string ErrorShouldNotBeEquals = "변수 [{0}] 값 [{1}] != [{2}] 이여야 합니다.";
        public const string ErrorShouldBeGreater = "변수 [{0}]값 [{1}] > [{2}] 이여야 합니다.";
        public const string ErrorShouldBeGreaterOrEqual = "변수 [{0}]값 [{1}] >= [{2}] 이여야 합니다.";
    }
}