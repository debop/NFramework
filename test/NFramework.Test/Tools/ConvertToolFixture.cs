using System;
using System.Globalization;
using NUnit.Framework;

namespace NSoft.NFramework.Tools {
    [Microsoft.Silverlight.Testing.Tag("Core.Utils")]
    [TestFixture]
    public class ConvertToolFixture : AbstractFixture {
        #region << DefValue >>

        [Test]
        public void DefValueTest() {
            object value = 1;
            int i = ConvertTool.DefValue(value, 0);

            Assert.AreEqual(1, i);

            value = 12.5m;
            decimal convertedDecimal = ConvertTool.DefValue(value, 0m);
            Assert.AreEqual(12.5m, convertedDecimal);

            value = 9L;
            decimal convertedLong = ConvertTool.DefValue(value, 0L);
            Assert.AreEqual(9L, convertedLong);
        }

        [Test]
        public void DefValue_EmptyString_ToInt32() {
            object value = string.Empty;
            int i = ConvertTool.DefValue(value, 5);
            Assert.AreEqual(5, i);
        }

        [Test]
        public void DBNullConvert() {
            object value = DBNull.Value;
            string converted = ConvertTool.DefValue(value, "IS NULL");
            Assert.AreEqual(converted, "IS NULL");
        }

        [Test]
        public void ConvertValueTest() {
            object s = "123";
            var i = (int)ConvertTool.ConvertValue(s, typeof(int));

            Console.WriteLine(i);
        }

        [Test]
        public void ToStringTest() {
            const int Value = 123;
            string str = ConvertTool.ToString(Value, "X2", null);

            Console.WriteLine(str);
        }

        [Test]
        public void ConvertEnumTest() {
            object value = "Third";

            var type = ConvertTool.ConvertEnum<Quarters>(value, Quarters.First);

            Assert.AreEqual(Quarters.Third, type);
        }

        [Test]
        public void ConvertDateTimeTest() {
            var dt = DateTime.Now - TimeSpan.FromSeconds(5);
            string dateStr = dt.ToString();

            var converted = ConvertTool.DefValue(dateStr, DateTime.Now, true);

            Console.WriteLine("[ dt:" + dt + @" ]");
            Console.WriteLine("[ converted: " + converted + @" ]");

            Assert.AreEqual(dt.ToString(), converted.ToString());
            Assert.AreEqual(dt.Date.Ticks, converted.Date.Ticks);
        }

        /// <summary>
        /// NOTE: Nullable 수형에 대해 ConvertTool.DefValue가 제대로 지원하는 지 검사 (기존버전에서 버그가 있었음
        /// </summary>
        [Test]
        public void CanConvertNullableDateTime() {
            DateTime? value = DateTime.Today;

            var converted = ConvertTool.DefValue<DateTime?>(value, (DateTime?)null);
            Assert.AreEqual(value, converted, "#1");

            converted = ConvertTool.DefValue<DateTime?>(null, (DateTime?)null);
            Assert.AreEqual(null, converted, "#2");
        }

        #endregion

        #region << AsValue, AsValueNullable, AsInt, AsIntNullable ... >>

        [Test]
        public void AsValueWithNull() {
            object source = null;

            Assert.AreEqual(source.AsValue<string>(), source.AsValue(typeof(string)));
            Assert.AreEqual(source.AsValue<int>(), source.AsValue(typeof(int), () => default(int)));
        }

        [Test]
        public void AsValueWithDbNull() {
            var source = DBNull.Value;

            Assert.AreEqual(DBNull.Value, source.AsValue(typeof(DBNull)));
            Assert.AreEqual(DBNull.Value, source.ToString().AsValue(typeof(DBNull)));
            Assert.AreEqual(null, source.AsValue(typeof(object)));
        }

        [Test]
        public void AsValueWithString() {
            var source = "32";

            Assert.AreEqual(source.AsValue<string>(), source.AsValue(typeof(string)));

            Assert.AreEqual(source.AsValue<byte>(), source.AsValue(typeof(byte)));
            Assert.AreEqual(source.AsValue<short>(), source.AsValue(typeof(short)));
            Assert.AreEqual(source.AsValue<int>(), source.AsValue(typeof(int)));
            Assert.AreEqual(source.AsValue<long>(), source.AsValue(typeof(long)));

            Assert.AreEqual(source.AsValue<decimal>(), source.AsValue(typeof(decimal)));
            Assert.AreEqual(source.AsValue<float>(), source.AsValue(typeof(float)));
            Assert.AreEqual(source.AsValue<double>(), source.AsValue(typeof(double)));

            DateTime sourceTime = DateTime.Today;

            Assert.AreEqual(sourceTime, sourceTime.AsValue(typeof(DateTime)));
            Assert.AreEqual(sourceTime, sourceTime.AsValue(typeof(DateTime?)));


            Guid sourceGuid = Guid.NewGuid();

            Assert.AreEqual(sourceGuid, sourceGuid.ToString().AsValue(typeof(Guid)));

            object timespan = TimeSpan.FromDays(1);
            Assert.AreEqual(timespan, timespan.ToString().AsValue(typeof(TimeSpan)));
        }

