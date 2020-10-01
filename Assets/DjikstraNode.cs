using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//--Class purpose-- 
//An extension of MapGraph.MapNode for running Dijkstra's algorithm
//Instances are created once a start and endnote for PathFinder.cs are created.
public class DjikstraNode {
    public MapGraph.MapNode node;
    public DjikstraNode nearestToStart = null;
    public int cost;
    public bool visited;
    public int minCostToStart;

    public DjikstraNode(MapGraph.MapNode baseNode) : base() {
        node = baseNode;
        cost = 1;
        visited = false;
        minCostToStart = 99;
    }

}
