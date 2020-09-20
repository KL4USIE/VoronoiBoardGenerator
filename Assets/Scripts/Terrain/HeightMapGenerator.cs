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
        Debug.Log("Heightmap resolution: "+width+" x "+height);/*
            if (node.nodeType == MapGraph.MapNodeType.Mountain) { 
                values = RaiseMountain(graph.mountainNodes, graph, values);//raise mountain nodes
            }
            if (node.nodeType == MapGraph.MapNodeType.Snow) { //raise Snow nodes 
                values = RaiseSnow(node, graph, values);
            }*/
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
            Debug.Log("Snow Node coordinates: " + node.centerPoint.x + " x " + node.centerPoint.z);
            //iterate through all points in bounding rectangle, check if they are parallel to the inside of each edge, raise them
            /* //broken
            Rect rect = node.GetBoundingRectangle();
            List<MapGraph.MapPoint> cornerList = node.GetCorners().ToList<MapGraph.MapPoint>(); //corners of current node
            for (float x=rect.xMin; x<rect.xMax; x++) {
                for(float y=rect.yMin; y<rect.yMax; y++) {
                    Vector2 check = new Vector2(x, y);   //Point being checked for raising                   
                    List<Vector2> vecList = new List<Vector2>();
                    foreach(MapGraph.MapPoint p in cornerList) {
                        vecList.Add(new Vector2(p.position.x, p.position.y)); //conversion to Vector2
                    }
                    Vector2[] vecArray = vecList.ToArray(); //conversion to array
                    Debug.Log("CHECKING RAISE");                      
                    if (ContainsPoint(vecArray, check)) { //NEVER TRUE NEED TO INVESTIGATE
                        values[(int)x-1, (int)y-1] = 10;
                        Debug.Log("RAISED");
                    } 
                }
            }*/
            values[(int)node.centerPoint.x, (int)node.centerPoint.z] = 8; //raise node center; works; USE Z
            Debug.Log("RAISED");
            /* //broken
            List<MapGraph.MapPoint> corners = new List<MapGraph.MapPoint>(node.GetCorners());
            for(int i = 0; i <= corners.Count; i+=2) {
                Vector2 v = corners[i+1].position - corners[i].position;
                values[(int)v.x, (int)v.y] = 10;
            }
            Vector2Int nodeCenter = new Vector2Int((int)node.centerPoint.x, (int)node.centerPoint.y);
            values[nodeCenter.x, nodeCenter.y] = 10;
            //Debug.Log("RAISED");
            */
            foreach (MapGraph.MapPoint corner in node.GetCorners()) { //raise corners
                values[(int)corner.position.x, (int)corner.position.z] = 4;
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