        [Test]
        public void AsValueWithTypeAndDefaultValue() {
            object source = "ABC";
            Assert.AreEqual(DateTime.Today, source.AsValue(typeof(DateTime), () => DateTime.Today));
            Assert.AreEqual(100, source.AsValue(typeof(int?), () => (int?)100));

            source = DBNull.Value;
            Assert.AreEqual(DateTime.Today, source.AsValue(typeof(DateTime), () => DateTime.Today));

            source = null;
            Assert.AreEqual(DateTime.Today, source.AsValue(typeof(DateTime), () => DateTime.Today));
        }

        [Test]
        public void AsValueTest() {
            object source = null;
            const int defaultValue = 100;

            Assert.AreEqual(default(int), source.AsValue<int>());
            Assert.AreEqual(defaultValue, source.AsValue<int>(defaultValue));
            Assert.AreEqual(defaultValue, source.AsValue<int>(() => defaultValue));

            Assert.AreEqual(123, "123".AsValue<int>());
            Assert.AreEqual(123, "123".AsValue<int>(0));
            Assert.AreEqual(123, "123".AsValue<int>(() => 0));

            Assert.AreEqual(123L, "123".AsValue<long>());
            Assert.AreEqual(123L, "123".AsValue<long>(0L));
            Assert.AreEqual(123L, "123".AsValue<long>(() => 0L));

            var today = DateTime.Today;
            var todayStr = today.ToSortableString();

            Assert.AreEqual(today, todayStr.AsValue<DateTime>());
            Assert.AreEqual(today, todayStr.AsValue<DateTime>(today));
            Assert.AreEqual(today, todayStr.AsValue<DateTime>(() => DateTime.Today));

            DateTime? todayNullable = today;
            Assert.AreEqual(todayNullable, todayStr.AsValue<DateTime?>());
            Assert.AreEqual(todayNullable, todayStr.AsValue<DateTime?>(todayNullable));
            Assert.AreEqual(todayNullable, todayStr.AsValue<DateTime?>(() => (DateTime?)DateTime.Today));
            Assert.AreEqual(todayNullable, today.ToSortableString().AsDateTimeNullable());
        }

        [Test]
        public void AsBoolTest() {
            object source = null;
            const bool defaultValue = true;

            Assert.AreEqual(default(bool), source.AsBool());
            Assert.AreEqual(defaultValue, source.AsBool(defaultValue));
            Assert.AreEqual(defaultValue, source.AsBool(() => defaultValue));
            Assert.AreEqual(null, source.AsBoolNullable());
        }

        [Test]
        public void AsCharTest() {
            object source = null;
            const char defaultValue = char.MaxValue;

            Assert.AreEqual(default(char), source.AsChar());
            Assert.AreEqual(defaultValue, source.AsChar(defaultValue));
            Assert.AreEqual(defaultValue, source.AsChar(() => defaultValue));
            Assert.AreEqual(null, source.AsCharNullable());
            Assert.AreEqual(defaultValue, defaultValue.ToString().AsCharNullable());
        }

        [Test]
        public void AsByteTest() {
            object source = null;
            const byte defaultValue = byte.MaxValue;

            Assert.AreEqual(default(byte), source.AsByte());
            Assert.AreEqual(defaultValue, source.AsByte(defaultValue));
            Assert.AreEqual(defaultValue, source.AsByte(() => defaultValue));
            Assert.AreEqual(null, source.AsByteNullable());
            Assert.AreEqual(defaultValue, defaultValue.ToString().AsByteNullable());
        }

        [Test]
        public void AsShortTest() {
            object source = null;
            const short defaultValue = short.MaxValue;

            Assert.AreEqual(default(short), source.AsShort());
            Assert.AreEqual(defaultValue, source.AsShort(defaultValue));
            Assert.AreEqual(defaultValue, source.AsShort(() => defaultValue));
            Assert.AreEqual(null, source.AsShortNullable());
            Assert.AreEqual(defaultValue, defaultValue.ToString().AsShortNullable());
        }

        [Test]
        public void AsIntTest() {
            object source = null;
            const int defaultValue = int.MaxValue;

            Assert.AreEqual(default(int), source.AsInt());
            Assert.AreEqual(defaultValue, source.AsInt(defaultValue));
            Assert.AreEqual(defaultValue, source.AsInt(() => defaultValue));
            Assert.AreEqual(null, source.AsIntNullable());
            Assert.AreEqual(defaultValue, defaultValue.ToString().AsIntNullable());
        }

