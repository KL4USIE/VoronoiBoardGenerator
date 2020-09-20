using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(MeshColoringRam))]
public class MeshColoringRamEditor : Editor
{

    RamSpline[] ramSplines;
    LakePolygon[] lakePolygons;
    MeshFilter[] meshFilterInScene;


    void OnEnable()
    {
        ramSplines = FindObjectsOfType<RamSpline>();
        lakePolygons = FindObjectsOfType<LakePolygon>();
        meshFilterInScene = Resources.FindObjectsOfTypeAll<MeshFilter>();
    }

    public override void OnInspectorGUI()
    {
        MeshColoringRam coloringMesh = (MeshColoringRam)target;

        if (coloringMesh.autoColor && GUILayout.Button("Auto Color On"))
        {
            coloringMesh.autoColor = false;

        }

        if (!coloringMesh.autoColor && GUILayout.Button("Auto Color Off"))
        {
            coloringMesh.autoColor = true;

        }

        if (!coloringMesh.autoColor && GUILayout.Button("Color Mesh Vertex"))
        {
            ClearColors(coloringMesh);
            ColorMesh(coloringMesh);
        }

        EditorGUI.BeginChangeCheck();

        coloringMesh.threshold = EditorGUILayout.FloatField("Threshold", coloringMesh.threshold);
        coloringMesh.height = EditorGUILayout.FloatField("Height above water", coloringMesh.height);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(coloringMesh, "Coloring mesh change values");
            ClearColors(coloringMesh);
            ColorMesh(coloringMesh);
        }

        if (GUILayout.Button("Clear Mesh Vertex Color"))
        {
            ClearColors(coloringMesh);

        }

        coloringMesh.newMesh = EditorGUILayout.Toggle("Create new mesh on copy", coloringMesh.newMesh);

