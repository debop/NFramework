using System;
using System.IO;

namespace NSoft.NFramework.Parallelism.DataStructures {
    /// <summary>
    /// Thread-safe 한 Stream의 기본 클래스입니다.
    /// </summary>
    [Serializable]
    public abstract class AbstractStream : Stream {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private const string NeedOverrideMessage = @"이 속성은 상속받은 클래스에서 재정의를 해야 합니다.";

        /// <summary>
        /// 파생 클래스에서 재정의될 때 현재 스트림이 읽기를 지원하는지 여부를 나타내는 값을 가져옵니다.
        /// </summary>
        /// <returns>
        /// 스트림이 읽기를 지원하면 true이고, 그렇지 않으면 false입니다.
        /// </returns>
        public override bool CanRead {
            get { return false; }
        }

        /// <summary>
        /// 파생 클래스에서 재정의될 때 현재 스트림이 쓰기를 지원하는지 여부를 나타내는 값을 가져옵니다.
        /// </summary>
        /// <returns>
        /// 스트림이 쓰기를 지원하면 true이고, 그렇지 않으면 false입니다.
        /// </returns>
        public override bool CanWrite {
            get { return false; }
        }

        /// <summary>
        /// 파생 클래스에서 재정의될 때 현재 스트림이 검색을 지원하는지 여부를 나타내는 값을 가져옵니다.
        /// </summary>
        /// <returns>
        /// 스트림이 검색을 지원하면 true이고, 그렇지 않으면 false입니다.
        /// </returns>
        public override bool CanSeek {
            get { return false; }
        }

        /// <summary>
        /// 파생 클래스에서 재정의될 때 이 스트림에 대해 모든 버퍼를 지우고 버퍼링된 데이터가 내부 장치에 쓰여지도록 합니다.
        /// </summary>
        /// <exception cref="T:System.IO.IOException">I/O 오류가 발생하는 경우 </exception>
        public override void Flush() {}

        /// <summary>
        /// 파생 클래스에서 재정의된 경우 스트림 바이트의 길이를 가져옵니다.
        /// </summary>
        /// <returns>
        /// 스트림 길이(바이트)를 나타내는 long 값입니다.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">Stream에서 파생된 클래스가 검색을 지원하지 않는 경우 </exception>
        /// <exception cref="T:System.ObjectDisposedException">스트림이 닫힌 후 메서드가 호출된 경우 </exception>
        public override long Length {
            get { throw new NotSupportedException(NeedOverrideMessage); }
        }

        /// <summary>
        /// 파생 클래스에서 재정의되면 현재 스트림 내의 위치를 가져오거나 설정합니다.
        /// </summary>
        /// <returns>
        /// 스트림 내의 현재 위치입니다.
        /// </returns>
        /// <exception cref="T:System.IO.IOException">I/O 오류가 발생하는 경우 </exception>
        /// <exception cref="T:System.NotSupportedException">스트림이 검색을 지원하지 않는 경우 </exception>
        /// <exception cref="T:System.ObjectDisposedException">스트림이 닫힌 후 메서드가 호출된 경우 </exception>
        public override long Position {
            get { throw new NotSupportedException(NeedOverrideMessage); }
            set { throw new NotSupportedException(NeedOverrideMessage); }
        }

        /// <summary>
        /// 파생 클래스에서 재정의될 때 현재 스트림에서 바이트의 시퀀스를 읽고 읽은 바이트 수만큼 스트림 내에서 앞으로 이동합니다.
        /// </summary>
        /// <returns>
        /// 버퍼로 읽어온 총 바이트 수입니다. 이 바이트 수는 현재 바이트가 충분하지 않은 경우 요청된 바이트 수보다 작을 수 있으며 스트림의 끝에 도달하면 0이 됩니다.
        /// </returns>
        /// <param name="buffer">바이트 배열입니다. 이 메서드가 반환될 때 버퍼에는 지정된 바이트 배열의 값이 <paramref name="offset"/> 및 (<paramref name="offset"/> + <paramref name="count"/> - 1) 사이에서 현재 소스로부터 읽어온 바이트로 교체된 상태로 포함됩니다. </param>
        /// <param name="offset">현재 스트림에서 읽은 데이터를 저장하기 시작하는 <paramref name="buffer"/>의 바이트 오프셋(0부터 시작)입니다. </param>
        /// <param name="count">현재 스트림에서 읽을 최대 바이트 수입니다. </param>
        /// <exception cref="T:System.ArgumentException"><paramref name="offset"/>와 <paramref name="count"/>의 합계가 버퍼 길이보다 큰 경우 </exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="buffer"/>가 null인 경우 </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="offset"/> 또는 <paramref name="count"/>가 음수인 경우 </exception>
        /// <exception cref="T:System.IO.IOException">I/O 오류가 발생하는 경우 </exception>
        /// <exception cref="T:System.NotSupportedException">스트림이 읽기를 지원하지 않는 경우 </exception>
        /// <exception cref="T:System.ObjectDisposedException">스트림이 닫힌 후 메서드가 호출된 경우 </exception>
        public override int Read(byte[] buffer, int offset, int count) {
            throw new NotSupportedException(NeedOverrideMessage);
        }

