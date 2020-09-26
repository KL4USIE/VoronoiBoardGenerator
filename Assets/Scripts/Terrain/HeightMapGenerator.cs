using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
            values[(int)node.centerPoint.x, (int)node.centerPoint.z] = 3; //raise node center, works, USE Z           
            foreach(MapGraph.MapPoint corner in node.GetCorners()) { //go through all corners of all mountain nodes, corners between mountains will appear multiple times.
                if(cornerDic.ContainsKey(corner)) {                  //...if a corner appears three times, raise it
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
            //Debug.Log("RAISED");        
            foreach (MapGraph.MapPoint corner in node.GetCorners()) { //raise corners
                Vector2Int pos = new Vector2Int((int)corner.position.x, (int)corner.position.z);
                if (pos.x < (graph.GetCenter().x * 2) && pos.x < (graph.GetCenter().z * 2)) {
                    values[pos.x, pos.y] = 4;
                }
                //values[(int)corner.position.x, (int)corner.position.z] = 4;
            }
        }       
        return values;
    }
    public static float[,] LowerWater(List<MapGraph.MapNode> waterNodes, MapGraph graph, float[,] values) { //Same approach as RaiseMountains()
        Dictionary<MapGraph.MapPoint, int> cornerDic = new Dictionary<MapGraph.MapPoint, int>();
        foreach (MapGraph.MapNode node in waterNodes) {
            values[(int)node.centerPoint.x, (int)node.centerPoint.z] = -6; //raise node center, works, USE Z           
            foreach (MapGraph.MapPoint corner in node.GetCorners()) { //go through all corners of all mountain nodes, corners between mountains will appear multiple times.
                if (cornerDic.ContainsKey(corner)) {                  //...if a corner appears three times, raise it
                    cornerDic[corner]++;
                    if (cornerDic[corner] == 3) {
                        values[(int)corner.position.x, (int)corner.position.z] = -6;
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

