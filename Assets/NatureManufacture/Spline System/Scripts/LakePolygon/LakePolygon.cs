
using System;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using UnityEngine;
using UnityEngine.Rendering;

#if VEGETATION_STUDIO
using AwesomeTechnologies;
using AwesomeTechnologies.VegetationStudio;
#endif
#if VEGETATION_STUDIO_PRO
using AwesomeTechnologies.VegetationSystem.Biomes;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif


[RequireComponent(typeof(MeshFilter))]
public class LakePolygon : MonoBehaviour
{

    public int toolbarInt = 0;

    public LakePolygonProfile currentProfile;
    public LakePolygonProfile oldProfile;


    public List<Vector3> points = new List<Vector3>();
    public List<Vector3> splinePoints = new List<Vector3>();

    public AnimationCurve terrainCarve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(10, -2) });
    public float distSmooth = 5;
    public float terrainSmoothMultiplier = 5;
    public bool overrideLakeRender = false;
    public float uvScale = 1;

    public bool receiveShadows = false;
    public ShadowCastingMode shadowCastingMode = ShadowCastingMode.Off;

    public AnimationCurve terrainPaintCarve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });
    public int currentSplatMap = 1;

    public float distanceClearFoliage = 1;
    public float distanceClearFoliageTrees = 1;

    public bool mixTwoSplatMaps = false;
    public int secondSplatMap = 1;

    public bool addCliffSplatMap = false;
    public int cliffSplatMap = 1;
    public float cliffAngle = 25;
    public float cliffBlend = 1;

    public int cliffSplatMapOutside = 1;
    public float cliffAngleOutside = 45;
    public float cliffBlendOutside = 1;

    public bool noiseCarve = false;
    public float noiseMultiplierInside = 1f;
    public float noiseMultiplierOutside = 0.25f;
    public float noiseSizeX = 0.2f;
    public float noiseSizeZ = 0.2f;

    public bool noisePaint = false;
    public float noiseMultiplierInsidePaint = 1f;
    public float noiseMultiplierOutsidePaint = 0.5f;
    public float noiseSizeXPaint = 0.2f;
    public float noiseSizeZPaint = 0.2f;

    public float maximumTriangleSize = 50;
    public float traingleDensity = 0.2f;

    public float height = 0;
    public bool lockHeight = true;



    public float yOffset = 0;


    public float trianglesGenerated = 0;
    public float vertsGenerated = 0;
    public Mesh currentMesh;

    public MeshFilter meshfilter;
    public bool showVertexColors;
    public bool showFlowMap;
    public bool overrideFlowMap = false;
    public float automaticFlowMapScale = 0.2f;

    public bool noiseflowMap = false;
    public float noiseMultiplierflowMap = 1f;
    public float noiseSizeXflowMap = 0.2f;
    public float noiseSizeZflowMap = 0.2f;

    public bool drawOnMesh = false;
    public bool drawOnMeshFlowMap = false;
    public Color drawColor = Color.black;
    public bool drawColorR = true;
    public bool drawColorG = true;
    public bool drawColorB = true;
    public bool drawColorA = true;
    public bool drawOnMultiple = false;

    public float opacity = 0.1f;
    public float drawSize = 1f;

    public Material oldMaterial;
    public Color[] colors;
    public List<Vector2> colorsFlowMap = new List<Vector2>();

    public float floatSpeed = 10;

    public float flowSpeed = 1f;
    public float flowDirection = 0f;


    public float closeDistanceSimulation = 5f;
    public int angleSimulation = 5;
    public float checkDistanceSimulation = 50;
    public bool removeFirstPointSimulation = true;


#if VEGETATION_STUDIO_PRO
    public float biomMaskResolution = 0.5f;
    public float vegetationBlendDistance = 1;
    public float vegetationMaskSize = 3;
    public BiomeMaskArea biomeMaskArea;
    public bool refreshMask = false;
#endif
#if VEGETATION_STUDIO

    public float vegetationMaskResolution = 0.5f;
    public float vegetationMaskPerimeter = 5;
    public VegetationMaskArea vegetationMaskArea;

