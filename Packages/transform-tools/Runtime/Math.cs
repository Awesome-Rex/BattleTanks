using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools.TransformTools
{
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



        public static float CustomRound(this float f, float increment, float offset)
        {
            return (Mathf.Round((f - offset) / increment) * increment) + offset;
        }
        public static float CustomRound(this float f, float increment)
        {
            return CustomRound(f, increment, 0f);
        }

        public static bool Even (this int f)
        {
            return f % 2 == 0;
        }
        public static bool Odd(this int f)
        {
            return f % 2 == 1;
        }
    }
}