        EditorGUILayout.Space();
        coloringMesh.colorMeshLive = EditorGUILayout.Toggle("Play mode coloring", coloringMesh.colorMeshLive);
        coloringMesh.layer = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(
            EditorGUILayout.MaskField("R.A.M Layer", InternalEditorUtility.LayerMaskToConcatenatedLayersMask(coloringMesh.layer),
            InternalEditorUtility.layers));

    }

    protected virtual void OnSceneGUI()
    {
        // if (Selection.gameObjects.Length > 0)
        //     return;


        MeshColoringRam coloringMesh = (MeshColoringRam)target;


        if (coloringMesh.autoColor && Vector3.Distance(coloringMesh.transform.position, coloringMesh.oldPosition) > 0.01f)
        {
            ClearColors(coloringMesh);
            ColorMesh(coloringMesh);
            coloringMesh.oldPosition = coloringMesh.transform.position;

        }

    }


    public void ClearColors(MeshColoringRam coloringMesh)
    {
        MeshFilter[] meshFilters = coloringMesh.gameObject.GetComponentsInChildren<MeshFilter>();
        foreach (var meshFilter in meshFilters)
        {

            if (meshFilter.sharedMesh != null)
            {
                Mesh mesh = meshFilter.sharedMesh;
                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(mesh)))
                {
                    mesh = Instantiate<Mesh>(meshFilter.sharedMesh);
                    meshFilter.sharedMesh = mesh;

                }

                int vertLength = mesh.vertices.Length;
                Vector3[] vertices = mesh.vertices;
                Color[] colors = mesh.colors;
                Transform transform = meshFilter.transform;
                if (colors.Length == 0)
                {
                    colors = new Color[vertLength];
                    for (int i = 0; i < colors.Length; i++)
                    {
                        colors[i] = Color.white;
                    }

                }

                for (int i = 0; i < vertLength; i++)
                {
                    colors[i] = Color.white;
                }


                mesh.colors = colors;

            }
        }
    }

    public void ColorMesh(MeshColoringRam coloringMesh)
    {
        MeshFilter[] meshFilters = coloringMesh.gameObject.GetComponentsInChildren<MeshFilter>();

        Ray ray = new Ray();
        ray.direction = Vector3.up;
        RaycastHit hit;
        Vector3 upVector = -Vector3.up * (coloringMesh.height + coloringMesh.threshold);
        foreach (var meshFilter in meshFilters)
        {
            bool addedMeshColldier = false;
            MeshCollider meshColliderGO = meshFilter.GetComponent<MeshCollider>();

            if (meshColliderGO == null)
            {
                addedMeshColldier = true;
                meshColliderGO = meshFilter.gameObject.AddComponent<MeshCollider>();
            }


            bool backFace = Physics.queriesHitBackfaces;
            Physics.queriesHitBackfaces = true;

            Mesh mesh = meshFilter.sharedMesh;

            if (meshFilter.sharedMesh != null)
            {

                int copyMeshCount = 0;

                if (meshFilterInScene != null)
                    foreach (var meshInScene in meshFilterInScene)
                    {
                        if (mesh == meshInScene.sharedMesh)
                            copyMeshCount++;
                        if (copyMeshCount > 1)
                            break;
                    }


                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(mesh)) || (copyMeshCount > 1 && coloringMesh.newMesh))
                {
                    mesh = Instantiate<Mesh>(meshFilter.sharedMesh);
                    meshFilter.sharedMesh = mesh;
                }

                int vertLength = mesh.vertices.Length;
                Vector3[] vertices = mesh.vertices;
                Color[] colors = mesh.colors;
                Transform transform = meshFilter.transform;

                for (int i = 0; i < vertLength; i++)
                {
                    vertices[i] = transform.TransformPoint(vertices[i]) + upVector;
                }


                foreach (var item in ramSplines)
                {
                    bool addedCollider = false;
                    MeshCollider meshCollider = item.GetComponent<MeshCollider>();
                    if (meshCollider == false)
                    {
                        addedCollider = true;
                        meshCollider = item.gameObject.AddComponent<MeshCollider>();

                    }


                    if (colors.Length == 0)
                    {
                        colors = new Color[vertLength];
                        for (int i = 0; i < colors.Length; i++)
                        {
                            colors[i] = Color.white;
                        }

                    }

                    for (int i = 0; i < vertLength; i++)
                    {
                        ray.origin = vertices[i];


                        if (meshCollider.Raycast(ray, out hit, 1000))
                        {
                            if (hit.distance > coloringMesh.threshold)
                                colors[i].r = 0;
                            else
                                colors[i].r = Mathf.Lerp(1, 0, hit.distance / (float)coloringMesh.threshold);
                        }
                    }

                    if (addedCollider)
                    {
                        DestroyImmediate(meshCollider);
                    }

                }

                foreach (var item in lakePolygons)
                {
                    bool addedCollider = false;
                    MeshCollider meshCollider = item.GetComponent<MeshCollider>();
                    if (meshCollider == false)
                    {
                        addedCollider = true;
                        meshCollider = item.gameObject.AddComponent<MeshCollider>();

                    }


                    if (colors.Length == 0)
                    {
                        colors = new Color[vertLength];
                        for (int i = 0; i < colors.Length; i++)
                        {
                            colors[i] = Color.white;
                        }

                    }

                    for (int i = 0; i < vertLength; i++)
                    {
                        ray.origin = vertices[i];


                        if (meshCollider.Raycast(ray, out hit, 1000))
                        {
                            if (hit.distance > coloringMesh.threshold)
                                colors[i].r = 0;
                            else
                                colors[i].r = Mathf.Lerp(1, 0, hit.distance / (float)coloringMesh.threshold);
                        }
                    }

                    if (addedCollider)
                    {
                        DestroyImmediate(meshCollider);
                    }

                }

                mesh.colors = colors;

            }

            if (addedMeshColldier)
                DestroyImmediate(meshColliderGO);

            Physics.queriesHitBackfaces = backFace;
        }
    }

}
