using System;
using System.Threading.Tasks;

class Program
{
    [STAThread]
    static void Main()
    {
        EmptyBodyOverhead.Run();
        //EmptyBodyGcs.Run();
        //ReadAsync.Run();
        //DictionaryCaching.Run();
        //CapturingContext.Run();
        //FlowingContext.Run();
    }
}