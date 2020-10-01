using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Delaunay;
//--Class purpose-- 
//Generates types for each node.

public static class MapGenerator {
    public static void GenerateMap(MapGraph graph, int landConnectionCycles, int genVersion, int mountainReductionCycles, ColliderManager cManager) {
        switch (genVersion) {
            case 1:
                GenerateV1(graph, landConnectionCycles);
                break;
            case 2:
                GenerateV2(graph, mountainReductionCycles);
                break;
            case 3:
                GenerateV3(graph, mountainReductionCycles, cManager);
                break;
            default:
                Console.WriteLine("Default case");
                break;
        }
        
        

/*
        SetNodesToGrass(graph);
        SetLowNodesToWater(graph, 0.2f);
        SetEdgesToWater(graph);
        FillOcean(graph);
        SetBeaches(graph);
        FindRivers(graph, 12f);
        CreateLakes(graph);
        AddMountains(graph);
        AverageCenterPoints(graph);
        FindCities(graph, 0.5f, 8f, 3);
*/
       
    }
    //ADDED BY NOTH
    private static void GenerateV1(MapGraph graph, int landConnectionCycles) {
        SetAllUndetermined(graph, null);
        FindWaterNodesV1(graph, landConnectionCycles);
        FindMountainNodes(graph, 0);
        FindSnowNodesV1(graph);
        FindBeachNodes(graph);
    }
    private static void GenerateV2(MapGraph graph, int mountainReductionCycles) { //reworked Water generation
        SetAllUndetermined(graph, null);
        FindWaterNodesV2(graph);
        FindMountainNodes(graph, mountainReductionCycles);
        FindSnowNodesV1(graph);
        FindDesertNodes(graph);
    }
    private static void GenerateV3(MapGraph graph,int mountainReductionCycles, ColliderManager cManager) { //reworked Mountain generation
        SetAllUndetermined(graph, cManager);
        FindWaterNodesV2(graph);
        FindMountainNodesV3(graph, mountainReductionCycles);
        FindSnowNodesV2(graph);
        FindDesertNodes(graph);
        FindVegetationNodes(graph);
        FindSecondaryTypes(graph);
        FindRivers(graph, 5);
    }
    private static void SetAllUndetermined(MapGraph graph, ColliderManager cManager) {
        cManager.ClearColliders();
        foreach(var node in graph.nodesByCenterPosition.Values) {
            node.nodeType = MapGraph.MapNodeType.Undetermined;
            graph.undetNodes.Add(node);
            if(cManager != null) {
                cManager.AddCollider(node);
            }        
        }
    }   
    private static void FindWaterNodesV1(MapGraph graph, int landConnectionCycles) { //Places Water completely randomly, then optionally removes some thats sorrounded by land
        graph.waterNodes.Clear(); //Prepare for generation
        //List<MapGraph.MapNode> waterList = new List<MapGraph.MapNode>(); //moved to MapGraph
        foreach (MapGraph.MapNode node in graph.nodesByCenterPosition.Values) { //Set random water nodes
            if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.4) {
                node.nodeType = MapGraph.MapNodeType.SaltWater;
                graph.waterNodes.Add(node); //add water
                graph.undetNodes.Remove(node); //remove undet
            } else {
                graph.undetNodes.Add(node); //add water
                graph.undetNodes.Remove(node); //remove undet
            }
        }
        ReduceWaterNodes(graph, landConnectionCycles);
        foreach(MapGraph.MapNode waterNode in graph.waterNodes) { //Find freshwater lakes
            if (waterNode.nodeType != MapGraph.MapNodeType.SaltWater) break; //Important as to not revert land connections           
            if(CheckLake(graph, waterNode)) {
                waterNode.nodeType = MapGraph.MapNodeType.FreshWater;                
            }
        }           
    }
    private static void ReduceWaterNodes(MapGraph graph, int landConnectionCycles) {
        for (int i = 0; i < landConnectionCycles; i++) { //remove water sorrounded by land to make landmasses more coherent, impacted by Land Connection Cycles
            foreach (MapGraph.MapNode waterNode in new List<MapGraph.MapNode>(graph.waterNodes)) {
                int undetNeighbourCount = 0;
                foreach (MapGraph.MapNode neighbourNode in waterNode.GetNeighborNodes()) {
                    if (neighbourNode.nodeType == MapGraph.MapNodeType.Undetermined) {
                        undetNeighbourCount++;
                    }
                }
                if (undetNeighbourCount >= 5) {
                    waterNode.nodeType = MapGraph.MapNodeType.Undetermined;
                    graph.waterNodes.Remove(waterNode); //cant remove while list is being looped, Update: Copyconstructor to the rescue!    
                    graph.undetNodes.Add(waterNode);
                }
            }
        }
    }
    private static Boolean CheckLake(MapGraph graph, MapGraph.MapNode node) {
        foreach (MapGraph.MapNode neighbourNode in node.GetNeighborNodes()) {
            if (neighbourNode.nodeType == MapGraph.MapNodeType.SaltWater) {
                return false;
            }
        }
        return true;      
    }
    private static void FindWaterNodesV2(MapGraph graph) { //Places Water by Setting very few "Mother"-nodes, who then convert sorrounding nodes, creating ocean-like watermasses
        graph.waterNodes.Clear(); //Prepare for generation
        foreach (MapGraph.MapNode node in graph.nodesByCenterPosition.Values) { //Saltwater generation: Set random water node
            if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.020) { //0.02 for land-dominated; 0.025 for water-dominated              
                node.nodeType = MapGraph.MapNodeType.SaltWater;
                graph.waterNodes.Add(node); //add water
                graph.undetNodes.Remove(node); //remove undet
                //WaterNodeRecursion(graph, node.GetNeighborNodes(), 1.0);
                AnyNodeRecursion(graph, node.GetNeighborNodes(), 1.0, MapGraph.MapNodeType.SaltWater, graph.waterNodes, 0.3); //begin recursion                                  
            }
        }
        foreach (MapGraph.MapNode node in graph.nodesByCenterPosition.Values) { //Freshwater generation
            if (node.nodeType == MapGraph.MapNodeType.Undetermined) {
                if (CheckLake(graph, node) && UnityEngine.Random.Range(0.0f, 1.0f) < 0.03) { //Randomly place FreshWater origin
                    node.nodeType = MapGraph.MapNodeType.FreshWater;
                    graph.waterNodes.Add(node); //add water
                    graph.undetNodes.Remove(node); //remove undet
                    foreach (MapGraph.MapNode neighbourNode in node.GetNeighborNodes()) { //Expand freshwater
                        if (CheckLake(graph, neighbourNode) && UnityEngine.Random.Range(0.0f, 1.0f) < 0.025) { //
                            neighbourNode.nodeType = MapGraph.MapNodeType.FreshWater;
                            graph.waterNodes.Add(neighbourNode); //add water   
                            graph.undetNodes.Remove(neighbourNode); //remove undet
                        }
                    }
                }
            }
        }
    }
    private static void WaterNodeRecursion(MapGraph graph, List<MapGraph.MapNode> nodeList, double propability) {
        if (propability > 0.0) { //just optimization
            foreach (MapGraph.MapNode node in nodeList) {
                if (node.nodeType != MapGraph.MapNodeType.SaltWater && UnityEngine.Random.Range(0.0f, 1.0f) < propability) {
                    node.nodeType = MapGraph.MapNodeType.SaltWater;
                    graph.waterNodes.Add(node); //add water
                    graph.undetNodes.Remove(node); //remove undet
                    WaterNodeRecursion(graph, node.GetNeighborNodes(), (propability - 0.3));
                }
            }
        }
        return;
    }
    private static void FindMountainNodes(MapGraph graph, int mountainReductionCycles) {
        graph.mountainNodes.Clear();
        foreach (var node in graph.nodesByCenterPosition.Values) {
            if(node.nodeType == MapGraph.MapNodeType.Undetermined) {
                int undetNeighbourCount = 0;
                foreach (MapGraph.MapNode neighbourNode in node.GetNeighborNodes()) {
                    if (neighbourNode.nodeType == MapGraph.MapNodeType.Undetermined || neighbourNode.nodeType == MapGraph.MapNodeType.Mountain) {
                        undetNeighbourCount++;
                    }
                }
                if (undetNeighbourCount >= 6 && UnityEngine.Random.Range(0.0f, 1.0f) < 0.8) { //Randomness
                    node.nodeType = MapGraph.MapNodeType.Mountain;
                    graph.mountainNodes.Add(node);
                    graph.undetNodes.Remove(node);
                }/*
                if (undetNeighbourCount >= 6) { //NO Randomness
                    node.nodeType = MapGraph.MapNodeType.Mountain;
                    graph.mountainNodes.Add(node);
                }*/
            }
        }
        ReduceMountains(graph, mountainReductionCycles);
    }
    private static void ReduceMountains(MapGraph graph, int reductionCycles) {
        for (int i = 0; i < reductionCycles; i++) { //remove water sorrounded by land to make landmasses more coherent, impacted by Land Connection Cycles
            foreach (MapGraph.MapNode mountainNode in new List<MapGraph.MapNode>(graph.mountainNodes)) {
                int mountainNeighbourCount = 0;
                foreach (MapGraph.MapNode neighbourNode in mountainNode.GetNeighborNodes()) {
                    if (neighbourNode.nodeType == MapGraph.MapNodeType.Mountain || neighbourNode.nodeType == MapGraph.MapNodeType.Snow) {
                        mountainNeighbourCount++;
                    }
                }
                if (mountainNeighbourCount <= 1) {
                    mountainNode.nodeType = MapGraph.MapNodeType.Undetermined;
                    graph.mountainNodes.Remove(mountainNode); //cant remove while list is being looped, Update: Copyconstructor to the rescue!    
                    graph.undetNodes.Add(mountainNode);
                }
            }
        }
    }
    private static void FindMountainNodesV2(MapGraph graph) { // causes issues, should be scrapped, instead writing V3
        foreach (var node in new List<MapGraph.MapNode>(graph.undetNodes)) {
            if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.025 && (node.centerPoint.x < graph.GetCenter().x * 1.95 && node.centerPoint.z < graph.GetCenter().y * 1.95)) { //0.02 for land-dominated; 0.025 for water-dominated              
                node.nodeType = MapGraph.MapNodeType.Mountain;
                graph.mountainNodes.Add(node); //add mountain
                graph.undetNodes.Remove(node); //remove undet
                //AnyNodeRecursion(graph, node.GetNeighborNodes(), 1.0, MapGraph.MapNodeType.Mountain, graph.mountainNodes, 0.4); //begin recursion    
                MountainNodeRecursion(graph, node.GetNeighborNodes(), 1.0);
            }
        }
    }
    private static void MountainNodeRecursion(MapGraph graph, List<MapGraph.MapNode> nodeList, double propability) {
        if (propability > 0.0) { //just optimization
            foreach (MapGraph.MapNode node in nodeList) {
                //if (node.nodeType != MapGraph.MapNodeType.Mountain && UnityEngine.Random.Range(0.0f, 1.0f) < propability) {
                if (node.nodeType == MapGraph.MapNodeType.Undetermined && UnityEngine.Random.Range(0.0f, 1.0f) < propability) {
                    node.nodeType = MapGraph.MapNodeType.Mountain;
                    graph.mountainNodes.Add(node); //add water
                    graph.undetNodes.Remove(node); //remove undet
                    MountainNodeRecursion(graph, node.GetNeighborNodes(), (propability - 0.45));
                }
            }
        }
        return;
    }
    private static void FindMountainNodesV3(MapGraph graph, int mountainConnectionCycles) {
        graph.mountainNodes.Clear();
        foreach (var node in new List<MapGraph.MapNode>(graph.undetNodes)) {
            int undetNeighbourCount = 0;
            foreach (MapGraph.MapNode neighbourNode in node.GetNeighborNodes()) {
                if (neighbourNode.nodeType == MapGraph.MapNodeType.Undetermined || neighbourNode.nodeType == MapGraph.MapNodeType.Mountain) {
                    undetNeighbourCount++;
                }
            }
            if (undetNeighbourCount >= 6 && UnityEngine.Random.Range(0.0f, 1.0f) < 0.8) { //Randomness
                node.nodeType = MapGraph.MapNodeType.Mountain;
                graph.mountainNodes.Add(node);
                graph.undetNodes.Remove(node);
            }/*
            if (undetNeighbourCount >= 6) { //NO Randomness
                node.nodeType = MapGraph.MapNodeType.Mountain;
                graph.mountainNodes.Add(node);
            }*/
        }
        ReduceMountains(graph, mountainConnectionCycles);
        ConnectMountains(graph, mountainConnectionCycles);
    }
    private static void ConnectMountains(MapGraph graph, int connectionCycles) {
        for (int i = 0; i < connectionCycles; i++) { //remove water sorrounded by land to make landmasses more coherent, impacted by Land Connection Cycles
            foreach (MapGraph.MapNode undetNode in new List<MapGraph.MapNode>(graph.undetNodes)) {
                int mountainNeighbourCount = 0;
                foreach (MapGraph.MapNode neighbourNode in undetNode.GetNeighborNodes()) {
                    if (neighbourNode.nodeType == MapGraph.MapNodeType.Mountain || neighbourNode.nodeType == MapGraph.MapNodeType.Snow) {
                        mountainNeighbourCount++;
                    }
                }
                if (mountainNeighbourCount >= 4) {
                    undetNode.nodeType = MapGraph.MapNodeType.Mountain;
                    graph.mountainNodes.Add(undetNode); //cant remove while list is being looped, Update: Copyconstructor to the rescue!    
                    graph.undetNodes.Remove(undetNode);
                }
            }
        }
    }
    private static void FindSnowNodesV1(MapGraph graph) {
        graph.snowNodes.Clear();
        foreach (var node in graph.nodesByCenterPosition.Values) {
            if(node.nodeType != MapGraph.MapNodeType.FreshWater || node.nodeType != MapGraph.MapNodeType.SaltWater) {               
                if (CheckSnowV1(graph, node)) {
                    node.nodeType = MapGraph.MapNodeType.Snow;
                    graph.snowNodes.Add(node);
                    graph.undetNodes.Remove(node);
                }                 
            }
        }
        foreach(var node in new List<MapGraph.MapNode>(graph.snowNodes)) { //Essentially another Iteration of the above, saving time by only going through neighbours of identified snow nodes. 
            //Useful in case a newly created snow node created conditions for a node that has already been checked to also become a snow node. Same deal as V1 Water generation
            foreach(var neighbourNode in node.GetNeighborNodes()) { 
                if (neighbourNode.nodeType != MapGraph.MapNodeType.FreshWater || neighbourNode.nodeType != MapGraph.MapNodeType.SaltWater) {
                    if (CheckSnowV1(graph, neighbourNode)) {
                        neighbourNode.nodeType = MapGraph.MapNodeType.Snow;
                        graph.snowNodes.Add(neighbourNode);
                        graph.undetNodes.Remove(neighbourNode);
                    }
                }
            }
        }
    }
    private static Boolean CheckSnowV1(MapGraph graph, MapGraph.MapNode node) {
        foreach (MapGraph.MapNode neighbourNode in node.GetNeighborNodes()) {
            if (!(neighbourNode.nodeType == MapGraph.MapNodeType.Mountain || neighbourNode.nodeType == MapGraph.MapNodeType.Snow)) { //if any neighbours are neither Mountain nor Snow
                return false;
            }
        }
        return true;
    }
    private static void FindSnowNodesV2(MapGraph graph) {
        graph.snowNodes.Clear();
        foreach (var node in new List<MapGraph.MapNode>(graph.mountainNodes)) {
            if (node.nodeType != MapGraph.MapNodeType.FreshWater || node.nodeType != MapGraph.MapNodeType.SaltWater) {
                if (CheckSnowV2(graph, node)) {
                    node.nodeType = MapGraph.MapNodeType.Snow;
                    graph.snowNodes.Add(node);
                    graph.mountainNodes.Remove(node);
                }
            }
        }
        foreach (var node in new List<MapGraph.MapNode>(graph.snowNodes)) { //Essentially another Iteration of the above, saving time by only going through neighbours of identified snow nodes. 
            //Useful in case a newly created snow node created conditions for a node that has already been checked to also become a snow node. Same deal as V1 Water generation
            foreach (var neighbourNode in node.GetNeighborNodes()) {
                if (neighbourNode.nodeType != MapGraph.MapNodeType.FreshWater || neighbourNode.nodeType != MapGraph.MapNodeType.SaltWater) {
                    if (CheckSnowV2(graph, neighbourNode)) {
                        neighbourNode.nodeType = MapGraph.MapNodeType.Snow;
                        graph.snowNodes.Add(neighbourNode);
                        graph.undetNodes.Remove(neighbourNode);
                    }
                }
            }
        }
        foreach(var snowNode in graph.snowNodes) {
            if (snowNode.centerPoint.z <= (graph.GetCenter().z * 0.6)) {
                snowNode.nodeType = MapGraph.MapNodeType.Highland;
            }
        }
    }   
    private static Boolean CheckSnowV2(MapGraph graph, MapGraph.MapNode node) {
        foreach (MapGraph.MapNode neighbourNode in node.GetNeighborNodes()) {
            if (!(neighbourNode.nodeType == MapGraph.MapNodeType.Mountain || neighbourNode.nodeType == MapGraph.MapNodeType.Snow)) { //if any neighbours are neither Mountain nor Snow, dont allow snow
                return false;
            }
            foreach (MapGraph.MapNode dNeighbourNode in neighbourNode.GetNeighborNodes()) { //if any neighbours of neighbours (...), make it less likely to allow snow; this means that snow will usually be sorrounded by 2 layers of mountain, but not always
                if (dNeighbourNode.nodeType != MapGraph.MapNodeType.Mountain && dNeighbourNode.nodeType != MapGraph.MapNodeType.Snow && UnityEngine.Random.Range(0.0f, 1.0f) <= 0.1) {
                    //Debug.Log("doublefalse");
                    return false;
                }
            }
        }
        return true;
    }
    private static void FindBeachNodes(MapGraph graph) { //Beaches scrapped, making deserts instead; this function is not being called
        foreach (var node in new List<MapGraph.MapNode>(graph.undetNodes)) {           
            int waterNeighbours = 0;
            foreach (var neighbourNode in node.GetNeighborNodes()) {
                if (neighbourNode.nodeType == MapGraph.MapNodeType.FreshWater || neighbourNode.nodeType == MapGraph.MapNodeType.SaltWater) {
                    waterNeighbours++;
                }
            }
            if (waterNeighbours >= 4) {
                node.nodeType = MapGraph.MapNodeType.Sand;
                graph.undetNodes.Remove(node);
            }
        }
    }
    private static void FindDesertNodes(MapGraph graph) {
        foreach (var node in new List<MapGraph.MapNode>(graph.undetNodes)) {
            if(node.centerPoint.z <= (graph.GetCenter().z * 0.6) && UnityEngine.Random.Range(0.0f, 1.0f) < 0.06) { //if below horizontal center, meaning Deserts will only generate in the south; propability for desert spawn
                node.nodeType = MapGraph.MapNodeType.Sand;
                graph.sandNodes.Add(node); //add sand
                graph.undetNodes.Remove(node); //remove undet
                SandNodeRecursion(graph, node.GetNeighborNodes(), 1.0); //begin recursion
            }
        }
    }
    private static void SandNodeRecursion(MapGraph graph, List<MapGraph.MapNode> nodeList, double propability) {
        if (propability > 0.0) { //just optimization
            foreach (MapGraph.MapNode node in nodeList) {
                if (node.nodeType == MapGraph.MapNodeType.Undetermined && UnityEngine.Random.Range(0.0f, 1.0f) < propability) {
                    node.nodeType = MapGraph.MapNodeType.Sand;
                    graph.sandNodes.Add(node); //add water
                    graph.undetNodes.Remove(node); //remove undet
                    SandNodeRecursion(graph, node.GetNeighborNodes(), (propability - 0.3));                   
                }
            }
        }
        return;
    }
    private static void FindVegetationNodes(MapGraph graph) {
        foreach (var node in new List<MapGraph.MapNode>(graph.undetNodes)) {
            if(UnityEngine.Random.Range(0.0f, 1.0f) <= 0.5) { //decide between forest and grass
                if(node.centerPoint.z <= (graph.GetCenter().z * 0.6)) { //southern grass becomes steppe
                    node.nodeType = MapGraph.MapNodeType.Steppe;
                } else {
                    node.nodeType = MapGraph.MapNodeType.Grass;
                }              
            } else {
                if(node.centerPoint.z >= (graph.GetCenter().z * 1.4)) { //northern forest becomes pine forest
                    node.nodeType = MapGraph.MapNodeType.PineForest;
                } else {
                    node.nodeType = MapGraph.MapNodeType.Forest;
                }             
            }
            graph.undetNodes.Remove(node);
        }
    }

    private static void AnyNodeRecursion(MapGraph graph, List<MapGraph.MapNode> nodeList, double propability, MapGraph.MapNodeType recursionType, List<MapGraph.MapNode> typeList, double propabilityReduction) {
        if (propability > 0.0) { //just optimization
            foreach (MapGraph.MapNode node in nodeList) {
                if (node.nodeType != recursionType && UnityEngine.Random.Range(0.0f, 1.0f) < propability) {
                    node.nodeType = recursionType;
                    typeList.Add(node); //add water
                    graph.undetNodes.Remove(node); //remove undet
                    AnyNodeRecursion(graph, node.GetNeighborNodes(), (propability - propabilityReduction), recursionType, typeList, propabilityReduction);
                }
            }
        }
        return;
    }
    private static void FindSecondaryTypes(MapGraph graph) {
        SetWaterSecondaries(graph); 
    }
    private static void SetWaterSecondaries(MapGraph graph) {
    //Sets secondary types Oasis, Cliff and Coast, combined into one iteration for performance since they are all water-based
        foreach(var node in graph.waterNodes) {
            int sandNeighbours = 0;          
            foreach(var neighbourNode in node.GetNeighborNodes()) {
                if(neighbourNode.nodeType != MapGraph.MapNodeType.SaltWater && neighbourNode.nodeType != MapGraph.MapNodeType.FreshWater) {
                    int landNeighbours = 0;
                    foreach(var doubleNeighbour in neighbourNode.GetNeighborNodes()) { //for Island and Peninsula
                        if(doubleNeighbour.nodeType != MapGraph.MapNodeType.SaltWater) landNeighbours++;                       
                    }                  
                    if(neighbourNode.nodeType == MapGraph.MapNodeType.Mountain) { //set CoastalCliff
                        neighbourNode.secondType = MapGraph.SecondType.CoastalCliff;
                    } else { //set Coast
                        neighbourNode.secondType = MapGraph.SecondType.Coast;
                    }                    
                }
                if(neighbourNode.nodeType == MapGraph.MapNodeType.Sand) sandNeighbours++;//oasis                              
            }
            if(sandNeighbours >= 4) { //set Oasis
                node.secondType = MapGraph.SecondType.Oasis;
            }
        }
    }
    //NOT ADDED BY NOTH
    private static void SetEdgesToWater(MapGraph graph) {
        foreach (var node in graph.nodesByCenterPosition.Values) {
            if (node.IsEdge()) node.nodeType = MapGraph.MapNodeType.FreshWater;
        }
    }

    private static void AverageCenterPoints(MapGraph graph) {
        foreach (var node in graph.nodesByCenterPosition.Values) {
            node.centerPoint = new Vector3(node.centerPoint.x, node.GetCorners().Average(x => x.position.y), node.centerPoint.z);
        }
    }

    private static void AddMountains(MapGraph graph) {
        foreach (var node in graph.nodesByCenterPosition.Values) {
            if (node.GetElevation() > 15f || node.GetHeightDifference() > 7f) {
                node.nodeType = MapGraph.MapNodeType.Mountain;
            }
            if (node.GetElevation() > 17f) {
                node.nodeType = MapGraph.MapNodeType.Snow;
            }
        }
    }

    private static void CreateLakes(MapGraph graph) {
        foreach (var node in graph.nodesByCenterPosition.Values) {
            var edges = node.GetEdges();
            if (!edges.Any(x => x.water == 0)) {
                CreateLake(node);
            }
        }
    }

    private static void FindRivers(MapGraph graph, float minElevation) {
        var riverCount = 0;
        foreach (var node in graph.nodesByCenterPosition.Values) {
            var elevation = node.GetElevation();
            if (elevation > minElevation) {
                var waterSource = node.GetLowestCorner();
                var lowestEdge = waterSource.GetDownSlopeEdge();
                if (lowestEdge == null) continue;
                CreateRiver(graph, lowestEdge);
                riverCount++;
            }
        }
        //Debug.Log(string.Format("{0} rivers drawn", riverCount));
    }

    private static void CreateRiver(MapGraph graph, MapGraph.MapNodeHalfEdge startEdge) {
        bool heightUpdated = false;
        // Once a river has been generated, it tries again to see if a quicker route has been created.
        // This sets how many times we should go over the same river.
        var maxIterations = 1;
        var iterationCount = 0;

        // Make sure that the river generation code doesn't get stuck in a loop.
        var maxChecks = 100;
        var checkCount = 0;

        var previousRiverEdges = new List<MapGraph.MapNodeHalfEdge>();
        do {
            heightUpdated = false;

            var riverEdges = new List<MapGraph.MapNodeHalfEdge>();
            var previousEdge = startEdge;
            var nextEdge = startEdge;

            while (nextEdge != null) {
                if (checkCount >= maxChecks) {
                    Debug.LogError("Unable to find route for river. Maximum number of checks reached");
                    return;
                }
                checkCount++;

                var currentEdge = nextEdge;

                // We've already seen this edge and it's flowing back up itself.
                if (riverEdges.Contains(currentEdge) || riverEdges.Contains(currentEdge.opposite)) break;
                riverEdges.Add(currentEdge);
                currentEdge.AddWater();

                // Check that we haven't reached the sea
                if (currentEdge.destination.GetNodes().Any(x => x.nodeType == MapGraph.MapNodeType.SaltWater)) break;

                nextEdge = GetDownSlopeEdge(currentEdge, riverEdges);

                if (nextEdge == null && previousEdge != null) {
                    // We need to start carving a path for the river.
                    nextEdge = GetNewCandidateEdge(graph.GetCenter(), currentEdge, riverEdges, previousRiverEdges);

                    // If we can't get a candidate edge, then backtrack and try again
                    var previousEdgeIndex = riverEdges.Count - 1;
                    while (nextEdge == null || previousEdgeIndex == 0) {
                        previousEdge = riverEdges[previousEdgeIndex];
                        previousEdge.water--;
                        nextEdge = GetNewCandidateEdge(graph.GetCenter(), previousEdge, riverEdges, previousRiverEdges);
                        riverEdges.Remove(previousEdge);
                        previousEdgeIndex--;
                    }
                    if (nextEdge != null) {
                        if (nextEdge.previous.destination.position.y != nextEdge.destination.position.y) {
                            LevelEdge(nextEdge);
                            heightUpdated = true;
                        }
                    }
                    else {
                        // We've tried tunneling, backtracking, and we're still lost.
                        Debug.LogError("Unable to find route for river");
                    }
                }
                previousEdge = currentEdge;
            }
            if (maxIterations <= iterationCount) break;
            iterationCount++;

            // If the height was updated, we need to recheck the river again.
            if (heightUpdated) {
                foreach (var edge in riverEdges) {
                    if (edge.water > 0) edge.water--;
                }
                previousRiverEdges = riverEdges;
            }
        } while (heightUpdated);
    }

    private static void CreateLake(MapGraph.MapNode node) {
        var lowestCorner = node.GetLowestCorner();
        node.nodeType = MapGraph.MapNodeType.FreshWater;

        // Set all of the heights equal to where the water came in.
        SetNodeHeightToCornerHeight(node, lowestCorner);
    }

    private static void LevelEdge(MapGraph.MapNodeHalfEdge currentEdge) {
        currentEdge.destination.position = new Vector3(currentEdge.destination.position.x, currentEdge.previous.destination.position.y, currentEdge.destination.position.z);
    }

    private static MapGraph.MapNodeHalfEdge GetDownSlopeEdge(MapGraph.MapNodeHalfEdge source, List<MapGraph.MapNodeHalfEdge> seenEdges) {
        var corner = source.destination;

        var candidates = corner.GetEdges().Where(x =>
            x.destination.position.y < corner.position.y &&
            !seenEdges.Contains(x) &&
            x.opposite != null && !seenEdges.Contains(x.opposite) &&
            x.node.nodeType != MapGraph.MapNodeType.FreshWater &&
            x.opposite.node.nodeType != MapGraph.MapNodeType.FreshWater);

        // Make sure the river prefers to follow existing rivers
        var existingRiverEdge = candidates.FirstOrDefault(x => x.water > 0);
        if (existingRiverEdge != null) return existingRiverEdge;

        return candidates.OrderByDescending(x => x.GetSlopeAngle()).FirstOrDefault();
    }

    private static MapGraph.MapNodeHalfEdge GetNewCandidateEdge(Vector3 center, MapGraph.MapNodeHalfEdge source, List<MapGraph.MapNodeHalfEdge> seenEdges, List<MapGraph.MapNodeHalfEdge> previousEdges) {
        var corner = source.destination;

        var edges = corner.GetEdges().Where(x =>
            !seenEdges.Contains(x) &&
            x.opposite != null &&
            !seenEdges.Contains(x.opposite)).ToList();

        // Make sure the river prefers to follow existing rivers
        var existingRiverEdge = edges.FirstOrDefault(x => x.water > 0);
        if (existingRiverEdge != null) return existingRiverEdge;

        // Make the river prefer to follow previous iterations
        existingRiverEdge = edges.FirstOrDefault(x => previousEdges.Contains(x));
        if (existingRiverEdge != null) return existingRiverEdge;

        var awayFromCenterEdges = edges.Where(x => Vector3.Dot(x.destination.position - x.previous.destination.position, x.destination.position - center) >= 0);
        if (awayFromCenterEdges.Any()) edges = awayFromCenterEdges.ToList();
        return edges.OrderBy(x => x.destination.position.y).FirstOrDefault();
    }


    private static void SetNodeHeightToCornerHeight(MapGraph.MapNode node, MapGraph.MapPoint targetCorner) {
        foreach (var corner in node.GetCorners()) {
            corner.position = new Vector3(corner.position.x, targetCorner.position.y, corner.position.z);
        }
        node.centerPoint = new Vector3(node.centerPoint.x, targetCorner.position.y, node.centerPoint.z);
    }

    private static void FillOcean(MapGraph graph) {
        var startNode = graph.nodesByCenterPosition.FirstOrDefault(x => x.Value.IsEdge() && x.Value.nodeType == MapGraph.MapNodeType.FreshWater).Value;
        FloodFill(startNode, MapGraph.MapNodeType.FreshWater, MapGraph.MapNodeType.SaltWater);
    }

    private static void FloodFill(MapGraph.MapNode node, MapGraph.MapNodeType targetType, MapGraph.MapNodeType replacementType) {
        if (targetType == replacementType) return;
        if (node.nodeType != targetType) return;
        node.nodeType = replacementType;
        foreach (var neighbor in node.GetNeighborNodes()) {
            FloodFill(neighbor, targetType, replacementType);
        }
    }

    private static void SetBeaches(MapGraph graph) {
        foreach (var node in graph.nodesByCenterPosition.Values) {
            if (node.nodeType == MapGraph.MapNodeType.Grass) {
                foreach (var neighbor in node.GetNeighborNodes()) {
                    if (neighbor.nodeType == MapGraph.MapNodeType.SaltWater) {
                        if (node.GetHeightDifference() < 0.8f) {
                            node.nodeType = MapGraph.MapNodeType.Sand;
                        }
                        break;
                    }
                }
            }
        }
    }

    private static void FindCities(MapGraph graph, float minElevation, float maxElevation, int maxCities) {
        var preferredElevation = 6f;
        var heightDifferenceWeighting = 4f;

        int cityCount = 0;
        while (cityCount < maxCities) {
            var candidate = GetCityCandidate(graph, preferredElevation, heightDifferenceWeighting).FirstOrDefault();
            if (candidate == null) break;
            candidate.nodeType = MapGraph.MapNodeType.City;
            cityCount++;
        }
    }

    private static IOrderedEnumerable<MapGraph.MapNode> GetCityCandidate(MapGraph graph, float preferredElevation, float heightDifferenceWeighting) {
        var candidates = graph.nodesByCenterPosition.Values.Where((node) => {
            return node.nodeType == MapGraph.MapNodeType.Grass &&
            node.GetEdges().Any(x => x.water > 0) && // Has a river
            !node.GetNeighborNodes().Any(x => x.nodeType == MapGraph.MapNodeType.City); // Not next to another city
        }).OrderBy((node) => {
            var heightDifference = node.GetHeightDifference();
            var elevation = node.GetElevation();
            return Mathf.Abs(elevation - preferredElevation) + heightDifference * heightDifferenceWeighting;
        });
        return candidates;
    }

    private static void SetNodesToGrass(MapGraph graph) {
        foreach (var node in graph.nodesByCenterPosition.Values)
        {
            if (node.nodeType != MapGraph.MapNodeType.Error) node.nodeType = MapGraph.MapNodeType.Grass;
        }
    }

    private static void SetLowNodesToWater(MapGraph graph, float cutoff) {
        foreach (var node in graph.nodesByCenterPosition.Values) {
            if (node.centerPoint.y <= cutoff) {
                var allZero = true;
                foreach (var edge in node.GetEdges()) {
                    if (edge.destination.position.y > cutoff) {
                        allZero = false;
                        break;
                    }
                }
                if (allZero && node.nodeType != MapGraph.MapNodeType.Error) node.nodeType = MapGraph.MapNodeType.FreshWater;
            }
        }
    }
}
