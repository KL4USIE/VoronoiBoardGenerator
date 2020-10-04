using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//--Class purpose-- 
//Finds the shortest path between two nodes
//Nodes are set using Q and E
//R resets the inputs
public class PathFinder : MonoBehaviour {
    MapGraph graph; //reference to MapGraph for getting nodes
    MapGraph.MapNode fromNode = null; //saves start-node
    MapGraph.MapNode toNode = null; //saves target-node    
    public ColliderManager cManager; //for getting highlighted nodes
    List<GameObject> markerObjects = new List<GameObject>(); //list of the spawned objects, for later deletion

    public void SetGraph(MapGraph graph) {
        this.graph = graph;
    }
    void Update() {
        if(Input.GetKeyDown(KeyCode.Q)) { //set start    
            fromNode = cManager.GetActiveNode();             
            Debug.Log("PATHFINDER: Set start");
            if(fromNode == toNode) {
                toNode = null;
            }
            if(fromNode != null && toNode != null) {               
                GetShortestPath(); //also returns shortest path
            }           
        }
        if (Input.GetKeyDown(KeyCode.E)) { //set target
            toNode = cManager.GetActiveNode();
            Debug.Log("PATHFINDER: Set end");
            if (toNode == fromNode) {
                fromNode = null;
            }
            if (fromNode != null && toNode != null) {
                List<MapGraph.MapNode> path = GetShortestPath();
            }
        }
        if (Input.GetKeyDown(KeyCode.R)) { //reset pathfinder
            fromNode = null;
            toNode = null;
            ClearMarkers();
        }
    }
    public void ClearMarkers() { //deletes all generated markers by destroying their gameobjects; used for cleanup of old generation
        foreach (GameObject obj in markerObjects) {
            DestroyImmediate(obj);
        }
        markerObjects.Clear();
    }
    private void SpawnMarkers(List<MapGraph.MapNode> nodes) { //Spawns a marker above each node inside the given list
        ClearMarkers(); //to start, deletes the old markers
        foreach(var node in nodes) {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.parent = this.transform; //sets as child in scene hierarchy
            go.layer = 5; //UI Layer
            go.transform.position = new Vector3(node.centerPoint.x, 8, node.centerPoint.z);
            go.transform.localScale = new Vector3(4, 4, 4); //scale of marker
            markerObjects.Add(go);
        }      
    }
    public List<MapGraph.MapNode> GetShortestPath() { //Returns shortest path, for now also displays it, return isnt used.
        Debug.Log("PATHFINDER: Calculating path...");
        foreach(var node in graph.nodesByCenterPosition.Values) { //reset 
            node.ResetDijkstra();
        }
        DijkstraSearch();
        var shortestPath = new List<MapGraph.MapNode>();
        shortestPath.Add(toNode);
        BuildShortestPath(shortestPath, toNode);
        shortestPath.Reverse();     
        SpawnMarkers(shortestPath);
        //ShortestPathToString(shortestPath);
        return shortestPath;
    }
    private void ShortestPathToString(List<MapGraph.MapNode> shortestPath) {
        string output = "Coordinates: ";
        foreach (var node in shortestPath) {
            output += "(" + node.centerPoint.x + " " + node.centerPoint.z + "), ";
        }
        Debug.Log(output);
    }
    private void BuildShortestPath(List<MapGraph.MapNode> list, MapGraph.MapNode node) { //sorts the nodes according to the Dijkstra variables
        if (node.nearestToStart == null) return;
        list.Add(node.nearestToStart);
        BuildShortestPath(list, node.nearestToStart);
    }
    private void DijkstraSearch() { //assigns each node the Dijkstra related variables
        fromNode.minCostToStart = 0;     
        List<MapGraph.MapNode> prioQueue = new List<MapGraph.MapNode>();
        prioQueue.Add(fromNode);
        do {
            prioQueue = prioQueue.OrderBy(x => x.minCostToStart).ToList();
            MapGraph.MapNode node = prioQueue.First();
            prioQueue.Remove(node);
            //Debug.Log("prioQueue length: "+prioQueue.Count);        
            foreach (var childNode in node.GetNeighborNodes().OrderBy(x => x.cost)) {                       
                if (childNode.visited) continue;
                if (childNode.minCostToStart == 99 || node.minCostToStart + childNode.cost < childNode.minCostToStart) {
                    childNode.minCostToStart = node.minCostToStart + childNode.cost;
                    childNode.nearestToStart = node;
                    if (!prioQueue.Contains(childNode)) prioQueue.Add(childNode);
                }
            }
            node.visited = true;
            if (node == toNode) return;
        } while (prioQueue.Any());
    }
}
