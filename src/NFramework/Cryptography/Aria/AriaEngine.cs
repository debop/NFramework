using System;

namespace NSoft.NFramework.Cryptography.Aria {
    /// <summary>
    /// ARIA 알고리즘을 사용하여, 암호회/복호화를 수행합니다.
    /// </summary>
    [Serializable]
    public sealed class AriaEngine {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 기본 키 사이즈 (256)
        /// </summary>
        public const int DefaultAriaKeySize = 256;

        internal const string InvalidKeySizeMessage = "키 크기가 잘못되었습니다. 128, 192, AriaKeySize 중에 한 값이어야 합니다. keySize=[{0}]";

        private static readonly char[] HEX_DIGITS = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

        private static readonly long[][] KRK = {
                                                   new long[] { 0x517cc1b7, 0x27220a94, 0xfe13abe8, 0xfa9a6ee0 },
                                                   new long[] { 0x6db14acc, 0x9e21c820, 0xff28b1d5, 0xef5de2b0 },
                                                   new long[] { 0xdb92371d, 0x2126e970, 0x03249775, 0x04e8c90e }
                                               };

        private static readonly byte[] S1 = new byte[DefaultAriaKeySize];
        private static readonly byte[] S2 = new byte[DefaultAriaKeySize];
        private static readonly byte[] X1 = new byte[DefaultAriaKeySize];
        private static readonly byte[] X2 = new byte[DefaultAriaKeySize];

        private static readonly int[] TS1 = new int[DefaultAriaKeySize];
        private static readonly int[] TS2 = new int[DefaultAriaKeySize];
        private static readonly int[] TX1 = new int[DefaultAriaKeySize];
        private static readonly int[] TX2 = new int[DefaultAriaKeySize];

        /// <summary>
        /// Static Initializer
        /// </summary>
        static AriaEngine() {
            if(log.IsInfoEnabled)
                log.Info("Aria 암호화 Engine을 초기화합니다...");

            var expArr = new int[DefaultAriaKeySize];
            var logArr = new int[DefaultAriaKeySize];

            expArr[0] = 1;

            for(var i = 1; i < DefaultAriaKeySize; i++) {
                var j = (expArr[i - 1] << 1) ^ expArr[i - 1];
                if((j & 0x100) != 0)
                    j ^= 0x11b;
                expArr[i] = j;
            }

            for(var i = 1; i < 255; i++)
                logArr[expArr[i]] = i;

            int[][] A = {
                            new[] { 1, 0, 0, 0, 1, 1, 1, 1 },
                            new[] { 1, 1, 0, 0, 0, 1, 1, 1 },
                            new[] { 1, 1, 1, 0, 0, 0, 1, 1 },
                            new[] { 1, 1, 1, 1, 0, 0, 0, 1 },
                            new[] { 1, 1, 1, 1, 1, 0, 0, 0 },
                            new[] { 0, 1, 1, 1, 1, 1, 0, 0 },
                            new[] { 0, 0, 1, 1, 1, 1, 1, 0 },
                            new[] { 0, 0, 0, 1, 1, 1, 1, 1 }
                        };
            int[][] B = {
                            new[] { 0, 1, 0, 1, 1, 1, 1, 0 },
                            new[] { 0, 0, 1, 1, 1, 1, 0, 1 },
                            new[] { 1, 1, 0, 1, 0, 1, 1, 1 },
                            new[] { 1, 0, 0, 1, 1, 1, 0, 1 },
                            new[] { 0, 0, 1, 0, 1, 1, 0, 0 },
                            new[] { 1, 0, 0, 0, 0, 0, 0, 1 },
                            new[] { 0, 1, 0, 1, 1, 1, 0, 1 },
                            new[] { 1, 1, 0, 1, 0, 0, 1, 1 }
                        };

            for(var i = 0; i < DefaultAriaKeySize; i++) {
                var t = 0;
                var p = (i == 0) ? 0 : expArr[255 - logArr[i]];

                for(var j = 0; j < 8; j++) {
                    var s = 0;
                    for(var k = 0; k < 8; k++) {
                        if(((p >> (7 - k)) & 0x01) != 0)
                            s ^= A[k][j];
                    }
                    t = (t << 1) ^ s;
                }
                t ^= 0x63;
                S1[i] = (byte)t;
                X1[t] = (byte)i;
            }

            for(var i = 0; i < DefaultAriaKeySize; i++) {
                var t = 0;
                var p = i == 0 ? 0 : expArr[(247 * logArr[i]) % 255];

                for(var j = 0; j < 8; j++) {
                    var s = 0;
                    for(var k = 0; k < 8; k++) {
                        if(((p >> k) & 0x01) != 0)
                            s ^= B[7 - j][k];
                    }
                    t = (t << 1) ^ s;
                }
                t ^= 0xe2;
                S2[i] = (byte)t;
                X2[t] = (byte)i;
            }

            for(var i = 0; i < DefaultAriaKeySize; i++) {
                TS1[i] = 0x00010101 * (S1[i] & 0xff);
                TS2[i] = 0x01000101 * (S2[i] & 0xff);
                TX1[i] = 0x01010001 * (X1[i] & 0xff);
                TX2[i] = 0x01010100 * (X2[i] & 0xff);
            }

            if(log.IsInfoEnabled)
                log.Info("Aria 암호화 Engine을 초기화했습니다!!!");
        }

