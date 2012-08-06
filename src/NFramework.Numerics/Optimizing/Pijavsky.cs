using System;

namespace NSoft.NFramework.Numerics.Optimizing {
    public class Pijavsky {
        // Fields
        private double result;

        // Methods
        public Pijavsky(Func<double, double> func, double a, double b, double l, int n) {
            double[] numArray = new double[0];
            double[] numArray2 = new double[0];
            double[] numArray3 = new double[0];
            int index = 0;
            int num2 = 0;
            double num3 = 0.0;
            double num4 = 0.0;
            int num5 = 0;
            double num6 = 0.0;
            double num7 = 0.0;
            numArray = new double[(n + 1) + 1];
            numArray2 = new double[(n + 1) + 1];
            numArray3 = new double[(n + 1) + 1];
            numArray[0] = a;
            numArray[1] = b;
            numArray2[0] = func(a);
            numArray2[1] = func(b);
            for(index = 2; index <= (n + 1); index++) {
                num2 = 1;
                while(num2 <= (index - 1)) {
                    numArray3[num2] = ((l / 2.0) * (numArray[num2] - numArray[num2 - 1])) -
                                      (0.5 * (numArray2[num2] + numArray2[num2 - 1]));
                    num2++;
                }
                num4 = numArray3[1];
                num5 = 1;
                num2 = 2;
                while(num2 <= (index - 1)) {
                    if(numArray3[num2] > num4) {
                        num5 = num2;
                        num4 = numArray3[num2];
                    }
                    num2++;
                }
                numArray[index] = (0.5 * (numArray[num5] + numArray[num5 - 1])) - ((0.5 / l) * (numArray2[num5] - numArray2[num5 - 1]));
                numArray2[index] = func(numArray[index]);
                for(num2 = index; num2 >= 2; num2--) {
                    if(numArray[num2] < numArray[num2 - 1]) {
                        num3 = numArray[num2];
                        numArray[num2] = numArray[num2 - 1];
                        numArray[num2 - 1] = num3;
                        num3 = numArray2[num2];
                        numArray2[num2] = numArray2[num2 - 1];
                        numArray2[num2 - 1] = num3;
                    }
                    else {
                        break;
                    }
                }
            }
            num6 = numArray[0];
            num7 = numArray2[0];
            for(index = 1; index <= (n + 1); index++) {
                if(numArray2[index] < num7) {
                    num7 = numArray2[index];
                    num6 = numArray[index];
                }
            }
            result = num6;
        }

        public double GetSolution() {
            return result;
        }
    }
}