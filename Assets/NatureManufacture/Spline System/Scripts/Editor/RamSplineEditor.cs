using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using UnityEngine.Rendering;
using System.Linq;


#if VEGETATION_STUDIO_PRO
using AwesomeTechnologies;
using AwesomeTechnologies.VegetationSystem;
using AwesomeTechnologies.VegetationSystem.Biomes;
#endif

#if VEGETATION_STUDIO
using AwesomeTechnologies;
using AwesomeTechnologies.VegetationStudio;
#endif



[CustomEditor(typeof(RamSpline)), CanEditMultipleObjects]
public class RamSplineEditor : Editor
{
    //Vector2 scrollPos;

    RamSpline[] splines;
    RamSpline spline;

    Texture2D logo;
    int selectedPosition = -1;
    Vector3 pivotChange = Vector3.zero;

    bool terrainShapeShow = false;
    List<List<Vector4>> positionArray;

    int splitPoint = 1;

    Mesh meshTerrain;
    public string[] shadowcastingOptions = new string[] { "Off", "On", "TwoSided", "ShadowsOnly" };

    public string[] toolbarStrings = new string[] {
        "Basic ",
        "Points",
        "Vertex Color",
        "Flow Map",
        "Simulate",
        "Terrain",
        "File Points",
        "Tips",
        "Manual",
        "Video Tutorials"
        #if VEGETATION_STUDIO
        ,
        "Vegetation Studio"
        #endif
#if VEGETATION_STUDIO_PRO
        ,
        "Vegetation Studio Pro"
#endif
    };
    //, "Debug" };

    //	/// <summary>
    //	/// The button editing style.
    //	/// </summary>
    //	GUIStyle buttonEditingStyle;

