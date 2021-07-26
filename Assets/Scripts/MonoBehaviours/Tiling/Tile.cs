using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using REXTools.TransformTools;

namespace REXTools.Tiling {
    [CreateAssetMenu(fileName = "New REX Tile", menuName = "REX/Tiling/Tile", order = 1)]
    public class Tile : ScriptableObject
    {
        //[Header("Tile Space")]
        public Grid grid;
        public int subdivisions = 1; //+++++++DOESNT NEED TO BE USED ANYMORE

        //[Header("Tile Prefab")]
        public GameObject prefab;

        public Vector3 offsetPosition = Vector3.zero;
        public Quaternion offsetRotation = Quaternion.identity;
        public Vector3 offsetScale = Vector3.one;

        public List<string> tags;

        public static bool Intersects(Tile a, Tile b)
        {
            foreach (string tag in a.tags)
            {
                if (b.tags.Contains(tag))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Intersects(Tile tile)
        {
            return Intersects(this, tile);
        }

        //creates new tile
        protected TileState CreateTile(GridOrientation orientation, Transform parent = null)
        {
            TileState tileInstance = Instantiate(prefab).AddComponent<TileStateConstant>();

            tileInstance.transform.SetParent(parent, true);
            tileInstance.tile = this;
            tileInstance.grid = orientation;

            return tileInstance;
        }
        public TileState CreateTile(GridOrientation orientation, Vector3 position, float subdivisions = 1, Quaternion rotation = default, Transform parent = null)
        {
            TileState tileInstance = CreateTile(orientation, parent);

            tileInstance.subdivisionsRaw = subdivisions;
            tileInstance.positionRaw = position;
            tileInstance.rotationRaw = rotation;

            TileOccupation.CreateTileOccupation(tileInstance);

            return tileInstance;
        }
        public TileState CreateTile(GridOrientation orientation, Vector3 position, Quaternion rotation = default, Transform parent = null)
        {
            return CreateTile(orientation, position, subdivisions, rotation, parent);
        }
        public TileState CreateTile(GridOrientation orientation, UnityEngine.Vector3Int position, int subdivisions = 1, UnityEngine.Vector3Int rotation = default, Transform parent = null)
        {
            TileState tileInstance = CreateTile(orientation, parent);

            tileInstance.subdivisions = subdivisions;
            tileInstance.position = position;
            tileInstance.rotation = rotation;

            TileOccupation.CreateTileOccupation(tileInstance);

            return tileInstance;
        }
        public TileState CreateTile(GridOrientation orientation, UnityEngine.Vector3Int position, UnityEngine.Vector3Int rotation = default, Transform parent = null)
        {
            return CreateTile(orientation, position, subdivisions, rotation, parent);
        }

        protected TileState CreateTileMatch(GridOrientation orientation, Transform parent = null)
        {
            TileState tileInstance = Instantiate(prefab).AddComponent<TileStateMatch>();

            tileInstance.transform.SetParent(parent, true);
            tileInstance.tile = this;
            tileInstance.grid = orientation;

            return tileInstance;
        }
        public TileState CreateTileMatch(GridOrientation orientation, Vector3 position, float subdivisions = 1, Quaternion rotation = default, Transform parent = null)
        {
            TileState tileInstance = CreateTileMatch(orientation, parent);

            tileInstance.subdivisionsRaw = subdivisions;
            tileInstance.positionRaw = position;
            tileInstance.rotationRaw = rotation;

            TileOccupation.CreateTileOccupation(tileInstance);

            return tileInstance;
        }
        public TileState CreateTileMatch(GridOrientation orientation, Vector3 position, Quaternion rotation = default, Transform parent = null)
        {
            return CreateTileMatch(orientation, position, subdivisions, rotation, parent);
        }
        public TileState CreateTileMatch(GridOrientation orientation, UnityEngine.Vector3Int position, int subdivisions = 1, UnityEngine.Vector3Int rotation = default, Transform parent = null)
        {
            TileState tileInstance = CreateTileMatch(orientation, parent);

            tileInstance.subdivisions = subdivisions;
            tileInstance.position = position;
            tileInstance.rotation = rotation;

            TileOccupation.CreateTileOccupation(tileInstance);

            return tileInstance;
        }
        public TileState CreateTileMatch(GridOrientation orientation, UnityEngine.Vector3Int position, UnityEngine.Vector3Int rotation = default, Transform parent = null)
        {
            return CreateTileMatch(orientation, position, subdivisions, rotation, parent);
        }
    }
}