        private int _keySize;
        private int _numberOfRounds;
        private byte[] _masterKey;
        private int[] _encRoundKeys;
        private int[] _decRoundKeys;

        public AriaEngine() : this(DefaultAriaKeySize) {}

        public AriaEngine(int keySize) {
            if(IsDebugEnabled)
                log.Debug("AriaEngine 인스턴스를 생성합니다. keySize=[{0}]", keySize);

            KeySize = keySize;
        }

        public int KeySize {
            get { return _keySize; }
            set {
                AssertValidKeySize(value);
                Reset();

                _keySize = value;
                switch(_keySize) {
                    case 128:
                        _numberOfRounds = 12;
                        break;
                    case 192:
                        _numberOfRounds = 14;
                        break;
                    case DefaultAriaKeySize:
                        _numberOfRounds = 16;
                        break;
                }
            }
        }

        public void Reset() {
            _keySize = 0;
            _numberOfRounds = 0;
            _masterKey = null;
            _encRoundKeys = null;
            _decRoundKeys = null;
        }

        public void SetKey(byte[] masterKey) {
            masterKey.ShouldNotBeNull("masterKey");
            Guard.Assert(() => masterKey.Length * 8 >= _keySize,
                         @"MasterKey 크기 [{0}] * 8 이 최소한 keySize [{1}] 이상 이어야 합니다.", masterKey.Length, _keySize);

            _decRoundKeys = null;
            _encRoundKeys = null;
            _masterKey = (byte[])masterKey.Clone();
        }

        public void SetupRoundKeys() {
            SetupDecRoundKeys();
        }

        private void SetupEncRoundKeys() {
            _keySize.ShouldNotBeZero("_keySize");
            _masterKey.ShouldNotBeNull("_masterKey");

            if(_encRoundKeys == null)
                _encRoundKeys = new int[4 * (_numberOfRounds + 1)];
            _decRoundKeys = null;

            DoEncKeySetup(_masterKey, _encRoundKeys, _keySize);
        }

        private void SetupDecRoundKeys() {
            _keySize.ShouldNotBeZero("_keySize");

            if(_encRoundKeys == null) {
                _masterKey.ShouldNotBeNull("_masterKey");
                SetupEncRoundKeys();
            }

            _decRoundKeys = (int[])_encRoundKeys.Clone();
            DoDecKeySetup(_encRoundKeys, _keySize);
        }

