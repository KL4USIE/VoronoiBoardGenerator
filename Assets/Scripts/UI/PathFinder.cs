﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Finds the shortest path between two nodes
/// Also responsible for some GUI
/// Nodes are set using Q and E
/// R resets the inputs
/// T toggles ignoring individual node costs
/// </summary>
/// <param name="graph">reference to MapGraph for getting nodes</param>
/// <param name="fromNode">Saves start-node</param>
/// <param name="toNode">Saves target-node</param>
/// <param name="cManager">reference for getting highlighted node</param>
/// <param name="markerObjects">List of the spawned objects, for later deletion</param>
/// <param name="ignoreCost">toggle for whether cost should be ignored during pathfinding</param>
public class PathFinder : MonoBehaviour {
    private MapGraph graph; 
    private MapGraph.MapNode fromNode = null; //saves start-node
    private MapGraph.MapNode toNode = null; //saves target-node    
    public ColliderManager cManager; //for getting highlighted nodes
    private List<GameObject> markerObjects = new List<GameObject>(); //list of the spawned objects, for later deletion
    private bool ignoreCost = false;
    private bool showCostRules = false;
    private GUIStyle guiStyle = new GUIStyle(); //for OnGUI()

    public void Start() {
        guiStyle.fontSize = 25;
        guiStyle.fontStyle = new FontStyle();
    }
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
                GetShortestPath(); 
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
        if (Input.GetKeyDown(KeyCode.F)) { //toggle rule textbox
            showCostRules = !showCostRules;
        }
        if (Input.GetKeyDown(KeyCode.T)) { //toggle cost-ignorance
            ignoreCost = !ignoreCost;
            if (fromNode != null && toNode != null) {
                List<MapGraph.MapNode> path = GetShortestPath();
            }
        }
    }
    /// <summary>
    /// deletes all generated markers by destroying their gameobjects; used for cleanup of old pathfinding
    /// </summary>
    private void ClearMarkers() { 
        foreach (GameObject obj in markerObjects) {
            DestroyImmediate(obj);
        }
        markerObjects.Clear();
    }
    /// <summary>
    /// Spawns a marker above each node inside the given list
    /// </summary>
    /// <param name="nodes">A list of nodes that you want markers on. Markers will be refencered in markerObjects. Should be a path generated by GetShortestPath()</param>
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
    /// <summary>
    /// finds shortest path between fromNode and toNode, for now also displays it, return isnt used.
    /// </summary>
    /// <returns>List of the nodes making up the shortest path, including stand and end</returns>
    private List<MapGraph.MapNode> GetShortestPath() { //
        Debug.Log("PATHFINDER: Calculating path...");
        foreach(var node in graph.nodesByCenterPosition.Values) { //reset 
            node.ResetDijkstra();
        }
        if(ignoreCost) DijkstraSearchIgnoreCost();
        else DijkstraSearch();
        var shortestPath = new List<MapGraph.MapNode>();
        shortestPath.Add(toNode);
        BuildShortestPath(shortestPath, toNode);
        shortestPath.Reverse();     
        SpawnMarkers(shortestPath);
        //ShortestPathToString(shortestPath);
        return shortestPath;
    }
    /// <summary>
    /// Debug.Log()'s the shortest path
    /// </summary>
    /// <param name="shortestPath">A list of nodes that you want stringed. Should be a path generated by GetShortestPath()</param>
    /// <returns>returns the same string it Logs</returns>
    private string ShortestPathToString(List<MapGraph.MapNode> shortestPath) {
        string output = "Coordinates: ";
        foreach (var node in shortestPath) {
            output += "(" + node.centerPoint.x + " " + node.centerPoint.z + "), ";
        }
        Debug.Log(output);
        return output;
    }
    /// <summary>
    /// Sorts the nodes according to the Dijkstra variables, calls itself several times.
    /// </summary>
    /// <param name="list">toNode should be the only element inside this list in preparation. shortest path is assembled inside it.</param>
    /// <param name="node">should also be set to toNode. </param>
    private void BuildShortestPath(List<MapGraph.MapNode> list, MapGraph.MapNode node) {
        if (node.nearestToStart == null) return; //should occure once we arrive at fromNode
        list.Add(node.nearestToStart);
        BuildShortestPath(list, node.nearestToStart);
    }
    /// <summary>
    /// Assigns each node the Dijkstra related variables
    /// </summary>
    private void DijkstraSearch() {
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
    /// <summary>
    /// Assigns each node the Dijkstra related variables, assuming the cost of all nodes is 1
    /// </summary>
    private void DijkstraSearchIgnoreCost() {
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
                if (childNode.minCostToStart == 99 || node.minCostToStart + 1 < childNode.minCostToStart) {
                    childNode.minCostToStart = node.minCostToStart + 1;
                    childNode.nearestToStart = node;
                    if (!prioQueue.Contains(childNode)) prioQueue.Add(childNode);
                }
            }
            node.visited = true;
            if (node == toNode) return;
        } while (prioQueue.Any());
    }
    /// <summary>
    /// Draws tutorial text on the top-left
    /// </summary>
    private void OnGUI() {
        if(ignoreCost) {
            GUI.Label(new Rect(10, 90, 200, 120), "Q - Set start node \n" +
                                                  "E - Set target node \n" +
                                                  "R - Reset markers \n" +
                                                  "F - Toggle Cost Rules \n" + 
                                                  "T - Toggle Cost-ignorance \n" +
                                                  "Cost is being ignored");
        } else {
            GUI.Label(new Rect(10, 90, 200, 120), "Q - Set start node \n" +
                                                 "E - Set target node \n" +
                                                 "R - Reset markers \n" +
                                                 "F - Toggle Cost Rules \n" +
                                                 "T - Toggle Cost-ignorance");
        }      
        if(showCostRules) {
            GUI.Label(new Rect(210, 50, 400, 120), "1-Cost Types: Grass, Steppe, Sand \n" +
                                                   "2-Cost Types: Forest, PineForest, SaltWater, FreshWater \n" +
                                                   "3-Cost Types: Mountain, Highland \n" +
                                                   "4-Cost Types: Snow \n" +
                                                   "Secondary type CoastalCliff adds +1 cost. \n" +
                                                   "Secondary type CoastalWaters adds +2 cost.",
                                                   guiStyle);

        }
    }
}
