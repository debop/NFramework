namespace NSoft.NFramework.Web
{
    /// <summary>
    /// 선택한 항목을 어떤 UI 컨트롤에 출력할 것인가?
    /// </summary>
    public enum SelectionItemDisplayMode
    {
        /// <summary>
        /// 출력 컨트롤을 숨긴다.
        /// </summary>
        Hide,

        /// <summary>
        /// TextBox로 출력한다.
        /// </summary>
        TextBox,

        /// <summary>
        /// 라벨로 출력한다.
        /// </summary>
        Label,

        /// <summary>
        /// Div 에 출력한다.
        /// </summary>
        Div
    }
}