        private static void DoCrypt(byte[] i, int ioffset, int[] rk, int nr, byte[] o, int ooffset) {
            var j = 0;

            long t0 = ToInt(i[0 + ioffset], i[1 + ioffset], i[2 + ioffset], i[3 + ioffset]);
            long t1 = ToInt(i[4 + ioffset], i[5 + ioffset], i[6 + ioffset], i[7 + ioffset]);
            long t2 = ToInt(i[8 + ioffset], i[9 + ioffset], i[10 + ioffset], i[11 + ioffset]);
            long t3 = ToInt(i[12 + ioffset], i[13 + ioffset], i[14 + ioffset], i[15 + ioffset]);

            for(var r = 1; r < nr / 2; r++) {
                t0 ^= rk[j++];
                t1 ^= rk[j++];
                t2 ^= rk[j++];
                t3 ^= rk[j++];
                t0 = TS1[(t0 >> 24) & 0xff] ^ TS2[(t0 >> 16) & 0xff] ^ TX1[(t0 >> 8) & 0xff] ^ TX2[t0 & 0xff];
                t1 = TS1[(t1 >> 24) & 0xff] ^ TS2[(t1 >> 16) & 0xff] ^ TX1[(t1 >> 8) & 0xff] ^ TX2[t1 & 0xff];
                t2 = TS1[(t2 >> 24) & 0xff] ^ TS2[(t2 >> 16) & 0xff] ^ TX1[(t2 >> 8) & 0xff] ^ TX2[t2 & 0xff];
                t3 = TS1[(t3 >> 24) & 0xff] ^ TS2[(t3 >> 16) & 0xff] ^ TX1[(t3 >> 8) & 0xff] ^ TX2[t3 & 0xff];
                t1 ^= t2;
                t2 ^= t3;
                t0 ^= t1;
                t3 ^= t1;
                t2 ^= t0;
                t1 ^= t2;
                t1 = Badc(t1);
                t2 = Cdab(t2);
                t3 = Dcba(t3);
                t1 ^= t2;
                t2 ^= t3;
                t0 ^= t1;
                t3 ^= t1;
                t2 ^= t0;
                t1 ^= t2;

                t0 ^= rk[j++];
                t1 ^= rk[j++];
                t2 ^= rk[j++];
                t3 ^= rk[j++];
                t0 = TX1[(t0 >> 24) & 0xff] ^ TX2[(t0 >> 16) & 0xff] ^ TS1[(t0 >> 8) & 0xff] ^ TS2[t0 & 0xff];
                t1 = TX1[(t1 >> 24) & 0xff] ^ TX2[(t1 >> 16) & 0xff] ^ TS1[(t1 >> 8) & 0xff] ^ TS2[t1 & 0xff];
                t2 = TX1[(t2 >> 24) & 0xff] ^ TX2[(t2 >> 16) & 0xff] ^ TS1[(t2 >> 8) & 0xff] ^ TS2[t2 & 0xff];
                t3 = TX1[(t3 >> 24) & 0xff] ^ TX2[(t3 >> 16) & 0xff] ^ TS1[(t3 >> 8) & 0xff] ^ TS2[t3 & 0xff];
                t1 ^= t2;
                t2 ^= t3;
                t0 ^= t1;
                t3 ^= t1;
                t2 ^= t0;
                t1 ^= t2;
                t3 = Badc(t3);
                t0 = Cdab(t0);
                t1 = Dcba(t1);
                t1 ^= t2;
                t2 ^= t3;
                t0 ^= t1;
                t3 ^= t1;
                t2 ^= t0;
                t1 ^= t2;
            }
            t0 ^= rk[j++];
            t1 ^= rk[j++];
            t2 ^= rk[j++];
            t3 ^= rk[j++];
            t0 = TS1[(t0 >> 24) & 0xff] ^ TS2[(t0 >> 16) & 0xff] ^ TX1[(t0 >> 8) & 0xff] ^ TX2[t0 & 0xff];
            t1 = TS1[(t1 >> 24) & 0xff] ^ TS2[(t1 >> 16) & 0xff] ^ TX1[(t1 >> 8) & 0xff] ^ TX2[t1 & 0xff];
            t2 = TS1[(t2 >> 24) & 0xff] ^ TS2[(t2 >> 16) & 0xff] ^ TX1[(t2 >> 8) & 0xff] ^ TX2[t2 & 0xff];
            t3 = TS1[(t3 >> 24) & 0xff] ^ TS2[(t3 >> 16) & 0xff] ^ TX1[(t3 >> 8) & 0xff] ^ TX2[t3 & 0xff];
            t1 ^= t2;
            t2 ^= t3;
            t0 ^= t1;
            t3 ^= t1;
            t2 ^= t0;
            t1 ^= t2;
            t1 = Badc(t1);
            t2 = Cdab(t2);
            t3 = Dcba(t3);
            t1 ^= t2;
            t2 ^= t3;
            t0 ^= t1;
            t3 ^= t1;
            t2 ^= t0;
            t1 ^= t2;

            t0 ^= rk[j++];
            t1 ^= rk[j++];
            t2 ^= rk[j++];
            t3 ^= rk[j++];
            o[0 + ooffset] = (byte)(X1[0xff & (t0 >> 24)] ^ (rk[j] >> 24));
            o[1 + ooffset] = (byte)(X2[0xff & (t0 >> 16)] ^ (rk[j] >> 16));
            o[2 + ooffset] = (byte)(S1[0xff & (t0 >> 8)] ^ (rk[j] >> 8));
            o[3 + ooffset] = (byte)(S2[0xff & (t0)] ^ (rk[j]));
            o[4 + ooffset] = (byte)(X1[0xff & (t1 >> 24)] ^ (rk[j + 1] >> 24));
            o[5 + ooffset] = (byte)(X2[0xff & (t1 >> 16)] ^ (rk[j + 1] >> 16));
            o[6 + ooffset] = (byte)(S1[0xff & (t1 >> 8)] ^ (rk[j + 1] >> 8));
            o[7 + ooffset] = (byte)(S2[0xff & (t1)] ^ (rk[j + 1]));
            o[8 + ooffset] = (byte)(X1[0xff & (t2 >> 24)] ^ (rk[j + 2] >> 24));
            o[9 + ooffset] = (byte)(X2[0xff & (t2 >> 16)] ^ (rk[j + 2] >> 16));
            o[10 + ooffset] = (byte)(S1[0xff & (t2 >> 8)] ^ (rk[j + 2] >> 8));
            o[11 + ooffset] = (byte)(S2[0xff & (t2)] ^ (rk[j + 2]));
            o[12 + ooffset] = (byte)(X1[0xff & (t3 >> 24)] ^ (rk[j + 3] >> 24));
            o[13 + ooffset] = (byte)(X2[0xff & (t3 >> 16)] ^ (rk[j + 3] >> 16));
            o[14 + ooffset] = (byte)(S1[0xff & (t3 >> 8)] ^ (rk[j + 3] >> 8));
            o[15 + ooffset] = (byte)(S2[0xff & (t3)] ^ (rk[j + 3]));
        }