    [MenuItem("GameObject/3D Object/Create River Spline")]
    static public void CreateSpline()
    {

        Selection.activeGameObject = RamSpline.CreateSpline(AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat")).gameObject;
    }

    void OnEnable()
    {
        splines = FindObjectsOfType<RamSpline>();
#if VEGETATION_STUDIO
        spline = (RamSpline)target;
        spline.vegetationMaskArea = spline.gameObject.GetComponent<VegetationMaskArea>();
#endif
        SceneView.duringSceneGui -= this.OnSceneGUIInvoke;
        SceneView.duringSceneGui += this.OnSceneGUIInvoke;
    }

    private void OnDisable()
    {
        if (spline != null && spline.meshGO != null)
            DestroyImmediate(spline.meshGO);

        SceneView.duringSceneGui -= this.OnSceneGUIInvoke;
    }


    private void OnDestroy()
    {
        if (spline != null && spline.meshGO != null)
            DestroyImmediate(spline.meshGO);


    }


    void CheckRotations()
    {
        bool nan = false;
        if (spline.controlPointsRotations == null)
        {
            spline.controlPointsRotations = new List<Quaternion>();
            nan = true;
        }
        if (spline.controlPoints.Count > spline.controlPointsRotations.Count)
        {
            nan = true;
            for (int i = 0; i < spline.controlPoints.Count - spline.controlPointsRotations.Count; i++)
            {
                spline.controlPointsRotations.Add(Quaternion.identity);
            }
        }
        for (int i = 0; i < spline.controlPointsRotations.Count; i++)
        {

            if (float.IsNaN(spline.controlPointsRotations[i].x) || float.IsNaN(spline.controlPointsRotations[i].y) || float.IsNaN(spline.controlPointsRotations[i].z) || float.IsNaN(spline.controlPointsRotations[i].w))
            {
                spline.controlPointsRotations[i] = Quaternion.identity;
                nan = true;
            }
            if (spline.controlPointsRotations[i].x == 0 && spline.controlPointsRotations[i].y == 0 && spline.controlPointsRotations[i].z == 0 && spline.controlPointsRotations[i].w == 0)
            {

                spline.controlPointsRotations[i] = Quaternion.identity;
                nan = true;
            }

            spline.controlPointsRotations[i] = Quaternion.Euler(spline.controlPointsRotations[i].eulerAngles);
        }



        if (nan)
            spline.GenerateSpline();
    }

    public override void OnInspectorGUI()
    {


        EditorGUILayout.Space();
        logo = (Texture2D)Resources.Load("logoRAM");



        Color baseCol = GUI.color;

        spline = (RamSpline)target;

        CheckRotations();

        if (spline.controlPoints.Count > spline.controlPointsSnap.Count)
        {

            for (int i = 0; i < spline.controlPoints.Count - spline.controlPointsSnap.Count; i++)
            {
                spline.controlPointsSnap.Add(0);
            }
        }

        if (spline.controlPoints.Count > spline.controlPointsMeshCurves.Count)
        {

            for (int i = 0; i < spline.controlPoints.Count - spline.controlPointsMeshCurves.Count; i++)
            {
                spline.controlPointsMeshCurves.Add(new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 0) }));
            }
        }



        if (spline.controlPoints.Count > spline.controlPointsOrientation.Count)
            spline.GenerateSpline();


        GUIContent btnTxt = new GUIContent(logo);

        var rt = GUILayoutUtility.GetRect(btnTxt, GUI.skin.label, GUILayout.ExpandWidth(false));
        rt.center = new Vector2(EditorGUIUtility.currentViewWidth / 2, rt.center.y);

        GUI.Button(rt, btnTxt, GUI.skin.label);

        EditorGUI.BeginChangeCheck();

        //  GUILayout.Toolbar(spline.toolbarInt, toolbarStrings);
        int toolbarNew = GUILayout.SelectionGrid(spline.toolbarInt, toolbarStrings, 3, GUILayout.Height(125));

        if (toolbarNew == 8)
        {
            toolbarNew = spline.toolbarInt;
            string[] guids1 = AssetDatabase.FindAssets("River Auto and Lava Volcano Environment Manual 2019");
            Application.OpenURL("file:///" + Application.dataPath.Replace("Assets", "") + AssetDatabase.GUIDToAssetPath(guids1[0]));
        }
        if (toolbarNew == 9)
        {
            toolbarNew = spline.toolbarInt;
            Application.OpenURL("https://www.youtube.com/playlist?list=PLWMxYDHySK5PkIlklmHKLYvRWK2sjDYXX");


        }

        if (spline.toolbarInt != toolbarNew)
        {
            if (spline.meshGO != null)
                DestroyImmediate(spline.meshGO);
        }

        spline.toolbarInt = toolbarNew;

        EditorGUILayout.Space();


        spline.drawOnMesh = false;
        spline.drawOnMeshFlowMap = false;


        if (spline.showFlowMap)
        {
            if (spline.uvRotation)
                spline.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_RotateUV", 1);
            else
                spline.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_RotateUV", 0);
        }

        if (spline.transform.eulerAngles.magnitude != 0 || spline.transform.localScale.x != 1 || spline.transform.localScale.y != 1 || spline.transform.localScale.z != 1)
            EditorGUILayout.HelpBox("River should have scale (1,1,1) and rotation (0,0,0) during edit!", MessageType.Error);


        if (spline.toolbarInt == 0)
        {

            EditorGUILayout.HelpBox("Add Point  - CTRL + Left Mouse Button Click \n" +
                "Add point between existing points - SHIFT + Left Button Click \n" +
                "Remove point - CTRL + SHIFT + Left Button Click", MessageType.Info);
            EditorGUI.indentLevel++;

            AddPointAtEnd();

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            MeshSettings();



            GUILayout.Label("UV settings:", EditorStyles.boldLabel);
            if (spline.beginningSpline == null && spline.endingSpline == null)
            {
                spline.uvScale = EditorGUILayout.FloatField("UV scale (texture tiling)", spline.uvScale);
            }
            else
            {

                spline.uvScaleOverride = EditorGUILayout.Toggle("Parent UV scale override", spline.uvScaleOverride);
                if (!spline.uvScaleOverride)
                {
                    if (spline.beginningSpline != null)
                        spline.uvScale = spline.beginningSpline.uvScale;
                    if (spline.endingSpline != null)
                        spline.uvScale = spline.endingSpline.uvScale;


                    GUI.enabled = false;
                }
                spline.uvScale = EditorGUILayout.FloatField("UV scale (texture tiling)", spline.uvScale);
                GUI.enabled = true;



            }
            spline.invertUVDirection = EditorGUILayout.Toggle("Invert UV direction", spline.invertUVDirection);


            spline.uvRotation = EditorGUILayout.Toggle("Rotate UV", spline.uvRotation);


            GUILayout.Label("Lightning settings:", EditorStyles.boldLabel);
            spline.receiveShadows = EditorGUILayout.Toggle("Receive Shadows", spline.receiveShadows);



            spline.shadowCastingMode = (ShadowCastingMode)EditorGUILayout.EnumPopup("Shadow Casting Mode", spline.shadowCastingMode);


            EditorGUILayout.Space();

            //SetMaterials ();


            EditorGUILayout.Space();

            ParentingSplineUI();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUILayout.Label("Mesh spliting:", EditorStyles.boldLabel);
            spline.generateMeshParts = EditorGUILayout.Toggle("Split mesh into submeshes", spline.generateMeshParts);
            if (spline.generateMeshParts)
            {
                spline.meshPartsCount = EditorGUILayout.IntSlider("Parts", spline.meshPartsCount, 2, (int)((1 / (float)spline.traingleDensity) * (spline.controlPoints.Count - 1) * 0.5));

            }


            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Split spline into 2 splines:");

            EditorGUI.indentLevel++;
            if (spline.controlPoints.Count > 2)
            {
                splitPoint = EditorGUILayout.IntSlider("Split point", splitPoint, 1, spline.controlPoints.Count - 1);
                if (GUILayout.Button("Split spline"))
                {
                    SplitRiver(splitPoint);
                }
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            GUILayout.Label("Object settings:", EditorStyles.boldLabel);


            EditorGUILayout.Space();
            if (GUILayout.Button("Set object pivot to center"))
            {
                Vector3 center = spline.meshfilter.sharedMesh.bounds.center;

                ChangePivot(center);

            }
            EditorGUILayout.BeginHorizontal();
            {

                if (GUILayout.Button("Set object pivot position"))
                {
                    ChangePivot(pivotChange - spline.transform.position);
                }
                pivotChange = EditorGUILayout.Vector3Field("", pivotChange);



            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.Space();






            EditorGUILayout.Space();
            if (GUILayout.Button(new GUIContent("Regenerate spline", "Racalculates whole mesh")))
            {
                spline.GenerateSpline();
#if VEGETATION_STUDIO_PRO
                RegeneratBiomeMask();
#endif
            }
            spline.generateOnStart = EditorGUILayout.Toggle("Regenerate on Start", spline.generateOnStart);

            EditorGUILayout.Space();
            if (GUILayout.Button("Export as mesh"))
            {

                string path = EditorUtility.SaveFilePanelInProject("Save river mesh", "", "asset", "Save river mesh");


                if (path.Length != 0 && spline.meshfilter.sharedMesh != null)
                {

                    AssetDatabase.CreateAsset(spline.meshfilter.sharedMesh, path);

                    AssetDatabase.Refresh();
                    spline.GenerateSpline();
                }

            }



            EditorGUILayout.Space();
            GUILayout.Label("Debug Settings: ", EditorStyles.boldLabel);


            spline.debug = EditorGUILayout.Toggle("Show debug gizmos", spline.debug);

            if (spline.debug)
            {
                EditorGUI.indentLevel++;
                // spline.debugMesh = EditorGUILayout.Toggle("Debug Mesh", spline.debugMesh);
                spline.distanceToDebug = EditorGUILayout.DelayedFloatField("Debug distance", spline.distanceToDebug);
                spline.debugTangents = EditorGUILayout.Toggle("Show tangents", spline.debugTangents);
                // spline.debugBitangent = EditorGUILayout.Toggle("Show bitangents", spline.debugBitangent);
                spline.debugNormals = EditorGUILayout.Toggle("Show normals", spline.debugNormals);
                spline.debugFlowmap = EditorGUILayout.Toggle("Show flow map", spline.debugFlowmap);
                spline.debugPoints = EditorGUILayout.Toggle("Show points", spline.debugPoints);
                spline.debugPointsConnect = EditorGUILayout.Toggle("Show points connect", spline.debugPointsConnect);

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();





        }
        else if (spline.toolbarInt == 1)
        {

            PointsUI();

        }
        else if (spline.toolbarInt == 2)
        {

            DrawVertexColorsUI();

        }
        else if (spline.toolbarInt == 3)
        {

            DrawFlowColorsUI();

        }
        else if (spline.toolbarInt == 4)
        {

            EditorGUILayout.HelpBox("\nSet 1 point and R.A.M will show potential river direction.\n", MessageType.Info);
            GUILayout.Label("River simulation:", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            spline.simulatedRiverLength = EditorGUILayout.FloatField("Simulation length", spline.simulatedRiverLength);
            if (spline.simulatedRiverLength < 1)
                spline.simulatedRiverLength = 1;
            spline.simulatedRiverPoints = EditorGUILayout.IntSlider("Simulation points interval", spline.simulatedRiverPoints, 1, 100);
            spline.simulatedMinStepSize = EditorGUILayout.Slider("Simulation sampling interval", spline.simulatedMinStepSize, 0.5f, 5);
            spline.simulatedNoUp = EditorGUILayout.Toggle("Simulation block uphill", spline.simulatedNoUp);
            spline.simulatedBreakOnUp = EditorGUILayout.Toggle("Simulation break on uphill", spline.simulatedBreakOnUp);

            spline.noiseWidth = EditorGUILayout.Toggle("Add width noise", spline.noiseWidth);
            if (spline.noiseWidth)
            {
                EditorGUI.indentLevel++;
                spline.noiseMultiplierWidth = EditorGUILayout.FloatField("Noise Multiplier Width", spline.noiseMultiplierWidth);
                spline.noiseSizeWidth = EditorGUILayout.FloatField("Noise Scale Width", spline.noiseSizeWidth);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Show simulated River"))
            {
                spline.SimulateRiver(false);
            }
            if (GUILayout.Button("Generate Simulated River"))
            {
                Undo.RecordObject(spline, "Spline changed");
                spline.SimulateRiver();
            }
            if (GUILayout.Button("Remove points except first"))
            {
                spline.RemovePoints(0);
            }
            EditorGUI.indentLevel--;
        }
        else if (spline.toolbarInt == 5)
        {
            Terrain terrain = Terrain.activeTerrain;
            if (terrain != null && terrain.terrainData != null && terrain.terrainData.terrainLayers != null)
            {

                RiverChannel();
            }
            else
                EditorGUILayout.HelpBox("No terrain on scene.", MessageType.Warning);
        }
        else if (spline.toolbarInt == 6)
        {
            FilesManager();
        }
        else if (spline.toolbarInt == 7)
        {

            Tips();
        }

#if VEGETATION_STUDIO
            if (spline.toolbarInt == 10)
            {
                EditorGUILayout.Space();
                GUILayout.Label("Vegetation Studio: ", EditorStyles.boldLabel);
                spline.vegetationMaskPerimeter = EditorGUILayout.FloatField("Vegetation Mask Perimeter", spline.vegetationMaskPerimeter);
                if (spline.vegetationMaskArea == null && GUILayout.Button("Add Vegetation Mask Area"))
                {
                    spline.vegetationMaskArea = spline.gameObject.AddComponent<VegetationMaskArea>();
                    RegenerateVegetationMask();
                }
                if (spline.vegetationMaskArea != null && GUILayout.Button("Calculate hull outline"))
                {

                    RegenerateVegetationMask();
                }

            }

#endif

#if VEGETATION_STUDIO_PRO
        if (spline.toolbarInt == 10)
        {
            EditorGUILayout.Space();
            GUILayout.Label("Vegetation Studio Pro: ", EditorStyles.boldLabel);
            spline.vegetationMaskSize = EditorGUILayout.FloatField("Vegetation Mask Size", spline.vegetationMaskSize);
            spline.vegetationBlendDistance = EditorGUILayout.FloatField("Vegetation Blend Distance", spline.vegetationBlendDistance);
            spline.biomMaskResolution = EditorGUILayout.Slider("Mask Resolution", spline.biomMaskResolution, 0.1f, 1);
            if (spline.biomeMaskArea != null)
                spline.refreshMask = EditorGUILayout.Toggle("Auto Refresh Biome Mask", spline.refreshMask);

            if (GUILayout.Button("Add Vegetation Biome Mask Area"))
            {
                spline.GenerateSpline();
                if (spline.biomeMaskArea == null)
                {
                    spline.biomeMaskArea = spline.GetComponentInChildren<BiomeMaskArea>();
                    if (spline.biomeMaskArea == null)
                    {
                        GameObject maskObject = new GameObject("MyMask");
                        maskObject.transform.SetParent(spline.transform);
                        spline.biomeMaskArea = maskObject.AddComponent<BiomeMaskArea>();
                        spline.biomeMaskArea.BiomeType = BiomeType.River;
                    }
                }

                if (spline.biomeMaskArea == null)
                    return;

                RegeneratBiomeMask(false);

            }
        }


#endif

        //if (spline.toolbarInt == 6)
        //{

        //    DebugOptions();
        //}


        if (EditorGUI.EndChangeCheck())
        {
            if (spline != null)
            {
                Undo.RecordObject(spline, "Spline changed");
                spline.GenerateSpline();
#if VEGETATION_STUDIO_PRO
                RegeneratBiomeMask();
#endif
            }

        }

        EditorGUILayout.Space();

        if (spline.beginningSpline)
        {
            if (!spline.beginningSpline.endingChildSplines.Contains(spline))
            {
                spline.beginningSpline.endingChildSplines.Add(spline);

            }
        }

        if (spline.endingSpline)
        {
            if (!spline.endingSpline.beginnigChildSplines.Contains(spline))
            {
                spline.endingSpline.beginnigChildSplines.Add(spline);

            }
        }
    }

#if VEGETATION_STUDIO
    private void RegenerateVegetationMask()
    {
        spline.vegetationMaskArea.AdditionalGrassPerimiterMax = spline.vegetationMaskPerimeter;
        spline.vegetationMaskArea.AdditionalLargeObjectPerimiterMax = spline.vegetationMaskPerimeter;
        spline.vegetationMaskArea.AdditionalObjectPerimiterMax = spline.vegetationMaskPerimeter;
        spline.vegetationMaskArea.AdditionalPlantPerimiterMax = spline.vegetationMaskPerimeter;
        spline.vegetationMaskArea.AdditionalTreePerimiterMax = spline.vegetationMaskPerimeter;
        spline.vegetationMaskArea.GenerateHullNodes(spline.vegetationMaskArea.ReductionTolerance);

        spline.GenerateSpline();
        List<Vector3> worldspacePointList = new List<Vector3>();
        for (int i = 0; i < spline.pointsUp.Count; i += 5)
        {
            Vector3 position = spline.transform.TransformPoint(spline.pointsUp[i]) + (spline.transform.TransformPoint(spline.pointsUp[i]) - spline.transform.TransformPoint(spline.pointsDown[i])).normalized * spline.vegetationMaskPerimeter;

            worldspacePointList.Add(position);
        }
        for (int i = 0; i < spline.pointsDown.Count; i += 5)
        {
            int ind = spline.pointsDown.Count - i - 1;
            Vector3 position = spline.transform.TransformPoint(spline.pointsDown[ind]) + (spline.transform.TransformPoint(spline.pointsDown[ind]) - spline.transform.TransformPoint(spline.pointsUp[ind])).normalized * spline.vegetationMaskPerimeter;
            worldspacePointList.Add(position);
        }

        spline.vegetationMaskArea.ClearNodes();

        for (var i = 0; i <= worldspacePointList.Count - 1; i++)
        {
            spline.vegetationMaskArea.AddNodeToEnd(worldspacePointList[i]);
        }
        spline.vegetationMaskArea.UpdateVegetationMask();
    }
#endif

#if VEGETATION_STUDIO_PRO
    void RegeneratBiomeMask(bool checkAuto = true)
    {

        if (checkAuto && !spline.refreshMask)
            return;

        if (spline.biomeMaskArea == null)
            return;


        List<bool> disableEdges = new List<bool>();
        List<Vector3> worldspacePointList = new List<Vector3>();
        for (int i = 0; i < spline.pointsUp.Count; i += (int)(1 / (float)spline.biomMaskResolution))
        {
            Vector3 position = spline.transform.TransformPoint(spline.pointsUp[i]) + (spline.transform.TransformPoint(spline.pointsUp[i]) - spline.transform.TransformPoint(spline.pointsDown[i])).normalized * spline.vegetationMaskSize;

            worldspacePointList.Add(position);

            if (i == 0 || (i + (int)(1 / (float)spline.biomMaskResolution)) >= spline.pointsUp.Count)
            {
                disableEdges.Add(true);
            }
            else
            {
                disableEdges.Add(false);
            }
        }
        for (int i = 0; i < spline.pointsDown.Count; i += (int)(1 / (float)spline.biomMaskResolution))
        {
            int ind = spline.pointsDown.Count - i - 1;
            Vector3 position = spline.transform.TransformPoint(spline.pointsDown[ind]) + (spline.transform.TransformPoint(spline.pointsDown[ind]) - spline.transform.TransformPoint(spline.pointsUp[ind])).normalized * spline.vegetationMaskSize;
            worldspacePointList.Add(position);

            if (i == 0 || (i + (int)(1 / (float)spline.biomMaskResolution)) >= spline.pointsDown.Count)
            {
                disableEdges.Add(true);
            }
            else
            {
                disableEdges.Add(false);
            }
        }

        spline.biomeMaskArea.ClearNodes();

        spline.biomeMaskArea.AddNodesToEnd(worldspacePointList.ToArray(), disableEdges.ToArray());



        //these have default values but you can set them if you want a different default setting
        spline.biomeMaskArea.BlendDistance = spline.vegetationBlendDistance;
        spline.biomeMaskArea.NoiseScale = 5;
        spline.biomeMaskArea.UseNoise = true;

        if (spline.currentProfile != null)
        {
            spline.biomeMaskArea.BiomeType = (BiomeType)spline.currentProfile.biomeType;

        }
        else
            spline.biomeMaskArea.BiomeType = BiomeType.River;

        //These 3 curves holds the blend curves for vegetation and textures. they have default values;
        //biomeMaskArea.BlendCurve;
        //biomeMaskArea.InverseBlendCurve;
        //biomeMaskArea.TextureBlendCurve;

        spline.biomeMaskArea.UpdateBiomeMask();

    }
#endif

    void RiverChannel()
    {



        EditorGUI.BeginChangeCheck();
        EditorGUILayout.Space();
        GUILayout.Label("Terrain carve:", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        spline.terrainCarve = EditorGUILayout.CurveField("Terrain carve", spline.terrainCarve);
        spline.terrainSmoothMultiplier = EditorGUILayout.Slider("Smooth", spline.terrainSmoothMultiplier, 0, 20);
        spline.distSmooth = EditorGUILayout.FloatField("Smooth distance", spline.distSmooth);

        spline.noiseCarve = EditorGUILayout.Toggle("Add noise", spline.noiseCarve);

        EditorGUILayout.Space();
        if (spline.noiseCarve)
        {
            EditorGUI.indentLevel++;
            spline.noiseMultiplierInside = EditorGUILayout.FloatField("Noise Multiplier Inside", spline.noiseMultiplierInside);
            spline.noiseMultiplierOutside = EditorGUILayout.FloatField("Noise Multiplier Outside", spline.noiseMultiplierOutside);
            spline.noiseSizeX = EditorGUILayout.FloatField("Noise scale X", spline.noiseSizeX);
            spline.noiseSizeZ = EditorGUILayout.FloatField("Noise scale Z", spline.noiseSizeZ);
            EditorGUI.indentLevel--;
        }
        //spline.distSmoothStart = EditorGUILayout.FloatField("Smooth start distance", spline.distSmoothStart);
        EditorGUI.indentLevel--;

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "River terrain changed");
            if (terrainShapeShow)
                spline.ShowTerrainCarve();
        }

        if (!terrainShapeShow && GUILayout.Button("Show terrain shape"))
        {
            spline.ShowTerrainCarve();
            terrainShapeShow = true;
        }

        if (terrainShapeShow && GUILayout.Button("Hide terrain shape"))
        {
            if (spline.meshGO != null)
                DestroyImmediate(spline.meshGO);

            terrainShapeShow = false;
        }


        EditorGUI.BeginChangeCheck();
        EditorGUI.indentLevel++;
        spline.overrideRiverRender = EditorGUILayout.Toggle("Debug Override river render", spline.overrideRiverRender);
        EditorGUI.indentLevel--;

        if (GUILayout.Button("Carve terrain"))
        {
            spline.ShowTerrainCarve();
            spline.TerrainCarve();
            terrainShapeShow = false;
        }

        EditorGUILayout.Space();
        GUILayout.Label("Terrain paint:", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;


        spline.terrainPaintCarve = EditorGUILayout.CurveField("Terrain paint", spline.terrainPaintCarve);


        Terrain terrain = Terrain.activeTerrain;

        int splatNumber = terrain.terrainData.terrainLayers.Length;
        string[] options = new string[splatNumber];
        for (int i = 0; i < splatNumber; i++)
        {
            options[i] = i + " - ";
            if (terrain.terrainData.terrainLayers[i] != null && terrain.terrainData.terrainLayers[i].diffuseTexture != null)
            {
                options[i] += terrain.terrainData.terrainLayers[i].diffuseTexture.name;
            }
        }


        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("    Splat id:");



        spline.currentSplatMap = EditorGUILayout.Popup(spline.currentSplatMap, options);
        EditorGUILayout.EndHorizontal();

        spline.noisePaint = EditorGUILayout.Toggle("Add noise", spline.noisePaint);
        if (spline.noisePaint)
        {
            EditorGUI.indentLevel++;
            spline.noiseMultiplierInsidePaint = EditorGUILayout.FloatField("Noise Multiplier Inside", spline.noiseMultiplierInsidePaint);
            spline.noiseMultiplierOutsidePaint = EditorGUILayout.FloatField("Noise Multiplier Outside", spline.noiseMultiplierOutsidePaint);
            spline.noiseSizeXPaint = EditorGUILayout.FloatField("Noise scale X", spline.noiseSizeXPaint);
            spline.noiseSizeZPaint = EditorGUILayout.FloatField("Noise scale Z", spline.noiseSizeZPaint);
            EditorGUI.indentLevel--;
        }
        spline.mixTwoSplatMaps = EditorGUILayout.Toggle("Mix two splat maps", spline.mixTwoSplatMaps);
        if (spline.mixTwoSplatMaps)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("    Second splat id:");
            spline.secondSplatMap = EditorGUILayout.Popup(spline.secondSplatMap, options);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        spline.addCliffSplatMap = EditorGUILayout.Toggle("Add cliff splatmap", spline.addCliffSplatMap);
        if (spline.addCliffSplatMap)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("    Cliff splat id:");
            spline.cliffSplatMap = EditorGUILayout.Popup(spline.cliffSplatMap, options);
            EditorGUILayout.EndHorizontal();
            spline.cliffAngle = EditorGUILayout.FloatField("Cliff angle", spline.cliffAngle);
            spline.cliffBlend = EditorGUILayout.FloatField("Cliff blend", spline.cliffBlend);

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("    Cliff outside splat id:");
            spline.cliffSplatMapOutside = EditorGUILayout.Popup(spline.cliffSplatMapOutside, options);
            EditorGUILayout.EndHorizontal();
            spline.cliffAngleOutside = EditorGUILayout.FloatField("Cliff outside angle", spline.cliffAngleOutside);
            spline.cliffBlendOutside = EditorGUILayout.FloatField("Cliff outside blend", spline.cliffBlendOutside);

            EditorGUI.indentLevel--;
        }
        EditorGUILayout.Space();

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "River terrain changed");

            if (spline.meshGO != null)
            {
                if (spline.overrideRiverRender)
                    spline.meshGO.GetComponent<MeshRenderer>().sharedMaterial.renderQueue = 5000;
                else
                    spline.meshGO.GetComponent<MeshRenderer>().sharedMaterial.renderQueue = 2980;
            }
        }



        EditorGUI.indentLevel--;

        //Debug.Log(splatNumber);
        //Debug.Log(spline.currentSplatMap);
        if (splatNumber > 0 && splatNumber > spline.currentSplatMap)
        {
            if (GUILayout.Button("Paint Terrain"))
            {

                spline.ShowTerrainCarve();
                spline.TerrainPaintMeshBased();
                terrainShapeShow = false;
            }
        }
        else
            EditorGUILayout.HelpBox("No splat id selected to paint", MessageType.Error);



        EditorGUILayout.Space();
        GUILayout.Label("Terrain clear foliage:", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        spline.distanceClearFoliage = EditorGUILayout.FloatField("Remove Details Distance", spline.distanceClearFoliage);

        if (GUILayout.Button("Remove Details Foliage"))
        {
            spline.ShowTerrainCarve(spline.distanceClearFoliage);
            spline.TerrainClearFoliage();
            terrainShapeShow = false;
        }

        spline.distanceClearFoliageTrees = EditorGUILayout.FloatField("Remove Trees Distance", spline.distanceClearFoliageTrees);

        if (GUILayout.Button("Remove Trees"))
        {
            spline.ShowTerrainCarve(spline.distanceClearFoliageTrees);
            spline.TerrainClearFoliage(false);
            terrainShapeShow = false;
        }
        EditorGUI.indentLevel--;
    }



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


    void DebugOptions()
    {
        EditorGUILayout.LabelField("splitParameter", spline.uvBeginning.ToString());
        EditorGUILayout.LabelField("beginningMinWidth", spline.beginningMinWidth.ToString());
        EditorGUILayout.LabelField("beginningMaxWidth", spline.beginningMaxWidth.ToString());
        EditorGUILayout.LabelField("minMaxWidth", spline.minMaxWidth.ToString());
        EditorGUILayout.LabelField("uvBeginning", spline.uvBeginning.ToString());
        EditorGUILayout.LabelField("uvWidth", spline.uvWidth.ToString());
        if (GUILayout.Button(new GUIContent("Regenerate spline", "Racalculates whole mesh")))
        {
            spline.GenerateSpline();
        }
    }


    void AddPointAtEnd()
    {
        if (GUILayout.Button("Add point at end"))
        {

            int i = spline.controlPoints.Count - 1;
            Vector4 position = Vector3.zero;
            position.w = spline.width;
            if (i < spline.controlPoints.Count - 1 && spline.controlPoints.Count > i + 1)
            {
                position = spline.controlPoints[i];
                Vector4 positionSecond = spline.controlPoints[i + 1];
                if (Vector3.Distance((Vector3)positionSecond, (Vector3)position) > 0)
                    position = (position + positionSecond) * 0.5f;
                else
                    position.x += 1;
            }
            else if (spline.controlPoints.Count > 1 && i == spline.controlPoints.Count - 1)
            {
                position = spline.controlPoints[i];
                Vector4 positionSecond = spline.controlPoints[i - 1];
                if (Vector3.Distance((Vector3)positionSecond, (Vector3)position) > 0)
                    position = position + (position - positionSecond);
                else
                    position.x += 1;
            }
            else if (spline.controlPoints.Count > 0)
            {
                position = spline.controlPoints[i];
                position.x += 1;
            }
            spline.controlPointsRotations.Add(Quaternion.identity);
            spline.controlPoints.Add(position);
            spline.controlPointsSnap.Add(0);
            spline.controlPointsMeshCurves.Add(new AnimationCurve(new Keyframe[] {
                new Keyframe (0, 0),
                new Keyframe (1, 0)
            }));
            spline.GenerateSpline();
#if VEGETATION_STUDIO_PRO
            RegeneratBiomeMask();
#endif
        }
    }

    void SplitRiver(int pointID)
    {
        if (pointID < 0 || pointID >= spline.controlPoints.Count)
            return;

        Undo.RecordObject(spline, "Split river");


        List<Vector4> ramFirstPoints = new List<Vector4>();
        List<Vector4> ramSecondPoints = new List<Vector4>();


        for (int i = 0; i < spline.controlPoints.Count; i++)
        {
            if (i <= pointID)
            {
                ramFirstPoints.Add(spline.controlPoints[i]);
            }
            if (i >= pointID)
            {
                ramSecondPoints.Add(spline.controlPoints[i]);
            }
        }
        MeshRenderer ren = spline.GetComponent<MeshRenderer>();


        RamSpline ramFirst = RamSpline.CreateSpline(ren.sharedMaterial, ramFirstPoints, spline.name + "_1");
        RamSpline ramSecond = RamSpline.CreateSpline(ren.sharedMaterial, ramSecondPoints, spline.name + "_2");

        if (spline.currentProfile != null)
        {
            ramFirst.currentProfile = spline.currentProfile;
            ramSecond.currentProfile = spline.currentProfile;
        }

        //
        ramFirst.meshCurve = new AnimationCurve(spline.meshCurve.keys);
        ramFirst.flowFlat = new AnimationCurve(spline.flowFlat.keys);
        ramFirst.flowWaterfall = new AnimationCurve(spline.flowWaterfall.keys);
        ramFirst.terrainCarve = new AnimationCurve(spline.terrainCarve.keys);
        ramFirst.terrainPaintCarve = new AnimationCurve(spline.terrainPaintCarve.keys);

        for (int i = 0; i < ramFirst.controlPointsMeshCurves.Count; i++)
        {
            ramFirst.controlPointsMeshCurves[i] = new AnimationCurve(spline.meshCurve.keys);
        }


        ramFirst.minVal = spline.minVal;
        ramFirst.maxVal = spline.maxVal;
        ramFirst.traingleDensity = spline.traingleDensity;
        ramFirst.vertsInShape = spline.vertsInShape;
        ramFirst.uvScale = spline.uvScale;
        ramFirst.uvRotation = spline.uvRotation;
        ramFirst.noiseflowMap = spline.noiseflowMap;
        ramFirst.noiseMultiplierflowMap = spline.noiseMultiplierflowMap;
        ramFirst.noiseSizeXflowMap = spline.noiseSizeXflowMap;
        ramFirst.noiseSizeZflowMap = spline.noiseSizeZflowMap;
        ramFirst.floatSpeed = spline.floatSpeed;
        ramFirst.distSmooth = spline.distSmooth;
        ramFirst.distSmoothStart = spline.distSmoothStart;
        ramFirst.noiseCarve = spline.noiseCarve;
        ramFirst.noiseMultiplierInside = spline.noiseMultiplierInside;
        ramFirst.noiseMultiplierOutside = spline.noiseMultiplierOutside;
        ramFirst.noiseSizeX = spline.noiseSizeX;
        ramFirst.noiseSizeZ = spline.noiseSizeZ;
        ramFirst.terrainSmoothMultiplier = spline.terrainSmoothMultiplier;
        ramFirst.currentSplatMap = spline.currentSplatMap;
        ramFirst.mixTwoSplatMaps = spline.mixTwoSplatMaps;
        ramFirst.secondSplatMap = spline.secondSplatMap;
        ramFirst.addCliffSplatMap = spline.addCliffSplatMap;
        ramFirst.cliffSplatMap = spline.cliffSplatMap;
        ramFirst.cliffAngle = spline.cliffAngle;
        ramFirst.cliffBlend = spline.cliffBlend;
        ramFirst.cliffSplatMapOutside = spline.cliffSplatMapOutside;
        ramFirst.cliffAngleOutside = spline.cliffAngleOutside;
        ramFirst.cliffBlendOutside = spline.cliffBlendOutside;
        ramFirst.distanceClearFoliage = spline.distanceClearFoliage;
        ramFirst.distanceClearFoliageTrees = spline.distanceClearFoliageTrees;
        ramFirst.noisePaint = spline.noisePaint;
        ramFirst.noiseMultiplierInsidePaint = spline.noiseMultiplierInsidePaint;
        ramFirst.noiseMultiplierOutsidePaint = spline.noiseMultiplierOutsidePaint;
        ramFirst.noiseSizeXPaint = spline.noiseSizeXPaint;
        ramFirst.noiseSizeZPaint = spline.noiseSizeZPaint;
        ramFirst.simulatedRiverLength = spline.simulatedRiverLength;
        ramFirst.simulatedRiverPoints = spline.simulatedRiverPoints;
        ramFirst.simulatedMinStepSize = spline.simulatedMinStepSize;
        ramFirst.simulatedNoUp = spline.simulatedNoUp;
        ramFirst.simulatedBreakOnUp = spline.simulatedBreakOnUp;
        ramFirst.noiseWidth = spline.noiseWidth;
        ramFirst.noiseMultiplierWidth = spline.noiseMultiplierWidth;
        ramFirst.noiseSizeWidth = spline.noiseSizeWidth;
        ramFirst.receiveShadows = spline.receiveShadows;
        ramFirst.shadowCastingMode = spline.shadowCastingMode;

        ramSecond.meshCurve = new AnimationCurve(spline.meshCurve.keys);
        ramSecond.flowFlat = new AnimationCurve(spline.flowFlat.keys);
        ramSecond.flowWaterfall = new AnimationCurve(spline.flowWaterfall.keys);
        ramSecond.terrainCarve = new AnimationCurve(spline.terrainCarve.keys);
        ramSecond.terrainPaintCarve = new AnimationCurve(spline.terrainPaintCarve.keys);

        for (int i = 0; i < ramSecond.controlPointsMeshCurves.Count; i++)
        {
            ramSecond.controlPointsMeshCurves[i] = new AnimationCurve(spline.meshCurve.keys);
        }


        ramSecond.minVal = spline.minVal;
        ramSecond.maxVal = spline.maxVal;
        ramSecond.traingleDensity = spline.traingleDensity;
        ramSecond.vertsInShape = spline.vertsInShape;
        ramSecond.uvScale = spline.uvScale;
        ramSecond.uvRotation = spline.uvRotation;
        ramSecond.noiseflowMap = spline.noiseflowMap;
        ramSecond.noiseMultiplierflowMap = spline.noiseMultiplierflowMap;
        ramSecond.noiseSizeXflowMap = spline.noiseSizeXflowMap;
        ramSecond.noiseSizeZflowMap = spline.noiseSizeZflowMap;
        ramSecond.floatSpeed = spline.floatSpeed;
        ramSecond.distSmooth = spline.distSmooth;
        ramSecond.distSmoothStart = spline.distSmoothStart;
        ramSecond.noiseCarve = spline.noiseCarve;
        ramSecond.noiseMultiplierInside = spline.noiseMultiplierInside;
        ramSecond.noiseMultiplierOutside = spline.noiseMultiplierOutside;
        ramSecond.noiseSizeX = spline.noiseSizeX;
        ramSecond.noiseSizeZ = spline.noiseSizeZ;
        ramSecond.terrainSmoothMultiplier = spline.terrainSmoothMultiplier;
        ramSecond.currentSplatMap = spline.currentSplatMap;
        ramSecond.mixTwoSplatMaps = spline.mixTwoSplatMaps;
        ramSecond.secondSplatMap = spline.secondSplatMap;
        ramSecond.addCliffSplatMap = spline.addCliffSplatMap;
        ramSecond.cliffSplatMap = spline.cliffSplatMap;
        ramSecond.cliffAngle = spline.cliffAngle;
        ramSecond.cliffBlend = spline.cliffBlend;
        ramSecond.cliffSplatMapOutside = spline.cliffSplatMapOutside;
        ramSecond.cliffAngleOutside = spline.cliffAngleOutside;
        ramSecond.cliffBlendOutside = spline.cliffBlendOutside;
        ramSecond.distanceClearFoliage = spline.distanceClearFoliage;
        ramSecond.distanceClearFoliageTrees = spline.distanceClearFoliageTrees;
        ramSecond.noisePaint = spline.noisePaint;
        ramSecond.noiseMultiplierInsidePaint = spline.noiseMultiplierInsidePaint;
        ramSecond.noiseMultiplierOutsidePaint = spline.noiseMultiplierOutsidePaint;
        ramSecond.noiseSizeXPaint = spline.noiseSizeXPaint;
        ramSecond.noiseSizeZPaint = spline.noiseSizeZPaint;
        ramSecond.simulatedRiverLength = spline.simulatedRiverLength;
        ramSecond.simulatedRiverPoints = spline.simulatedRiverPoints;
        ramSecond.simulatedMinStepSize = spline.simulatedMinStepSize;
        ramSecond.simulatedNoUp = spline.simulatedNoUp;
        ramSecond.simulatedBreakOnUp = spline.simulatedBreakOnUp;
        ramSecond.noiseWidth = spline.noiseWidth;
        ramSecond.noiseMultiplierWidth = spline.noiseMultiplierWidth;
        ramSecond.noiseSizeWidth = spline.noiseSizeWidth;
        ramSecond.receiveShadows = spline.receiveShadows;
        ramSecond.shadowCastingMode = spline.shadowCastingMode;




        //
        ramFirst.endingSpline = ramSecond;
        ramFirst.endingMinWidth = 0;
        ramFirst.endingMaxWidth = 1;
        ramSecond.GenerateSpline();
        ramFirst.GenerateSpline();

        ramFirst.transform.position = spline.transform.position;
        ramSecond.transform.position = spline.transform.position;

        Undo.DestroyObjectImmediate(spline.gameObject);



    }

    void MeshSettings()
    {
        GUILayout.Label("Mesh settings:", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        spline.currentProfile = (SplineProfile)EditorGUILayout.ObjectField("Spline profile", spline.currentProfile, typeof(SplineProfile), false);

        if (GUILayout.Button("Create profile from settings"))
        {


            SplineProfile asset = ScriptableObject.CreateInstance<SplineProfile>();


            //asset.meshCurve = spline.meshCurve;

            asset.meshCurve = new AnimationCurve(spline.meshCurve.keys);
            asset.flowFlat = new AnimationCurve(spline.flowFlat.keys);
            asset.flowWaterfall = new AnimationCurve(spline.flowWaterfall.keys);
            asset.terrainCarve = new AnimationCurve(spline.terrainCarve.keys);
            asset.terrainPaintCarve = new AnimationCurve(spline.terrainPaintCarve.keys);

            MeshRenderer ren = spline.GetComponent<MeshRenderer>();
            asset.splineMaterial = ren.sharedMaterial;

            asset.minVal = spline.minVal;
            asset.maxVal = spline.maxVal;


            asset.traingleDensity = spline.traingleDensity;
            asset.vertsInShape = spline.vertsInShape;

            asset.uvScale = spline.uvScale;

            asset.uvRotation = spline.uvRotation;

            //asset.flowFlat = spline.flowFlat;
            //asset.flowWaterfall = spline.flowWaterfall;

            asset.noiseflowMap = spline.noiseflowMap;
            asset.noiseMultiplierflowMap = spline.noiseMultiplierflowMap;
            asset.noiseSizeXflowMap = spline.noiseSizeXflowMap;
            asset.noiseSizeZflowMap = spline.noiseSizeZflowMap;

            asset.floatSpeed = spline.floatSpeed;

            // asset.terrainCarve = spline.terrainCarve;
            asset.distSmooth = spline.distSmooth;
            asset.distSmoothStart = spline.distSmoothStart;
            // asset.terrainPaintCarve = spline.terrainPaintCarve;

            asset.noiseCarve = spline.noiseCarve;
            asset.noiseMultiplierInside = spline.noiseMultiplierInside;
            asset.noiseMultiplierOutside = spline.noiseMultiplierOutside;
            asset.noiseSizeX = spline.noiseSizeX;
            asset.noiseSizeZ = spline.noiseSizeZ;
            asset.terrainSmoothMultiplier = spline.terrainSmoothMultiplier;
            asset.currentSplatMap = spline.currentSplatMap;
            asset.mixTwoSplatMaps = spline.mixTwoSplatMaps;
            asset.secondSplatMap = spline.secondSplatMap;
            asset.addCliffSplatMap = spline.addCliffSplatMap;
            asset.cliffSplatMap = spline.cliffSplatMap;
            asset.cliffAngle = spline.cliffAngle;
            asset.cliffBlend = spline.cliffBlend;
            asset.cliffSplatMapOutside = spline.cliffSplatMapOutside;
            asset.cliffAngleOutside = spline.cliffAngleOutside;
            asset.cliffBlendOutside = spline.cliffBlendOutside;


            asset.distanceClearFoliage = spline.distanceClearFoliage;
            asset.distanceClearFoliageTrees = spline.distanceClearFoliageTrees;
            asset.noisePaint = spline.noisePaint;
            asset.noiseMultiplierInsidePaint = spline.noiseMultiplierInsidePaint;
            asset.noiseMultiplierOutsidePaint = spline.noiseMultiplierOutsidePaint;
            asset.noiseSizeXPaint = spline.noiseSizeXPaint;
            asset.noiseSizeZPaint = spline.noiseSizeZPaint;



            asset.simulatedRiverLength = spline.simulatedRiverLength;
            asset.simulatedRiverPoints = spline.simulatedRiverPoints;
            asset.simulatedMinStepSize = spline.simulatedMinStepSize;
            asset.simulatedNoUp = spline.simulatedNoUp;
            asset.simulatedBreakOnUp = spline.simulatedBreakOnUp;
            asset.noiseWidth = spline.noiseWidth;
            asset.noiseMultiplierWidth = spline.noiseMultiplierWidth;
            asset.noiseSizeWidth = spline.noiseSizeWidth;


            asset.receiveShadows = spline.receiveShadows;
            asset.shadowCastingMode = spline.shadowCastingMode;


            string path = EditorUtility.SaveFilePanelInProject("Save new spline profile", spline.gameObject.name + ".asset", "asset", "Please enter a file name to save the spline profile to");

            if (!string.IsNullOrEmpty(path))
            {

                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
                spline.currentProfile = asset;
            }
        }
        if (spline.currentProfile != null && GUILayout.Button("Save profile from settings"))
        {

            //spline.currentProfile.meshCurve = spline.meshCurve;

            MeshRenderer ren = spline.GetComponent<MeshRenderer>();
            spline.currentProfile.splineMaterial = ren.sharedMaterial;

            spline.currentProfile.minVal = spline.minVal;
            spline.currentProfile.maxVal = spline.maxVal;

            spline.currentProfile.meshCurve = new AnimationCurve(spline.meshCurve.keys);
            spline.currentProfile.flowFlat = new AnimationCurve(spline.flowFlat.keys);
            spline.currentProfile.flowWaterfall = new AnimationCurve(spline.flowWaterfall.keys);
            spline.currentProfile.terrainCarve = new AnimationCurve(spline.terrainCarve.keys);
            spline.currentProfile.terrainPaintCarve = new AnimationCurve(spline.terrainPaintCarve.keys);


            spline.currentProfile.traingleDensity = spline.traingleDensity;
            spline.currentProfile.vertsInShape = spline.vertsInShape;

            spline.currentProfile.uvScale = spline.uvScale;

            spline.currentProfile.uvRotation = spline.uvRotation;

            // spline.currentProfile.flowFlat = spline.flowFlat;
            //spline.currentProfile.flowWaterfall = spline.flowWaterfall;

            spline.currentProfile.noiseflowMap = spline.noiseflowMap;
            spline.currentProfile.noiseMultiplierflowMap = spline.noiseMultiplierflowMap;
            spline.currentProfile.noiseSizeXflowMap = spline.noiseSizeXflowMap;
            spline.currentProfile.noiseSizeZflowMap = spline.noiseSizeZflowMap;

            spline.currentProfile.floatSpeed = spline.floatSpeed;

            //spline.currentProfile.terrainCarve = spline.terrainCarve;
            spline.currentProfile.distSmooth = spline.distSmooth;
            spline.currentProfile.distSmoothStart = spline.distSmoothStart;
            //spline.currentProfile.terrainPaintCarve = spline.terrainPaintCarve;

            spline.currentProfile.noiseCarve = spline.noiseCarve;
            spline.currentProfile.noiseMultiplierInside = spline.noiseMultiplierInside;
            spline.currentProfile.noiseMultiplierOutside = spline.noiseMultiplierOutside;
            spline.currentProfile.noiseSizeX = spline.noiseSizeX;
            spline.currentProfile.noiseSizeZ = spline.noiseSizeZ;
            spline.currentProfile.terrainSmoothMultiplier = spline.terrainSmoothMultiplier;
            spline.currentProfile.currentSplatMap = spline.currentSplatMap;
            spline.currentProfile.mixTwoSplatMaps = spline.mixTwoSplatMaps;
            spline.currentProfile.secondSplatMap = spline.secondSplatMap;
            spline.currentProfile.addCliffSplatMap = spline.addCliffSplatMap;
            spline.currentProfile.cliffSplatMap = spline.cliffSplatMap;
            spline.currentProfile.cliffAngle = spline.cliffAngle;
            spline.currentProfile.cliffBlend = spline.cliffBlend;

            spline.currentProfile.cliffSplatMapOutside = spline.cliffSplatMapOutside;
            spline.currentProfile.cliffAngleOutside = spline.cliffAngleOutside;
            spline.currentProfile.cliffBlendOutside = spline.cliffBlendOutside;

            spline.currentProfile.distanceClearFoliage = spline.distanceClearFoliage;
            spline.currentProfile.distanceClearFoliageTrees = spline.distanceClearFoliageTrees;
            spline.currentProfile.noisePaint = spline.noisePaint;
            spline.currentProfile.noiseMultiplierInsidePaint = spline.noiseMultiplierInsidePaint;
            spline.currentProfile.noiseMultiplierOutsidePaint = spline.noiseMultiplierOutsidePaint;
            spline.currentProfile.noiseSizeXPaint = spline.noiseSizeXPaint;
            spline.currentProfile.noiseSizeZPaint = spline.noiseSizeZPaint;

            spline.currentProfile.simulatedRiverLength = spline.simulatedRiverLength;
            spline.currentProfile.simulatedRiverPoints = spline.simulatedRiverPoints;
            spline.currentProfile.simulatedMinStepSize = spline.simulatedMinStepSize;
            spline.currentProfile.simulatedNoUp = spline.simulatedNoUp;
            spline.currentProfile.simulatedBreakOnUp = spline.simulatedBreakOnUp;
            spline.currentProfile.noiseWidth = spline.noiseWidth;
            spline.currentProfile.noiseMultiplierWidth = spline.noiseMultiplierWidth;
            spline.currentProfile.noiseSizeWidth = spline.noiseSizeWidth;


            spline.currentProfile.receiveShadows = spline.receiveShadows;
            spline.currentProfile.shadowCastingMode = spline.shadowCastingMode;


            AssetDatabase.SaveAssets();
        }


        if (spline.currentProfile != null && spline.currentProfile != spline.oldProfile)
        {

            ResetToProfile();
            EditorUtility.SetDirty(spline);

        }

        bool profileChanged = CheckProfileChange();



        if (spline.currentProfile != null && GUILayout.Button("Reset to profile" + (profileChanged ? " (Profile data changed)" : "")))
        {
            ResetToProfile();
        }


        EditorGUILayout.Space();


        string meshResolution = "Triangles density";
        if (spline.meshfilter != null && spline.meshfilter.sharedMesh != null)
        {
            float tris = spline.meshfilter.sharedMesh.triangles.Length / 3;
            meshResolution += " (" + tris + " tris)";
        }
        else if (spline.meshfilter != null && spline.meshfilter.sharedMesh == null)
        {
            spline.GenerateSpline();
        }


        EditorGUILayout.LabelField(meshResolution);
        EditorGUI.indentLevel++;
        spline.traingleDensity = 1 / (float)EditorGUILayout.IntSlider("U", (int)(1 / (float)spline.traingleDensity), 1, 100);

        if (spline.beginningSpline == null && spline.endingSpline == null)
        {

            spline.vertsInShape = EditorGUILayout.IntSlider("V", spline.vertsInShape - 1, 1, 20) + 1;

        }
        else
        {
            GUI.enabled = false;
            if (spline.beginningSpline != null)
            {
                spline.vertsInShape = (int)Mathf.Round((spline.beginningSpline.vertsInShape - 1) * (spline.beginningMaxWidth - spline.beginningMinWidth) + 1);
            }
            else if (spline.endingSpline != null)
                spline.vertsInShape = (int)Mathf.Round((spline.endingSpline.vertsInShape - 1) * (spline.endingMaxWidth - spline.endingMinWidth) + 1);

            EditorGUILayout.IntSlider("V", spline.vertsInShape - 1, 1, 20);
            GUI.enabled = true;

        }

        EditorGUI.indentLevel--;
        EditorGUILayout.Space();


        EditorGUILayout.BeginHorizontal();
        {
            spline.width = EditorGUILayout.FloatField("River width", spline.width);
            if (GUILayout.Button("Change width for whole river"))
            {
                if (spline.width > 0)
                {
                    for (int i = 0; i < spline.controlPoints.Count; i++)
                    {
                        Vector4 point = spline.controlPoints[i];
                        point.w = spline.width;
                        spline.controlPoints[i] = point;
                    }
                    spline.GenerateSpline();
#if VEGETATION_STUDIO_PRO
                    RegeneratBiomeMask();
#endif
                }
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel++;
        spline.noiseWidth = EditorGUILayout.Toggle("Add width noise", spline.noiseWidth);
        if (spline.noiseWidth)
        {
            EditorGUI.indentLevel++;
            spline.noiseMultiplierWidth = EditorGUILayout.FloatField("Noise Multiplier Width", spline.noiseMultiplierWidth);
            spline.noiseSizeWidth = EditorGUILayout.FloatField("Noise scale Width", spline.noiseSizeWidth);
            EditorGUI.indentLevel--;
            if (GUILayout.Button("Add noise to width for whole river"))
            {
                Undo.RecordObject(spline, "Change widths");
                spline.AddNoiseToWidths();
                spline.GenerateSpline();
#if VEGETATION_STUDIO_PRO
                RegeneratBiomeMask();
#endif
            }
        }
        EditorGUI.indentLevel--;
        EditorGUILayout.Space();


        spline.meshCurve = EditorGUILayout.CurveField("Mesh curve", spline.meshCurve);
        if (GUILayout.Button("Set all mesh curves"))
        {
            for (int i = 0; i < spline.controlPointsMeshCurves.Count; i++)
            {
                spline.controlPointsMeshCurves[i] = new AnimationCurve(spline.meshCurve.keys);
            }
        }
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Vertice distribution: " + spline.minVal.ToString() + " " + spline.maxVal.ToString());
        EditorGUILayout.MinMaxSlider(ref spline.minVal, ref spline.maxVal, 0, 1);

        //Debug.Log (spline.minVal + " " + spline.maxVal);

        spline.minVal = (int)(spline.minVal * 100) * 0.01f;
        spline.maxVal = (int)(spline.maxVal * 100) * 0.01f;


        if (spline.minVal > 0.5f)
            spline.minVal = 0.5f;

        if (spline.minVal < 0.01f)
            spline.minVal = 0.01f;

        if (spline.maxVal < 0.5f)
            spline.maxVal = 0.5f;

        if (spline.maxVal > 0.99f)
            spline.maxVal = 0.99f;

        EditorGUILayout.Space();

        if (GUILayout.Button("Snap/Unsnap mesh to terrain"))
        {
            spline.snapToTerrain = !spline.snapToTerrain;
            for (int i = 0; i < spline.controlPointsSnap.Count; i++)
            {
                spline.controlPointsSnap[i] = spline.snapToTerrain == true ? 1 : 0;
            }
        }
        ///spline.snapMask = EditorGUILayout.MaskField ("Layers", spline.snapMask, InternalEditorUtility.layers);
        spline.snapMask = LayerMaskField("Layers", spline.snapMask, true);

        spline.normalFromRaycast = EditorGUILayout.Toggle("Take Normal from terrain", spline.normalFromRaycast);
        EditorGUILayout.Space();
    }

    bool CheckProfileChange()
    {
        if (spline.currentProfile == null)
            return false;
        //if (ren.material != spline.currentProfile.splineMaterial)
        //	return true;

        if (spline.minVal != spline.currentProfile.minVal)
            return true;
        if (spline.maxVal != spline.currentProfile.maxVal)
            return true;

        if (spline.traingleDensity != spline.currentProfile.traingleDensity)
            return true;
        if (spline.vertsInShape != spline.currentProfile.vertsInShape)
            return true;

        if (spline.uvScale != spline.currentProfile.uvScale)
            return true;

        if (spline.uvRotation != spline.currentProfile.uvRotation)
            return true;


        // if (spline.flowFlat != spline.currentProfile.flowFlat)
        //     return true;

        // if (spline.flowWaterfall != spline.currentProfile.flowWaterfall)
        //     return true;

        if (spline.noiseflowMap != spline.currentProfile.noiseflowMap)
            return true;

        if (spline.noiseMultiplierflowMap != spline.currentProfile.noiseMultiplierflowMap)
            return true;

        if (spline.noiseSizeXflowMap != spline.currentProfile.noiseSizeXflowMap)
            return true;

        if (spline.noiseSizeZflowMap != spline.currentProfile.noiseSizeZflowMap)
            return true;

        if (spline.floatSpeed != spline.currentProfile.floatSpeed)
            return true;

        // if (spline.terrainCarve != spline.currentProfile.terrainCarve)
        //    return true;

        if (spline.distSmooth != spline.currentProfile.distSmooth)
            return true;

        if (spline.distSmoothStart != spline.currentProfile.distSmoothStart)
            return true;

        // if (spline.terrainPaintCarve != spline.currentProfile.terrainPaintCarve)
        //     return true;

        if (spline.currentProfile.noiseCarve != spline.noiseCarve)
            return true;
        if (spline.currentProfile.noiseMultiplierInside != spline.noiseMultiplierInside)
            return true;
        if (spline.currentProfile.noiseMultiplierOutside != spline.noiseMultiplierOutside)
            return true;
        if (spline.currentProfile.noiseSizeX != spline.noiseSizeX)
            return true;
        if (spline.currentProfile.noiseSizeZ != spline.noiseSizeZ)
            return true;
        if (spline.currentProfile.terrainSmoothMultiplier != spline.terrainSmoothMultiplier)
            return true;
        if (spline.currentProfile.currentSplatMap != spline.currentSplatMap)
            return true;
        if (spline.currentProfile.mixTwoSplatMaps != spline.mixTwoSplatMaps)
            return true;
        if (spline.currentProfile.secondSplatMap != spline.secondSplatMap)
            return true;
        if (spline.currentProfile.addCliffSplatMap != spline.addCliffSplatMap)
            return true;
        if (spline.currentProfile.cliffSplatMap != spline.cliffSplatMap)
            return true;
        if (spline.currentProfile.cliffAngle != spline.cliffAngle)
            return true;
        if (spline.currentProfile.cliffBlend != spline.cliffBlend)
            return true;
        if (spline.currentProfile.cliffSplatMapOutside != spline.cliffSplatMapOutside)
            return true;
        if (spline.currentProfile.cliffAngleOutside != spline.cliffAngleOutside)
            return true;
        if (spline.currentProfile.cliffBlendOutside != spline.cliffBlendOutside)
            return true;

        if (spline.currentProfile.distanceClearFoliage != spline.distanceClearFoliage)
            return true;
        if (spline.currentProfile.distanceClearFoliageTrees != spline.distanceClearFoliageTrees)
            return true;
        if (spline.currentProfile.noisePaint != spline.noisePaint)
            return true;
        if (spline.currentProfile.noiseMultiplierInsidePaint != spline.noiseMultiplierInsidePaint)
            return true;
        if (spline.currentProfile.noiseMultiplierOutsidePaint != spline.noiseMultiplierOutsidePaint)
            return true;
        if (spline.currentProfile.noiseSizeXPaint != spline.noiseSizeXPaint)
            return true;
        if (spline.currentProfile.noiseSizeZPaint != spline.noiseSizeZPaint)
            return true;

        if (spline.currentProfile.simulatedRiverLength != spline.simulatedRiverLength)
            return true;
        if (spline.currentProfile.simulatedRiverPoints != spline.simulatedRiverPoints)
            return true;
        if (spline.currentProfile.simulatedMinStepSize != spline.simulatedMinStepSize)
            return true;
        if (spline.currentProfile.simulatedNoUp != spline.simulatedNoUp)
            return true;
        if (spline.currentProfile.simulatedBreakOnUp != spline.simulatedBreakOnUp)
            return true;
        if (spline.currentProfile.noiseWidth != spline.noiseWidth)
            return true;
        if (spline.currentProfile.noiseMultiplierWidth != spline.noiseMultiplierWidth)
            return true;
        if (spline.currentProfile.noiseSizeWidth != spline.noiseSizeWidth)
            return true;


        if (spline.receiveShadows != spline.currentProfile.receiveShadows)
            return true;

        if (spline.shadowCastingMode != spline.currentProfile.shadowCastingMode)
            return true;

        return false;
    }

    public void ResetToProfile()
    {
        if (spline == null)
            spline = (RamSpline)target;

        //spline.meshCurve = spline.currentProfile.meshCurve;
        spline.meshCurve = new AnimationCurve(spline.currentProfile.meshCurve.keys);
        spline.flowFlat = new AnimationCurve(spline.currentProfile.flowFlat.keys);
        spline.flowWaterfall = new AnimationCurve(spline.currentProfile.flowWaterfall.keys);
        spline.terrainCarve = new AnimationCurve(spline.currentProfile.terrainCarve.keys);
        spline.terrainPaintCarve = new AnimationCurve(spline.currentProfile.terrainPaintCarve.keys);

        for (int i = 0; i < spline.controlPointsMeshCurves.Count; i++)
        {
            spline.controlPointsMeshCurves[i] = new AnimationCurve(spline.meshCurve.keys);
        }
        MeshRenderer ren = spline.GetComponent<MeshRenderer>();
        ren.sharedMaterial = spline.currentProfile.splineMaterial;

        spline.minVal = spline.currentProfile.minVal;
        spline.maxVal = spline.currentProfile.maxVal;


        spline.traingleDensity = spline.currentProfile.traingleDensity;
        spline.vertsInShape = spline.currentProfile.vertsInShape;

        spline.uvScale = spline.currentProfile.uvScale;

        spline.uvRotation = spline.currentProfile.uvRotation;

        //spline.flowFlat = spline.currentProfile.flowFlat;
        //spline.flowWaterfall = spline.currentProfile.flowWaterfall;

        spline.noiseflowMap = spline.currentProfile.noiseflowMap;
        spline.noiseMultiplierflowMap = spline.currentProfile.noiseMultiplierflowMap;
        spline.noiseSizeXflowMap = spline.currentProfile.noiseSizeXflowMap;
        spline.noiseSizeZflowMap = spline.currentProfile.noiseSizeZflowMap;

        spline.floatSpeed = spline.currentProfile.floatSpeed;

        //spline.terrainCarve = spline.currentProfile.terrainCarve;



        spline.distSmooth = spline.currentProfile.distSmooth;
        spline.distSmoothStart = spline.currentProfile.distSmoothStart;
        //spline.terrainPaintCarve = spline.currentProfile.terrainPaintCarve;


        spline.noiseCarve = spline.currentProfile.noiseCarve;
        spline.noiseMultiplierInside = spline.currentProfile.noiseMultiplierInside;
        spline.noiseMultiplierOutside = spline.currentProfile.noiseMultiplierOutside;
        spline.noiseSizeX = spline.currentProfile.noiseSizeX;
        spline.noiseSizeZ = spline.currentProfile.noiseSizeZ;
        spline.terrainSmoothMultiplier = spline.currentProfile.terrainSmoothMultiplier;
        spline.currentSplatMap = spline.currentProfile.currentSplatMap;
        spline.mixTwoSplatMaps = spline.currentProfile.mixTwoSplatMaps;
        spline.secondSplatMap = spline.currentProfile.secondSplatMap;
        spline.addCliffSplatMap = spline.currentProfile.addCliffSplatMap;
        spline.cliffSplatMap = spline.currentProfile.cliffSplatMap;
        spline.cliffAngle = spline.currentProfile.cliffAngle;
        spline.cliffBlend = spline.currentProfile.cliffBlend;

        spline.cliffSplatMapOutside = spline.currentProfile.cliffSplatMapOutside;
        spline.cliffAngleOutside = spline.currentProfile.cliffAngleOutside;
        spline.cliffBlendOutside = spline.currentProfile.cliffBlendOutside;


        spline.distanceClearFoliage = spline.currentProfile.distanceClearFoliage;
        spline.distanceClearFoliageTrees = spline.currentProfile.distanceClearFoliageTrees;
        spline.noisePaint = spline.currentProfile.noisePaint;
        spline.noiseMultiplierInsidePaint = spline.currentProfile.noiseMultiplierInsidePaint;
        spline.noiseMultiplierOutsidePaint = spline.currentProfile.noiseMultiplierOutsidePaint;
        spline.noiseSizeXPaint = spline.currentProfile.noiseSizeXPaint;
        spline.noiseSizeZPaint = spline.currentProfile.noiseSizeZPaint;


        spline.simulatedRiverLength = spline.currentProfile.simulatedRiverLength;
        spline.simulatedRiverPoints = spline.currentProfile.simulatedRiverPoints;
        spline.simulatedMinStepSize = spline.currentProfile.simulatedMinStepSize;
        spline.simulatedNoUp = spline.currentProfile.simulatedNoUp;
        spline.simulatedBreakOnUp = spline.currentProfile.simulatedBreakOnUp;
        spline.noiseWidth = spline.currentProfile.noiseWidth;
        spline.noiseMultiplierWidth = spline.currentProfile.noiseMultiplierWidth;
        spline.noiseSizeWidth = spline.currentProfile.noiseSizeWidth;

        spline.receiveShadows = spline.currentProfile.receiveShadows;
        spline.shadowCastingMode = spline.currentProfile.shadowCastingMode;

        spline.GenerateSpline();
#if VEGETATION_STUDIO_PRO
        RegeneratBiomeMask();
#endif
        spline.oldProfile = spline.currentProfile;


    }

    void DrawVertexColorsUI()
    {
        spline.drawOnMesh = true;
        if (spline.drawOnMesh)
        {


            EditorGUILayout.HelpBox("R - Slow Water G - Small Cascade B - Big Cascade A - Opacity", MessageType.Info);
            EditorGUILayout.Space();
            spline.drawColor = EditorGUILayout.ColorField("Draw color", spline.drawColor);

            spline.opacity = EditorGUILayout.FloatField("Opacity", spline.opacity);
            spline.drawSize = EditorGUILayout.FloatField("Size", spline.drawSize);
            if (spline.drawSize < 0)
            {
                spline.drawSize = 0;
            }

            spline.drawColorR = EditorGUILayout.Toggle("Draw R", spline.drawColorR);
            spline.drawColorG = EditorGUILayout.Toggle("Draw G", spline.drawColorG);
            spline.drawColorB = EditorGUILayout.Toggle("Draw B", spline.drawColorB);
            spline.drawColorA = EditorGUILayout.Toggle("Draw A", spline.drawColorA);

            EditorGUILayout.Space();
            spline.drawOnMultiple = EditorGUILayout.Toggle("Draw on multiple rivers", spline.drawOnMultiple);
        }

        EditorGUILayout.Space();
        if (!spline.showVertexColors)
        {
            if (GUILayout.Button("Show vertex colors"))
            {

                if (!spline.showFlowMap && !spline.showVertexColors)
                    spline.oldMaterial = spline.GetComponent<MeshRenderer>().sharedMaterial;
                ResetMaterial();
                spline.GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("NatureManufacture Shaders/Debug/Vertex color"));
                spline.showVertexColors = true;
            }
        }
        else
        {
            if (GUILayout.Button("Hide vertex colors"))
            {
                ResetMaterial();
                spline.GetComponent<MeshRenderer>().sharedMaterial = spline.oldMaterial;
                spline.showVertexColors = false;
            }
        }

        if (GUILayout.Button("Reset vertex colors") && EditorUtility.DisplayDialog("Reset vertex colors?",
                "Are you sure you want to reset f vertex colors?", "Yes", "No"))
        {
            spline.colors = null;
            spline.GenerateSpline();
#if VEGETATION_STUDIO_PRO
            RegeneratBiomeMask();
#endif
        }
    }

    void DrawFlowColorsUI()
    {

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Sharp gradient could generate bugged effect. Keep flow changes smooth.", MessageType.Info);
        GUILayout.Label("Flow Map Manual: ", EditorStyles.boldLabel);
        spline.drawOnMeshFlowMap = true;
        if (spline.drawOnMeshFlowMap)
        {

            EditorGUILayout.Space();
            spline.flowSpeed = EditorGUILayout.Slider("Flow U Speed", spline.flowSpeed, -1, 1);
            spline.flowDirection = EditorGUILayout.Slider("Flow V Speed", spline.flowDirection, -1, 1);
            spline.opacity = EditorGUILayout.FloatField("Opacity", spline.opacity);
            spline.drawSize = EditorGUILayout.FloatField("Size", spline.drawSize);
            if (spline.drawSize < 0)
            {
                spline.drawSize = 0;
            }


            EditorGUILayout.Space();
            spline.drawOnMultiple = EditorGUILayout.Toggle("Draw on multiple rivers", spline.drawOnMultiple);
        }

        EditorGUILayout.Space();
        if (!spline.showFlowMap)
        {



            if (GUILayout.Button("Show flow directions"))
            {
                if (!spline.showFlowMap && !spline.showVertexColors)
                    spline.oldMaterial = spline.GetComponent<MeshRenderer>().sharedMaterial;
                ResetMaterial();
                spline.GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("NatureManufacture Shaders/Debug/Flowmap Direction"));
                spline.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_Direction", Resources.Load<Texture2D>("Debug_Arrow"));



                spline.showFlowMap = true;
            }
            if (GUILayout.Button("Show flow smoothness"))
            {
                if (!spline.showFlowMap && !spline.showVertexColors)
                    spline.oldMaterial = spline.GetComponent<MeshRenderer>().sharedMaterial;
                ResetMaterial();
                spline.GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("NatureManufacture Shaders/Debug/FlowMapUV4"));
                spline.showFlowMap = true;
            }
        }

        if (spline.showFlowMap)
        {

            if (GUILayout.Button("Hide flow"))
            {
                ResetMaterial();
                spline.GetComponent<MeshRenderer>().sharedMaterial = spline.oldMaterial;
            }
        }

        EditorGUILayout.Space();
        GUILayout.Label("Flow Map Automatic: ", EditorStyles.boldLabel);
        spline.flowFlat = EditorGUILayout.CurveField("Flow curve flat speed", spline.flowFlat);
        spline.flowWaterfall = EditorGUILayout.CurveField("Flow curve waterfall speed", spline.flowWaterfall);

        spline.noiseflowMap = EditorGUILayout.Toggle("Add noise", spline.noiseflowMap);
        if (spline.noiseflowMap)
        {
            EditorGUI.indentLevel++;
            spline.noiseMultiplierflowMap = EditorGUILayout.FloatField("Noise multiplier inside", spline.noiseMultiplierflowMap);
            spline.noiseSizeXflowMap = EditorGUILayout.FloatField("Noise scale X", spline.noiseSizeXflowMap);
            spline.noiseSizeZflowMap = EditorGUILayout.FloatField("Noise scale Z", spline.noiseSizeZflowMap);
            EditorGUI.indentLevel--;
        }



        EditorGUILayout.Space();
        if (GUILayout.Button("Reset flow to automatic") && EditorUtility.DisplayDialog("Reset flow to automatic?",
                "Are you sure you want to reset flow to automatic?", "Yes", "No"))
        {
            spline.overrideFlowMap = false;
            spline.GenerateSpline();
#if VEGETATION_STUDIO_PRO
            RegeneratBiomeMask();
#endif
        }
        EditorGUILayout.Space();
        GUILayout.Label("Flow Map Physic: ", EditorStyles.boldLabel);
        spline.floatSpeed = EditorGUILayout.FloatField("River float speed", spline.floatSpeed);
    }

    void ResetMaterial()
    {
        //if (spline.oldMaterial != null)
        //	spline.GetComponent<MeshRenderer> ().sharedMaterial = spline.oldMaterial;
        spline.showFlowMap = false;
        spline.showVertexColors = false;
    }

    void FilesManager()
    {
        if (GUILayout.Button("Save points to csv file"))
        {
            PointsToFile();
        }

        if (GUILayout.Button("Load points from csv file"))
        {
            PointsFromFile();
        }
    }

    void Tips()
    {
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("\nReflections - Use box projection in reflection probes to get proper render even at multiple river conection.\n", MessageType.Info);
        EditorGUILayout.HelpBox("\nKeep resonable quasi- square vertex shapes at river mesh, " + "this will give better tesselation result. Don't worry about low amount of poly, tesselation will smooth shapes.\n", MessageType.Info);
        EditorGUILayout.HelpBox("\nBy rotating point you could get simmilar effect as vertex color painting.\n" + "You could adjust waterfalls or add noise in the river. " + "Note that if rotation will be bigger then +/- 90 degree you could invert normals.\n", MessageType.Info);
        EditorGUILayout.HelpBox("\nUse low resolution reflection probes, and only around the water. " + "\nFar clip planes also should be short, you probably only need colors from the surounding world.\n", MessageType.Info);
        EditorGUILayout.HelpBox("\nPut reflection probes behind, in and after dark area (tunel, cave) so you will get exelent result in lighting and reflections.\n", MessageType.Info);
        EditorGUILayout.HelpBox("\nTry to keep quite simmilar distance between spline points. Huge distance between them could create strange result.\n", MessageType.Info);
        EditorGUILayout.HelpBox("\nWhen you use multiple connected rivers, you shoud put reflection probe at fork of the rivers to keep proper reflections\n", MessageType.Info);


        EditorGUILayout.Space();
    }



    void ParentingSplineUI()
    {
        GUILayout.Label("Rivers connections", EditorStyles.boldLabel);


        spline.beginningSpline = (RamSpline)EditorGUILayout.ObjectField("Beginning spline", spline.beginningSpline, typeof(RamSpline), true);

        if (spline.beginningSpline == spline)
            spline.beginningSpline = null;

        spline.endingSpline = (RamSpline)EditorGUILayout.ObjectField("Ending spline", spline.endingSpline, typeof(RamSpline), true);
        if (spline.endingSpline == spline)
            spline.endingSpline = null;


        if (spline.beginningSpline != null)
        {
            if (spline.controlPoints.Count > 0 && spline.beginningSpline.points.Count > 0)
            {
                spline.beginningMinWidth = spline.beginningMinWidth * (spline.beginningSpline.vertsInShape - 1);
                spline.beginningMaxWidth = spline.beginningMaxWidth * (spline.beginningSpline.vertsInShape - 1);
                EditorGUILayout.MinMaxSlider("Part parent", ref spline.beginningMinWidth, ref spline.beginningMaxWidth, 0, spline.beginningSpline.vertsInShape - 1);
                spline.beginningMinWidth = (int)spline.beginningMinWidth;
                spline.beginningMaxWidth = (int)spline.beginningMaxWidth;
                spline.beginningMinWidth = Mathf.Clamp(spline.beginningMinWidth, 0, spline.beginningSpline.vertsInShape - 1);
                spline.beginningMaxWidth = Mathf.Clamp(spline.beginningMaxWidth, 0, spline.beginningSpline.vertsInShape - 1);
                if (spline.beginningMinWidth == spline.beginningMaxWidth)
                {
                    if (spline.beginningMinWidth > 0)
                        spline.beginningMinWidth--;
                    else
                        spline.beginningMaxWidth++;
                }
                spline.vertsInShape = (int)(spline.beginningMaxWidth - spline.beginningMinWidth) + 1;
                spline.beginningMinWidth = spline.beginningMinWidth / (float)(spline.beginningSpline.vertsInShape - 1);
                spline.beginningMaxWidth = spline.beginningMaxWidth / (float)(spline.beginningSpline.vertsInShape - 1);

                spline.GenerateBeginningParentBased();
            }
        }
        else
        {
            spline.beginningMaxWidth = 1;
            spline.beginningMinWidth = 0;
        }


        if (spline.endingSpline != null)
        {
            if (spline.controlPoints.Count > 1 && spline.endingSpline.points.Count > 0)
            {
                spline.endingMinWidth = spline.endingMinWidth * (spline.endingSpline.vertsInShape - 1);
                spline.endingMaxWidth = spline.endingMaxWidth * (spline.endingSpline.vertsInShape - 1);

                EditorGUILayout.MinMaxSlider("Part parent", ref spline.endingMinWidth, ref spline.endingMaxWidth, 0, spline.endingSpline.vertsInShape - 1);

                spline.endingMinWidth = (int)spline.endingMinWidth;
                spline.endingMaxWidth = (int)spline.endingMaxWidth;
                spline.endingMinWidth = Mathf.Clamp(spline.endingMinWidth, 0, spline.endingSpline.vertsInShape - 1);
                spline.endingMaxWidth = Mathf.Clamp(spline.endingMaxWidth, 0, spline.endingSpline.vertsInShape - 1);
                if (spline.endingMinWidth == spline.endingMaxWidth)
                {
                    if (spline.endingMinWidth > 0)
                        spline.endingMinWidth--;
                    else
                        spline.endingMaxWidth++;
                }
                spline.vertsInShape = (int)(spline.endingMaxWidth - spline.endingMinWidth) + 1;
                spline.endingMinWidth = spline.endingMinWidth / (float)(spline.endingSpline.vertsInShape - 1);
                spline.endingMaxWidth = spline.endingMaxWidth / (float)(spline.endingSpline.vertsInShape - 1);

                spline.GenerateEndingParentBased();
            }
        }
        else
        {
            spline.endingMaxWidth = 1;
            spline.endingMinWidth = 0;
        }

    }

    void PointsUI()
    {
        if (GUILayout.Button(new GUIContent("Remove all points", "Removes all points")))
        {
            spline.RemovePoints();

        }

        for (int i = 0; i < spline.controlPoints.Count; i++)
        {

            GUILayout.Label("Point: " + i.ToString(), EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            spline.controlPoints[i] = EditorGUILayout.Vector4Field("", spline.controlPoints[i]);
            if (spline.controlPoints[i].w <= 0)
            {
                Vector4 vec4 = spline.controlPoints[i];
                vec4.w = 0;
                spline.controlPoints[i] = vec4;
            }
            if (GUILayout.Button(new GUIContent("A", "Add point after this point"), GUILayout.MaxWidth(20)))
            {

                spline.AddPointAfter(i);
                spline.GenerateSpline();
#if VEGETATION_STUDIO_PRO
                RegeneratBiomeMask();
#endif
            }
            if (GUILayout.Button(new GUIContent("R", "Remove this Point"), GUILayout.MaxWidth(20)))
            {

                spline.RemovePoint(i);
                spline.GenerateSpline();
#if VEGETATION_STUDIO_PRO
                RegeneratBiomeMask();
#endif
            }
            if (GUILayout.Toggle(selectedPosition == i, new GUIContent("S", "Select point"), "Button", GUILayout.MaxWidth(20)))
            {
                selectedPosition = i;
            }
            else if (selectedPosition == i)
            {
                selectedPosition = -1;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (spline.controlPointsRotations.Count > i)
                spline.controlPointsRotations[i] = Quaternion.Euler(EditorGUILayout.Vector3Field("", spline.controlPointsRotations[i].eulerAngles));
            if (GUILayout.Button(new GUIContent("    Clear rotation    ", "Clear Rotation")))
            {
                spline.controlPointsRotations[i] = Quaternion.identity;
                spline.GenerateSpline();
#if VEGETATION_STUDIO_PRO
                RegeneratBiomeMask();
#endif
            }
            EditorGUILayout.EndHorizontal();

            if (spline.controlPointsSnap.Count > i)
                spline.controlPointsSnap[i] = EditorGUILayout.Toggle("Snap to terrain", spline.controlPointsSnap[i] == 1 ? true : false) == true ? 1 : 0;
            if (spline.controlPointsMeshCurves.Count > i)
                spline.controlPointsMeshCurves[i] = EditorGUILayout.CurveField("Mesh curve", spline.controlPointsMeshCurves[i]);
            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
        }
    }

    void SetMaterials()
    {
        GUILayout.Label("Set materials: ", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        {

            if (GUILayout.Button("Basic", GUILayout.MinWidth(80)))
            {
                try
                {
                    string materialName = "RAM_River_Material_Gamma";
                    if (PlayerSettings.colorSpace == ColorSpace.Linear)
                        materialName = "RAM_River_Material_Linear";
                    Material riverMat = (Material)Resources.Load(materialName);
                    if (riverMat != null)
                    {
                        spline.GetComponent<MeshRenderer>().sharedMaterial = riverMat;
                    }
                }
                catch
                {
                }
            }
            if (GUILayout.Button("Vertex color", GUILayout.MinWidth(80)))
            {
                try
                {
                    string materialName = "RAM_River_Material_Gamma_Vertex_Color";
                    if (PlayerSettings.colorSpace == ColorSpace.Linear)
                        materialName = "RAM_River_Material_Linear_Vertex_Color";
                    Material riverMat = (Material)Resources.Load(materialName);
                    if (riverMat != null)
                    {
                        spline.GetComponent<MeshRenderer>().sharedMaterial = riverMat;
                    }
                }
                catch
                {
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Basic tesseled", GUILayout.MinWidth(80)))
            {
                try
                {
                    string materialName = "RAM_River_Material_Gamma_Tess";
                    if (PlayerSettings.colorSpace == ColorSpace.Linear)
                        materialName = "RAM_River_Material_Linear_Tess";
                    Material riverMat = (Material)Resources.Load(materialName);
                    if (riverMat != null)
                    {
                        spline.GetComponent<MeshRenderer>().sharedMaterial = riverMat;
                    }
                }
                catch
                {
                }
            }

            if (GUILayout.Button("Basic tesseled - vertex color", GUILayout.MinWidth(80)))
            {
                try
                {
                    string materialName = "RAM_River_Material_Gamma_Tess_Vertex_Color";
                    if (PlayerSettings.colorSpace == ColorSpace.Linear)
                        materialName = "RAM_River_Material_Linear_Tess_Vertex_Color";
                    Material riverMat = (Material)Resources.Load(materialName);
                    if (riverMat != null)
                    {
                        spline.GetComponent<MeshRenderer>().sharedMaterial = riverMat;
                    }
                }
                catch
                {
                }
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    void ChangePivot(Vector3 center)
    {
        Vector3 position = spline.transform.position;
        spline.transform.position += center;
        for (int i = 0; i < spline.controlPoints.Count; i++)
        {
            Vector4 vec = spline.controlPoints[i];
            vec.x -= center.x;
            vec.y -= center.y;
            vec.z -= center.z;
            spline.controlPoints[i] = vec;
        }
        spline.GenerateSpline();
#if VEGETATION_STUDIO_PRO
        RegeneratBiomeMask();
#endif
    }




    protected virtual void OnSceneGUIInvoke(SceneView sceneView)
    {

        if (spline == null)
            spline = (RamSpline)target;

        Color baseColor = Handles.color;
        int controlId = GUIUtility.GetControlID(FocusType.Passive);

        if (spline != null)
        {
            Camera sceneCamera = SceneView.lastActiveSceneView.camera;
            CheckRotations();



            if (spline.drawOnMesh || spline.drawOnMeshFlowMap)
            {
                Tools.current = Tool.None;
                if (spline.meshfilter != null)
                {
                    Handles.color = Color.magenta;
                    Vector3[] vertices = spline.meshfilter.sharedMesh.vertices;
                    Vector2[] uv4 = spline.meshfilter.sharedMesh.uv4;
                    Vector3[] normals = spline.meshfilter.sharedMesh.normals;
                    Quaternion up = Quaternion.Euler(90, 0, 0);
                    for (int i = 0; i < vertices.Length; i += 5)
                    {
                        Vector3 item = vertices[i];
                        Vector3 handlePos = spline.transform.TransformPoint(item);

                        if (spline.drawOnMesh)
                            Handles.RectangleHandleCap(0, handlePos, up, 0.05f, EventType.Repaint);
                    }

                }
                if (spline.drawOnMesh)
                    DrawOnVertexColors();
                else
                    DrawOnFlowMap();
                return;
            }

            if (Event.current.commandName == "UndoRedoPerformed")
            {

                spline.GenerateSpline();
#if VEGETATION_STUDIO_PRO
                RegeneratBiomeMask();
#endif
                return;
            }

            if (selectedPosition >= 0 && selectedPosition < spline.controlPoints.Count)
            {
                Handles.color = Color.red;
                Handles.SphereHandleCap(0, (Vector3)spline.controlPoints[selectedPosition] + spline.transform.position, Quaternion.identity, 1, EventType.Repaint);

            }

            if (spline.debug)
            {
                ShowDebugHandles();
            }

            int controlPointToDelete = -1;




            for (int j = 0; j < spline.controlPoints.Count; j++)
            {



                EditorGUI.BeginChangeCheck();



                Vector3 handlePos = (Vector3)spline.controlPoints[j] + spline.transform.position;




                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.red;

                Vector3 screenPoint = Camera.current.WorldToScreenPoint(handlePos);

                if (screenPoint.z > 0)
                {

                    Handles.Label(handlePos + Vector3.up * HandleUtility.GetHandleSize(handlePos), "Point: " + j.ToString(), style);

                }
                float width = spline.controlPoints[j].w;
                if (Event.current.control && Event.current.shift && spline.controlPoints.Count > 1)
                {
                    int id = GUIUtility.GetControlID(FocusType.Passive);



                    if (HandleUtility.nearestControl == id)
                    {
                        Handles.color = Color.white;
                        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                            controlPointToDelete = j;
                    }
                    else
                        Handles.color = Handles.xAxisColor;

                    float size = 0.6f;
                    size = HandleUtility.GetHandleSize(handlePos) * size;
                    if (Event.current.type == EventType.Repaint)
                    {
                        Handles.SphereHandleCap(id, (Vector3)spline.controlPoints[j] + spline.transform.position, Quaternion.identity, size, EventType.Repaint);
                    }
                    else if (Event.current.type == EventType.Layout)
                    {
                        Handles.SphereHandleCap(id, (Vector3)spline.controlPoints[j] + spline.transform.position, Quaternion.identity, size, EventType.Layout);
                    }

                }
                else if (Tools.current == Tool.Move)
                {

                    float size = 0.6f;
                    size = HandleUtility.GetHandleSize(handlePos) * size;

                    Handles.color = Handles.xAxisColor;
                    Vector4 pos = Handles.Slider((Vector3)spline.controlPoints[j] + spline.transform.position, Vector3.right, size, Handles.ArrowHandleCap, 0.01f) - spline.transform.position;
                    Handles.color = Handles.yAxisColor;
                    pos = Handles.Slider((Vector3)pos + spline.transform.position, Vector3.up, size, Handles.ArrowHandleCap, 0.01f) - spline.transform.position;
                    Handles.color = Handles.zAxisColor;
                    pos = Handles.Slider((Vector3)pos + spline.transform.position, Vector3.forward, size, Handles.ArrowHandleCap, 0.01f) - spline.transform.position;

                    Vector3 halfPos = (Vector3.right + Vector3.forward) * size * 0.3f;
                    Handles.color = Handles.yAxisColor;
                    pos = Handles.Slider2D((Vector3)pos + spline.transform.position + halfPos, Vector3.up, Vector3.right, Vector3.forward, size * 0.3f, Handles.RectangleHandleCap, 0.01f) - spline.transform.position - halfPos;
                    halfPos = (Vector3.right + Vector3.up) * size * 0.3f;
                    Handles.color = Handles.zAxisColor;
                    pos = Handles.Slider2D((Vector3)pos + spline.transform.position + halfPos, Vector3.forward, Vector3.right, Vector3.up, size * 0.3f, Handles.RectangleHandleCap, 0.01f) - spline.transform.position - halfPos;
                    halfPos = (Vector3.up + Vector3.forward) * size * 0.3f;
                    Handles.color = Handles.xAxisColor;
                    pos = Handles.Slider2D((Vector3)pos + spline.transform.position + halfPos, Vector3.right, Vector3.up, Vector3.forward, size * 0.3f, Handles.RectangleHandleCap, 0.01f) - spline.transform.position - halfPos;

                    pos.w = width;
                    spline.controlPoints[j] = pos;


                }
                else if (Tools.current == Tool.Rotate)
                {

                    if (spline.controlPointsRotations.Count > j && spline.controlPointsOrientation.Count > j)
                    {

                        if (!((spline.beginningSpline && j == 0) || (spline.endingSpline && j == spline.controlPoints.Count - 1)))
                        {
                            float size = 0.6f;
                            size = HandleUtility.GetHandleSize(handlePos) * size;

                            Handles.color = Handles.zAxisColor;
                            Quaternion rotation = Handles.Disc(spline.controlPointsOrientation[j], handlePos, spline.controlPointsOrientation[j] * new Vector3(0, 0, 1), size, true, 0.1f);

                            Handles.color = Handles.yAxisColor;
                            rotation = Handles.Disc(rotation, handlePos, rotation * new Vector3(0, 1, 0), size, true, 0.1f);

                            Handles.color = Handles.xAxisColor;
                            rotation = Handles.Disc(rotation, handlePos, rotation * new Vector3(1, 0, 0), size, true, 0.1f);



                            spline.controlPointsRotations[j] *= (Quaternion.Inverse(spline.controlPointsOrientation[j]) * rotation);

                            if (float.IsNaN(spline.controlPointsRotations[j].x) || float.IsNaN(spline.controlPointsRotations[j].y) || float.IsNaN(spline.controlPointsRotations[j].z) || float.IsNaN(spline.controlPointsRotations[j].w))
                            {
                                spline.controlPointsRotations[j] = Quaternion.identity;
                                spline.GenerateSpline();
#if VEGETATION_STUDIO_PRO
                                RegeneratBiomeMask();
#endif
                            }
                            Handles.color = baseColor;
                            Handles.FreeRotateHandle(Quaternion.identity, handlePos, size);

                            Handles.CubeHandleCap(0, handlePos, spline.controlPointsOrientation[j], size * 0.3f, EventType.Repaint);

                            Handles.DrawLine(spline.controlPointsUp[j] + spline.transform.position, spline.controlPointsDown[j] + spline.transform.position);
                        }


                    }

                }
                else if (Tools.current == Tool.Scale)
                {

                    Handles.color = Handles.xAxisColor;
                    //Vector3 handlePos = (Vector3)spline.controlPoints [j] + spline.transform.position;

                    width = Handles.ScaleSlider(spline.controlPoints[j].w, (Vector3)spline.controlPoints[j] + spline.transform.position, new Vector3(0, 0.5f, 0),
                        Quaternion.Euler(-90, 0, 0), HandleUtility.GetHandleSize(handlePos), 0.01f);

                    Vector4 pos = spline.controlPoints[j];
                    pos.w = width;
                    spline.controlPoints[j] = pos;

                }



                if (EditorGUI.EndChangeCheck())
                {

                    CheckRotations();
                    Undo.RecordObject(spline, "Change Position");
                    spline.GenerateSpline();
#if VEGETATION_STUDIO_PRO
                    RegeneratBiomeMask();
#endif

                }

            }

            if (controlPointToDelete >= 0)
            {
                Undo.RecordObject(spline, "Remove point");


                spline.RemovePoint(controlPointToDelete);

                spline.GenerateSpline();

                GUIUtility.hotControl = controlId;
                Event.current.Use();
                HandleUtility.Repaint();
                controlPointToDelete = -1;
            }

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.control && !Event.current.shift)
            {


                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Undo.RecordObject(spline, "Add point");

                    Vector4 position = hit.point - spline.transform.position;
                    spline.AddPoint(position);

                    spline.GenerateSpline();

                    GUIUtility.hotControl = controlId;
                    Event.current.Use();
                    HandleUtility.Repaint();
                }


            }
            if (!Event.current.control && Event.current.shift && spline.controlPoints.Count > 1)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    int idMin = -1;
                    float distanceMin = float.MaxValue;

                    for (int j = 0; j < spline.controlPoints.Count; j++)
                    {
                        Vector3 handlePos = (Vector3)spline.controlPoints[j] + spline.transform.position;

                        float pointDist = Vector3.Distance(hit.point, handlePos);
                        if (pointDist < distanceMin)
                        {
                            distanceMin = pointDist;
                            idMin = j;
                        }
                    }

                    Vector3 posOne = (Vector3)spline.controlPoints[idMin] + spline.transform.position;
                    Vector3 posTwo;



                    if (idMin == 0)
                    {
                        posTwo = (Vector3)spline.controlPoints[1] + spline.transform.position;
                    }
                    else if (idMin == spline.controlPoints.Count - 1)
                    {
                        posTwo = (Vector3)spline.controlPoints[spline.controlPoints.Count - 2] + spline.transform.position;

                        idMin = idMin - 1;
                    }
                    else
                    {
                        Vector3 posPrev = (Vector3)spline.controlPoints[idMin - 1] + spline.transform.position;
                        Vector3 posNext = (Vector3)spline.controlPoints[idMin + 1] + spline.transform.position;

                        if (Vector3.Distance(hit.point, posPrev) > Vector3.Distance(hit.point, posNext))
                            posTwo = posNext;
                        else
                        {
                            posTwo = posPrev;
                            idMin = idMin - 1;
                        }

                    }


                    Handles.DrawLine(hit.point, posOne);
                    Handles.DrawLine(hit.point, posTwo);

                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {

                        Undo.RecordObject(spline, "Add point");

                        Vector4 position = hit.point - spline.transform.position;
                        spline.AddPointAfter(idMin);
                        spline.ChangePointPosition(idMin + 1, position);

                        spline.GenerateSpline();

                        GUIUtility.hotControl = controlId;
                        Event.current.Use();
                        HandleUtility.Repaint();

#if VEGETATION_STUDIO_PRO
                        RegeneratBiomeMask();
#endif
                    }

                }

            }

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && Event.current.control)
            {
                GUIUtility.hotControl = 0;

            }
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && Event.current.shift)
            {
                GUIUtility.hotControl = 0;

            }



        }


    }

    private void ShowDebugHandles()
    {
        Vector3[] points = new Vector3[spline.controlPoints.Count];


        for (int i = 0; i < spline.controlPoints.Count; i++)
        {
            points[i] = (Vector3)spline.controlPoints[i] + spline.transform.position;
        }


        Handles.color = Color.white;
        Handles.DrawPolyLine(points);

        Handles.color = new Color(1, 0, 0, 0.5f);

        for (int i = 0; i < spline.pointsDown.Count; i++)
        {

            Vector3 handlePos = (Vector3)spline.pointsDown[i] + spline.transform.position;
            Vector3 handlePos2 = (Vector3)spline.pointsUp[i] + spline.transform.position;
            if (spline.debugPointsConnect)
                Handles.DrawLine(handlePos, handlePos2);
        }


        Handles.color = Color.blue;


        points = new Vector3[spline.pointsDown.Count];


        for (int i = 0; i < spline.pointsDown.Count; i++)
        {
            if (spline.debugPoints)
                Handles.SphereHandleCap(0, spline.pointsDown[i] + spline.transform.position, Quaternion.identity, 0.1f, EventType.Repaint);
            points[i] = (Vector3)spline.pointsDown[i] + spline.transform.position;
        }

        Handles.DrawPolyLine(points);


        points = new Vector3[spline.pointsUp.Count];

        for (int i = 0; i < spline.pointsUp.Count; i++)
        {
            if (spline.debugPoints)
                Handles.SphereHandleCap(0, spline.pointsUp[i] + spline.transform.position, Quaternion.identity, 0.1f, EventType.Repaint);
            points[i] = (Vector3)spline.pointsUp[i] + spline.transform.position;
        }

        Handles.DrawPolyLine(points);


        spline.debugMesh = true;

        //Normals, tangents
        //if (!spline.debugMesh)
        //{
        //points = spline.points.ToArray();
        //for (int i = 0; i < points.Length; i++)
        //{
        //    points[i] += spline.transform.position;
        //    Handles.color = Color.green;
        //    if (spline.debugNormals)
        //    {
        //        Handles.DrawLine(points[i], points[i] + spline.normalsList[i]);


        //    }
        //    if (spline.debugBitangent)
        //    {

        //        Vector3 posUp = spline.orientations[i] * Vector3.right;
        //        Handles.DrawLine(points[i], points[i] + posUp);
        //    }
        //    Handles.color = Color.red;
        //    if (spline.debugTangents)
        //        Handles.DrawLine(points[i] - spline.tangents[i], points[i] + spline.tangents[i]);
        //}
        //}
        //else if (spline.debugMesh)
        //{
        Vector3 camPosition = SceneView.lastActiveSceneView.camera.transform.position;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.red;

        Mesh mesh = spline.meshfilter.sharedMesh;
        if (mesh)
        {
            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;
            Vector4[] tangents = mesh.tangents;
            Vector2[] uv4 = mesh.uv4;
            float distDebug = spline.distanceToDebug * spline.distanceToDebug;
            for (int i = 0; i < vertices.Length; i++)
            {


                vertices[i] += spline.transform.position;

                Vector3 offset = vertices[i] - camPosition;
                float sqrLen = offset.sqrMagnitude;

                if (sqrLen > distDebug)
                    continue;

                Handles.color = Color.green;
                if (spline.debugNormals)
                {
                    Handles.DrawLine(vertices[i], vertices[i] + normals[i]);

                }
                Handles.color = Color.red;
                if (spline.debugTangents)
                    Handles.DrawLine(vertices[i] - (Vector3)tangents[i], vertices[i] + (Vector3)tangents[i]);

                Handles.color = Color.magenta;

                if (spline.debugFlowmap)
                {
                    Handles.DrawLine(vertices[i], vertices[i] + new Vector3(uv4[i].x, uv4[i].y, 0) * 2);
                    Handles.Label(vertices[i] + new Vector3(uv4[i].x, uv4[i].y, 0) * 2, uv4[i].x + " " + uv4[i].y, style);
                }

            }

        }
        //}

    }

    void DrawOnVertexColors()
    {
        if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
        {
            Undo.RegisterCompleteObjectUndo(spline, "Painted");
        }
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        //Camera sceneCamera = SceneView.lastActiveSceneView.camera;
        //Vector2 mousePos = Event.current.mousePosition;
        //mousePos.y = Screen.height - mousePos.y - 40;

        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);



        List<MeshCollider> meshColliders = new List<MeshCollider>();
        foreach (var item in splines)
        {
            meshColliders.Add(item.gameObject.AddComponent<MeshCollider>());
        }


        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

        GameObject go = null;
        Vector3 hitPosition = Vector3.zero;
        Vector3 hitNormal = Vector3.zero;
        RamSpline hitedSpline = null;
        if (hits.Length > 0)
        {

            foreach (var hit in hits)
            {
                if (hit.collider is MeshCollider)
                {
                    go = hit.collider.gameObject;
                    hitedSpline = go.GetComponent<RamSpline>();

                    if (hitedSpline != null && (spline.drawOnMultiple || hitedSpline == spline))
                    {

                        hitPosition = hit.point;
                        hitNormal = hit.normal;
                        break;
                    }
                    else
                        go = null;
                }
            }

        }
        foreach (var item in meshColliders)
        {
            if (item != null)
                DestroyImmediate(item);
        }



        if (go != null)
        {
            Handles.color = new Color(spline.drawColor.r, spline.drawColor.g, spline.drawColor.b, 1);
            Handles.DrawLine(hitPosition, hitPosition + hitNormal * 2);
            Handles.CircleHandleCap(
                0,
                hitPosition,
                Quaternion.LookRotation(hitNormal),
                spline.drawSize,
                EventType.Repaint
            );
            Handles.color = Color.black;
            Handles.CircleHandleCap(
                0,
                hitPosition,
                Quaternion.LookRotation(hitNormal),
                spline.drawSize - 0.1f,
                EventType.Repaint
            );

            if (!(Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) || Event.current.button != 0)
                return;
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
            }

            MeshFilter meshFilter = hitedSpline.GetComponent<MeshFilter>();
            if (meshFilter.sharedMesh != null)
            {
                Mesh mesh = meshFilter.sharedMesh;
                if (hitedSpline.colors.Length == 0)
                    hitedSpline.colors = new Color[mesh.vertices.Length];

                int length = mesh.vertices.Length;
                float dist = 0;
                hitPosition -= hitedSpline.transform.position;
                Vector3[] vertices = mesh.vertices;
                Color[] colors = hitedSpline.colors;

                for (int i = 0; i < length; i++)
                {
                    dist = Vector3.Distance(hitPosition, vertices[i]);

                    if (dist < hitedSpline.drawSize)
                    {

                        if (Event.current.shift)
                        {
                            if (spline.drawColorR)
                                colors[i].r = Mathf.Lerp(colors[i].r, 0, spline.opacity);
                            if (spline.drawColorG)
                                colors[i].g = Mathf.Lerp(colors[i].g, 0, spline.opacity);
                            if (spline.drawColorB)
                                colors[i].b = Mathf.Lerp(colors[i].b, 0, spline.opacity);
                            if (spline.drawColorA)
                                colors[i].a = Mathf.Lerp(colors[i].a, 1, spline.opacity);
                        }
                        else
                        {
                            if (spline.drawColorR)
                                colors[i].r = Mathf.Lerp(colors[i].r, spline.drawColor.r, spline.opacity);
                            if (spline.drawColorG)
                                colors[i].g = Mathf.Lerp(colors[i].g, spline.drawColor.g, spline.opacity);
                            if (spline.drawColorB)
                                colors[i].b = Mathf.Lerp(colors[i].b, spline.drawColor.b, spline.opacity);
                            if (spline.drawColorA)
                                colors[i].a = Mathf.Lerp(colors[i].a, spline.drawColor.a, spline.opacity);
                        }

                    }
                }

                mesh.colors = colors;
                meshFilter.sharedMesh = mesh;
                if (hitedSpline.generateMeshParts)
                    hitedSpline.GenerateMeshParts(mesh);
            }
        }
    }

    void DrawOnFlowMap()
    {
        if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
        {
            Undo.RegisterCompleteObjectUndo(spline, "Painted");
        }
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        //Camera sceneCamera = SceneView.lastActiveSceneView.camera;
        //Vector2 mousePos = Event.current.mousePosition;
        //mousePos.y = Screen.height - mousePos.y - 40;

        //Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        List<MeshCollider> meshColliders = new List<MeshCollider>();
        foreach (var item in splines)
        {
            meshColliders.Add(item.gameObject.AddComponent<MeshCollider>());
        }


        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

        GameObject go = null;
        Vector3 hitPosition = Vector3.zero;
        Vector3 hitNormal = Vector3.zero;
        RamSpline hitedSpline = null;
        if (hits.Length > 0)
        {

            foreach (var hit in hits)
            {
                if (hit.collider is MeshCollider)
                {
                    go = hit.collider.gameObject;
                    hitedSpline = go.GetComponent<RamSpline>();


                    if (hitedSpline != null && (spline.drawOnMultiple || hitedSpline == spline))
                    {

                        hitPosition = hit.point;
                        hitNormal = hit.normal;
                        break;
                    }
                    else
                        go = null;
                }
            }

        }

        foreach (var item in meshColliders)
        {
            if (item != null)
                DestroyImmediate(item);
        }


        if (go != null)
        {

            Handles.color = new Color(spline.flowDirection, spline.flowSpeed, 0, 1);
            Handles.DrawLine(hitPosition, hitPosition + hitNormal * 2);
            Handles.CircleHandleCap(
                0,
                hitPosition,
                Quaternion.LookRotation(hitNormal),
                spline.drawSize,
                EventType.Repaint
            );
            Handles.color = Color.black;
            Handles.CircleHandleCap(
                0,
                hitPosition,
                Quaternion.LookRotation(hitNormal),
                spline.drawSize - 0.1f,
                EventType.Repaint
            );

            if (!(Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) || Event.current.button != 0)
                return;
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
            }

            hitedSpline.overrideFlowMap = true;
            MeshFilter meshFilter = hitedSpline.GetComponent<MeshFilter>();


            if (meshFilter.sharedMesh != null)
            {
                Mesh mesh = meshFilter.sharedMesh;


                List<Vector2> colorsFlowMap = hitedSpline.colorsFlowMap;
                int length = mesh.vertices.Length;
                float dist = 0;
                float distValue = 0;
                hitPosition -= hitedSpline.transform.position;
                Vector3[] vertices = mesh.vertices;

                for (int i = 0; i < length; i++)
                {
                    dist = Vector3.Distance(hitPosition, vertices[i]);
                    if (dist < spline.drawSize)
                    {
                        distValue = (spline.drawSize - dist) / (float)spline.drawSize;
                        if (Event.current.shift)
                        {
                            colorsFlowMap[i] = Vector2.Lerp(colorsFlowMap[i], new Vector2(0, 0), spline.opacity);

                        }
                        else
                        {
                            colorsFlowMap[i] = Vector2.Lerp(colorsFlowMap[i], new Vector2(spline.flowDirection, spline.flowSpeed), spline.opacity * distValue);

                        }

                    }
                }

                mesh.uv4 = colorsFlowMap.ToArray();
                hitedSpline.colorsFlowMap = colorsFlowMap;
                meshFilter.sharedMesh = mesh;
                if (hitedSpline.generateMeshParts)
                    hitedSpline.GenerateMeshParts(mesh);
            }

        }
    }

    public static LayerMask LayerMaskField(string label, LayerMask selected, bool showSpecial)
    {

        List<string> layers = new List<string>();
        List<int> layerNumbers = new List<int>();

        string selectedLayers = "";

        for (int i = 0; i < 32; i++)
        {
            string layerName = LayerMask.LayerToName(i);
            if (layerName != "")
            {
                if (selected == (selected | (1 << i)))
                {
                    if (selectedLayers == "")
                    {
                        selectedLayers = layerName;
                    }
                    else
                    {
                        selectedLayers = "Mixed";
                    }
                }
            }
        }

        EventType lastEvent = Event.current.type;

        if (Event.current.type != EventType.MouseDown && Event.current.type != EventType.ExecuteCommand)
        {
            if (selected.value == 0)
            {
                layers.Add("Nothing");
            }
            else if (selected.value == -1)
            {
                layers.Add("Everything");
            }
            else
            {
                layers.Add(selectedLayers);
            }
            layerNumbers.Add(-1);
        }

        if (showSpecial)
        {
            layers.Add((selected.value == 0 ? "[X] " : "      ") + "Nothing");
            layerNumbers.Add(-2);

            layers.Add((selected.value == -1 ? "[X] " : "      ") + "Everything");
            layerNumbers.Add(-3);
        }

        for (int i = 0; i < 32; i++)
        {

            string layerName = LayerMask.LayerToName(i);

            if (layerName != "")
            {
                if (selected == (selected | (1 << i)))
                {
                    layers.Add("[X] " + i + ": " + layerName);
                }
                else
                {
                    layers.Add("     " + i + ": " + layerName);
                }
                layerNumbers.Add(i);
            }
        }

        bool preChange = GUI.changed;

        GUI.changed = false;

        int newSelected = 0;

        if (Event.current.type == EventType.MouseDown)
        {
            newSelected = -1;
        }

        newSelected = EditorGUILayout.Popup(label, newSelected, layers.ToArray(), EditorStyles.layerMaskField);

        if (GUI.changed && newSelected >= 0)
        {
            if (showSpecial && newSelected == 0)
            {
                selected = 0;
            }
            else if (showSpecial && newSelected == 1)
            {
                selected = -1;
            }
            else
            {

                if (selected == (selected | (1 << layerNumbers[newSelected])))
                {
                    selected &= ~(1 << layerNumbers[newSelected]);
                }
                else
                {
                    selected = selected | (1 << layerNumbers[newSelected]);
                }
            }
        }
        else
        {
            GUI.changed = preChange;
        }

        return selected;
    }

    public void PointsToFile()
    {
        var path = EditorUtility.SaveFilePanelInProject(
            "Save Spline Points",
            spline.name + "Points.csv",
            "csv",
            "Save Spline " + spline.name + " Points in CSV");

        if (string.IsNullOrEmpty(path))
            return;

        string fileData = "";

        foreach (Vector4 v in spline.controlPoints)
        {
            fileData += v.x + ";" + v.y + ";" + v.z + ";" + v.w + "\n";
        }
        if (fileData.Length > 0)
            fileData.Remove(fileData.Length - 1, 1);

        // Debug.Log(fileData);
        File.WriteAllText(path, fileData);

    }

    public void PointsFromFile()
    {
        string path = EditorUtility.OpenFilePanel("Read Spline Points from CSV", Application.dataPath, "csv");

        if (string.IsNullOrEmpty(path))
            return;

        string fileData = File.ReadAllText(path);

        string[] lines = fileData.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        Vector4[] vectors = new Vector4[lines.Length];

        for (int i = 0; i < vectors.Length; i++)
        {
            string[] values = lines[i].Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);

            if (values.Length != 4)
                Debug.LogError("Wrong file data");
            else
            {
                try
                {
                    vectors[i] = new Vector4(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
                }
                catch (System.Exception)
                {

                    Debug.LogError("Wrong file data");
                    return;

                }
            }

        }
        Undo.RecordObject(spline, "Spline changed");
        if (vectors.Length > 0)
        {
            foreach (var item in vectors)
            {
                spline.AddPoint(item);
            }

        }

    }

}
