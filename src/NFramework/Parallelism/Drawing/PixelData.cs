using System;

namespace NSoft.NFramework.Parallelism.Drawing {
    /// <summary>
    /// Pixel RGB Data
    /// </summary>
    [Serializable]
    public struct PixelData : IEquatable<PixelData> {
        public PixelData(byte r, byte g, byte b) : this() {
            R = r;
            G = g;
            B = b;
        }

        /// <summary>
        /// Red
        /// </summary>
        public byte R;

        /// <summary>
        /// Green
        /// </summary>
        public byte G;

        /// <summary>
        /// Blue
        /// </summary>
        public byte B;

        public bool Equals(PixelData other) {
            return GetHashCode().Equals(other.GetHashCode());
        }

        public override bool Equals(object obj) {
            return (obj != null) && (obj is PixelData) && Equals((PixelData)obj);
        }

        public override int GetHashCode() {
            return R * 65536 + G * 256 + B;
        }

        public override string ToString() {
            return string.Format("PixelData# RGB[{0},{1},{2}]", R, G, B);
        }
    }
}