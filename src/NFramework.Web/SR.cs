namespace NSoft.NFramework.Web
{
    /// <summary>
    /// Internal String Resources
    /// </summary>
    internal static class SR
    {
        internal const string CloseWindowJavascript = @"
if(this.opener) {
    window.top.close();
}
else if(this.parent) {
    window.opener = parent;
    parent.close();
} else {
    window.opener = this;
    this.close();
} ";
    }
}