        /// <summary>
        /// 암호화를 수행하여, 결과를 <paramref name="o"/>에 저장합니다.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="ioffset"></param>
        /// <param name="o"></param>
        /// <param name="ooffset"></param>
        public void Encrypt(byte[] i, int ioffset, byte[] o, int ooffset) {
            _keySize.ShouldNotBeNull("_keySize");

            if(_encRoundKeys == null) {
                _masterKey.ShouldNotBeNull("_masterKey");
                SetupEncRoundKeys();
            }
            DoCrypt(i, ioffset, _encRoundKeys, _numberOfRounds, o, ooffset);
        }

        /// <summary>
        /// 암호화를 수행하여, 결과를 반환합니다.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="ioffset"></param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] i, int ioffset) {
            byte[] o = new byte[16];
            Encrypt(i, ioffset, o, 0);
            return o;
        }

        /// <summary>
        /// 복호화를 수행하여, 결과를 대상 버퍼에 씁니다.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="ioffset"></param>
        /// <param name="o"></param>
        /// <param name="ooffset"></param>
        public void Decrypt(byte[] i, int ioffset, byte[] o, int ooffset) {
            _keySize.ShouldNotBeZero("_keySize");

            if(_encRoundKeys == null) {
                _masterKey.ShouldNotBeNull("_masterKey");
                SetupDecRoundKeys();
            }
            DoCrypt(i, ioffset, _decRoundKeys, _numberOfRounds, o, ooffset);
        }

