public static class AStar
{
    const int DD = 14; // diagonal distance estimate
    const int OD = 10; // orthagonal distance estimate
    const int DD_MOD = DD - OD;

    public enum NodeState
    {
        UNSEEN,
        OPEN,
        CLOSED
    }

    private sealed class Node(Point point, int g, int h, int weight, Node? parent, NodeState state)
    {
        public Point Point = point; // coords on map
        public int G = g; // movement cost from starting point to this tile
        public int H = h; // estimated cost from tile to target
        public int F = g + h * weight; // total of estimated and current
        public Node? Parent = parent;
        public NodeState State = state;
    }
    private static int DistanceTo(int x1, int y1, int x2, int y2) // use octile distance to determine h value
    {
        int dX = Math.Abs(x1 - x2);
        int dY = Math.Abs(y1 - y2);
        if (dX > dY)
            return OD * dX + DD * dY;
        else
            return OD * dY + DD * dX;
    }

    public static List<Point> GetPathTo(Point start, Point target, AStarGrid grid, bool removeStart = true, int maxVisits = -1) // get the path to a specified point based on what's solid. Remove solid is to remove the starting point from the path which is useful for quickly moving somewhere
    {
        // make size estimate for path to avoid reallocating lists
        int estimate = grid.Width * grid.Height / 4;

        PriorityQueue<Node, int> open = new(estimate);

        Dictionary<Point, Node> nodes = new(estimate);

        Node startingNode = new Node(start, 0, DistanceTo(start.X, start.Y, target.X, target.Y), grid.GetCell(start).MoveCost, null, NodeState.OPEN);
        open.Enqueue(startingNode, startingNode.G + startingNode.H);
        nodes.Add(startingNode.Point, startingNode);

        Node bestNode = startingNode; // if the target can't be reached, the path to the node with the lowest f value will be used instead
        int visitedNodes = 0; // used to check if the limit of howmany nodes can be visited has been reached

        while (open.Count > 0 && (maxVisits == -1 || visitedNodes < maxVisits) ) // keep processing while nodes are open and the maximum number of tiles hasn't been reached (-1 means infite reach)
        {
            Node current = open.Dequeue();

            // check if on the closed list due to duplicate occuring when updating F score values in the open list when checking neighbors
            if (nodes.TryGetValue(current.Point, out Node? closedNode) && closedNode.State == NodeState.CLOSED)
                continue;

            visitedNodes++; // increase visitedNode counter

            if (current.Point == target) // return the path if it's reached
            {
                return ReconstructPath(current, removeStart);
            }

            // check if this is the new best node
            if (current.H < bestNode.H)
                bestNode = current;

            // put into closed
            current.State = NodeState.CLOSED;

            // add neighbor tiles to list
            for(int y = -1; y <= 1; y++)
            {
                for(int x = -1; x <= 1; x++)
                {
                    Point neighbor = (current.Point.X + x, current.Point.Y + y);

                    // return early to ignore points that are the same as the current node, or out of bounds
                    if ( neighbor == current.Point || !grid.IsInBounds(neighbor))
                        continue;
                    // check if the node is solid. The target is always counted as an open tile
                    AStarGrid.AstarTile tile = grid.GetCell(neighbor);
                    if (neighbor != target && tile.NSolids > tile.SolidThreshold)
                        continue;

                    // calculate the tentative g value taking in acount new move cost
                    int tentativeG = current.G + ( (x != 0 && y != 0)? DD : OD );

                    // handle duplicate nodes
                    if ( nodes.TryGetValue(neighbor, out Node? node) ) // don't calculate already processed nodes
                    {
                        if (node.State == NodeState.CLOSED) // skip node if already fully processed
                            continue;
                        else if ( node != null && node.State == NodeState.OPEN ) // check if neighbor is already in open list to try and optimize path
                        {
                            if ( node.G > tentativeG) // check if the current node is better than the old one and replace it if so
                            {
                                node.G = tentativeG;
                                node.F = tentativeG + node.H * grid.GetCell(neighbor).MoveCost;
                                node.Parent = current;

                                // re-enqueue with the new priority
                                open.Enqueue(node, node.F);
                            }
                            continue;
                        }
                    }
                    
                    // add new node to open list
                    Node neighborNode = new(neighbor, tentativeG, DistanceTo(neighbor.X, neighbor.Y, target.X, target.Y), grid.GetCell(neighbor).MoveCost, current, NodeState.OPEN);
                    open.Enqueue(neighborNode, neighborNode.F);
                    nodes.TryAdd(neighborNode.Point, neighborNode);
                }
            }
                

        }

        return ReconstructPath(bestNode, removeStart); // return path to the closest point
    }

    private static List<Point> ReconstructPath(Node? current, bool removeStart)
    {
        List<Point> path = [];
        while (current != null)
        {
            path.Add(current.Point);
            current = current.Parent;
        }
        path.Reverse(); // reverse so the first element is the starting point
        if (removeStart) // remove the starting point from the path
            path.RemoveAt(0);
        return path;
    }
}