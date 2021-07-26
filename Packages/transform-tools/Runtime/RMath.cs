using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace REXTools.TransformTools
{
    public enum Sign { Neutral, Positive, Negative }
    public enum AverageType { Least, Greatest, Mean, Median, Mode, Range, MidRange }

    public static class RMath
    {
        public static bool SignGreater(this float a, float b)
        {
            if (a >= 0)
            {
                return a > b;
            }
            else
            {
                return a < b;
            }
        }
        public static bool SignLess(this float a, float b)
        {
            if (a >= 0)
            {
                return a < b;
            }
            else
            {
                return a > b;
            }
        }


        public static float SignZeroed(this float f)
        {
            if (f > 0f)
            {
                return 1f;
            }
            else if (f < 0f)
            {
                return -1f;
            }
            else
            {
                return 0f;
            }
        }

        public static int SignZeroedToInt(this float f)
        {
            if (f > 0)
            {
                return 1;
            }
            else if (f < 0)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }


        //float ceil/floor
        public static float SignCeil(this float f, bool includeZero = false)
        {
            if (f < 0)
            {
                return Mathf.Floor(f);
            }
            else if (f > 0)
            {
                return Mathf.Ceil(f);
            }
            else
            {
                if (includeZero)
                {
                    return 0f;
                }
                else
                {
                    return 1f;
                }
            }
        }
        public static float SignFloor(this float f/*, bool includeZero = false*/)
        {
            if (f < 0)
            {
                return Mathf.Ceil(f);
            }
            else if (f > 0)
            {
                return Mathf.Floor(f);
            }
            else
            {
                return 0f;

                //if (includeZero)
                //{
                //    return 0f;
                //}
                //else
                //{

                //}
            }
        }

        //integer ceil/floor
        public static int SignCeilToInt(this float f, bool includeZero = false)
        {
            if (f < 0)
            {
                return Mathf.FloorToInt(f);
            }
            else if (f > 0)
            {
                return Mathf.CeilToInt(f);
            }
            else
            {
                if (includeZero)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
        }
        public static int SignFloorToInt(this float f/*, bool includeZero = false*/)
        {
            if (f < 0)
            {
                return Mathf.CeilToInt(f);
            }
            else if (f > 0)
            {
                return Mathf.FloorToInt(f);
            }
            else
            {
                return 0;
            }
        }



        private static float CustomRound(this float f, System.Func<float, float> func, float increment = 1f, float offset = 0f)
        {
            return (func((f - offset) / increment) * increment) + offset;
        }

        public static float CustomRound(this float f, float increment = 1f, float offset = 0f)
        {
            return f.CustomRound((n) => Mathf.Round(n), increment, offset);
        }
        public static float CustomFloor(this float f, float increment = 1f, float offset = 0f)
        {
            return f.CustomRound((n) => Mathf.Floor(n), increment, offset);
        }
        public static float CustomCeil(this float f, float increment = 1f, float offset = 0f)
        {
            return f.CustomRound((n) => Mathf.Ceil(n), increment, offset);
        }
        public static float CustomSignFloor(this float f, float increment = 1f, float offset = 0f)
        {
            return f.CustomRound((n) => RMath.SignFloor(n), increment, offset);
        }
        public static float CustomSignCeil(this float f, float increment = 1f, float offset = 0f)
        {
            return f.CustomRound((n) => RMath.SignCeil(n), increment, offset);
        }

        public static int CustomRound(this int i, int increment = 1, int offset = 0)
        {
            return (Mathf.RoundToInt((i - offset) / increment) * increment) + offset;
        }



        public static bool Even(this int f)
        {
            return f % 2 == 0;
        }
        public static bool Odd(this int f)
        {
            return f % 2 == 1;
        }

        public static float ClampMin(float value, float min)
        {
            return Mathf.Clamp(value, min, float.MaxValue);
        }
        public static float ClampMax(float value, float max)
        {
            return Mathf.Clamp(value, float.MinValue, max);
        }
        public static int ClampMin(int value, int min)
        {
            return Mathf.Clamp(value, min, int.MaxValue);
        }
        public static int ClampMax(int value, int max)
        {
            return Mathf.Clamp(value, int.MinValue, max);
        }


        public static float GetAverage<T>(this List<T> list, System.Func<T, float> property, AverageType averageType = AverageType.Mean)
        {
            if (averageType == AverageType.Least)
            {
                return list.Min(property);
            }
            else if (averageType == AverageType.Greatest)
            {
                return list.Max(property);
            }
            else if (averageType == AverageType.Mean)
            {
                return list.Average(property);
            }
            else if (averageType == AverageType.Median)
            {
                list = list.OrderBy(property).ToList<T>();

                if (RMath.Odd(list.Count))
                {
                    //++++++++++++NEEDS TO BE FIXED
                    //USE System.Func on induvidual value to get property
                    return property(list[(int)(Mathf.Ceil(list.Count / 2f) - 1)]);
                }
                else if (RMath.Even(list.Count))
                {
                    return (
                        property(list[(list.Count / 2) - 1]) +
                        property(list[(list.Count / 2) + 1 - 1])
                        ) / 2f;
                }
            }
            else if (averageType == AverageType.Mode)
            {
                Dictionary<float, int> ocurrances = new Dictionary<float, int>();

                foreach (T i in list)
                {
                    if (ocurrances.ContainsKey(property(i)))
                    {
                        ocurrances[property(i)] = ocurrances[property(i)] + 1;
                    }
                    else
                    {
                        ocurrances[property(i)] = 1;
                    }
                }

                return list.Max((f) => ocurrances[property(f)]);
            }
            else if (averageType == AverageType.Range)
            {
                return list.Max(property) - list.Min(property);
            }
            else if (averageType == AverageType.MidRange)
            {
                return (list.Min(property) + list.Max(property)) / 2f;
            }

            return 1f;
        }
        public static float GetAverage<T>(this T[] list, System.Func<T, float> property, AverageType averageType = AverageType.Mean)
        {
            List<T> listConverted = list.ToList<T>();

            return listConverted.GetAverage<T>(property, averageType);
        }

        public static float GetAverage(this List<float> list, AverageType averageType = AverageType.Mean)
        {
            return list.GetAverage(num => num, averageType);
        }
        public static float GetAverage(this float[] list, AverageType averageType = AverageType.Mean)
        {
            return list.ToList().GetAverage(num => num, averageType);
        }


        //Enum extension methods
        public static Sign ToSign(this float f)
        {
            if (f > 0f)
            {
                return Sign.Positive;
            }
            else if (f < 0f)
            {
                return Sign.Negative;
            }
            else if (f == 0f)
            {
                return Sign.Neutral;
            }

            return default;
        }
        public static Sign ToSign(this int f)
        {
            if (f > 0)
            {
                return Sign.Positive;
            }
            else if (f < 0)
            {
                return Sign.Negative;
            }
            else if (f == 0)
            {
                return Sign.Neutral;
            }

            return default;
        }

        public static int ToInt(this Sign sign)
        {
            if (sign == Sign.Positive)
            {
                return 1;
            }
            else if (sign == Sign.Negative)
            {
                return -1;
            }
            else if (sign == Sign.Neutral)
            {
                return 0;
            }

            return default;
        }
        public static float ToFloat(this Sign sign)
        {
            if (sign == Sign.Positive)
            {
                return 1f;
            }
            else if (sign == Sign.Negative)
            {
                return -1f;
            }
            else if (sign == Sign.Neutral)
            {
                return 0f;
            }

            return default;
        }

        public static Sign Negative(this Sign sign)
        {
            if (sign == Sign.Positive)
            {
                return Sign.Negative;
            }
            else if (sign == Sign.Negative)
            {
                return Sign.Positive;
            }
            else if (sign == Sign.Neutral)
            {
                return Sign.Neutral;
            }

            return default;
        }


        //Geometry methods
        public static bool Intersects(float minA, float maxA, float minB, float maxB, bool trim = false, bool allowance = false)
        {
            return
                (
                    Intersects(minA, minB, maxB, trim, allowance) ||
                    Intersects(maxA, minB, maxB, trim, allowance)
                ) 
                ||
                (
                    Intersects(minB, minA, maxA, trim, allowance) ||
                    Intersects(maxB, minA, maxA, trim, allowance)
                );
        }
        public static bool Adjacent(float minA, float maxA, float minB, float maxB, bool allowance = false)
        {
            return
                !Intersects(minA, maxA, minB, maxB, true, allowance) && !Contains(minA, maxA, minB, maxB, allowance) && 
                (
                    Adjacent(minA, minB, maxB, allowance) ||
                    Adjacent(maxA, minB, maxB, allowance)
                );
        }
        public static bool Contains(float minA, float maxA, float minB, float maxB, bool trim = false, bool allowance = false)
        {
            return
                Contains(minA, minB, maxB, trim, allowance) &&
                Contains(maxA, minB, maxB, trim, allowance);
        }

        public static bool Intersects(float a, float minB, float maxB, bool trim = false, bool allowance = false)
        {
            return Contains(a, minB, maxB, trim, allowance);
        }
        public static bool Adjacent(float a, float minB, float maxB, bool allowance = false)
        {
            if (!allowance)
            {
                return
                    a == Mathf.Min(minB, maxB) ||
                    a == Mathf.Max(minB, maxB);
            }
            else
            {
                return
                    (a >= Mathf.Min(minB, maxB) && a <= Mathf.Min(minB, maxB) + Geometry.faceTrim) ||
                    (a <= Mathf.Max(minB, maxB) && a >= Mathf.Max(minB, maxB) - Geometry.faceTrim);
            }
        }
        public static bool Contains(float a, float minB, float maxB, bool trim = false, bool allowance = false)
        {
            if (!allowance)
            {
                if (!trim)
                {
                    return
                        a >= Mathf.Min(minB, maxB) && a <= Mathf.Max(minB, maxB);
                }
                else
                {
                    return
                        a > Mathf.Min(minB, maxB) && a < Mathf.Max(minB, maxB);
                }
            }
            else 
            {
                if (!trim)
                {
                    return
                        a >= Mathf.Min(minB, maxB) && a <= Mathf.Max(minB, maxB);
                }
                else
                {
                    return
                        a >= (Mathf.Min(minB, maxB) + Geometry.faceTrim) && a <= (Mathf.Max(minB, maxB) - Geometry.faceTrim);
                }
            }
        }
    }
}