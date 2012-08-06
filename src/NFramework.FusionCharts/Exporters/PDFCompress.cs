using System.IO;

namespace NSoft.NFramework.FusionCharts {
    // TODO: 새로 만들어야 한다.

    /// <summary>
    /// This is an ad-hoc class to compress PDF stream.
    /// Currently this class compresses binary (byte) stream using RLE which 
    /// PDF 1.3 specification has thus formulated:
    /// 
    /// The RunLengthDecode filter decodes data that has been encoded in a simple 
    /// byte-oriented format based on run length. The encoded data is a sequence of 
    /// runs, where each run consists of a length byte followed by 1 to 128 bytes of data. If 
    /// the length byte is in the range 0 to 127, the following length + 1 (1 to 128) bytes 
    /// are copied literally during decompression. If length is in the range 129 to 255, the 
    /// following single byte is to be copied 257 − length (2 to 128) times during decompression. 
    /// A length value of 128 denotes EOD.
    /// 
    /// The chart image compression ratio comes to around 10:3 
    /// 
    /// </summary>
    public class PDFCompress {
        /// <summary>
        /// stores the output compressed data in MemoryStream object later to be converted to byte[] array
        /// </summary>
        private readonly MemoryStream _Compressed = new MemoryStream();

        /// <summary>
        ///  Uncompresses data as byte[] array
        /// </summary>
        private readonly byte[] _UnCompressed;

        /// <summary>
        /// Takes the uncompressed byte array
        /// </summary>
        /// <param name="UnCompressed">uncompressed data</param>
        public PDFCompress(byte[] UnCompressed) {
            _UnCompressed = UnCompressed;
        }

        /// <summary>
        /// Write compressed data as RunLength
        /// </summary>
        /// <param name="length">The length of repeated data</param>
        /// <param name="encodee">The byte to be repeated</param>
        /// <returns></returns>
        private int WriteRunLength(int length, byte encodee) {
            // write the repeat length
            _Compressed.WriteByte((byte)(257 - length));
            // write the byte to be repeated
            _Compressed.WriteByte(encodee);

            //re-set repeat length
            length = 1;
            return length;
        }

        private void WriteNoRepeater(MemoryStream NoRepeatBytes) {
            // write the length of non repeted data
            _Compressed.WriteByte((byte)((int)NoRepeatBytes.Length - 1));
            // write the non repeated data put literally
            _Compressed.Write(NoRepeatBytes.ToArray(), 0, (int)NoRepeatBytes.Length);

            // re-set non repeat byte storage stream
            NoRepeatBytes.SetLength(0);
        }

        /// <summary>
        /// compresses uncompressed data to compressed data in byte array
        /// </summary>
        /// <returns></returns>
        public byte[] RLECompress() {
            // stores non repeatable data
            var NoRepeat = new MemoryStream();

            // repeat counter
            int _RL = 1;

            // 2 consecutive bytes to compare
            byte preByte = 0, postByte = 0;

            // iterate through the uncompressed bytes
            for(int i = 0; i < _UnCompressed.Length - 1; i++) {
                // get 2 consecutive bytes
                preByte = _UnCompressed[i];
                postByte = _UnCompressed[i + 1];

                // if both are same there is scope for repitition
                if(preByte == postByte) {
                    // but flush the non repeatable data (if present) to compressed stream 
                    if(NoRepeat.Length > 0)
                        WriteNoRepeater(NoRepeat);

                    // increase repeat count
                    _RL++;

                    // if repeat count reaches limit of repeat i.e. 128 
                    // write the repeat data and reset the repeat counter
                    if(_RL > 128)
                        _RL = WriteRunLength(_RL - 1, preByte);
                }
                else {
                    // when consecutive bytes do not match

                    // store non-repeatable data
                    if(_RL == 1)
                        NoRepeat.WriteByte(preByte);

                    // write repeated length and byte (if present ) to output stream
                    if(_RL > 1)
                        _RL = WriteRunLength(_RL, preByte);

                    // write non repeatable data to out put stream if the length reaches limit
                    if(NoRepeat.Length == 128)
                        WriteNoRepeater(NoRepeat);
                }
            }

            // at the end of iteration 
            // take care of the last byte

            // if repeated 
            if(_RL > 1) {
                // write run length and byte (if present ) to output stream
                _RL = WriteRunLength(_RL, preByte);
            }
            else {
                // if non repeated byte is left behind
                // write non repeatable data to output stream 
                NoRepeat.WriteByte(postByte);
                WriteNoRepeater(NoRepeat);
            }

            // wrote EOD
            _Compressed.WriteByte(128);

            //close streams
            NoRepeat.Close();
            _Compressed.Close();

            // return compressed data in byte array
            return _Compressed.ToArray();
        }
    }
}