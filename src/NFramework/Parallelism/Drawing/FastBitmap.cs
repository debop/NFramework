using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace NSoft.NFramework.Parallelism.Drawing {
    /// <summary>
    /// <see cref="Bitmap"/>의 Pixel 처리를 신속하게 하기 위해, 기존 Bitmap을 Wrapping한 클래스입니다.
    /// lock/unlock을 통해 Bitmap Pixel 조작을 빠르게 수행할 수 있습니다.
    /// </summary>
    [Serializable]
    public unsafe class FastBitmap : IDisposable {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private int _width;
        private BitmapData _bitmapData;
        private byte* _pBase = null;
        private PixelData* _pInitPixel = null;
        private bool _locked;

        private static readonly int PixelDataSize = sizeof(PixelData);

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="bitmap">실제 처리할 Bitmap 인스턴스</param>
        public FastBitmap(Bitmap bitmap) {
            bitmap.ShouldNotBeNull("bitmap");

            Bitmap = bitmap;
            Size = new Size(bitmap.Width, bitmap.Height);

            LockBitmap();
        }

        /// <summary>
        /// 실제 처리할 래핑된 Bitmap
        /// </summary>
        protected Bitmap Bitmap { get; private set; }

        /// <summary>
        /// Bitmap Size
        /// </summary>
        public Size Size { get; private set; }

        /// <summary>
        /// 지정한 Row의 전체 Pixel 정보를 제공하기 위해, 첫번째 컬럼의 PixelData 선두 번지를 제공합니다.
        /// </summary>
        /// <param name="rowNumber"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public PixelData* GetInitialPixelForRow(int rowNumber) {
            return (PixelData*)(_pBase + rowNumber + _width);
        }

        /// <summary>
        /// <see cref="PixelData"/>의 Pointer를 제공하는 인덱서입니다.
        /// </summary>
        /// <param name="x">row index</param>
        /// <param name="y">column index</param>
        /// <returns><see cref="PixelData"/>의 포인터</returns>
        [CLSCompliant(false)]
        public PixelData* this[int x, int y] {
            get { return (PixelData*)(_pBase + y * _width + x * PixelDataSize); }
        }

        /// <summary>
        /// 지정된 위치의 색상을 구합니다.
        /// </summary>
        /// <param name="x">row index</param>
        /// <param name="y">column index</param>
        /// <returns></returns>
        public Color GetColor(int x, int y) {
            var pixel = this[x, y];
            return Color.FromArgb(pixel->R, pixel->G, pixel->B);
        }

        /// <summary>
        /// 지정된 위치의 Pixel의 색상을 설정합니다.
        /// </summary>
        public void SetColor(int x, int y, Color c) {
            var pixel = this[x, y];

            pixel->R = c.R;
            pixel->G = c.G;
            pixel->B = c.B;
        }

        private void LockBitmap() {
            Guard.Assert(_locked == false, "Already locked.");

            if(IsDebugEnabled)
                log.Debug("Bitmap 이미지 정보를 Lock을 겁니다...");

            var bounds = new Rectangle(0, 0, Bitmap.Width, Bitmap.Height);

            // Bitmap의 행의 byte 수를 계산한다. 4의 배수가 되어야 한다.
            //
            _width = bounds.Width * sizeof(PixelData);
            if(_width % 4 != 0)
                _width = 4 * (_width / 4 + 1);

            // lock bitmap
            _bitmapData = Bitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            _pBase = (byte*)_bitmapData.Scan0.ToPointer();
            _locked = true;
        }

        private void UnlockBitmap() {
            Guard.Assert(_locked, "Not currently locked.");

            if(IsDebugEnabled)
                log.Debug("Bitmap 이미지 정보의 Lock을 풉니다...");

            try {
                Bitmap.UnlockBits(_bitmapData);
                _bitmapData = null;
                _pBase = null;
            }
            finally {
                _locked = false;
            }
        }

        private void InitCurrentPixel() {
            _pInitPixel = (PixelData*)_pBase;
        }

        public void Dispose() {
            if(_locked)
                UnlockBitmap();
        }
    }
}