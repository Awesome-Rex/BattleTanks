using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools.Tiling
{
    public class RuleTileProfile : ScriptableObject
    {
        public Grid grid;

        private Dictionary<TilingRule, List<Tile>> _tilingRules = new Dictionary<TilingRule, List<Tile>>();

        //compacted all values to same key in list
        public Dictionary<TilingRule, List<Tile>> tilingRules
        {
            get
            {
                return _tilingRules;
            }
            set
            {
                //filters contents to be compact

                Dictionary<TilingRule, List<Tile>> newRules = new Dictionary<TilingRule, List<Tile>>();

                //Creates new list for every tiling rule
                foreach (KeyValuePair<TilingRule, List<Tile>> rule in value)
                {
                    newRules[rule.Key] = new List<Tile>();
                }
                //Prevents repetition in tiling rules (Keys)
                foreach (KeyValuePair<TilingRule, List<Tile>> rule in value)
                {
                    List<Tile> newTiles = new List<Tile>();

                    foreach (Tile tile in rule.Value)
                    {
                        if (tile.grid == grid)
                        {
                            newTiles.Add(tile);
                        }
                    }

                    newRules[rule.Key].AddRange(newTiles);
                }
                //prevents tiling rules assigned to empty lists (Values)
                foreach (KeyValuePair<TilingRule, List<Tile>> rule in newRules)
                {
                    if (rule.Value.Count == 0)
                    {
                        newRules.Remove(rule.Key);
                    }
                }

                _tilingRules = newRules;
            }
        }
    }
}
