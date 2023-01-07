using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incapsulation.RationalNumbers
{
    public struct Rational
    {
        public float Numerator { get; }
        public float Denominator { get; }
        public bool IsNan;
        
        private static bool IsNumDenomWrong(float numerator, float denominator)
        {
            return denominator == 0 || 
                   float.IsNegativeInfinity(denominator) ||
                   float.IsPositiveInfinity(denominator) ||
                   float.IsNegativeInfinity(numerator) ||
                   float.IsPositiveInfinity(numerator);
        }
        
        private static float Nod(Rational rational)
        {
            var denominator = rational.Denominator;
            var numerator = rational.Numerator;
            if (denominator == 0 || numerator == 0) return 1;
            while (numerator != 0)
            {
                var t = numerator;
                numerator = denominator % numerator;
                denominator = t;
            }
            return denominator;
        }

        public Rational(float numerator, float denominator = 1)
        {
            Numerator = numerator;
            Denominator = denominator;
            if (Numerator == 0 && Denominator != 0) 
                Denominator = 1;
            IsNan = IsNumDenomWrong(Numerator,Denominator);

            if (Denominator != 0 && Numerator % 2 == 0 && Denominator % 2 == 0)
            {
                var divider = Nod(this);
                Denominator /= divider;
                Numerator /= divider;
            }
            while (Denominator != 0 && Numerator % 5 == 0 && Denominator % 5 == 0)
            {
                Denominator /= 5;
                Numerator /= 5;
            }
         
            if (Denominator < 0 || Denominator < 0 && Numerator < 0)
            {
                Denominator *= -1;
                Numerator *= -1;
            }
        }
        // var a = new Rational(1,2);
        // var b = new Rational(1,2);
        // var c = a.Sum(a,b);

        public static Rational operator +(Rational first, Rational second)
        {
            var newNumerator = first.Numerator * second.Denominator + second.Numerator * first.Denominator;
            var newDenominator = first.Denominator * second.Denominator;
            return new Rational(newNumerator, newDenominator);
        }
        
        public static Rational operator -(Rational first, Rational second)
        {
            var newNumerator = first.Numerator * second.Denominator - second.Numerator * first.Denominator;
            var newDenominator = first.Denominator * second.Denominator;
            return new Rational(newNumerator, newDenominator);
        }
        
        public static Rational operator *(Rational first, Rational second)
        {
            var newNumerator = first.Numerator * second.Numerator;
            var newDenominator = first.Denominator * second.Denominator;
            return new Rational(newNumerator, newDenominator);
        }
        
        public static Rational operator /(Rational first, Rational second)
        {
            // 1/4 / -1/2
            // -1/2
            var newNumerator = first.Numerator * second.Denominator;
            var newDenominator = first.Denominator * second.Numerator;
            var result = new Rational(newNumerator, newDenominator);
            if (first.IsNan || second.IsNan)
                result.IsNan = true;
            return result;
        }
        
        // var old = new Rational();
        // implicit - неявный: double new = old;
        // explicit - явный: double new = (double)old;
        
        public static implicit operator double(Rational rational)
        {
            if (rational.IsNan) return double.NaN;
            return rational.Numerator / rational.Denominator;
        }

        public static implicit operator Rational(int number)
        {
            return new Rational(number);
        }

        public static explicit operator int(Rational rational)
        {
            if (rational.Numerator % rational.Denominator == 0) 
                return (int)(rational.Numerator / rational.Denominator);
            throw new Exception("Can't convert");
        }   
    }
}
