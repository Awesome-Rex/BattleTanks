using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools.TransformTools
{
    [System.Serializable]
    public class Vector3Sign : Vector3T<Sign>
    {
        public Vector3 direction
        {
            get
            {
                return new Vector3(x.ToFloat(), y.ToFloat(), z.ToFloat());
            }
        }
        public Vector3 normalDirection
        {
            get
            {
                return direction.normalized;
            }
        }
        public UnityEngine.Vector3Int directionInt
        {
            get
            {
                return direction.RoundToInt();
            }
        }

        public Vector3Sign invert
        {
            get
            {
                return new Vector3Sign(x.Negative(), y.Negative(), z.Negative());
            }
        }


        public Vector3Sign Rotate(UnityEngine.Vector3Int eulers, Space space = Space.Self)
        {
            //return (Quaternion.Euler((Vector3)euler * 90f).Add(Quaternion.LookRotation(this.direction), space) * Vector3.forward).ToSign();
            return (Quaternion.LookRotation(this.direction).Add(Quaternion.Euler((Vector3)eulers * 90f), space) * Vector3.forward).Operate((s, a) => RMath.CustomRound(a, Geometry.zeroRound)).ToSign();
            //return Geometry.RotateSign(this, euler);
        }
        public Vector3Sign Mirror(Axis axis)
        {
            return this.SetAxis(axis, this.GetAxis(axis).Negative()) as Vector3Sign;
        }

        public BoxAdjacency adjacency
        {
            get {
                int zeroCount = this.ToList().FindAll(axis => axis == Sign.Neutral).Count; //Optimization

                if (zeroCount == 0)
                {
                    return BoxAdjacency.Object;
                }
                else if (zeroCount == 1)
                {
                    return BoxAdjacency.Face;
                }
                else if (zeroCount == 2)
                {
                    return BoxAdjacency.Edge;
                }
                else if (zeroCount == 3)
                {
                    return BoxAdjacency.Corner;
                }

                return default;
            }
        }

        //conversion methods
        public UnityEngine.Vector3Int ToInt()
        {
            return new UnityEngine.Vector3Int(x.ToInt(), y.ToInt(), z.ToInt());
        }
        public Vector3 ToFloat()
        {
            return new Vector3(x.ToFloat(), y.ToFloat(), z.ToFloat());
        }

        public Vector3Sign Negative()
        {
            return new Vector3Sign(x.Negative(), y.Negative(), z.Negative());
        }

        public Vector3Sign(Vector3T<Sign> vector)
        {
            this.x = vector.x;
            this.y = vector.y;
            this.z = vector.z;
        }

        public Vector3Sign(Sign x = Sign.Neutral, Sign y = Sign.Neutral, Sign z = Sign.Neutral)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3Sign(Vector2Sign vector2Sign, Sign z = Sign.Neutral)
        {
            this.x = vector2Sign.x;
            this.y = vector2Sign.y;
            this.z = z;
        }

        public Vector3Sign(Vector3 vector)
        {
            this.x = vector.x.ToSign();
            this.y = vector.y.ToSign();
            this.z = vector.z.ToSign();
        }

        public static readonly Vector3Sign zero = new Vector3Sign(Sign.Neutral, Sign.Neutral, Sign.Neutral);
        public static readonly Vector3Sign one = new Vector3Sign(Sign.Positive, Sign.Positive, Sign.Positive);

        public static readonly Vector3Sign right = new Vector3Sign(Sign.Positive, Sign.Neutral, Sign.Neutral);
        public static readonly Vector3Sign up = new Vector3Sign(Sign.Neutral, Sign.Positive, Sign.Neutral);
        public static readonly Vector3Sign forward = new Vector3Sign(Sign.Neutral, Sign.Neutral, Sign.Positive);
        public static readonly Vector3Sign left = new Vector3Sign(Sign.Negative, Sign.Neutral, Sign.Neutral);
        public static readonly Vector3Sign down = new Vector3Sign(Sign.Neutral, Sign.Negative, Sign.Neutral);
        public static readonly Vector3Sign back = new Vector3Sign(Sign.Neutral, Sign.Neutral, Sign.Negative);
    }
}
