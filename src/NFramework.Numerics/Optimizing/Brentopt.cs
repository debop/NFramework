using System;

namespace NSoft.NFramework.Numerics.Optimizing {
    public class Brentopt {
        // Fields
        private double result;
        private double resultMin;

        // Methods
        public Brentopt(Func<double, double> f, double a, double b, double epsilon) {
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            double num5 = 0.0;
            double num6 = 0.0;
            double num7 = 0.0;
            double num8 = 0.0;
            double num9 = 0.0;
            double num10 = 0.0;
            int num11 = 0;
            double num12 = 0.0;
            double num13 = 0.0;
            double num14 = 0.0;
            double x = 0.0;
            double num16 = 0.0;
            double num17 = 0.0;
            double num18 = 0.0;
            double num19 = 0.0;
            double num20 = 0.0;
            num20 = 0.381966;
            num3 = 0.5 * (a + b);
            if(a < b) {
                num = a;
            }
            else {
                num = b;
            }
            if(a > b) {
                num2 = a;
            }
            else {
                num2 = b;
            }
            num16 = num3;
            num17 = num16;
            num18 = num16;
            num5 = 0.0;
            num10 = f(num18);
            num8 = num10;
            num9 = num10;
            for(num11 = 1; num11 <= 100; num11++) {
                num19 = 0.5 * (num + num2);
                if(Math.Abs((double)(num18 - num19)) <= ((epsilon * 2.0) - (0.5 * (num2 - num)))) {
                    break;
                }
                if(Math.Abs(num5) > epsilon) {
                    num14 = (num18 - num17) * (num10 - num8);
                    num13 = (num18 - num16) * (num10 - num9);
                    num12 = ((num18 - num16) * num13) - ((num18 - num17) * num14);
                    num13 = 2.0 * (num13 - num14);
                    if(num13 > 0.0) {
                        num12 = -num12;
                    }
                    num13 = Math.Abs(num13);
                    num6 = num5;
                    num5 = num4;
                    if(
                        !(((Math.Abs(num12) >= Math.Abs((double)((0.5 * num13) * num6))) | (num12 <= (num13 * (num - num18)))) |
                          (num12 >= (num13 * (num2 - num18))))) {
                        num4 = num12 / num13;
                        x = num18 + num4;
                        if(((x - num) < (epsilon * 2.0)) | ((num2 - x) < (epsilon * 2.0))) {
                            num4 = mysign(epsilon, num19 - num18);
                        }
                    }
                    else {
                        if(num18 >= num19) {
                            num5 = num - num18;
                        }
                        else {
                            num5 = num2 - num18;
                        }
                        num4 = num20 * num5;
                    }
                }
                else {
                    if(num18 >= num19) {
                        num5 = num - num18;
                    }
                    else {
                        num5 = num2 - num18;
                    }
                    num4 = num20 * num5;
                }
                if(Math.Abs(num4) >= epsilon) {
                    x = num18 + num4;
                }
                else {
                    x = num18 + mysign(epsilon, num4);
                }
                num7 = f(x);
                if(num7 <= num10) {
                    if(x >= num18) {
                        num = num18;
                    }
                    else {
                        num2 = num18;
                    }
                    num16 = num17;
                    num8 = num9;
                    num17 = num18;
                    num9 = num10;
                    num18 = x;
                    num10 = num7;
                }
                else {
                    if(x < num18) {
                        num = x;
                    }
                    else {
                        num2 = x;
                    }
                    if((num7 <= num9) || (num17.ApproximateEqual(num18))) {
                        num16 = num17;
                        num8 = num9;
                        num17 = x;
                        num9 = num7;
                    }
                    else if((num7 <= num8) || num16.ApproximateEqual(num18) || num16.ApproximateEqual(2.0)) {
                        num16 = x;
                        num8 = num7;
                    }
                }
            }
            resultMin = num18;
            result = num10;
        }

        public double GetSolution() {
            return resultMin;
        }

        public double GetSolutionFunction() {
            return result;
        }

        private double mysign(double a, double b) {
            if(b > 0.0) {
                return Math.Abs(a);
            }
            return -Math.Abs(a);
        }
    }
}