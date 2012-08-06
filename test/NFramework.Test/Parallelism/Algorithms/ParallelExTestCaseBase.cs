namespace NSoft.NFramework.Parallelism.Algorithms {
    /// <summary>
    /// <see cref="ParallelTool"/> 를 테스트하기 위한 단위 테스트용 기본 클래스
    /// </summary>
    [Microsoft.Silverlight.Testing.Tag("Parallel")]
    public abstract class ParallelToolTestCaseBase : ParallelismFixtureBase {
        protected const int TestCount = 10;
        protected const int MaxTestCount = 100;
    }
}