        [Test]
        public void AsLongTest() {
            object source = null;
            const long defaultValue = long.MaxValue;

            Assert.AreEqual(default(long), source.AsLong());
            Assert.AreEqual(defaultValue, source.AsLong(defaultValue));
            Assert.AreEqual(defaultValue, source.AsLong(() => defaultValue));
            Assert.AreEqual(null, source.AsLongNullable());
            Assert.AreEqual(defaultValue, defaultValue.ToString().AsLongNullable());
        }

        [Test]
        public void AsDecimalTest() {
            object source = null;
            const decimal defaultValue = decimal.MaxValue;

            Assert.AreEqual(default(decimal), source.AsDecimal());
            Assert.AreEqual(defaultValue, source.AsDecimal(defaultValue));
            Assert.AreEqual(defaultValue, source.AsDecimal(() => defaultValue));
            Assert.AreEqual(null, source.AsDecimalNullable());
            Assert.AreEqual(defaultValue, defaultValue.ToString().AsDecimalNullable());
        }

        [Test]
        public void AsFloatTest() {
            for(int i = 0; i < 100; i++) {
                object source = null;
                var defaultValue = (float)Rnd.Next(int.MaxValue) * (float)Rnd.NextDouble();
                object value = defaultValue;

                Assert.AreEqual(default(float), source.AsFloat());
                Assert.AreEqual(defaultValue, source.AsFloat(defaultValue));
                Assert.AreEqual(defaultValue, source.AsFloat(() => defaultValue));
                Assert.AreEqual(null, source.AsFloatNullable());

                // NOTE: float 는 변환 시 정확한 값을 가지지 못합니다. double이나 decimal을 사용하세요!!!
                Assert.AreEqual(1.0f, defaultValue / value.AsFloatNullable().Value, 0.00001f);
            }
        }

        [Test]
        public void AsDoubleTest() {
            object source = null;
            double defaultValue = double.MaxValue;
            object value = defaultValue;

            Assert.AreEqual(default(double), source.AsDouble());
            Assert.AreEqual(defaultValue, source.AsDouble(defaultValue));
            Assert.AreEqual(defaultValue, source.AsDouble(() => defaultValue));
            Assert.AreEqual(null, source.AsDoubleNullable());
            Assert.AreEqual(defaultValue, value.AsDoubleNullable());

            for(int i = 0; i < 100; i++) {
                source = null;
                defaultValue = Rnd.NextDouble() * Rnd.Next(int.MaxValue);
                value = defaultValue;

                Assert.AreEqual(default(double), source.AsDouble());
                Assert.AreEqual(defaultValue, source.AsDouble(defaultValue));
                Assert.AreEqual(defaultValue, source.AsDouble(() => defaultValue));
                Assert.AreEqual(null, source.AsDoubleNullable());
                Assert.AreEqual(1.0d, defaultValue / value.AsDoubleNullable().Value, 0.00000001d);
            }
        }

        [Test]
        public void AsDateTimeTest() {
            object source = null;
            DateTime defaultValue = DateTime.Today;

            Assert.AreEqual(DateTime.MinValue, source.AsDateTime());
            Assert.AreEqual(defaultValue, source.AsDateTime(defaultValue));
            Assert.AreEqual(defaultValue, source.AsDateTime(() => defaultValue));
            Assert.AreEqual(null, source.AsDateTimeNullable());
            Assert.AreEqual(defaultValue, defaultValue.ToSortableString().AsDateTimeNullable());
        }

        [Test]
        public void AsTimeSpanTest() {
            object source = null;
            TimeSpan defaultValue = TimeSpan.MaxValue;

            Assert.AreEqual(TimeSpan.Zero, source.AsTimeSpan());
            Assert.AreEqual(defaultValue, source.AsTimeSpan(defaultValue));
            Assert.AreEqual(defaultValue, source.AsTimeSpan(() => defaultValue));
            Assert.AreEqual(null, source.AsTimeSpanNullable());
            Assert.AreEqual(defaultValue, defaultValue.ToString().AsTimeSpanNullable());
        }

        [Test]
        public void AsGuidTest() {
            object source = null;
            Guid defaultValue = Guid.NewGuid();

            Assert.AreEqual(Guid.Empty, source.AsGuid());

            Assert.AreEqual(defaultValue, source.AsGuid(defaultValue));
            Assert.AreEqual(defaultValue, source.AsGuid(() => defaultValue));
            Assert.AreEqual(null, source.AsGuidNullable());
            Assert.AreEqual(defaultValue, defaultValue.ToString().AsGuidNullable());
        }

        [Test]
        public void AsTextTest() {
            object source = null;
            Guid sourceGuid = Guid.NewGuid();

            string expecedText = sourceGuid.ToString();

            Assert.AreEqual(Guid.Empty, source.AsGuid(), "#1");

            Assert.AreEqual(expecedText, sourceGuid.AsText(), "#2");
            Assert.AreEqual(expecedText, sourceGuid.AsText(CultureInfo.InvariantCulture), "#3");
            Assert.AreEqual(expecedText, source.AsText(() => sourceGuid.ToString()), "#4");
        }

        #endregion
    }
}