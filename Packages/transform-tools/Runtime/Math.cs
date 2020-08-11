using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools.TransformTools
{
    public class RMath
    {
        public static bool SignGreater(float a, float b)
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
        public static bool SignLess(float a, float b)
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

        public static float SignZeroed(float f)
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

        public static float SignCeil(float f, bool includeZero = false)
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
        public static float SignFloor(float f/*, bool includeZero = false*/)
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

        public static float CustomRound(float f, float increment, float offset)
        {
            return (Mathf.Round((f - offset) / increment) * increment) + offset;
        }
        public static float CustomRound(float f, float increment)
        {
            return CustomRound(f, increment, 0f);
        }
    }
}