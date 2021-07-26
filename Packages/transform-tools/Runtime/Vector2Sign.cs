using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace REXTools.TransformTools
{
    [System.Serializable]
    public class Vector2Sign : Vector2T<Sign>
    {
        public Vector2 direction
        {
            get
            {
                return new Vector2(x.ToFloat(), y.ToFloat());
            }
        }
        public Vector2 normalDirection
        {
            get
            {
                return direction.normalized;
            }
        }
        public UnityEngine.Vector2Int directionInt
        {
            get
            {
                return direction.RoundToInt();
            }
        }

        public Vector2Sign invert
        {
            get
            {
                return new Vector2Sign(x.Negative(), y.Negative());
            }
        }


        public Vector2Sign Rotate(int euler)
        {
            return new Vector2Sign(
                (
                    Quaternion.Euler(Vector3.zero * euler * 90f) *
                    Quaternion.LookRotation(this.direction)
                ) *
                Vector3.forward
            );
        }
        public Vector2Sign Mirror(Axis axis)
        {
            return this.SetAxis(axis, this.GetAxis(axis).Negative()) as Vector2Sign;
        }

        //conversion methods
        public UnityEngine.Vector2Int ToInt()
        {
            return new UnityEngine.Vector2Int(x.ToInt(), y.ToInt());
        }
        public Vector2 ToFloat()
        {
            return new Vector2(x.ToFloat(), y.ToFloat());
        }

        public Vector2Sign Negative()
        {
            return new Vector2Sign(x.Negative(), y.Negative());
        }

        public Vector2Sign(Vector2T<Sign> vector)
        {
            this.x = vector.x;
            this.y = vector.y;
        }

        public Vector2Sign(Sign x = Sign.Neutral, Sign y = Sign.Neutral)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2Sign(Vector3Sign vector3Sign)
        {
            this.x = vector3Sign.x;
            this.y = vector3Sign.y;
        }

        public Vector2Sign(Vector2 vector)
        {
            this.x = vector.x.ToSign();
            this.y = vector.y.ToSign();
        }


        public static readonly Vector2Sign zero = new Vector2Sign(Sign.Neutral, Sign.Neutral);
        public static readonly Vector2Sign one = new Vector2Sign(Sign.Positive, Sign.Positive);

        public static readonly Vector2Sign right = new Vector2Sign(Sign.Positive, Sign.Neutral);
        public static readonly Vector2Sign up = new Vector2Sign(Sign.Neutral, Sign.Positive);
        public static readonly Vector2Sign left = new Vector2Sign(Sign.Negative, Sign.Neutral);
        public static readonly Vector2Sign down = new Vector2Sign(Sign.Neutral, Sign.Negative);
    }
}