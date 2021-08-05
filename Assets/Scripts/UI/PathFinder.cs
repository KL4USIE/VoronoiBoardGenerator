using System.Collections;
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
    private GameObject fromMarker; 
    private GameObject toMarker;
    public ColliderManager cManager; //for getting highlighted nodes
    private List<GameObject> markerObjects = new List<GameObject>(); //list of the spawned objects, for later deletion
    private bool ignoreCost = false;
    private bool showCostRules = false;
    private GUIStyle guiStyleLarge = new GUIStyle(); //for OnGUI()
    private GUIStyle guiStyleSmall = new GUIStyle(); //for OnGUI()
    private int pathFinderBudget = 0;
    public Material redMat;
    private List<MapGraph.MapNode> shortestPath = new List<MapGraph.MapNode>();

    public void Start() {
        guiStyleLarge.fontSize = 30;
        guiStyleLarge.fontStyle = new FontStyle();
        guiStyleLarge.normal.textColor = new Color(1, 1, 1);
        guiStyleSmall.fontSize = 20;
        guiStyleSmall.fontStyle = new FontStyle();
        guiStyleSmall.normal.textColor = new Color(1, 1, 1);
    }
    public void SetGraph(MapGraph graph) {
        this.graph = graph;
    }
    void Update() {
        if(Input.GetKeyDown(KeyCode.Q)) { //set start    
            fromNode = cManager.GetActiveNode();
            SpawnMarker(fromNode, true);
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
            SpawnMarker(toNode, false);
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
        if (Input.GetKeyDown(KeyCode.Alpha1)) { //Subtract 1 from budget
            if(pathFinderBudget > 0) {
                pathFinderBudget--;
                if (fromNode != null && toNode != null) {
                    List<MapGraph.MapNode> budgetPath = BudgetPath(shortestPath, pathFinderBudget); //budgeting
                    SpawnMarkers(budgetPath);
                }
            }         
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { //Add 1 to budget
            pathFinderBudget++;
            if (fromNode != null && toNode != null) {
                List<MapGraph.MapNode> budgetPath = BudgetPath(shortestPath, pathFinderBudget); //budgeting
                SpawnMarkers(budgetPath);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { //Set budget to 10
            pathFinderBudget = 10;
            if (fromNode != null && toNode != null) {
                List<MapGraph.MapNode> budgetPath = BudgetPath(shortestPath, pathFinderBudget); //budgeting
                SpawnMarkers(budgetPath);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { //Set budget to 25
            pathFinderBudget = 25;
            if (fromNode != null && toNode != null) {
                List<MapGraph.MapNode> budgetPath = BudgetPath(shortestPath, pathFinderBudget); //budgeting
                SpawnMarkers(budgetPath);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { //Set budget to 50
            pathFinderBudget = 50;
            if (fromNode != null && toNode != null) {
                List<MapGraph.MapNode> budgetPath = BudgetPath(shortestPath, pathFinderBudget); //budgeting
                SpawnMarkers(budgetPath);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha0)) { //Set budget to 0(ignoring budget)
            pathFinderBudget = 0;
            if (fromNode != null && toNode != null) {
                List<MapGraph.MapNode> budgetPath = BudgetPath(shortestPath, pathFinderBudget); //budgeting
                SpawnMarkers(budgetPath);
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
    private void SpawnMarkers(List<MapGraph.MapNode> nodes) {
        ClearMarkers(); //to start, deletes the old markers
        foreach(var node in nodes) {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.parent = this.transform; //sets as child in scene hierarchy
            go.layer = 5; //UI Layer
            go.transform.position = new Vector3(node.centerPoint.x, 7, node.centerPoint.z);
            go.transform.localScale = new Vector3(4, 4, 4); //scale of marker
            markerObjects.Add(go);
        }      
    }
    /// <summary>
    /// Spawns a marker above the given node and references it based on setFrom
    /// </summary>
    /// <param name="node">A node that you want a marker on. Marker will be refencered in toMarker or FromMarker, based on setFrom. Should be either fromNode or toNode</param>
    /// <param name="setFrom"> true to mark fromNode, false to mark toNode </param>
    private void SpawnMarker(MapGraph.MapNode node, bool setFrom) {
        //ClearMarkers(); //to start, deletes the old markers
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.parent = this.transform; //sets as child in scene hierarchy
        go.layer = 5; //UI Layer
        go.transform.position = new Vector3(node.centerPoint.x, 7, node.centerPoint.z);
        go.transform.localScale = new Vector3(3.5f, 3.5f, 3.5f); //scale of marker
        go.GetComponent<MeshRenderer>().material = redMat;
        if(setFrom) {
            DestroyImmediate(fromMarker);
            fromMarker = go;
        } else {
            DestroyImmediate(toMarker);
            toMarker = go;
        }
        
    }
    /// <summary>
    /// finds shortest path between fromNode and toNode, for now also displays it, return isnt used.
    /// </summary>
    /// <returns>List of the nodes making up the shortest path, including stand and end</returns>
    private List<MapGraph.MapNode> GetShortestPath() { 
        Debug.Log("PATHFINDER: Calculating path...");
        foreach(var node in graph.nodesByCenterPosition.Values) { //reset 
            node.ResetDijkstra();
        }
        if(ignoreCost) DijkstraSearchIgnoreCost(); else DijkstraSearch();
        shortestPath.Clear();
        shortestPath.Add(toNode);
        BuildShortestPath(shortestPath, toNode);
        shortestPath.Reverse();
        if (pathFinderBudget > 0) {
            List<MapGraph.MapNode> budgetPath = BudgetPath(shortestPath, pathFinderBudget); //budgeting
            SpawnMarkers(budgetPath);
            return budgetPath;
        }
        SpawnMarkers(shortestPath);       
        //ShortestPathToString(shortestPath);
        return shortestPath;
    }
    /// <summary>
    /// cuts off the given path according to budget
    /// </summary>
    /// <returns>List of the nodes making up the shortest path, including stand and end</returns>
    private List<MapGraph.MapNode> BudgetPath(List<MapGraph.MapNode> shortestPath, int budget) { //WARNING: ugly code below
        if (budget == 0) return shortestPath;
        LinkedList<MapGraph.MapNode> linkedSPath = new LinkedList<MapGraph.MapNode>(shortestPath);
        LinkedListNode<MapGraph.MapNode> currentNode = linkedSPath.First;
        while(budget > 0) { //while budget left         
                if (budget - currentNode.Next.Value.cost < 0 && !ignoreCost) { //if next node not affordable and calculate cost
                    while (currentNode.Next != null) { //remove unreachable nodes from path
                        linkedSPath.RemoveLast();
                    }
                    return new List<MapGraph.MapNode>(linkedSPath);                            
                } else { //if next node affordable or ignoreCost
                    if(currentNode.Next == linkedSPath.Last) { //if target node reached
                        return new List<MapGraph.MapNode>(linkedSPath);
                    } else { //advance to next node
                        currentNode = currentNode.Next;
                        if(ignoreCost) { budget--; } else { budget -= currentNode.Value.cost; }                   
                    }          
                }   
            
        }
        while (currentNode.Next != null) { //remove unreachable nodes from path
            linkedSPath.RemoveLast();
        }
        return new List<MapGraph.MapNode>(linkedSPath);
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
            GUI.Label(new Rect(10, 80, 200, 120), "Pathfinding Budget: " + pathFinderBudget + "\n" +
                                                  "1,2,3,4,5,0 - Control budget \n" +
                                                  "Q - Set start node \n" +
                                                  "E - Set target node \n" +
                                                  "R - Reset markers \n" +
                                                  "F - Toggle Cost Rules \n" + 
                                                  "T - Toggle Cost-ignorance \n" +
                                                  "Cost is being ignored",
                                                  guiStyleSmall);
        } else {
            GUI.Label(new Rect(10, 80, 200, 120), "Pathfinding Budget: " + pathFinderBudget + "\n" +
                                                  "1,2,3,4,5,0 - Control budget \n" +
                                                  "Q - Set start node \n" +
                                                  "E - Set target node \n" +
                                                  "R - Reset markers \n" +
                                                  "F - Toggle Cost Rules \n" +
                                                  "T - Toggle Cost-ignorance",
                                                  guiStyleSmall);
        }      
        if(showCostRules) {
            GUI.Label(new Rect(270, 80, 400, 120), "1-Cost Types: Grass, Steppe, Sand \n" +
                                                   "2-Cost Types: Forest, PineForest, SaltWater, FreshWater \n" +
                                                   "3-Cost Types: Mountain, Highland \n" +
                                                   "4-Cost Types: Snow \n" +
                                                   "Secondary type CoastalCliff adds +1 cost. \n" +
                                                   "Secondary type CoastalWaters adds +2 cost.",
                                                   guiStyleLarge);

        }
    }
}
