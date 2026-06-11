// namespace Astar
// {
//     public static class AStar
//     {
//         private class Node(Point point, float g, float h, Node? parent)
//         {
//             public Point Point = point; // coords on map
//             public float G = g; // movement cost from starting point to this tile
//             public float H = h; // estimated cost from tile to target
//             public Node? Parent = parent;
//         }
//         private static float DistanceTo(int x1, int y1, int x2, int y2) // use octile distance to determine h value
//         {
//             const float DD = 1.4142135623730951f - 1; // diagonal distance

//             int dX = Math.Abs(x1 - x2);
//             int dY = Math.Abs(y1 - y2);
//             return Math.Max(dX, dY) + DD * Math.Min(dX, dY);
//         }

//         private static List<Point> GetNeighboringTiles(Point point, Point endPoint, Map map, Func<Point, bool>? CheckIfSolidAt)
//         {
//             List<Point> result = new(8);

//             for(int y = -1; y <= 1; y++)
//             {
//                 for(int x = -1; x <= 1; x++)
//                 {
//                     Point newPoint = (point.X + x, point.Y + y);
//                     if ( newPoint != point && map.IsInBounds(newPoint) && ( CheckIfSolidAt == null || !CheckIfSolidAt(newPoint) || newPoint == endPoint) )
//                     {
//                         result.Add(newPoint);
//                     }
//                 }
//             }

//             return result;
//         }

//         public static List<Point> GetPathTo(Point start, Point target, Map map, Func<Point, bool>? CheckIfSolidAt, bool removeStart = true, int maxVisits = -1) // get the path to a specified point based on what's solid. Remove solid is to remove the starting point from the path which is useful for quickly moving somewhere
//         {
//             PriorityQueue<Node, float> open = new();
//             Dictionary<Point, Node> openLookUp = [];

//             HashSet<Point> closed = [];

//             Node startingNode = new Node(start, 0, DistanceTo(start.X, start.Y, target.X, target.Y), null);
//             open.Enqueue(startingNode, startingNode.G + startingNode.H);
//             openLookUp.Add(startingNode.Point, startingNode);

//             Node bestNode = startingNode; // if the target can't be reached, the path to the node with the lowest f value will be used instead
//             int visitedNodes = 0; // used to check if the limit of howmany nodes can be visited has been reached

//             while (open.Count > 0 && (maxVisits == -1 || visitedNodes < maxVisits) ) // keep processing while nodes are open and the maximum number of tiles hasn't been reached (-1 means infite reach)
//             {
//                 Node current = open.Dequeue();

//                 // check if on the closed list due to duplicate occuring when updating F score values in the open list when checking neighbors
//                 if (closed.Contains(current.Point))
//                     continue;

//                 visitedNodes++; // increase visitedNode counter

//                 if (current.Point == target) // return the path if it's reached
//                 {
//                     return ReconstructPath(current, removeStart);
//                 }

//                 // check if this is the new best node
//                 if (current.H < bestNode.H)
//                     bestNode = current;

//                 // put into closed
//                 closed.Add(current.Point);
//                 openLookUp.Remove(current.Point);

//                 foreach (Point neighbor in GetNeighboringTiles(current.Point, target, map, CheckIfSolidAt))
//                 {
//                     if (closed.Contains(neighbor)) // don't calculate already processed nodes
//                         continue;
                    
//                     // calculate the tentative g value
//                     float tentativeG = current.G + DistanceTo(current.Point.X, current.Point.Y, neighbor.X, neighbor.Y);

//                     // check if neighbor is already in open list
//                     if ( openLookUp.TryGetValue(neighbor, out Node? node) )
//                     {
//                         if ( node.G > tentativeG) // check if the current node is better than the old one and replace it if so
//                         {
//                             node.G = tentativeG;
//                             node.Parent = current;

//                             // re-enqueue with the new priority
//                             open.Enqueue(node, node.G + node.H);
//                         }
//                         continue;
//                     }
                    
//                     // add new node to open list
//                     Node neighborNode = new(neighbor, tentativeG, DistanceTo(neighbor.X, neighbor.Y, target.X, target.Y), current);
//                     open.Enqueue(neighborNode, neighborNode.G + neighborNode.H);
//                     openLookUp.Add(neighborNode.Point, neighborNode);
//                 }

//             }

//             return ReconstructPath(bestNode, removeStart); // return path to the closet point
//         }

//         private static List<Point> ReconstructPath(Node? current, bool removeStart)
//         {
//             List<Point> path = [];
//             while (current != null)
//             {
//                 path.Add(current.Point);
//                 current = current.Parent;
//             }
//             path.Reverse(); // reverse so the first element is the starting point
//             if (removeStart) // remove the starting point from the path
//                 path.RemoveAt(0);
//             return path;
//         }
//     }
// }