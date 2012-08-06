using System;

namespace NSoft.NFramework.Numerics.Optimizing {
    /// <summary>
    /// 함수의 지정된 구간에서의 최소 값을 가지는 X 를 Golden Section 방식으로 구한다.
    /// </summary>
    public class GoldenSection {
        // Fields
        private readonly double resultA;
        private readonly double resultB;

        // Methods
        public GoldenSection(Func<double, double> func, double a, double b, int n) {
            func.ShouldNotBeNull("func");

            int num = 0;
            double num2 = 0.0;
            double num3 = 0.0;
            double x = 0.0;
            double num5 = 0.0;
            double num6 = 0.0;
            double num7 = 0.0;

            num2 = (3.0 - Math.Sqrt(5.0)) / 2.0;
            num3 = (Math.Sqrt(5.0) - 1.0) / 2.0;
            x = a + (num2 * (b - a));
            num5 = a + (num3 * (b - a));
            num6 = func(x);
            num7 = func(num5);
            for(num = 1; num <= n; num++) {
                if(num6 <= num7) {
                    b = num5;
                    num5 = x;
                    num7 = num6;
                    x = a + (num2 * (b - a));
                    num6 = func(x);
                }
                else {
                    a = x;
                    x = num5;
                    num6 = num7;
                    num5 = a + (num3 * (b - a));
                    num7 = func(num5);
                }
            }
            resultA = a;
            resultB = b;
        }

        public double GetSolutionA() {
            return resultA;
        }

        public double GetSolutionB() {
            return resultB;
        }
    }
}