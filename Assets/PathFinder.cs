using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//--Class purpose-- 
//Finds the shortest path between two nodes
//Nodes are set using Q and E
public class PathFinder : MonoBehaviour {
    MapGraph graph;
    MapGraph.MapNode fromNode = null;
    DjikstraNode start;
    MapGraph.MapNode toNode = null;
    DjikstraNode end;
    public ColliderManager cManager;

    public void SetGraph(MapGraph graph) {
        this.graph = graph;
    }


    // Update is called once per frame
    void Update() {
        if(Input.GetKeyDown(KeyCode.Q)) {     
            fromNode = cManager.GetActiveNode();             
            Debug.Log("PATHFINDER: SET start");
            if(fromNode == toNode) {
                toNode = null;
            }
            if(fromNode != null && toNode != null) {
                Debug.Log("PATHFINDER: FINDING PATH");
                List<DjikstraNode> path = GetShortestPath();
                string output = "Coordinates: ";
                foreach (var dNode in path) {
                    output += "(" + dNode.node.centerPoint.x + " " + dNode.node.centerPoint.z + "), ";
                }
                Debug.Log(output);
            }           
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            toNode = cManager.GetActiveNode();
            Debug.Log("PATHFINDER: Set end");
            if (toNode == fromNode) {
                fromNode = null;
            }
            if (fromNode != null && toNode != null) {
                List<DjikstraNode> path = GetShortestPath();
            }
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            
        }
    }
    List<MapGraph.MapNode> FindShortestPath() {
        if(fromNode == null || toNode == null) {
            return null;
        }
        start = new DjikstraNode(fromNode);
        end = new DjikstraNode(toNode);
        /*
        Dictionary<MapGraph.MapNode, int> nodeDictionary = new Dictionary<MapGraph.MapNode, int>(); //bool will be true when the node has been checked for pathing already
        foreach(MapGraph.MapNode node in graph.nodesByCenterPosition.Values) {
            nodeDictionary.Add(node, 99);
        }
        //nodeDictionary[fromNode] = true;
        foreach(var neighbourNode in fromNode.GetNeighborNodes()) {
            if(neighbourNode.Equals(toNode)) {
                //Path found
                
            }
        }*/
        return null;

    }
    public List<DjikstraNode> GetShortestPath() {
        Debug.Log("PATHFINDER: Calculating path");
        start = new DjikstraNode(fromNode);
        end = new DjikstraNode(toNode);
        DijkstraSearch();
        var shortestPath = new List<DjikstraNode>();
        shortestPath.Add(end);
        BuildShortestPath(shortestPath, end);
        shortestPath.Reverse();
        return shortestPath;
    }
    private void BuildShortestPath(List<DjikstraNode> list, DjikstraNode node) {
        if (node.nearestToStart == null) return;
        list.Add(node.nearestToStart);
        BuildShortestPath(list, node.nearestToStart);
    }
    /*
    public int CompareNodesByMinCostToStart(DjikstraNode x, DjikstraNode y) {
        if(x.minCostToStart == y.minCostToStart)  return 0;       
        if(x.minCostToStart > y.minCostToStart) {
            return -1;
        } else return 1;               
    }
    public int CompareNodesByCost(DjikstraNode x, DjikstraNode y) {
        if (x.cost == y.cost) return 0;       
        if (x.cost > y.cost) {
            return -1;
        } else return 1;   
    }
    */
    private void DijkstraSearch() {
        start.minCostToStart = 0;
        List<DjikstraNode> prioQueue = new List<DjikstraNode>();
        prioQueue.Add(start);
        do {
            //prioQueue.Sort(CompareNodesByMinCostToStart);
            prioQueue = prioQueue.OrderBy(x => x.minCostToStart).ToList();
            DjikstraNode node = prioQueue.First();
            prioQueue.Remove(node);
            Debug.Log("prioQueue length: "+prioQueue.Count);
            List<DjikstraNode> neighbourList = new List<DjikstraNode>();
            foreach (var neighbourNode in node.node.GetNeighborNodes()) {
                neighbourList.Add(new DjikstraNode(neighbourNode));
            }
            //neighbourList.Sort(CompareNodesByCost);
            foreach (var childNode in neighbourList.OrderBy(x => x.cost)) {
               //var childNode = cnn;
                if (childNode.visited) continue;
                if (childNode.minCostToStart == 99 || node.minCostToStart + childNode.cost < childNode.minCostToStart) {
                    childNode.minCostToStart = node.minCostToStart + childNode.cost;
                    childNode.nearestToStart = node;
                    if (!prioQueue.Contains(childNode)) prioQueue.Add(childNode);
                }
            }
            node.visited = true;
            if (node == end) return;
        } while (prioQueue.Any());
    }
}
