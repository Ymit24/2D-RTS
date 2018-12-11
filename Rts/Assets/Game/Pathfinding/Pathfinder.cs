using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Game;

namespace Game.Pathfinding
{
    public class Pathfinder
    {
        private Node[,] nodes;
        private int width;
        private int height;

        private bool fourWay;

        public Pathfinder(int width, int height, bool fourWay = false)
        {
            this.width = width;
            this.height = height;
            this.fourWay = fourWay;

            nodes = new Node[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    nodes[x, y] = new Node(x,y);
                }
            }

            GenerateGraph();
        }

        public void WeightGraph(bool[,] obstacles)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    nodes[x, y].obstacle = obstacles[x, y];
                }
            }
        }
        
        private void GenerateGraph()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (y > 0)
                        nodes[x,y].neighbours.Add(nodes[x,y-1]);
                    if (y < height - 1)
                        nodes[x,y].neighbours.Add(nodes[x,y+1]);
                    if (x > 0)
                        nodes[x,y].neighbours.Add(nodes[x-1,y]);
                    if (x < width - 1)
                        nodes[x,y].neighbours.Add(nodes[x+1,y]);
                    if (fourWay == false)
                    {
                        if (y > 0 && x > 0)
                            nodes[x, y].neighbours.Add(nodes[x - 1, y - 1]);
                        if (y < height - 1 && x < width - 1)
                            nodes[x, y].neighbours.Add(nodes[x + 1, y + 1]);
                        if (y < height - 1 && x > 0)
                            nodes[x, y].neighbours.Add(nodes[x - 1, y + 1]);
                        if (y > 0 && x < width - 1)
                            nodes[x, y].neighbours.Add(nodes[x + 1, y - 1]);
                    }
                }
            }
        }
        
        public Node[] Solve(TileCoord startCoord, TileCoord endCoord)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    nodes[x, y].visited = false;
                    nodes[x, y].localGoal = Mathf.Infinity;
                    nodes[x, y].globalGoal = Mathf.Infinity;
                    nodes[x, y].parent = null;
                }
            }

            System.Func<Node, Node, float> distance = (a, b) =>
            {
                return Mathf.Sqrt(Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.y - b.y, 2));
            };

            System.Func<Node, Node, float> heuristic = (a, b) =>
            {
                return distance(a, b);
            };

            Node start = nodes[(int) startCoord.x, (int) startCoord.y];
            Node end = nodes[(int) endCoord.x, (int) endCoord.y];
            
            Node current = start;
            start.localGoal = 0;
            start.globalGoal = heuristic(start, end);

            List<Node> notTested = new List<Node>();
            notTested.Add(start);

            while (notTested.Count != 0 && current != end)
            {
                notTested.Sort((a, b) => a.globalGoal.CompareTo(b.globalGoal));
                
                while (notTested.Count != 0 && notTested[0].visited == true)
                    notTested.RemoveAt(0);
                if (notTested.Count == 0)
                    break;

                current = notTested[0];
                current.visited = true;
                
                foreach (Node node in current.neighbours)
                {
                    if (!node.visited && node.obstacle == false)
                    {
                        notTested.Add(node);
                    }

                    float possiblyLowerGoal = current.localGoal + distance(current, node);
                    if (possiblyLowerGoal < node.localGoal)
                    {
                        node.parent = current;
                        node.localGoal = possiblyLowerGoal;
                        node.globalGoal = node.localGoal + heuristic(node, end);
                    }
                }
            }

            List<Node> path = new List<Node>();
            Node p = end;
            if (p == null) return null;
            while (p.parent != null)
            {
                path.Add(p);
                p = p.parent;
            }
            path.Reverse();
            return path.ToArray();
        }
    }
}