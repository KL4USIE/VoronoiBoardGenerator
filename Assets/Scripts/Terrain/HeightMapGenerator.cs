using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//--Class purpose-- 
//Generates the heightmap based on the node types
public static class HeightMapGenerator {

	public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCentre) {
		float[,] values = Noise.GenerateNoiseMap (width, height, settings.noiseSettings, sampleCentre);

        float[,] falloff= new float[0,0];
        if (settings.useFalloff)
        {
            var curve = new AnimationCurve(settings.falloffCurve.keys);
            falloff = FalloffGenerator.GenerateFalloffMap(width, settings.falloffType, curve);
        }

		AnimationCurve threadSafeHeightCurve = new AnimationCurve (settings.heightCurve.keys);

		float minValue = float.MaxValue;
		float maxValue = float.MinValue;

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {

                if (settings.useFalloff)
                {
                    values[i, j] -= falloff[i, j];
                }

                values[i, j] *= threadSafeHeightCurve.Evaluate(values[i, j]) * settings.heightMultiplier;

                if (values [i, j] > maxValue) {
					maxValue = values [i, j];
				}
				if (values [i, j] < minValue) {
					minValue = values [i, j];
				}
			}
		}

		return new HeightMap (values, minValue, maxValue);
	}
    //ADDED BY NOTH
    public static HeightMap GenerateHeightMapFlat(int width, int height, MapGraph graph) {       
        float[,] values = new float[width, height];
        //Debug.Log("Heightmap resolution: "+width+" x "+height);
        values = RaiseMountains(graph.mountainNodes, graph, values); //handing over graph.mountainNodes is redundant, leaving for WIP
        values = RaiseSnow(graph.snowNodes, graph, values);
        values = LowerWater(graph.waterNodes, graph, values);

        HeightMapGenerator.HeightMap hMap = new HeightMap(values, -10, +10);
        graph.UpdateHeights(hMap);
        return hMap;
    }
    public static float[,] RaiseMountains(List<MapGraph.MapNode> mountainNodes, MapGraph graph, float[,] values) {
        Dictionary<MapGraph.MapPoint, int> cornerDic = new Dictionary<MapGraph.MapPoint, int>();
        foreach(MapGraph.MapNode node in mountainNodes) {
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
    public static float[,] RaiseSnow(List<MapGraph.MapNode> snowNodes, MapGraph graph, float[,] values) {
        foreach(MapGraph.MapNode node in snowNodes) {
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
    public static float[,] LowerWater(List<MapGraph.MapNode> waterNodes, MapGraph graph, float[,] values) { //Same approach as RaiseMountains()
        Dictionary<MapGraph.MapPoint, int> cornerDic = new Dictionary<MapGraph.MapPoint, int>();
        foreach (MapGraph.MapNode node in waterNodes) {
            values[(int)node.centerPoint.x, (int)node.centerPoint.z] = -8; //raise node center, works, USE Z           
            foreach (MapGraph.MapPoint corner in node.GetCorners()) { //go through all corners of all water nodes, corners between water will appear multiple times.
                if (cornerDic.ContainsKey(corner)) {                  //...if a corner appears three times, lower it
                    cornerDic[corner]++;
                    if (cornerDic[corner] == 3) {
                        List<MapGraph.MapNode> nodeList = new List<MapGraph.MapNode>(corner.GetNodes()); //fixed a bug where corners that...
                        bool allowLowering = true; //also cornered on landnodes sometimes got lowered. Now checking all nodes adjacent to qualifying corners to confirm they are sorrounded by water
                        //Debug.Log("CORNER: " + corner.position.x + " " + corner.position.z);
                        foreach(var neighbourNode in nodeList) {
                            //Debug.Log("NODE: " + neighbourNode.centerPoint.x + " " + neighbourNode.centerPoint.z);
                            if (neighbourNode.nodeType != MapGraph.MapNodeType.SaltWater && neighbourNode.nodeType != MapGraph.MapNodeType.FreshWater) {
                                allowLowering = false;                              
                            }
                        }
                        if(allowLowering) {
                            values[(int)corner.position.x, (int)corner.position.z] = -8;
                        }                      
                    }
                } else {
                    cornerDic.Add(corner, 1);
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

