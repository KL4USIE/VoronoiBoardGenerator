using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
/// <summary>
/// Generates the heightmap based on the node types
/// </summary>
public static class HeightMapGenerator {
    public static HeightMap GenerateHeightMapFlat(int width, int height) {
        float[,] values = new float[width + 1, height + 1];
        HeightMapGenerator.HeightMap hMap = new HeightMap(values, -10, +10);
        return hMap;
    }
    public static HeightMap GenerateHeightMapFlat(int width, int height, MapGraph graph) {
        //Debug.Log("ARRAY DIMENSIONS: " + width + " " + height);
        float[,] values = new float[width+1, height+1];
        //Debug.Log("Heightmap resolution: "+width+" x "+height);
        values = RaiseMountains(graph, values);
        values = RaiseSnow(graph, values);
        values = LowerWater(graph, values);

        HeightMapGenerator.HeightMap hMap = new HeightMap(values, -10, +10);
        graph.UpdateHeights(hMap);
        return hMap;
    }
    private static float[,] RaiseMountains(MapGraph graph, float[,] values) {
        Dictionary<MapGraph.MapPoint, int> cornerDic = new Dictionary<MapGraph.MapPoint, int>();
        foreach(MapGraph.MapNode node in graph.mountainNodes) {
            values[(int)node.centerPoint.x, (int)node.centerPoint.z] = 3;//raise node center, works, USE Z           
            foreach(MapGraph.MapPoint corner in node.GetCorners()) {     //go through all corners of all mountain nodes, corners between mountains will appear multiple times.
                if(cornerDic.ContainsKey(corner)) {                      //...if a corner appears three times, raise it
                    cornerDic[corner]++;
                    if(cornerDic[corner] == 3) {
                        values[(int)corner.position.x, (int)corner.position.z] = 3;
                    }
                } else {
                    cornerDic.Add(corner, 1);
                }
            }
        }
        return values;
    }
    private static float[,] RaiseSnow(MapGraph graph, float[,] values) {
        foreach(MapGraph.MapNode node in graph.snowNodes) {
            //Debug.Log("Snow Node coordinates: " + node.centerPoint.x + " x " + node.centerPoint.z);                   
            values[(int)node.centerPoint.x, (int)node.centerPoint.z] = 8; //raise node center; works; USE Z

            //node.centerPoint = new Vector3(node.centerPoint.x, 8, node.centerPoint.z);
            //Debug.Log("RAISED");        
            foreach (MapGraph.MapPoint corner in node.GetCorners()) { //raise corners
                Vector2Int pos = new Vector2Int((int)corner.position.x, (int)corner.position.z);
                if (pos.x < (graph.GetCenter().x * 2) && pos.x < (graph.GetCenter().z * 2)) {
                    values[pos.x, pos.y] = 4;
                }               
            }
        }       
        return values;
    }
    private static float[,] LowerWater(MapGraph graph, float[,] values) { //Same approach as RaiseMountains()
        Dictionary<MapGraph.MapPoint, int> cornerDic = new Dictionary<MapGraph.MapPoint, int>();
        foreach (MapGraph.MapNode node in graph.waterNodes) {
            values[(int)node.centerPoint.x, (int)node.centerPoint.z] = -8; //raise node center, works, USE Z           
            foreach (MapGraph.MapPoint corner in node.GetCorners()) {      //go through all corners of all water nodes, corners between water will appear multiple times.
                                                                           //...if a corner appears three times, lower it                               
                List<MapGraph.MapNode> nodeList = new List<MapGraph.MapNode>(corner.GetNodes()); //fixed a bug where corners that...
                bool allowLowering = true; //also cornered on landnodes sometimes got lowered. Now checking all nodes adjacent to qualifying corners to confirm they are sorrounded by water
                //Debug.Log("CORNER: " + corner.position.x + " " + corner.position.z);
                foreach(var adjNode in nodeList) {
                    //Debug.Log("NODE: " + neighbourNode.centerPoint.x + " " + neighbourNode.centerPoint.z);
                    if (adjNode.nodeType != MapGraph.MapNodeType.SaltWater && adjNode.nodeType != MapGraph.MapNodeType.FreshWater) {
                         allowLowering = false;                              
                    }
                }
                if(allowLowering) {
                    Vector2Int coords = new Vector2Int((int)corner.position.x, (int)corner.position.z);
                    //Debug.Log(values.Length);
                    //Debug.Log((int)corner.position.x +" "+ (int)corner.position.z);                  
                    values[coords.x, coords.y] = -8;                                       
                }
            }                            
        }
        return values;
    } 
    public struct HeightMap {
	    public readonly float[,] values;
	    public readonly float minValue;
	    public readonly float maxValue;

	    public HeightMap (float[,] values, float minValue, float maxValue) {
		    this.values = values;
		    this.minValue = minValue;
		    this.maxValue = maxValue;
	    }
    }
}

