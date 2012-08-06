namespace NSoft.NFramework.WindowsSystem {
    /// <summary>
    /// 레지스트리 값 읽기 Interface
    /// </summary>
    public interface IRegistryReader {
        ///<summary>
        /// 레지스트리의 서브키의 name에 해당하는 값을 가져온다. 
        ///</summary>
        ///<param name="subKey">레지스트리 서브키찾</param>
        ///<param name="name">찾고자하는 레지스트리 명</param>
        ///<returns></returns>
        string GetValue(string subKey, string name);
    }
}