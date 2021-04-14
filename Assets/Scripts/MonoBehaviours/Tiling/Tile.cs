using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.TransformTools;

namespace REXTools.Tiling {
    [CreateAssetMenu(fileName = "New REX Tile", menuName = "REX/Tiling/Tile")]
    public class Tile : TileBase
    {
        public GameObject prefab;

        public Vector3 offsetPosition;
        public Quaternion offsetRotation;
        public Vector3 offsetScale;

        public Dictionary<Axis, Vector3> correspondDirection
        {
            get {
                Vector3 rounded = offsetRotation.eulerAngles.Operate((s, a) => Mathf.Round(a / 90f) * 90f);

                return new Dictionary<Axis, Vector3>
                {
                    {Axis.X,
                        new Vector3(rounded.GetAxis(Axis.X), 0f, 0f)
                    },
                    {Axis.Y,
                        new Vector3(0f, rounded.GetAxis(Axis.Y), 0f)
                    },
                    {Axis.Z,
                        new Vector3(0f, 0f, rounded.GetAxis(Axis.Z))
                    }
                };
            }
        }

        public override void ClearArea()
        {
            throw new System.NotImplementedException();
        }

        public override void Occupy(GameObject prefab, Vector3 position)
        {
            throw new System.NotImplementedException();
        }

        public override void OccupyAdditive(GameObject prefab, Vector3 position)
        {
            throw new System.NotImplementedException();
        }
    }
}