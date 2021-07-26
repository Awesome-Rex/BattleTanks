using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using REXTools.TransformTools;

namespace REXTools.Tiling
{
    [System.Serializable]
    public struct TileArea
    {
        public List<UnityEngine.Vector3Int> tiles
        {
            get
            {
                _tiles = _tiles.Distinct().ToList();
                _tiles = _tiles.OrderByDescending(tile => tile.x).ThenByDescending(tile => tile.y).ThenByDescending(tile => tile.z).ToList();

                return _tiles;
            }
            set
            {
                _tiles = value;   
            }            
        }
        [SerializeField] private List<UnityEngine.Vector3Int> _tiles;

        //properties
        public UnityEngine.Vector3Int min
        {
            get
            {
                return new UnityEngine.Vector3Int
                (
                    tiles.Min(tile => tile.x),
                    tiles.Min(tile => tile.y),
                    tiles.Min(tile => tile.z)
                );
            }
        }
        public UnityEngine.Vector3Int max
        {
            get
            {
                return new UnityEngine.Vector3Int
                (
                    tiles.Max(tile => tile.x),
                    tiles.Max(tile => tile.y),
                    tiles.Max(tile => tile.z)
                );
            }
        }

        public List<List<UnityEngine.Vector3Int>> islands
        { //# of regions that arent adjacent
            get
            {
                //List<UnityEngine.Vector3Int> tilesRemaining = tiles;
                List<UnityEngine.Vector3Int> tilesRemaining = tiles.Select(item => item).ToList(); //Clones list
                List<List<UnityEngine.Vector3Int>> newIslands = new List<List<UnityEngine.Vector3Int>>();

                void FloodFill(UnityEngine.Vector3Int origin)
                {
                    List<UnityEngine.Vector3Int> confirmedFill = new List<UnityEngine.Vector3Int>();
                    List<UnityEngine.Vector3Int> newFill = new List<UnityEngine.Vector3Int>
                    {
                        origin
                    };
                    tilesRemaining.Remove(origin);

                    while (newFill.Count > 0)
                    {
                        UnityEngine.Vector3Int fillTile = newFill[0];

                        //foreach (UnityEngine.Vector3Int tile in tilesRemaining)
                        for (int i = 0; i < tilesRemaining.Count; i++)
                        {
                            UnityEngine.Vector3Int tile = tilesRemaining[i];

                            if ((new Bounds(fillTile, Vector3.one)).Adjacent(new Bounds(tile, Vector3.one)))
                            {
                                newFill.Add(tile);
                                tilesRemaining.Remove(tile);

                                i--;
                            }
                        }

                        newFill.Remove(fillTile);
                        confirmedFill.Add(fillTile);
                    }

                    List<UnityEngine.Vector3Int> fill = confirmedFill.Select(item => item).ToList();
                    fill.AddRange(newFill);
                    newIslands.Add(fill);
                }

                while (tilesRemaining.Count > 0)
                {
                    FloodFill(tilesRemaining[0]);
                }

                return newIslands;
            }
        }

        public bool isBox 
        {
            get 
            {
                Vector3 boundsArea = (max - min) + UnityEngine.Vector3Int.one;

                return tiles.Count == (boundsArea.x * boundsArea.y * boundsArea.z);
            }
        }

        public Vector3Bool symmetrical {
            get {
                throw new System.NotImplementedException();
            }
        }


        public void AddTile(UnityEngine.Vector3Int tile)
        {

        }


        public TileArea(UnityEngine.Vector3Int center)
        {
            _tiles = new List<UnityEngine.Vector3Int>();
            tiles.Add(center);
        }
        public TileArea(BoundsInt box)
        {
            _tiles = new List<UnityEngine.Vector3Int>();

            for (int x = box.min.x; x < box.max.x + 1; x++)
            {
                for (int y = box.min.y; y < box.max.y + 1; y++)
                {
                    for (int z = box.min.z; z < box.max.z + 1; z++)
                    {
                        tiles.Add(new UnityEngine.Vector3Int(x, y, z));
                    }
                }
            }
        }
        public TileArea(List<UnityEngine.Vector3Int> tiles)
        {
            this._tiles = tiles;
        }
    }
}