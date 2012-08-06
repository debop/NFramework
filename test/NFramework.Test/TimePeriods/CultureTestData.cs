using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 다양한 문화권에 대해 TimePeriod를 테스트 하기 위해 <see cref="CultureInfo"/> 를 제공합니다.
    /// </summary>
    [Serializable]
    public class CultureTestData : ValueObjectBase, IEnumerable<CultureInfo> {
        public static CultureTestData Default = new CultureTestData();

        public CultureTestData() {
            Cultures = new List<CultureInfo>
                       {
                           new CultureInfo("ar-DZ"),
                           new CultureInfo("ca-ES"),
                           new CultureInfo("zh-HK"),
                           new CultureInfo("cs-CZ"),
                           new CultureInfo("da-DK"),
                           new CultureInfo("de-DE"),
                           new CultureInfo("de-CH"),
                           new CultureInfo("el-GR"),
                           new CultureInfo("en-US"),
                           new CultureInfo("en-CA"),
                           new CultureInfo("en-AU"),
                           new CultureInfo("es-ES"),
                           new CultureInfo("fi-FI"),
                           new CultureInfo("ja-JP"),
                           new CultureInfo("ko-KR")
                       };
        }

        public IList<CultureInfo> Cultures { get; private set; }

        public CultureInfo this[int index] {
            get { return Cultures[index]; }
        }

        public IEnumerator<CultureInfo> GetEnumerator() {
            return Cultures.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}