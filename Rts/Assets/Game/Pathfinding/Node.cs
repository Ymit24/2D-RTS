using System.Collections;
using System.Collections.Generic;

namespace Game.Pathfinding {
    public class Node
    {
        public int x;
        public int y;
        public bool obstacle;
        public bool visited;
        public Node parent;
        public float globalGoal;
        public float localGoal;
        public List<Node> neighbours;

        public Node(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.obstacle = false;
            this.parent = null;
            this.visited = false;
            this.neighbours = new List<Node>();
        }
    }
}