#endif

    public LakePolygonCarveData lakePolygonCarveData;
    public LakePolygonCarveData lakePolygonPaintData;
    public LakePolygonCarveData lakePolygonClearData;
    public List<GameObject> meshGOs = new List<GameObject>();



    /// <summary>
    /// Add point at end of spline
    /// </summary>
    /// <param name="position">New point position</param>
    public void AddPoint(Vector3 position)
    {
        points.Add(position);
    }

    /// <summary>
    /// Add point in the middle of the spline
    /// </summary>
    /// <param name="i">Point id</param>
    public void AddPointAfter(int i)
    {

        Vector3 position = points[i];
        if (i < points.Count - 1 && points.Count > i + 1)
        {
            Vector3 positionSecond = points[i + 1];
            if (Vector3.Distance((Vector3)positionSecond, (Vector3)position) > 0)
                position = (position + positionSecond) * 0.5f;
            else
                position.x += 1;
        }
        else if (points.Count > 1 && i == points.Count - 1)
        {
            Vector3 positionSecond = points[i - 1];
            if (Vector3.Distance((Vector3)positionSecond, (Vector3)position) > 0)
                position = position + (position - positionSecond);
            else
                position.x += 1;
        }
        else
        {
            position.x += 1;
        }
        points.Insert(i + 1, position);

    }



    /// <summary>
    /// Changes point position, if new position doesn't have width old width will be taken
    /// </summary>
    /// <param name="i">Point id</param>
    /// <param name="position">New position</param>
    public void ChangePointPosition(int i, Vector3 position)
    {

        points[i] = position;
    }

    /// <summary>
    /// Removes point in spline
    /// </summary>
    /// <param name="i"></param>
    public void RemovePoint(int i)
    {
        if (i < points.Count)
        {
            points.RemoveAt(i);
        }
    }

    /// <summary>
    /// Removes points from point id forward
    /// </summary>
    /// <param name="fromID">Point id</param>
    public void RemovePoints(int fromID = -1)
    {
        int pointsCount = points.Count - 1;
        for (int i = pointsCount; i > fromID; i--)
        {
            RemovePoint(i);
        }

    }

    void CenterPivot()
    {
        Vector3 position = transform.position;
        Vector3 center = Vector3.zero;

        for (int i = 0; i < points.Count; i++)
        {
            center += points[i];
        }
        center /= points.Count;

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 vec = points[i];
            vec.x -= center.x;
            vec.y -= center.y;
            vec.z -= center.z;
            points[i] = vec;
        }

        transform.position += center;
    }


    public void GeneratePolygon()
    {


        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.receiveShadows = receiveShadows;
            meshRenderer.shadowCastingMode = shadowCastingMode;
        }


        if (lockHeight)
        {
            for (int i = 1; i < points.Count; i++)
            {
                Vector3 vec = points[i];
                vec.y = points[0].y;
                points[i] = vec;
            }

        }

        if (points.Count < 3)
            return;

        CenterPivot();

        splinePoints.Clear();
        for (int i = 0; i < points.Count; i++)
        {

            //if ((i == 0 || i == points.Count - 2 || i == points.Count - 1) && !true)
            //{
            //    continue;
            //}

            CalculateCatmullRomSpline(i);
        }





        List<Vector3> verticesList = new List<Vector3>();
        List<Vector3> verts = new List<Vector3>();
        List<int> indices = new List<int>();
        verticesList.AddRange(splinePoints.ToArray());


        //Triangulator traingulator = new Triangulator(verts2d.ToArray());
        // indices.AddRange(traingulator.Triangulate());


        Polygon polygon = new Polygon();

        List<Vertex> vertexs = new List<Vertex>();

        for (int i = 0; i < verticesList.Count; i++)
        {
            Vertex vert = new Vertex(verticesList[i].x, verticesList[i].z);
            vert.z = verticesList[i].y;
            vertexs.Add(vert);
        }
        polygon.Add(new Contour(vertexs));

        var options = new ConstraintOptions() { ConformingDelaunay = true };
        var quality = new QualityOptions() { MinimumAngle = 90, MaximumArea = maximumTriangleSize };



        TriangleNet.Mesh mesh = (TriangleNet.Mesh)polygon.Triangulate(options, quality);
        polygon.Triangulate(options, quality);

        indices.Clear();
        foreach (var triangle in mesh.triangles)
        {
            Vertex vertex = mesh.vertices[triangle.vertices[2].id];

            Vector3 v0 = new Vector3((float)vertex.x, (float)vertex.z, (float)vertex.y);

            vertex = mesh.vertices[triangle.vertices[1].id];
            Vector3 v1 = new Vector3((float)vertex.x, (float)vertex.z, (float)vertex.y);
            vertex = mesh.vertices[triangle.vertices[0].id];
            Vector3 v2 = new Vector3((float)vertex.x, (float)vertex.z, (float)vertex.y);

            indices.Add(verts.Count);
            indices.Add(verts.Count + 1);
            indices.Add(verts.Count + 2);

            verts.Add(v0);
            verts.Add(v1);
            verts.Add(v2);

        }


        Vector3[] vertices = verts.ToArray();
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y += yOffset;
        }
        int vertCount = vertices.Length;
        currentMesh = new Mesh();

        vertsGenerated = vertCount;

        if (vertCount > 65000)
        {
            currentMesh.indexFormat = IndexFormat.UInt32;
        }

        currentMesh.vertices = vertices;
        currentMesh.subMeshCount = 1;
        currentMesh.SetTriangles(indices, 0);
        Vector2[] uvs = new Vector2[vertCount];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z) * 0.01f * uvScale;

        }


        Vector3[] normals = new Vector3[vertCount];
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = Vector3.up;
        }


        colors = new Color[vertCount];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.black;
        }

        //if (colorsFlowMap.Count != vertCount)
        //    colorsFlowMap.Clear();

        if (overrideFlowMap)
        {
            while (colorsFlowMap.Count < vertCount)
            {
                colorsFlowMap.Add(new Vector2(0, 1));
            }

            while (colorsFlowMap.Count > vertCount)
            {
                colorsFlowMap.RemoveAt(colorsFlowMap.Count - 1);
            }
        }
        else
        {

            List<Vector2> lines = new List<Vector2>();
            List<Vector2> vert2 = new List<Vector2>();

            for (int i = 0; i < splinePoints.Count; i++)
            {
                lines.Add(new Vector2(splinePoints[i].x, splinePoints[i].z));
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                vert2.Add(new Vector2(vertices[i].x, vertices[i].z));
            }


            colorsFlowMap.Clear();

            Vector2 flow = Vector2.zero;
            for (int i = 0; i < vertCount; i++)
            {
                float minDist = float.MaxValue;
                Vector2 minPoint = vert2[i];
                for (int k = 0; k < splinePoints.Count; k++)
                {
                    int idOne = k;
                    int idTwo = (k + 1) % lines.Count;

                    Vector2 point;
                    float dist = DistancePointLine(vert2[i], lines[idOne], lines[idTwo], out point);
                    if (minDist > dist)
                    {
                        minDist = dist;
                        minPoint = point;
                    }

                }

                flow = minPoint - vert2[i];
                flow = -flow.normalized * (automaticFlowMapScale + (noiseflowMap ? Mathf.PerlinNoise(vert2[i].x * noiseSizeXflowMap, vert2[i].y * noiseSizeZflowMap) * noiseMultiplierflowMap - noiseMultiplierflowMap * 0.5f : 0));

                colorsFlowMap.Add(flow);

            }
        }
        //Debug.Log("poly");


        currentMesh.uv = uvs;
        currentMesh.uv4 = colorsFlowMap.ToArray();
        currentMesh.normals = normals;
        currentMesh.colors = colors;
        currentMesh.RecalculateTangents();
        currentMesh.RecalculateBounds();


        currentMesh.RecalculateTangents();
        currentMesh.RecalculateBounds();
        trianglesGenerated = indices.Count / 3;

        meshfilter = GetComponent<MeshFilter>();
        meshfilter.sharedMesh = currentMesh;


        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = currentMesh;
        }
    }


    public static LakePolygon CreatePolygon(Material material, List<Vector3> positions = null)
    {
        GameObject gameobject = new GameObject("Lake Polygon");
        gameobject.layer = LayerMask.NameToLayer("Water");

        LakePolygon polygon = gameobject.AddComponent<LakePolygon>();
        MeshRenderer meshRenderer = gameobject.AddComponent<MeshRenderer>();
        meshRenderer.receiveShadows = false;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        if (material != null)
            meshRenderer.sharedMaterial = material;

        if (positions != null)
            for (int i = 0; i < positions.Count; i++)
            {
                polygon.AddPoint(positions[i]);
            }

        return polygon;
    }

    void CalculateCatmullRomSpline(int pos)
    {
        Vector3 p0 = points[ClampListPos(pos - 1)];
        Vector3 p1 = points[pos];
        Vector3 p2 = points[ClampListPos(pos + 1)];
        Vector3 p3 = points[ClampListPos(pos + 2)];


        int loops = Mathf.FloorToInt(1f / traingleDensity);

        for (int i = 1; i <= loops; i++)
        {
            float t = i * traingleDensity;

            splinePoints.Add(GetCatmullRomPosition(t, p0, p1, p2, p3));
        }
    }

    public int ClampListPos(int pos)
    {
        if (pos < 0)
        {
            pos = points.Count - 1;
        }

        if (pos > points.Count)
        {
            pos = 1;
        }
        else if (pos > points.Count - 1)
        {
            pos = 0;
        }

        return pos;
    }



    Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        //The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
        Vector3 a = 2f * p1;
        Vector3 b = p2 - p0;
        Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
        Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

        //The cubic polynomial: a + b * t + c * t^2 + d * t^3
        Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

        return pos;
    }

    public float DistancePointLine(Vector2 point, Vector2 lineStart, Vector2 lineEnd, out Vector2 pointProject)
    {
        pointProject = ProjectPointLine(point, lineStart, lineEnd);
        return Vector2.Distance(pointProject, point);
    }
    public Vector2 ProjectPointLine(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
    {
        Vector2 rhs = point - lineStart;
        Vector2 vector2 = lineEnd - lineStart;
        float magnitude = vector2.magnitude;
        Vector2 lhs = vector2;
        if (magnitude > 1E-06f)
        {
            lhs = (Vector2)(lhs / magnitude);
        }
        float num2 = Mathf.Clamp(Vector2.Dot(lhs, rhs), 0f, magnitude);
        return (lineStart + ((Vector2)(lhs * num2)));
    }



    #region terrainFunctions

    public void TerrainCarve(bool terrainShow = false)
    {
        Terrain[] terrains = Terrain.activeTerrains;

        Physics.autoSyncTransforms = false;

        if (meshGOs != null && meshGOs.Count > 0)
        {
            foreach (var item in meshGOs)
            {
                DestroyImmediate(item);
            }
            meshGOs.Clear();
        }

        foreach (var terrain in terrains)
        {

            TerrainData terrainData = terrain.terrainData;
            float[,] heightmapData;
            float polygonHeight = transform.position.y;
            float posY = terrain.transform.position.y;
            float sizeX = terrain.terrainData.size.x;
            float sizeY = terrain.terrainData.size.y;
            float sizeZ = terrain.terrainData.size.z;


            if (lakePolygonCarveData == null || distSmooth != lakePolygonCarveData.distSmooth)
            {

#if UNITY_EDITOR
                Undo.RegisterCompleteObjectUndo(terrainData, "Lake curve");
#endif

                float minX = float.MaxValue;
                float maxX = float.MinValue;
                float minZ = float.MaxValue;
                float maxZ = float.MinValue;

                for (int i = 0; i < splinePoints.Count; i++)
                {
                    Vector3 point = transform.TransformPoint(splinePoints[i]);


                    if (minX > point.x)
                        minX = point.x;

                    if (maxX < point.x)
                        maxX = point.x;

                    if (minZ > point.z)
                        minZ = point.z;

                    if (maxZ < point.z)
                        maxZ = point.z;
                }



                //Debug.DrawLine(new Vector3(minX, 0, minZ), new Vector3(minX, 0, minZ) + Vector3.up * 100, Color.green, 3);
                // Debug.DrawLine(new Vector3(maxX, 0, maxZ), new Vector3(maxX, 0, maxZ) + Vector3.up * 100, Color.blue, 3);


                float terrainTowidth = (1 / sizeX * (terrainData.heightmapResolution - 1));
                float terrainToheight = (1 / sizeZ * (terrainData.heightmapResolution - 1));
                minX -= terrain.transform.position.x + distSmooth;
                maxX -= terrain.transform.position.x - distSmooth;

                minZ -= terrain.transform.position.z + distSmooth;
                maxZ -= terrain.transform.position.z - distSmooth;


                minX = minX * terrainToheight;
                maxX = maxX * terrainToheight;

                minZ = minZ * terrainTowidth;
                maxZ = maxZ * terrainTowidth;

                minX = (int)Mathf.Clamp(minX, 0, (terrainData.heightmapResolution));
                maxX = (int)Mathf.Clamp(maxX, 0, (terrainData.heightmapResolution));
                minZ = (int)Mathf.Clamp(minZ, 0, (terrainData.heightmapResolution));
                maxZ = (int)Mathf.Clamp(maxZ, 0, (terrainData.heightmapResolution));

                heightmapData = terrainData.GetHeights((int)minX, (int)minZ, (int)(maxX - minX), (int)(maxZ - minZ));

                Vector4[,] distances = new Vector4[heightmapData.GetLength(0), heightmapData.GetLength(1)];

                MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();

                Transform transformTerrain = terrain.transform;


                Vector3 position = Vector3.zero;
                position.y = polygonHeight;

                for (int x = 0; x < heightmapData.GetLength(0); x++)
                {
                    for (int z = 0; z < heightmapData.GetLength(1); z++)
                    {


                        position.x = (z + minX) / (float)terrainToheight + transformTerrain.position.x;//, polygonHeight
                        position.z = (x + minZ) / (float)terrainTowidth + transformTerrain.position.z;

                        Ray ray = new Ray(position + Vector3.up * 1000, Vector3.down);
                        RaycastHit hit;

                        if (meshCollider.Raycast(ray, out hit, 10000))
                        {
                            // Debug.DrawLine(hit.point, hit.point + Vector3.up * 30, Color.green, 3);

                            float minDist = float.MaxValue;
                            for (int i = 0; i < splinePoints.Count; i++)
                            {
                                int idOne = i;
                                int idTwo = (i + 1) % splinePoints.Count;

                                float dist = DistancePointLine(hit.point, transform.TransformPoint(splinePoints[idOne]), transform.TransformPoint(splinePoints[idTwo]));
                                if (minDist > dist)
                                    minDist = dist;
                            }
                            distances[x, z] = new Vector3(hit.point.x, minDist, hit.point.z);

                        }
                        else
                        {
                            float minDist = float.MaxValue;
                            for (int i = 0; i < splinePoints.Count; i++)
                            {
                                int idOne = i;
                                int idTwo = (i + 1) % splinePoints.Count;

                                float dist = DistancePointLine(position, transform.TransformPoint(splinePoints[idOne]), transform.TransformPoint(splinePoints[idTwo]));
                                if (minDist > dist)
                                    minDist = dist;
                            }
                            distances[x, z] = new Vector3(position.x, -minDist, position.z);
                        }



                    }
                }
                //Debug.DrawRay(new Vector3((minX) / (float)terrainToheight, polygonHeight, (minZ) / (float)terrainTowidth), Vector3.up * 30, Color.black, 3);
                //Debug.DrawRay(new Vector3((maxX) / (float)terrainToheight, polygonHeight, (maxZ) / (float)terrainTowidth), Vector3.up * 30, Color.black, 3);
                DestroyImmediate(meshCollider);

                lakePolygonCarveData = new LakePolygonCarveData();
                lakePolygonCarveData.minX = minX;
                lakePolygonCarveData.maxX = maxX;
                lakePolygonCarveData.minZ = minZ;
                lakePolygonCarveData.maxZ = maxZ;
                lakePolygonCarveData.distances = distances;
            }


            heightmapData = terrainData.GetHeights((int)lakePolygonCarveData.minX, (int)lakePolygonCarveData.minZ, (int)(lakePolygonCarveData.maxX - lakePolygonCarveData.minX), (int)(lakePolygonCarveData.maxZ - lakePolygonCarveData.minZ));



            float noise = 0;
            List<List<Vector4>> positionArray = new List<List<Vector4>>();
            for (int x = 0; x < heightmapData.GetLength(0); x++)
            {
                List<Vector4> positionArrayRow = new List<Vector4>();
                for (int z = 0; z < heightmapData.GetLength(1); z++)
                {


                    Vector3 distance = lakePolygonCarveData.distances[x, z];

                    if (distance.y > 0)
                    {
                        if (noiseCarve)
                            noise = Mathf.PerlinNoise(x * noiseSizeX, z * noiseSizeZ) * noiseMultiplierInside - noiseMultiplierInside * 0.5f;
                        else
                            noise = 0;

                        float minDist = distance.y;

                        heightmapData[x, z] = (noise + polygonHeight + terrainCarve.Evaluate(minDist) - posY) / (float)sizeY;

                        positionArrayRow.Add(new Vector4(distance.x, heightmapData[x, z] * sizeY + posY, distance.z, 1));

                    }
                    else if (-distance.y <= distSmooth)
                    {
                        if (noiseCarve)
                            noise = Mathf.PerlinNoise(x * noiseSizeX, z * noiseSizeZ) * noiseMultiplierOutside - noiseMultiplierOutside * 0.5f;
                        else
                            noise = 0;

                        float y = heightmapData[x, z] * sizeY + posY;

                        float height = polygonHeight + terrainCarve.Evaluate(distance.y);

                        float smoothValue = -distance.y / distSmooth;

                        smoothValue = Mathf.Pow(smoothValue, terrainSmoothMultiplier);

                        height = noise + Mathf.Lerp(height, y, smoothValue) - posY;

                        heightmapData[x, z] = height / sizeY;

                        positionArrayRow.Add(new Vector4(distance.x, heightmapData[x, z] * sizeY + posY, distance.z, Mathf.Pow(1 + distance.y / distSmooth, 0.5f)));
                    }
                    else
                    {
                        //float dist = lakePolygon.distSmooth * 0.8f;

                        positionArrayRow.Add(new Vector4(distance.x, (heightmapData[x, z]) * sizeY + posY, distance.z, 0));// distance.z,1- Mathf.Clamp(-(dist + distance.y),0, dist) / dist));
                    }



                }
                positionArray.Add(positionArrayRow);
            }


            if (terrainShow)
            {
                Mesh meshTerrain = new Mesh();
                meshTerrain.indexFormat = IndexFormat.UInt32;
                List<Vector3> vertices = new List<Vector3>();
                List<int> triangles = new List<int>();
                List<Color> colors = new List<Color>();

                foreach (var positionRow in positionArray)
                {
                    foreach (var vert in positionRow)
                    {
                        vertices.Add(vert);
                        colors.Add(new Color(vert.w, vert.w, vert.w, vert.w));
                    }
                }



                for (int i = 0; i < positionArray.Count - 1; i++)
                {
                    List<Vector4> rowPosition = positionArray[i];

                    for (int j = 0; j < rowPosition.Count - 1; j++)
                    {
                        triangles.Add(j + i * rowPosition.Count);
                        triangles.Add(j + (i + 1) * rowPosition.Count);
                        triangles.Add((j + 1) + i * rowPosition.Count);

                        triangles.Add((j + 1) + i * rowPosition.Count);
                        triangles.Add(j + (i + 1) * rowPosition.Count);
                        triangles.Add((j + 1) + (i + 1) * rowPosition.Count);

                    }
                }

                meshTerrain.SetVertices(vertices);
                meshTerrain.SetTriangles(triangles, 0);
                meshTerrain.SetColors(colors);

                meshTerrain.RecalculateNormals();
                meshTerrain.RecalculateTangents();
                meshTerrain.RecalculateBounds();



                GameObject meshGO = new GameObject("TerrainMesh");
                meshGO.transform.parent = transform;
                //meshGO.hideFlags = HideFlags.HideAndDontSave;
                meshGO.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = meshGO.AddComponent<MeshRenderer>();
                meshRenderer.sharedMaterial = new Material(Shader.Find("Debug Terrain Carve"));
                meshRenderer.sharedMaterial.color = new Color(0, 0.5f, 0);


                meshGO.transform.position = Vector3.zero;
                meshGO.GetComponent<MeshFilter>().sharedMesh = meshTerrain;

                if (overrideLakeRender)
                    meshGO.GetComponent<MeshRenderer>().sharedMaterial.renderQueue = 5000;
                else
                    meshGO.GetComponent<MeshRenderer>().sharedMaterial.renderQueue = 2980;

                meshGOs.Add(meshGO);
            }
            else
            {
                if (meshGOs != null && meshGOs.Count > 0)
                {
                    foreach (var item in meshGOs)
                    {
                        DestroyImmediate(item);
                    }
                    meshGOs.Clear();
                }

                terrainData.SetHeights((int)lakePolygonCarveData.minX, (int)lakePolygonCarveData.minZ, heightmapData);
                terrain.Flush();
                lakePolygonCarveData = null;
            }

        }
        Physics.autoSyncTransforms = true;
    }


    public void TerrainPaint(bool terrainShow = false)
    {
        Terrain[] terrains = Terrain.activeTerrains;

        Physics.autoSyncTransforms = false;

        if (meshGOs != null && meshGOs.Count > 0)
        {
            foreach (var item in meshGOs)
            {
                DestroyImmediate(item);
            }
            meshGOs.Clear();
        }


        float distSmooth = this.distSmooth;

        float minKey = float.MaxValue;
        foreach (var key in terrainPaintCarve.keys)
        {
            if (key.time < minKey)
                minKey = key.time;
        }
        if (minKey < 0)
            distSmooth = -minKey;

        foreach (var terrain in terrains)
        {

            TerrainData terrainData = terrain.terrainData;
            float[,,] alphamapData;
            float polygonHeight = transform.position.y;
            //float posY = terrain.transform.position.y;
            float sizeX = terrain.terrainData.size.x;
            //float sizeY = terrain.terrainData.size.y;
            float sizeZ = terrain.terrainData.size.z;


            if (lakePolygonPaintData == null || distSmooth != lakePolygonPaintData.distSmooth)
            {

#if UNITY_EDITOR
                Undo.RegisterCompleteObjectUndo(terrainData, "Paint lake");
                Undo.RegisterCompleteObjectUndo(terrain, "Terrain draw texture");
                Undo.RegisterCompleteObjectUndo(terrainData.alphamapTextures, "alpha");
#endif

                float minX = float.MaxValue;
                float maxX = float.MinValue;
                float minZ = float.MaxValue;
                float maxZ = float.MinValue;

                for (int i = 0; i < splinePoints.Count; i++)
                {
                    Vector3 point = transform.TransformPoint(splinePoints[i]);


                    if (minX > point.x)
                        minX = point.x;

                    if (maxX < point.x)
                        maxX = point.x;

                    if (minZ > point.z)
                        minZ = point.z;

                    if (maxZ < point.z)
                        maxZ = point.z;
                }



                //Debug.DrawLine(new Vector3(minX, 0, minZ), new Vector3(minX, 0, minZ) + Vector3.up * 100, Color.green, 3);
                // Debug.DrawLine(new Vector3(maxX, 0, maxZ), new Vector3(maxX, 0, maxZ) + Vector3.up * 100, Color.blue, 3);


                float terrainTowidth = (1 / sizeX * (terrainData.alphamapWidth - 1));
                float terrainToheight = (1 / sizeZ * (terrainData.alphamapHeight - 1));
                minX -= terrain.transform.position.x + distSmooth;
                maxX -= terrain.transform.position.x - distSmooth;

                minZ -= terrain.transform.position.z + distSmooth;
                maxZ -= terrain.transform.position.z - distSmooth;


                minX = minX * terrainToheight;
                maxX = maxX * terrainToheight;

                minZ = minZ * terrainTowidth;
                maxZ = maxZ * terrainTowidth;

                minX = (int)Mathf.Clamp(minX, 0, (terrainData.alphamapWidth));
                maxX = (int)Mathf.Clamp(maxX, 0, (terrainData.alphamapWidth));
                minZ = (int)Mathf.Clamp(minZ, 0, (terrainData.alphamapHeight));
                maxZ = (int)Mathf.Clamp(maxZ, 0, (terrainData.alphamapHeight));

                alphamapData = terrainData.GetAlphamaps((int)minX, (int)minZ, (int)(maxX - minX), (int)(maxZ - minZ));

                Vector4[,] distances = new Vector4[alphamapData.GetLength(0), alphamapData.GetLength(1)];

                MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();


                Transform transformTerrain = terrain.transform;


                Vector3 position = Vector3.zero;
                position.y = polygonHeight;

                for (int x = 0; x < alphamapData.GetLength(0); x++)
                {
                    for (int z = 0; z < alphamapData.GetLength(1); z++)
                    {


                        position.x = (z + minX) / (float)terrainToheight + transformTerrain.position.x;//, polygonHeight
                        position.z = (x + minZ) / (float)terrainTowidth + transformTerrain.position.z;

                        Ray ray = new Ray(position + Vector3.up * 1000, Vector3.down);
                        RaycastHit hit;

                        if (meshCollider.Raycast(ray, out hit, 10000))
                        {
                            // Debug.DrawLine(hit.point, hit.point + Vector3.up * 30, Color.green, 3);

                            float minDist = float.MaxValue;
                            for (int i = 0; i < splinePoints.Count; i++)
                            {
                                int idOne = i;
                                int idTwo = (i + 1) % splinePoints.Count;

                                float dist = DistancePointLine(hit.point, transform.TransformPoint(splinePoints[idOne]), transform.TransformPoint(splinePoints[idTwo]));
                                if (minDist > dist)
                                    minDist = dist;
                            }

                            float angle = 0;

                            if (addCliffSplatMap)
                            {
                                ray = new Ray(position + Vector3.up * 1000, Vector3.down);
                                TerrainCollider collider = terrain.GetComponent<TerrainCollider>();
                                if (collider.Raycast(ray, out hit, 10000))
                                {
                                    angle = Vector3.Angle(hit.normal, Vector3.up);
                                }
                            }

                            distances[x, z] = new Vector4(hit.point.x, minDist, hit.point.z, angle);

                        }
                        else
                        {
                            float minDist = float.MaxValue;
                            for (int i = 0; i < splinePoints.Count; i++)
                            {
                                int idOne = i;
                                int idTwo = (i + 1) % splinePoints.Count;

                                float dist = DistancePointLine(position, transform.TransformPoint(splinePoints[idOne]), transform.TransformPoint(splinePoints[idTwo]));
                                if (minDist > dist)
                                    minDist = dist;
                            }

                            float angle = 0;

                            if (addCliffSplatMap)
                            {
                                ray = new Ray(position + Vector3.up * 1000, Vector3.down);
                                TerrainCollider collider = terrain.GetComponent<TerrainCollider>();
                                if (collider.Raycast(ray, out hit, 10000))
                                {
                                    angle = Vector3.Angle(hit.normal, Vector3.up);
                                }
                            }
                            distances[x, z] = new Vector4(position.x, -minDist, position.z, angle);
                        }

                    }
                }

                DestroyImmediate(meshCollider);

                lakePolygonPaintData = new LakePolygonCarveData();
                lakePolygonPaintData.minX = minX;
                lakePolygonPaintData.maxX = maxX;
                lakePolygonPaintData.minZ = minZ;
                lakePolygonPaintData.maxZ = maxZ;
                lakePolygonPaintData.distances = distances;
            }


            alphamapData = terrainData.GetAlphamaps((int)lakePolygonPaintData.minX, (int)lakePolygonPaintData.minZ, (int)(lakePolygonPaintData.maxX - lakePolygonPaintData.minX), (int)(lakePolygonPaintData.maxZ - lakePolygonPaintData.minZ));


            float noise = 0;
            List<List<Vector4>> positionArray = new List<List<Vector4>>();
            for (int x = 0; x < alphamapData.GetLength(0); x++)
            {
                List<Vector4> positionArrayRow = new List<Vector4>();
                for (int z = 0; z < alphamapData.GetLength(1); z++)
                {


                    Vector4 distance = lakePolygonPaintData.distances[x, z];

                    if ((-distance.y <= distSmooth) || (distance.y > 0))
                    {
                        if (!mixTwoSplatMaps)
                        {
                            if (noisePaint)
                            {
                                if (distance.y > 0)
                                    noise = Mathf.PerlinNoise(x * noiseSizeXPaint, z * noiseSizeZPaint) * noiseMultiplierInsidePaint - noiseMultiplierInsidePaint * 0.5f;
                                else
                                    noise = Mathf.PerlinNoise(x * noiseSizeXPaint, z * noiseSizeZPaint) * noiseMultiplierOutsidePaint - noiseMultiplierOutsidePaint * 0.5f;
                            }
                            else
                                noise = 0;

                            float oldValue = alphamapData[x, z, currentSplatMap];

                            alphamapData[x, z, currentSplatMap] = Mathf.Clamp01(Mathf.Lerp(alphamapData[x, z, currentSplatMap], 1, terrainPaintCarve.Evaluate(distance.y) + noise * terrainPaintCarve.Evaluate(distance.y)));


                            for (int l = 0; l < terrainData.terrainLayers.Length; l++)
                            {
                                if (l != currentSplatMap)
                                {
                                    alphamapData[x, z, l] = oldValue == 1 ? 0 : Mathf.Clamp01(alphamapData[x, z, l] * ((1 - alphamapData[x, z, currentSplatMap]) / (1 - oldValue)));

                                }
                            }
                        }
                        else
                        {
                            if (distance.y > 0)
                                noise = Mathf.PerlinNoise(x * noiseSizeXPaint, z * noiseSizeZPaint) * noiseMultiplierInsidePaint - noiseMultiplierInsidePaint * 0.5f;
                            else
                                noise = Mathf.PerlinNoise(x * noiseSizeXPaint, z * noiseSizeZPaint) * noiseMultiplierOutsidePaint - noiseMultiplierOutsidePaint * 0.5f;


                            float oldValue = alphamapData[x, z, currentSplatMap];

                            alphamapData[x, z, currentSplatMap] = Mathf.Clamp01(Mathf.Lerp(alphamapData[x, z, currentSplatMap], 1, terrainPaintCarve.Evaluate(distance.y)));


                            for (int l = 0; l < terrainData.terrainLayers.Length; l++)
                            {
                                if (l != currentSplatMap)
                                {
                                    alphamapData[x, z, l] = oldValue == 1 ? 0 : Mathf.Clamp01(alphamapData[x, z, l] * ((1 - alphamapData[x, z, currentSplatMap]) / (1 - oldValue)));

                                }
                            }


                            if (noise > 0)
                            {
                                oldValue = alphamapData[x, z, secondSplatMap];
                                alphamapData[x, z, secondSplatMap] = Mathf.Clamp01(Mathf.Lerp(alphamapData[x, z, secondSplatMap], 1, noise));


                                for (int l = 0; l < terrainData.terrainLayers.Length; l++)
                                {
                                    if (l != secondSplatMap)
                                    {
                                        alphamapData[x, z, l] = oldValue == 1 ? 0 : Mathf.Clamp01(alphamapData[x, z, l] * ((1 - alphamapData[x, z, secondSplatMap]) / (1 - oldValue)));

                                    }
                                }
                            }
                        }

                        if (addCliffSplatMap)
                        {

                            float oldValue = alphamapData[x, z, cliffSplatMap];

                            // Debug.Log(lakePolygon.cliffAngle + " " + distance.w);

                            if (distance.y > 0)
                            {
                                if (distance.w > cliffAngle)
                                {
                                    alphamapData[x, z, cliffSplatMap] = cliffBlend;// Mathf.Clamp01(Mathf.Lerp(alphamapData[x, z, lakePolygon.cliffSplatMap], 1, lakePolygon.terrainPaintCarve.Evaluate(distance.y)));

                                    for (int l = 0; l < terrainData.terrainLayers.Length; l++)
                                    {
                                        if (l != cliffSplatMap)
                                        {
                                            alphamapData[x, z, l] = oldValue == 1 ? 0 : Mathf.Clamp01(alphamapData[x, z, l] * ((1 - alphamapData[x, z, cliffSplatMap]) / (1 - oldValue)));

                                        }
                                    }
                                }

                            }
                            else
                            {
                                if (distance.w > cliffAngleOutside)
                                {
                                    alphamapData[x, z, cliffSplatMapOutside] = cliffBlendOutside;// Mathf.Clamp01(Mathf.Lerp(alphamapData[x, z, lakePolygon.cliffSplatMap], 1, lakePolygon.terrainPaintCarve.Evaluate(distance.y)));

                                    for (int l = 0; l < terrainData.terrainLayers.Length; l++)
                                    {
                                        if (l != cliffSplatMapOutside)
                                        {
                                            alphamapData[x, z, l] = oldValue == 1 ? 0 : Mathf.Clamp01(alphamapData[x, z, l] * ((1 - alphamapData[x, z, cliffSplatMapOutside]) / (1 - oldValue)));

                                        }
                                    }
                                }
                            }

                        }
                    }
                }

            }


            if (meshGOs != null && meshGOs.Count > 0)
            {
                foreach (var item in meshGOs)
                {
                    DestroyImmediate(item);
                }
                meshGOs.Clear();
            }

            terrainData.SetAlphamaps((int)lakePolygonPaintData.minX, (int)lakePolygonPaintData.minZ, alphamapData);
            terrain.Flush();
            lakePolygonPaintData = null;


        }
        Physics.autoSyncTransforms = true;
    }

    public void TerrainClearTrees(bool details = true)
    {
        Terrain[] terrains = Terrain.activeTerrains;

        Physics.autoSyncTransforms = false;

        if (meshGOs != null && meshGOs.Count > 0)
        {
            foreach (var item in meshGOs)
            {
                DestroyImmediate(item);
            }
            meshGOs.Clear();
        }

        foreach (var terrain in terrains)
        {

            TerrainData terrainData = terrain.terrainData;

            Transform transformTerrain = terrain.transform;
            float polygonHeight = transform.position.y;
            float posY = terrain.transform.position.y;
            float sizeX = terrain.terrainData.size.x;
            float sizeY = terrain.terrainData.size.y;
            float sizeZ = terrain.terrainData.size.z;


            int[,] detailLayer;




#if UNITY_EDITOR
            Undo.RegisterCompleteObjectUndo(terrainData, "Paint lake");
            Undo.RegisterCompleteObjectUndo(terrain, "Terrain draw texture");
#endif
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minZ = float.MaxValue;
            float maxZ = float.MinValue;

            for (int i = 0; i < splinePoints.Count; i++)
            {
                Vector3 point = transform.TransformPoint(splinePoints[i]);


                if (minX > point.x)
                    minX = point.x;

                if (maxX < point.x)
                    maxX = point.x;

                if (minZ > point.z)
                    minZ = point.z;

                if (maxZ < point.z)
                    maxZ = point.z;
            }



            //Debug.DrawLine(new Vector3(minX, 0, minZ), new Vector3(minX, 0, minZ) + Vector3.up * 100, Color.green, 3);
            // Debug.DrawLine(new Vector3(maxX, 0, maxZ), new Vector3(maxX, 0, maxZ) + Vector3.up * 100, Color.blue, 3);


            float terrainTowidth = (1 / sizeX * (terrainData.detailWidth - 1));
            float terrainToheight = (1 / sizeZ * (terrainData.detailHeight - 1));
            minX -= terrain.transform.position.x + distanceClearFoliage;
            maxX -= terrain.transform.position.x - distanceClearFoliage;

            minZ -= terrain.transform.position.z + distanceClearFoliage;
            maxZ -= terrain.transform.position.z - distanceClearFoliage;


            minX = minX * terrainToheight;
            maxX = maxX * terrainToheight;

            minZ = minZ * terrainTowidth;
            maxZ = maxZ * terrainTowidth;

            minX = (int)Mathf.Clamp(minX, 0, (terrainData.detailWidth));
            maxX = (int)Mathf.Clamp(maxX, 0, (terrainData.detailWidth));
            minZ = (int)Mathf.Clamp(minZ, 0, (terrainData.detailHeight));
            maxZ = (int)Mathf.Clamp(maxZ, 0, (terrainData.detailHeight));

            detailLayer = terrainData.GetDetailLayer((int)minX, (int)minZ, (int)(maxX - minX), (int)(maxZ - minZ), 0);

            Vector4[,] distances = new Vector4[detailLayer.GetLength(0), detailLayer.GetLength(1)];

            MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();



            Vector3 position = Vector3.zero;
            position.y = polygonHeight;

            for (int x = 0; x < detailLayer.GetLength(0); x++)
            {
                for (int z = 0; z < detailLayer.GetLength(1); z++)
                {


                    position.x = (z + minX) / (float)terrainToheight + transformTerrain.position.x;//, polygonHeight
                    position.z = (x + minZ) / (float)terrainTowidth + transformTerrain.position.z;

                    Ray ray = new Ray(position + Vector3.up * 1000, Vector3.down);
                    RaycastHit hit;

                    if (meshCollider.Raycast(ray, out hit, 10000))
                    {
                        // Debug.DrawLine(hit.point, hit.point + Vector3.up * 30, Color.green, 3);

                        float minDist = float.MaxValue;
                        for (int i = 0; i < splinePoints.Count; i++)
                        {
                            int idOne = i;
                            int idTwo = (i + 1) % splinePoints.Count;

                            float dist = DistancePointLine(hit.point, transform.TransformPoint(splinePoints[idOne]), transform.TransformPoint(splinePoints[idTwo]));
                            if (minDist > dist)
                                minDist = dist;
                        }

                        float angle = 0;


                        distances[x, z] = new Vector4(hit.point.x, minDist, hit.point.z, angle);

                    }
                    else
                    {
                        float minDist = float.MaxValue;
                        for (int i = 0; i < splinePoints.Count; i++)
                        {
                            int idOne = i;
                            int idTwo = (i + 1) % splinePoints.Count;

                            float dist = DistancePointLine(position, transform.TransformPoint(splinePoints[idOne]), transform.TransformPoint(splinePoints[idTwo]));
                            if (minDist > dist)
                                minDist = dist;
                        }

                        float angle = 0;

                        distances[x, z] = new Vector4(position.x, -minDist, position.z, angle);
                    }

                }
            }

            if (!details)
            {
                List<TreeInstance> newTrees = new List<TreeInstance>();
                TreeInstance[] oldTrees = terrainData.treeInstances;

                position.y = polygonHeight;
                foreach (var tree in oldTrees)
                {
                    //Debug.DrawRay(new Vector3(, 0, tree.position.z * sizeZ) + terrain.transform.position, Vector3.up * 5, Color.red, 3);

                    position.x = tree.position.x * sizeX + transformTerrain.position.x;//, polygonHeight
                    position.z = tree.position.z * sizeZ + transformTerrain.position.z;

                    Ray ray = new Ray(position + Vector3.up * 1000, Vector3.down);
                    RaycastHit hit;

                    if (!meshCollider.Raycast(ray, out hit, 10000))
                    {
                        float minDist = float.MaxValue;
                        for (int i = 0; i < splinePoints.Count; i++)
                        {
                            int idOne = i;
                            int idTwo = (i + 1) % splinePoints.Count;

                            float dist = DistancePointLine(position, transform.TransformPoint(splinePoints[idOne]), transform.TransformPoint(splinePoints[idTwo]));
                            if (minDist > dist)
                                minDist = dist;
                        }

                        if (minDist > distanceClearFoliageTrees)
                        {
                            newTrees.Add(tree);
                        }

                    }
                }
                terrainData.treeInstances = newTrees.ToArray();
                DestroyImmediate(meshCollider);
            }

            lakePolygonClearData = new LakePolygonCarveData();
            lakePolygonClearData.minX = minX;
            lakePolygonClearData.maxX = maxX;
            lakePolygonClearData.minZ = minZ;
            lakePolygonClearData.maxZ = maxZ;
            lakePolygonClearData.distances = distances;



            // terrainData.treeInstances = newTrees.ToArray();
            if (details)
            {
                for (int l = 0; l < terrainData.detailPrototypes.Length; l++)
                {
                    detailLayer = terrainData.GetDetailLayer((int)lakePolygonClearData.minX, (int)lakePolygonClearData.minZ, (int)(lakePolygonClearData.maxX - lakePolygonClearData.minX), (int)(lakePolygonClearData.maxZ - lakePolygonClearData.minZ), l);

                    List<List<Vector4>> positionArray = new List<List<Vector4>>();
                    for (int x = 0; x < detailLayer.GetLength(0); x++)
                    {
                        List<Vector4> positionArrayRow = new List<Vector4>();
                        for (int z = 0; z < detailLayer.GetLength(1); z++)
                        {

                            Vector4 distance = lakePolygonClearData.distances[x, z];

                            if ((-distance.y <= distanceClearFoliage) || (distance.y > 0))
                            {
                                float oldValue = detailLayer[x, z];
                                detailLayer[x, z] = 0;

                            }
                        }

                    }

                    terrainData.SetDetailLayer((int)lakePolygonClearData.minX, (int)lakePolygonClearData.minZ, l, detailLayer);
                }
            }

            if (meshGOs != null && meshGOs.Count > 0)
            {
                foreach (var item in meshGOs)
                {
                    DestroyImmediate(item);
                }
                meshGOs.Clear();
            }


            terrain.Flush();
            lakePolygonClearData = null;


        }
        Physics.autoSyncTransforms = true;
    }

    public void Simulation()
    {
#if UNITY_EDITOR
        Undo.RegisterCompleteObjectUndo(this, "Simulate lake");
        Undo.RegisterCompleteObjectUndo(transform, "Simulate lake");
        if (meshfilter != null)
            Undo.RegisterCompleteObjectUndo(meshfilter, "Simulate lake");
#endif
        List<Vector3> vectorPoints = new List<Vector3>();
        vectorPoints.Add(transform.TransformPoint(points[0]));

        int iterations = 1;

        for (int i = 0; i < iterations; i++)
        {
            List<Vector3> newPoints = new List<Vector3>();
            foreach (var vec in vectorPoints)
            {

                for (int angle = 0; angle <= 360; angle += angleSimulation)
                {
                    RaycastHit hit;
                    Ray ray = new Ray(vec, (new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad)).normalized));
                    if (Physics.Raycast(ray, out hit, checkDistanceSimulation))
                    {
                        //Debug.Log(hit.distance);
                        //Debug.DrawRay(hit.point, Vector3.up * angle * 0.1f, Color.red, 1 + angle / (float)100);
                        // Debug.DrawLine(hit.point, vec, Color.green, 1 + angle / (float)100);
                        bool tooClose = false;
                        Vector3 point = hit.point;
                        foreach (var item in vectorPoints)
                        {
                            if (Vector3.Distance(point, item) < closeDistanceSimulation)
                            {
                                tooClose = true;
                                break;
                            }
                        }
                        foreach (var item in newPoints)
                        {
                            if (Vector3.Distance(point, item) < closeDistanceSimulation)
                            {
                                tooClose = true;
                                break;
                            }
                        }
                        if (!tooClose)
                        {
                            newPoints.Add(point + ray.direction * 0.3f);
                        }
                    }
                    else
                    {
                        bool tooClose = false;
                        Vector3 point = ray.origin + ray.direction * 50;
                        foreach (var item in vectorPoints)
                        {
                            if (Vector3.Distance(point, item) < closeDistanceSimulation)
                            {
                                tooClose = true;
                                break;
                            }
                        }
                        foreach (var item in newPoints)
                        {
                            if (Vector3.Distance(point, item) < closeDistanceSimulation)
                            {
                                tooClose = true;
                                break;
                            }
                        }
                        if (!tooClose)
                        {
                            newPoints.Add(point);
                        }
                    }
                }
            }
            if (i == 0)
                vectorPoints.AddRange(newPoints);
            else
            {

                for (int k = 0; k < newPoints.Count; k++)
                {
                    float min = float.MaxValue;
                    int idMin = -1;
                    Vector3 point = newPoints[k];
                    for (int p = 0; p < vectorPoints.Count; p++)
                    {
                        Vector3 posOne = vectorPoints[p];
                        Vector3 posTwo = vectorPoints[(p + 1) % vectorPoints.Count];

                        bool intersects = false;
                        for (int f = 0; f < vectorPoints.Count; f++)
                        {
                            if (p != f)
                            {
                                Vector3 posCheckOne = vectorPoints[f];
                                Vector3 posCheckTwo = vectorPoints[(f + 1) % vectorPoints.Count];

                                if (AreLinesIntersecting(posOne, point, posCheckOne, posCheckTwo) || AreLinesIntersecting(point, posTwo, posCheckOne, posCheckTwo))
                                {
                                    intersects = true;
                                    break;
                                }
                            }
                        }

                        if (!intersects)
                        {
                            float dist = Vector3.Distance(point, posTwo);
                            if (min > dist)
                            {
                                min = dist;
                                idMin = (p + 1) % vectorPoints.Count;
                            }
                            //vectorPoints.Insert(p + 1, point);
                            //break;
                        }

                    }
                    if (idMin > -1)
                        vectorPoints.Insert(idMin, point);
                }
            }
            if (i == 0 && removeFirstPointSimulation)
                vectorPoints.RemoveAt(0);
        }






        //List<Vector3> finalListPoints = new List<Vector3>();
        //for (int i = 0; i < 3; i++)
        //{
        //    finalListPoints.Add(vectorPoints[i]);
        //}
        //for (int i = 3; i < vectorPoints.Count; i++)
        //{
        //    Vector3 point = vectorPoints[i];
        //    bool foundPosition = false;
        //    for (int p = 0; p < finalListPoints.Count; p++)
        //    {
        //        Vector3 posOne = finalListPoints[p];
        //        Vector3 posTwo = finalListPoints[(p + 1) % finalListPoints.Count];

        //        bool intersects = false;
        //        for (int f = 0; f < finalListPoints.Count; f++)
        //        {
        //            if (p != f)
        //            {
        //                Vector3 posCheckOne = finalListPoints[f];
        //                Vector3 posCheckTwo = finalListPoints[(f + 1) % finalListPoints.Count];

        //                if (AreLinesIntersecting(posOne, point, posCheckOne, posCheckTwo) || AreLinesIntersecting(point, posTwo, posCheckOne, posCheckTwo))
        //                {
        //                    intersects = true;
        //                    break;
        //                }
        //            }
        //        }

        //        if (!intersects)
        //        {
        //            finalListPoints.Insert(p + 1, point);
        //            break;
        //        }

        //    }
        //}

        //for (int i = 0; i < vectorPoints.Count; i++)
        //{
        //    Debug.DrawRay(vectorPoints[i], Vector3.up * (i + 1) * 0.5f, Color.black);
        //}

        //for (int i = 0; i < vectorPoints.Count; i++)
        //{
        //    Debug.DrawLine(vectorPoints[i], vectorPoints[(i + 1) % vectorPoints.Count], Color.red, 3);
        //}

        points.Clear();

        foreach (var vec in vectorPoints)
        {
            points.Add(transform.InverseTransformPoint(vec));
        }
        GeneratePolygon();
    }

    public static bool AreLinesIntersecting(Vector3 l1_p1, Vector3 l1_p2, Vector3 l2_p1, Vector3 l2_p2, bool shouldIncludeEndPoints = true)
    {
        //To avoid floating point precision issues we can add a small value
        float epsilon = 0.00001f;

        bool isIntersecting = false;

        float denominator = (l2_p2.z - l2_p1.z) * (l1_p2.x - l1_p1.x) - (l2_p2.x - l2_p1.x) * (l1_p2.z - l1_p1.z);

        //Make sure the denominator is > 0, if not the lines are parallel
        if (denominator != 0f)
        {
            float u_a = ((l2_p2.x - l2_p1.x) * (l1_p1.z - l2_p1.z) - (l2_p2.z - l2_p1.z) * (l1_p1.x - l2_p1.x)) / denominator;
            float u_b = ((l1_p2.x - l1_p1.x) * (l1_p1.z - l2_p1.z) - (l1_p2.z - l1_p1.z) * (l1_p1.x - l2_p1.x)) / denominator;

            //Are the line segments intersecting if the end points are the same
            if (shouldIncludeEndPoints)
            {
                //Is intersecting if u_a and u_b are between 0 and 1 or exactly 0 or 1
                if (u_a >= 0f + epsilon && u_a <= 1f - epsilon && u_b >= 0f + epsilon && u_b <= 1f - epsilon)
                {
                    isIntersecting = true;
                }
            }
            else
            {
                //Is intersecting if u_a and u_b are between 0 and 1
                if (u_a > 0f + epsilon && u_a < 1f - epsilon && u_b > 0f + epsilon && u_b < 1f - epsilon)
                {
                    isIntersecting = true;
                }
            }
        }

        return isIntersecting;
    }


    #endregion
    public static float DistancePointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        return Vector3.Distance(ProjectPointLine(point, lineStart, lineEnd), point);
    }
    public static Vector3 ProjectPointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 rhs = point - lineStart;
        Vector3 vector2 = lineEnd - lineStart;
        float magnitude = vector2.magnitude;
        Vector3 lhs = vector2;
        if (magnitude > 1E-06f)
        {
            lhs = (Vector3)(lhs / magnitude);
        }
        float num2 = Mathf.Clamp(Vector3.Dot(lhs, rhs), 0f, magnitude);
        return (lineStart + ((Vector3)(lhs * num2)));
    }

}
public class LakePolygonCarveData
{
    public float distSmooth = 0;
    public float minX = float.MaxValue;
    public float maxX = float.MinValue;
    public float minZ = float.MaxValue;
    public float maxZ = float.MinValue;

    public Vector4[,] distances;

}