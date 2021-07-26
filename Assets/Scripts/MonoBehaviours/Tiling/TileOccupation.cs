using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools.Tiling
{
    public class TileOccupation : MonoBehaviour
    {
        public TileState tile
        {
            get
            {
                return _tile;
            }
            set
            {
                _tile = value;

                if (tile.tile as AreaTile == null)
                {
                    {
                        BoxCollider boxCollider = occupationSnap.gameObject.AddComponent<BoxCollider>();
                        boxCollider.isTrigger = true;
                    }
                    {
                        BoxCollider boxCollider = occupationRaw.gameObject.AddComponent<BoxCollider>();
                        boxCollider.isTrigger = true;
                    }
                }
                else if (tile.tile as AreaTile != null)
                {
                    foreach (UnityEngine.Vector3Int tile in (tile.tile as AreaTile).area)
                    {
                        {
                            BoxCollider boxCollider = occupationSnap.gameObject.AddComponent<BoxCollider>();
                            boxCollider.isTrigger = true;
                            boxCollider.center = tile;
                        }
                        {
                            BoxCollider boxCollider = occupationRaw.gameObject.AddComponent<BoxCollider>();
                            boxCollider.isTrigger = true;
                            boxCollider.center = tile;
                        }
                    }
                }

                Update();
            }
        }
        [SerializeField]
        private TileState _tile;

        private Transform occupationSnap;
        private Transform occupationRaw;
        
        public static TileOccupation CreateTileOccupation(TileState tile)
        {
            TileOccupation tileOccupationInstance = Instantiate(Resources.Load("TileOccupation") as GameObject).GetComponent<TileOccupation>();
            
            tileOccupationInstance.tile = tile;
            
            return tileOccupationInstance;
        }

        private void Awake()
        {
            occupationSnap = transform.GetChild(0);
            occupationRaw = transform.GetChild(1);
        }

        void Update()
        {
            //SNAPPED
            occupationSnap.transform.position = tile.grid.GridToWorld(tile.position, tile.subdivisions);
            occupationSnap.transform.rotation = tile.grid.finalRotation * Quaternion.Euler(tile.rotation * 90);
            occupationSnap.transform.localScale = tile.grid.subdividedTileSize(tile.subdivisions) * tile.grid.totalScale;

            //RAW - everything is raw except for subdivisions
            occupationRaw.transform.position = tile.grid.GridToWorld(tile.positionRaw, tile.subdivisions);
            occupationRaw.transform.rotation = tile.grid.finalRotation * tile.rotationRaw;
            occupationRaw.transform.localScale = tile.grid.subdividedTileSize(tile.subdivisions) * tile.grid.totalScale;
        }
    }
}