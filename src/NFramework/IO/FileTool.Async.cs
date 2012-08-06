using System.Text;
using System.Threading.Tasks;
using NSoft.NFramework.Parallelism.Tools;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.IO {

    public static partial class FileTool {

        /// <summary>
        /// 비동기 방식으로 <paramref name="text"/>를 파일(<paramref name="filepath"/>)에 저장합니다.
        /// 완료되었는지는 <see cref="Task.IsCompleted"/> 를 확인하세요. 
        /// 대기 시에는 ((IAsyncResult)task).AsyncWaitHandle.WaitOne() 을 사용하세요.
        /// </summary>
        /// <param name="filepath">저장할 파일의 전체경로</param>
        /// <param name="text">저장할 파일 내용</param>
        /// <param name="encoding">파일 내용의 인코딩 방식</param>
        /// <param name="overwrite">겹쳐쓰기 여부</param>
        /// <returns></returns>
        public static Task SaveTask(string filepath, string text, Encoding encoding = null, bool overwrite = false) {
            if(IsDebugEnabled)
                log.Debug("지정한 정보를 파일에 비동기 방식으로 저장합니다... filepath=[{0}], encoding=[{1}], overwrite=[{2}], text=[{3}]",
                          filepath, encoding, overwrite, text.EllipsisChar(50));

            var doNotSave = (overwrite == false && FileExists(filepath));

            if(doNotSave) {
                if(log.IsWarnEnabled)
                    log.Warn("파일 겹쳐쓰기를 허용하지 않았고, 파일이 존재하므로, 파일 저장을 수행하지 않습니다!!! filepath=[{0}]", filepath);

                // Dummy Task 를 반환합니다.
                //
                Task.Factory.StartNewDelayed(1);
            }

            return FileAsync.WriteAllText(filepath, text, encoding);
        }

        ///<summary>
        /// 비동기 방식으로 파일의 전체 내용을 읽어 문자열로 반환합니다.
        /// </summary>
        /// <param name="filepath">읽을 파일의 전체 경로</param>
        /// <param name="encoding">인코딩 방식</param>
        /// <returns>파일 내용</returns>
        public static Task<string> ReadTextTask(string filepath, Encoding encoding = null) {
            if(IsDebugEnabled)
                log.Debug("파일 내용을 비동기 방식으로 문자열로 모두 읽어옵니다... filepath=[{0}], encoding=[{1}]", filepath, encoding);

            return FileAsync.ReadAllText(filepath, encoding);
        }
    }
}