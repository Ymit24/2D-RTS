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
        private int reserveID = -1;

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

        public int ReserveId
        {
            get { return reserveID; }
        }

        public bool Walkable
        {
            get { return building == null; }
        }

        public bool IsAvailable(int reserveID)
        {
            return building == null && (this.reserveID!=-1 && this.reserveID != reserveID) == false;
        }
        
        public Tile(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void Reserve(int reserveID)
        {
            this.reserveID = reserveID;
        }

        public void PlaceBuilding(Building building)
        {
            this.building = building;
            reserveID = -1;
        }

        // technically we don't need this, could be replaced with PlaceBuilding(null)
        public void RemoveBuilding()
        {
            this.building = null;
        }
    }
}