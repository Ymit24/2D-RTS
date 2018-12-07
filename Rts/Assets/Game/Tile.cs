using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Tile
    {
        private int x;
        private int y;

        private Building building;

        public int X
        {
            get
            {
                return x;
            }
        }

        public int Y
        {
            get
            {
                return y;                
            }
        }

        public TileCoord TileCoord
        {
            get
            {
                return new TileCoord(x, y);
            }
        }
        
        public Building Building
        {
            get { return building; }
        }
        
        public Tile(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void PlaceBuilding(Building building)
        {
            this.building = building;
        }

        // technically we don't need this, could be replaced with PlaceBuilding(null)
        public void RemoveBuilding()
        {
            this.building = null;
        }
    }
}