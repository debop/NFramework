using System;

namespace NSoft.NFramework.Numerics {
    internal class Generator {
        public static Tuple<string, double> GenerateDouble(int index, int nanIndex) {
            if(index == nanIndex)
                return new Tuple<string, double>(string.Format("Value {0}", index), double.NaN);

            return new Tuple<string, double>(string.Format("Value {0}", index), index);
        }

        public static Tuple<string, double?> GenerateNullableDouble(int index, int nanIndex, int nullIndex) {
            if(index == nullIndex)
                return new Tuple<string, double?>(string.Format("Value {0}", index), null);

            if(index == nanIndex)
                return new Tuple<string, double?>(string.Format("Value {0}", index), double.NaN);

            return new Tuple<string, double?>(string.Format("Value {0}", index), index);
        }

        public static Tuple<string, float> GenerateFloat(int index, int nanIndex) {
            if(index == nanIndex)
                return new Tuple<string, float>(string.Format("Value {0}", index), float.NaN);

            return new Tuple<string, float>(string.Format("Value {0}", index), index);
        }

        public static Tuple<string, float?> GenerateNullableFloat(int index, int nanIndex, int nullIndex) {
            if(index == nullIndex)
                return new Tuple<string, float?>(string.Format("Value {0}", index), null);

            if(index == nanIndex)
                return new Tuple<string, float?>(string.Format("Value {0}", index), float.NaN);

            return new Tuple<string, float?>(string.Format("Value {0}", index), index);
        }

        public static Tuple<string, decimal> GenerateDecimal(int index) {
            return new Tuple<string, decimal>(string.Format("Value {0}", index), index);
        }

        public static Tuple<string, decimal?> GenerateNullableDecimal(int index, int nullIndex) {
            if(index == nullIndex)
                return new Tuple<string, decimal?>(string.Format("Value {0}", index), null);

            return new Tuple<string, decimal?>(string.Format("Value {0}", index), index);
        }

        public static Tuple<string, long> GenerateLong(int index) {
            return new Tuple<string, long>(string.Format("Value {0}", index), index);
        }

        public static Tuple<string, long?> GenerateNullableLong(int index, int nullIndex) {
            if(index == nullIndex)
                return new Tuple<string, long?>(string.Format("Value {0}", index), null);

            return new Tuple<string, long?>(string.Format("Value {0}", index), index);
        }

        public static Tuple<string, int> GenerateInt(int index) {
            return new Tuple<string, int>(string.Format("Value {0}", index), index);
        }

        public static Tuple<string, int?> GenerateNullableInt(int index, int nullIndex) {
            if(index == nullIndex)
                return new Tuple<string, int?>(string.Format("Value {0}", index), null);

            return new Tuple<string, int?>(string.Format("Value {0}", index), index);
        }
    }
}