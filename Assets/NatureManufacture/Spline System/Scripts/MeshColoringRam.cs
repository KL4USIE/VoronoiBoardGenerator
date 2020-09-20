using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MeshColoringRam : MonoBehaviour
{
    public float height = 0.5f;
    public float threshold = 0.5f;
    public bool autoColor = true;
    public bool newMesh = true;
    public Vector3 oldPosition = Vector3.zero;


    public bool colorMeshLive = false;

    public LayerMask layer;

    MeshFilter[] meshFilters;
    bool colored;
    static RamSpline[] ramSplines;
    static LakePolygon[] lakePolygons;


    private void Start()
    {
        if (colorMeshLive)
        {
            if (ramSplines == null)
                ramSplines = FindObjectsOfType<RamSpline>();
            if (lakePolygons == null)
                lakePolygons = FindObjectsOfType<LakePolygon>();
            colored = false;
            meshFilters = this.gameObject.GetComponentsInChildren<MeshFilter>();


        }
    }


    private void Update()
    {
        if (colorMeshLive)
        {
            ColorMeshLive();
        }
    }


    public void ColorMeshLive()
    {
        colored = true;


        Ray ray = new Ray();
        ray.direction = Vector3.up;
        RaycastHit hit;
        Vector3 upVector = -Vector3.up * (this.height + this.threshold);
        Color white = Color.white;

        List<MeshCollider> meshColliders = new List<MeshCollider>();
        foreach (var item in ramSplines)
        {
            meshColliders.Add(item.gameObject.AddComponent<MeshCollider>());

        }

        foreach (var item in lakePolygons)
        {
            meshColliders.Add(item.gameObject.AddComponent<MeshCollider>());

        }
        bool backFace = Physics.queriesHitBackfaces;
        Physics.queriesHitBackfaces = true;
        foreach (var meshFilter in meshFilters)
        {



            Mesh mesh = meshFilter.sharedMesh;


            if (meshFilter.sharedMesh != null)
            {

                if (!colored)
                {
                    mesh = Instantiate<Mesh>(meshFilter.sharedMesh);
                    meshFilter.sharedMesh = mesh;
                    colored = true;
                }

                int vertLength = mesh.vertices.Length;
                Vector3[] vertices = mesh.vertices;
                Color[] colors = mesh.colors;
                Transform transform = meshFilter.transform;

                float minY = float.MaxValue;
                Vector3 lowestPoint = vertices[0];
                for (int i = 0; i < vertLength; i++)
                {
                    vertices[i] = transform.TransformPoint(vertices[i]) + upVector;

                    if (vertices[i].y < minY)
                    {
                        minY = vertices[i].y;
                        lowestPoint = vertices[i];
                    }
                }

                if (colors.Length == 0)
                {
                    colors = new Color[vertLength];
                    for (int i = 0; i < colors.Length; i++)
                    {
                        colors[i] = white;
                    }

                }
                ray.origin = lowestPoint;

                minY = float.MinValue;
                
                if (Physics.Raycast(ray, out hit, 100, layer))
                {
                    minY = hit.point.y;

                }

                float dist;
                for (int i = 0; i < vertLength; i++)
                {

                    if (vertices[i].y < minY)
                    {
                        dist = Mathf.Abs(vertices[i].y - minY);

                        if (dist > threshold)
                            colors[i].r = 0;
                        else
                            colors[i].r = Mathf.Lerp(1, 0, dist / (float)threshold);
                    }
                    else
                        colors[i] = white;
                }

                mesh.colors = colors;
            }


        }

        foreach (var item in meshColliders)
        {
            Destroy(item);
        }
        Physics.queriesHitBackfaces = backFace;
    }

}
