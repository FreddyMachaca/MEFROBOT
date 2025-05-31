using System;
using System.Collections.Generic;
using System.Linq;

namespace MEFRobot
{
    public class Node
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int G { get; set; } // Costo desde el inicio hasta este nodo
        public int H { get; set; } // Heurística (distancia estimada hasta el destino)
        public int F => G + H;     // Puntuación total
        public Node Parent { get; set; } // Nodo padre para reconstruir el camino

        public Node(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Node other)
            {
                return X == other.X && Y == other.Y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
    }

    public class AStarPathfinder
    {
        private readonly int _gridWidth = 800;
        private readonly int _gridHeight = 450;
        private readonly int _paso = 1; // Tamaño del paso para el movimiento

        public List<Node> FindPath(int startX, int startY, int targetX, int targetY)
        {
            var startNode = new Node(startX, startY);
            var targetNode = new Node(targetX, targetY);

            var openList = new List<Node>();
            var closedList = new HashSet<Node>();

            startNode.G = 0;
            startNode.H = CalculateHeuristic(startNode, targetNode);

            openList.Add(startNode);

            while (openList.Count > 0)
            {
                // Encontrar el nodo con menor F en la lista abierta
                var currentNode = openList.OrderBy(n => n.F).First();

                if (currentNode.X == targetNode.X && currentNode.Y == targetNode.Y)
                {
                    return ReconstructPath(currentNode);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (var neighbor in GetNeighbors(currentNode))
                {
                    if (closedList.Any(n => n.X == neighbor.X && n.Y == neighbor.Y))
                        continue;

                    int newG = currentNode.G + _paso;

                    var existingNeighbor = openList.FirstOrDefault(n => n.X == neighbor.X && n.Y == neighbor.Y);

                    if (existingNeighbor == null)
                    {
                        neighbor.G = newG;
                        neighbor.H = CalculateHeuristic(neighbor, targetNode);
                        neighbor.Parent = currentNode;
                        openList.Add(neighbor);
                    }
                    else if (newG < existingNeighbor.G)
                    {
                        existingNeighbor.G = newG;
                        existingNeighbor.Parent = currentNode;
                    }
                }
            }
            return new List<Node>();
        }

        private int CalculateHeuristic(Node a, Node b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        private List<Node> GetNeighbors(Node node)
        {
            var neighbors = new List<Node>();

            // Nodos vecinos en las 8 direcciones
            if (node.X + _paso < _gridWidth) neighbors.Add(new Node(node.X + _paso, node.Y));
            if (node.X - _paso >= 0) neighbors.Add(new Node(node.X - _paso, node.Y));
            if (node.Y + _paso < _gridHeight) neighbors.Add(new Node(node.X, node.Y + _paso));
            if (node.Y - _paso >= 0) neighbors.Add(new Node(node.X, node.Y - _paso));
            if (node.X + _paso < _gridWidth && node.Y + _paso < _gridHeight) neighbors.Add(new Node(node.X + _paso, node.Y + _paso));
            if (node.X + _paso < _gridWidth && node.Y - _paso >= 0) neighbors.Add(new Node(node.X + _paso, node.Y - _paso));
            if (node.X - _paso >= 0 && node.Y + _paso < _gridHeight) neighbors.Add(new Node(node.X - _paso, node.Y + _paso));
            if (node.X - _paso >= 0 && node.Y - _paso >= 0) neighbors.Add(new Node(node.X - _paso, node.Y - _paso));

            return neighbors;
        }

        private List<Node> ReconstructPath(Node node)
        {
            var path = new List<Node>();
            var current = node;

            // Reconstruir el camino siguiendo los nodos padres
            while (current != null)
            {
                path.Add(current);
                current = current.Parent;
            }
            path.Reverse();
            return path;
        }
    }
}