        /// <summary>
        /// 복호화를 수행하여, 결과를 반환합니다.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="ioffset"></param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] i, int ioffset) {
            var o = new byte[16];
            Decrypt(i, ioffset, o, 0);
            return o;
        }

        private static void DoEncKeySetup(byte[] mk, int[] rk, int keyBits) {
            var j = 0;
            var w0 = new int[4];
            var w1 = new int[4];
            var w2 = new int[4];
            var w3 = new int[4];

            w0[0] = ToInt(mk[0], mk[1], mk[2], mk[3]);
            w0[1] = ToInt(mk[4], mk[5], mk[6], mk[7]);
            w0[2] = ToInt(mk[8], mk[9], mk[10], mk[11]);
            w0[3] = ToInt(mk[12], mk[13], mk[14], mk[15]);

            var q = (keyBits - 128) / 64;
            var t0 = w0[0] ^ KRK[q][0];
            var t1 = w0[1] ^ KRK[q][1];
            var t2 = w0[2] ^ KRK[q][2];
            var t3 = w0[3] ^ KRK[q][3];
            t0 = TS1[(t0 >> 24) & 0xff] ^ TS2[(t0 >> 16) & 0xff] ^ TX1[(t0 >> 8) & 0xff] ^ TX2[t0 & 0xff];
            t1 = TS1[(t1 >> 24) & 0xff] ^ TS2[(t1 >> 16) & 0xff] ^ TX1[(t1 >> 8) & 0xff] ^ TX2[t1 & 0xff];
            t2 = TS1[(t2 >> 24) & 0xff] ^ TS2[(t2 >> 16) & 0xff] ^ TX1[(t2 >> 8) & 0xff] ^ TX2[t2 & 0xff];
            t3 = TS1[(t3 >> 24) & 0xff] ^ TS2[(t3 >> 16) & 0xff] ^ TX1[(t3 >> 8) & 0xff] ^ TX2[t3 & 0xff];
            t1 ^= t2;
            t2 ^= t3;
            t0 ^= t1;
            t3 ^= t1;
            t2 ^= t0;
            t1 ^= t2;
            t1 = Badc(t1);
            t2 = Cdab(t2);
            t3 = Dcba(t3);
            t1 ^= t2;
            t2 ^= t3;
            t0 ^= t1;
            t3 ^= t1;
            t2 ^= t0;
            t1 ^= t2;

            if(keyBits > 128) {
                w1[0] = ToInt(mk[16], mk[17], mk[18], mk[19]);
                w1[1] = ToInt(mk[20], mk[21], mk[22], mk[23]);
                if(keyBits > 192) {
                    w1[2] = ToInt(mk[24], mk[25], mk[26], mk[27]);
                    w1[3] = ToInt(mk[28], mk[29], mk[30], mk[31]);
                }
                else {
                    w1[2] = w1[3] = 0;
                }
            }
            else {
                w1[0] = w1[1] = w1[2] = w1[3] = 0;
            }
            w1[0] ^= (int)t0;
            w1[1] ^= (int)t1;
            w1[2] ^= (int)t2;
            w1[3] ^= (int)t3;
            t0 = w1[0];
            t1 = w1[1];
            t2 = w1[2];
            t3 = w1[3];

            q = (q == 2) ? 0 : (q + 1);
            t0 ^= KRK[q][0];
            t1 ^= KRK[q][1];
            t2 ^= KRK[q][2];
            t3 ^= KRK[q][3];
            t0 = TX1[(t0 >> 24) & 0xff] ^ TX2[(t0 >> 16) & 0xff] ^ TS1[(t0 >> 8) & 0xff] ^ TS2[t0 & 0xff];
            t1 = TX1[(t1 >> 24) & 0xff] ^ TX2[(t1 >> 16) & 0xff] ^ TS1[(t1 >> 8) & 0xff] ^ TS2[t1 & 0xff];
            t2 = TX1[(t2 >> 24) & 0xff] ^ TX2[(t2 >> 16) & 0xff] ^ TS1[(t2 >> 8) & 0xff] ^ TS2[t2 & 0xff];
            t3 = TX1[(t3 >> 24) & 0xff] ^ TX2[(t3 >> 16) & 0xff] ^ TS1[(t3 >> 8) & 0xff] ^ TS2[t3 & 0xff];
            t1 ^= t2;
            t2 ^= t3;
            t0 ^= t1;
            t3 ^= t1;
            t2 ^= t0;
            t1 ^= t2;
            t3 = Badc(t3);
            t0 = Cdab(t0);
            t1 = Dcba(t1);
            t1 ^= t2;
            t2 ^= t3;
            t0 ^= t1;
            t3 ^= t1;
            t2 ^= t0;
            t1 ^= t2;
            t0 ^= w0[0];
            t1 ^= w0[1];
            t2 ^= w0[2];
            t3 ^= w0[3];
            w2[0] = (int)t0;
            w2[1] = (int)t1;
            w2[2] = (int)t2;
            w2[3] = (int)t3;

            q = (q == 2) ? 0 : (q + 1);
            t0 ^= KRK[q][0];
            t1 ^= KRK[q][1];
            t2 ^= KRK[q][2];
            t3 ^= KRK[q][3];
            t0 = TS1[(t0 >> 24) & 0xff] ^ TS2[(t0 >> 16) & 0xff] ^ TX1[(t0 >> 8) & 0xff] ^ TX2[t0 & 0xff];
            t1 = TS1[(t1 >> 24) & 0xff] ^ TS2[(t1 >> 16) & 0xff] ^ TX1[(t1 >> 8) & 0xff] ^ TX2[t1 & 0xff];
            t2 = TS1[(t2 >> 24) & 0xff] ^ TS2[(t2 >> 16) & 0xff] ^ TX1[(t2 >> 8) & 0xff] ^ TX2[t2 & 0xff];
            t3 = TS1[(t3 >> 24) & 0xff] ^ TS2[(t3 >> 16) & 0xff] ^ TX1[(t3 >> 8) & 0xff] ^ TX2[t3 & 0xff];
            t1 ^= t2;
            t2 ^= t3;
            t0 ^= t1;
            t3 ^= t1;
            t2 ^= t0;
            t1 ^= t2;
            t1 = Badc(t1);
            t2 = Cdab(t2);
            t3 = Dcba(t3);
            t1 ^= t2;
            t2 ^= t3;
            t0 ^= t1;
            t3 ^= t1;
            t2 ^= t0;
            t1 ^= t2;
            w3[0] = (int)t0 ^ w1[0];
            w3[1] = (int)t1 ^ w1[1];
            w3[2] = (int)t2 ^ w1[2];
            w3[3] = (int)t3 ^ w1[3];

            Gsrk(w0, w1, 19, rk, j);
            j += 4;
            Gsrk(w1, w2, 19, rk, j);
            j += 4;
            Gsrk(w2, w3, 19, rk, j);
            j += 4;
            Gsrk(w3, w0, 19, rk, j);
            j += 4;
            Gsrk(w0, w1, 31, rk, j);
            j += 4;
            Gsrk(w1, w2, 31, rk, j);
            j += 4;
            Gsrk(w2, w3, 31, rk, j);
            j += 4;
            Gsrk(w3, w0, 31, rk, j);
            j += 4;
            Gsrk(w0, w1, 67, rk, j);
            j += 4;
            Gsrk(w1, w2, 67, rk, j);
            j += 4;
            Gsrk(w2, w3, 67, rk, j);
            j += 4;
            Gsrk(w3, w0, 67, rk, j);
            j += 4;
            Gsrk(w0, w1, 97, rk, j);
            j += 4;
            if(keyBits > 128) {
                Gsrk(w1, w2, 97, rk, j);
                j += 4;
                Gsrk(w2, w3, 97, rk, j);
                j += 4;
            }
            if(keyBits > 192) {
                Gsrk(w3, w0, 97, rk, j);
                j += 4;
                Gsrk(w0, w1, 109, rk, j);
            }
        }

        private static void DoDecKeySetup(int[] rk, int keyBits) {
            var a = 0;
            var t = new int[4];

            var z = 32 + keyBits / 8;
            SwapBlocks(rk, 0, z);
            a += 4;
            z -= 4;

            for(; a < z; a += 4, z -= 4)
                SwapAndDiffuse(rk, a, z, t);

            Diff(rk, a, t, 0);
            rk[a] = t[0];
            rk[a + 1] = t[1];
            rk[a + 2] = t[2];
            rk[a + 3] = t[3];
        }

        private static int ToInt(byte b0, byte b1, byte b2, byte b3) {
            return (b0 & 0xff) << 24 ^ (b1 & 0xff) << 16 ^ (b2 & 0xff) << 8 ^ b3 & 0xff;
        }

        private static long M(long t) {
            return 0x00010101 * ((t >> 24) & 0xff) ^ 0x01000101 * ((t >> 16) & 0xff) ^ 0x01010001 * ((t >> 8) & 0xff) ^
                   0x01010100 * (t & 0xff);
        }

        private static long Badc(long t) {
            return ((t << 8) & 0xff00ff00) ^ ((t >> 8) & 0x00ff00ff);
        }

        private static long Cdab(long t) {
            return ((t << 16) & 0xffff0000) ^ ((t >> 16) & 0x0000ffff);
        }

        private static long Dcba(long t) {
            return (t & 0x000000ff) << 24 ^ (t & 0x0000ff00) << 8 ^ (t & 0x00ff0000) >> 8 ^ (t & 0xff000000) >> 24;
        }

        private static void Gsrk(int[] x, int[] y, int rot, int[] rk, int offset) {
            int q = 4 - (rot / 32), r = rot % 32, s = 32 - r;

            rk[offset] = x[0] ^ y[(q) % 4] >> r ^ y[(q + 3) % 4] << s;
            rk[offset + 1] = x[1] ^ y[(q + 1) % 4] >> r ^ y[(q) % 4] << s;
            rk[offset + 2] = x[2] ^ y[(q + 2) % 4] >> r ^ y[(q + 1) % 4] << s;
            rk[offset + 3] = x[3] ^ y[(q + 3) % 4] >> r ^ y[(q + 2) % 4] << s;
        }

        private static void Diff(int[] i, int offset1, int[] o, int offset2) {
            var t0 = M(i[offset1]);
            var t1 = M(i[offset1 + 1]);
            var t2 = M(i[offset1 + 2]);
            var t3 = M(i[offset1 + 3]);

            t1 ^= t2;
            t2 ^= t3;
            t0 ^= t1;
            t3 ^= t1;
            t2 ^= t0;
            t1 ^= t2;
            t1 = Badc(t1);
            t2 = Cdab(t2);
            t3 = Dcba(t3);
            t1 ^= t2;
            t2 ^= t3;
            t0 ^= t1;
            t3 ^= t1;
            t2 ^= t0;
            t1 ^= t2;
            o[offset2] = (int)t0;
            o[offset2 + 1] = (int)t1;
            o[offset2 + 2] = (int)t2;
            o[offset2 + 3] = (int)t3;
        }

        private static void SwapBlocks(int[] arr, int offset1, int offset2) {
            for(var i = 0; i < 4; i++) {
                var t = arr[offset1 + i];
                arr[offset1 + i] = arr[offset2 + i];
                arr[offset2 + i] = t;
            }
        }

        private static void SwapAndDiffuse(int[] arr, int offset1, int offset2, int[] tmp) {
            Diff(arr, offset1, tmp, 0);
            Diff(arr, offset2, arr, offset1);

            arr[offset2] = tmp[0];
            arr[offset2 + 1] = tmp[1];
            arr[offset2 + 2] = tmp[2];
            arr[offset2 + 3] = tmp[3];
        }

        private static void AssertValidKeySize(int keySize) {
            if(keySize != 128 && keySize != 192 && keySize != DefaultAriaKeySize) {
                throw new InvalidOperationException(string.Format(InvalidKeySizeMessage, keySize));
            }
        }
    }
}