        /// <summary>
        /// 파생 클래스를 재정의될 때 현재 스트림 내의 위치를 설정합니다.
        /// </summary>
        /// <returns>
        /// 현재 스트림 내의 새 위치입니다.
        /// </returns>
        /// <param name="offset"><paramref name="origin"/> 매개 변수에 상대적인 바이트 오프셋입니다. </param>
        /// <param name="origin">새 위치를 가져오는 데 사용되는 참조 위치를 나타내는 <see cref="T:System.IO.SeekOrigin"/> 형식의 값입니다. </param>
        /// <exception cref="T:System.IO.IOException">I/O 오류가 발생하는 경우 </exception>
        /// <exception cref="T:System.NotSupportedException">예를 들어, 스트림이 파이프 또는 콘솔 출력에서 생성되는 경우 스트림은 검색을 지원하지 않습니다. </exception>
        /// <exception cref="T:System.ObjectDisposedException">스트림이 닫힌 후 메서드가 호출된 경우 </exception>
        public override long Seek(long offset, SeekOrigin origin) {
            throw new NotSupportedException(NeedOverrideMessage);
        }

        /// <summary>
        /// 파생 클래스에 재정의될 때 현재 스트림의 길이를 설정합니다.
        /// </summary>
        /// <param name="value">원하는 현재 스트림의 길이(바이트)입니다. </param>
        /// <exception cref="T:System.IO.IOException">I/O 오류가 발생하는 경우 </exception>
        /// <exception cref="T:System.NotSupportedException">예를 들어, 스트림이 파이프 또는 콘솔 출력에서 생성되는 경우처럼 스트림이 쓰기와 검색을 모두 지원하지 않는 경우 </exception>
        /// <exception cref="T:System.ObjectDisposedException">스트림이 닫힌 후 메서드가 호출된 경우 </exception>
        public override void SetLength(long value) {
            throw new NotSupportedException(NeedOverrideMessage);
        }

        /// <summary>
        /// 파생 클래스를 재정의될 때 현재 스트림에 바이트의 시퀀스를 쓰고 쓰여진 바이트 수만큼 이 스트림 내에서 앞으로 이동합니다.
        /// </summary>
        /// <param name="buffer">바이트 배열입니다. 이 메서드는 <paramref name="buffer"/>의 <paramref name="count"/> 바이트를 현재 스트림으로 복사합니다. </param>
        /// <param name="offset">현재 스트림으로 바이트를 복사하기 시작할 <paramref name="buffer"/>의 바이트 오프셋(0부터 시작)입니다. </param>
        /// <param name="count">현재 스트림에 쓰는 바이트 수입니다. </param>
        /// <exception cref="T:System.ArgumentException"><paramref name="offset"/>과 <paramref name="count"/>의 합계가 버퍼 길이보다 큰 경우 </exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="buffer"/>가 null인 경우 </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="offset"/> 또는 <paramref name="count"/>가 음수인 경우 </exception>
        /// <exception cref="T:System.IO.IOException">I/O 오류가 발생하는 경우 </exception>
        /// <exception cref="T:System.NotSupportedException">스트림이 쓰기를 지원하지 않는 경우 </exception>
        /// <exception cref="T:System.ObjectDisposedException">스트림이 닫힌 후 메서드가 호출된 경우 </exception>
        public override void Write(byte[] buffer, int offset, int count) {
            throw new NotSupportedException(NeedOverrideMessage);
        }
    }
}