using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.Rendering;

#if VEGETATION_STUDIO_PRO
using AwesomeTechnologies;
using AwesomeTechnologies.VegetationSystem;
using AwesomeTechnologies.VegetationSystem.Biomes;
#endif

#if VEGETATION_STUDIO
using AwesomeTechnologies;
using AwesomeTechnologies.VegetationStudio;
#endif

[CustomEditor(typeof(LakePolygon))]
public class LakePolygonEditor : Editor
{
    LakePolygon[] lakes;

    LakePolygon lakePolygon;

    int selectedPosition = -1;
    Texture2D logo;
    bool showCarveTerrain = false;

    public string[] toolbarStrings = new string[] {
        "Basic",
        "Points",
        "Vertex Color",
        "Flow Map",
        "Simulate\n[ALPHA] ",
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

    Mesh meshTerrain;
    [MenuItem("GameObject/3D Object/Create Lake Polygon")]
    static public void CreatelakePolygon()
    {

        Selection.activeGameObject = LakePolygon.CreatePolygon(AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat")).gameObject;
    }

    void OnEnable()
    {
        lakes = FindObjectsOfType<LakePolygon>();

    }

    private void OnDisable()
    {
        if (lakePolygon == null)
            lakePolygon = (LakePolygon)target;
        if (lakePolygon.meshGOs != null && lakePolygon.meshGOs.Count > 0)
        {
            foreach (var item in lakePolygon.meshGOs)
            {
                DestroyImmediate(item);
            }
            lakePolygon.meshGOs.Clear();
        }
        if (lakePolygon.lakePolygonCarveData != null)
            lakePolygon.lakePolygonCarveData = null;
        if (lakePolygon.lakePolygonPaintData != null)
            lakePolygon.lakePolygonPaintData = null;
        if (lakePolygon.lakePolygonClearData != null)
            lakePolygon.lakePolygonClearData = null;
    }


    private void OnDestroy()
    {
        if (lakePolygon == null)
            lakePolygon = (LakePolygon)target;
        if (lakePolygon.meshGOs != null && lakePolygon.meshGOs.Count > 0)
        {
            foreach (var item in lakePolygon.meshGOs)
            {
                DestroyImmediate(item);
            }
            lakePolygon.meshGOs.Clear();
        }
        if (lakePolygon.lakePolygonCarveData != null)
            lakePolygon.lakePolygonCarveData = null;
        if (lakePolygon.lakePolygonPaintData != null)
            lakePolygon.lakePolygonPaintData = null;
        if (lakePolygon.lakePolygonClearData != null)
            lakePolygon.lakePolygonClearData = null;
    }



    public override void OnInspectorGUI()
    {
        if (lakePolygon == null)
            lakePolygon = (LakePolygon)target;

        EditorGUILayout.Space();
        logo = (Texture2D)Resources.Load("logoRAM");

        GUIContent btnTxt = new GUIContent(logo);

        var rt = GUILayoutUtility.GetRect(btnTxt, GUI.skin.label, GUILayout.ExpandWidth(false));
        rt.center = new Vector2(EditorGUIUtility.currentViewWidth / 2, rt.center.y);

        GUI.Button(rt, btnTxt, GUI.skin.label);

        int toolbarNew = GUILayout.SelectionGrid(lakePolygon.toolbarInt, toolbarStrings, 3, GUILayout.Height(125));


        lakePolygon.drawOnMesh = false;
        lakePolygon.drawOnMeshFlowMap = false;

        if (lakePolygon.transform.eulerAngles.magnitude != 0 || lakePolygon.transform.localScale.x != 1 || lakePolygon.transform.localScale.y != 1 || lakePolygon.transform.localScale.z != 1)
            EditorGUILayout.HelpBox("Lake should have scale (1,1,1) and rotation (0,0,0) during edit!", MessageType.Error);


        if (toolbarNew == 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Add Point  - CTRL + Left Mouse Button Click \n" +
"Add point between existing points - SHIFT + Left Button Click \n" +
"Remove point - CTRL + SHIFT + Left Button Click", MessageType.Info);
            EditorGUILayout.Space();
            

            lakePolygon.currentProfile = (LakePolygonProfile)EditorGUILayout.ObjectField("Lake profile", lakePolygon.currentProfile, typeof(LakePolygonProfile), false);


           

            if (GUILayout.Button("Create profile from settings"))
            {

                LakePolygonProfile asset = ScriptableObject.CreateInstance<LakePolygonProfile>();

                MeshRenderer ren = lakePolygon.GetComponent<MeshRenderer>();


                asset.terrainCarve = new AnimationCurve(lakePolygon.terrainCarve.keys);
                asset.terrainPaintCarve = new AnimationCurve(lakePolygon.terrainPaintCarve.keys);


                asset.lakeMaterial = ren.sharedMaterial;

                asset.distSmooth = lakePolygon.distSmooth;
                asset.uvScale = lakePolygon.uvScale;
                asset.terrainSmoothMultiplier = lakePolygon.terrainSmoothMultiplier;
                asset.currentSplatMap = lakePolygon.currentSplatMap;

                asset.maximumTriangleSize = lakePolygon.maximumTriangleSize;
                asset.traingleDensity = lakePolygon.traingleDensity;

                asset.receiveShadows = lakePolygon.receiveShadows;
                asset.shadowCastingMode = lakePolygon.shadowCastingMode;


                asset.automaticFlowMapScale = lakePolygon.automaticFlowMapScale;

                asset.noiseflowMap = lakePolygon.noiseflowMap;
                asset.noiseMultiplierflowMap = lakePolygon.noiseMultiplierflowMap;
                asset.noiseSizeXflowMap = lakePolygon.noiseSizeXflowMap;
                asset.noiseSizeZflowMap = lakePolygon.noiseSizeZflowMap;


                asset.noiseCarve = lakePolygon.noiseCarve;
                asset.noiseMultiplierInside = lakePolygon.noiseMultiplierInside;
                asset.noiseMultiplierOutside = lakePolygon.noiseMultiplierOutside;
                asset.noiseSizeX = lakePolygon.noiseSizeX;
                asset.noiseSizeZ = lakePolygon.noiseSizeZ;

                asset.noisePaint = lakePolygon.noisePaint;
                asset.noiseMultiplierInsidePaint = lakePolygon.noiseMultiplierInsidePaint;
                asset.noiseMultiplierOutsidePaint = lakePolygon.noiseMultiplierOutsidePaint;
                asset.noiseSizeXPaint = lakePolygon.noiseSizeXPaint;
                asset.noiseSizeZPaint = lakePolygon.noiseSizeZPaint;
                asset.mixTwoSplatMaps = lakePolygon.mixTwoSplatMaps;
                asset.secondSplatMap = lakePolygon.secondSplatMap;
                asset.addCliffSplatMap = lakePolygon.addCliffSplatMap;

                asset.cliffSplatMap = lakePolygon.cliffSplatMap;
                asset.cliffAngle = lakePolygon.cliffAngle;
                asset.cliffBlend = lakePolygon.cliffBlend;

                asset.cliffSplatMapOutside = lakePolygon.cliffSplatMapOutside;
                asset.cliffAngleOutside = lakePolygon.cliffAngleOutside;
                asset.cliffBlendOutside = lakePolygon.cliffBlendOutside;


                asset.distanceClearFoliage = lakePolygon.distanceClearFoliage;
                asset.distanceClearFoliageTrees = lakePolygon.distanceClearFoliageTrees;


                string path = EditorUtility.SaveFilePanelInProject("Save new spline profile", lakePolygon.gameObject.name + ".asset", "asset", "Please enter a file name to save the spline profile to");

                if (!string.IsNullOrEmpty(path))
                {

                    AssetDatabase.CreateAsset(asset, path);
                    AssetDatabase.SaveAssets();
                    lakePolygon.currentProfile = asset;
                }
            }

            if (lakePolygon.currentProfile != null && GUILayout.Button("Save profile from settings"))
            {


                MeshRenderer ren = lakePolygon.GetComponent<MeshRenderer>();

                lakePolygon.currentProfile.terrainCarve = new AnimationCurve(lakePolygon.terrainCarve.keys);
                lakePolygon.currentProfile.terrainPaintCarve = new AnimationCurve(lakePolygon.terrainPaintCarve.keys);

                lakePolygon.currentProfile.lakeMaterial = ren.sharedMaterial;

                lakePolygon.currentProfile.distSmooth = lakePolygon.distSmooth;
                lakePolygon.currentProfile.uvScale = lakePolygon.uvScale;
                lakePolygon.currentProfile.terrainSmoothMultiplier = lakePolygon.terrainSmoothMultiplier;
                lakePolygon.currentProfile.currentSplatMap = lakePolygon.currentSplatMap;

                lakePolygon.currentProfile.maximumTriangleSize = lakePolygon.maximumTriangleSize;
                lakePolygon.currentProfile.traingleDensity = lakePolygon.traingleDensity;

                lakePolygon.currentProfile.receiveShadows = lakePolygon.receiveShadows;
                lakePolygon.currentProfile.shadowCastingMode = lakePolygon.shadowCastingMode;


                lakePolygon.currentProfile.automaticFlowMapScale = lakePolygon.automaticFlowMapScale;

                lakePolygon.currentProfile.noiseflowMap = lakePolygon.noiseflowMap;
                lakePolygon.currentProfile.noiseMultiplierflowMap = lakePolygon.noiseMultiplierflowMap;
                lakePolygon.currentProfile.noiseSizeXflowMap = lakePolygon.noiseSizeXflowMap;
                lakePolygon.currentProfile.noiseSizeZflowMap = lakePolygon.noiseSizeZflowMap;


                lakePolygon.currentProfile.noiseCarve = lakePolygon.noiseCarve;
                lakePolygon.currentProfile.noiseMultiplierInside = lakePolygon.noiseMultiplierInside;
                lakePolygon.currentProfile.noiseMultiplierOutside = lakePolygon.noiseMultiplierOutside;
                lakePolygon.currentProfile.noiseSizeX = lakePolygon.noiseSizeX;
                lakePolygon.currentProfile.noiseSizeZ = lakePolygon.noiseSizeZ;


                lakePolygon.currentProfile.noisePaint = lakePolygon.noisePaint;
                lakePolygon.currentProfile.noiseMultiplierInsidePaint = lakePolygon.noiseMultiplierInsidePaint;
                lakePolygon.currentProfile.noiseMultiplierOutsidePaint = lakePolygon.noiseMultiplierOutsidePaint;
                lakePolygon.currentProfile.noiseSizeXPaint = lakePolygon.noiseSizeXPaint;
                lakePolygon.currentProfile.noiseSizeZPaint = lakePolygon.noiseSizeZPaint;
                lakePolygon.currentProfile.mixTwoSplatMaps = lakePolygon.mixTwoSplatMaps;
                lakePolygon.currentProfile.secondSplatMap = lakePolygon.secondSplatMap;
                lakePolygon.currentProfile.addCliffSplatMap = lakePolygon.addCliffSplatMap;
                lakePolygon.currentProfile.cliffSplatMap = lakePolygon.cliffSplatMap;
                lakePolygon.currentProfile.cliffAngle = lakePolygon.cliffAngle;
                lakePolygon.currentProfile.cliffBlend = lakePolygon.cliffBlend;

                lakePolygon.currentProfile.cliffSplatMapOutside = lakePolygon.cliffSplatMapOutside;
                lakePolygon.currentProfile.cliffAngleOutside = lakePolygon.cliffAngleOutside;
                lakePolygon.currentProfile.cliffBlendOutside = lakePolygon.cliffBlendOutside;

                lakePolygon.currentProfile.distanceClearFoliage = lakePolygon.distanceClearFoliage;
                lakePolygon.currentProfile.distanceClearFoliageTrees = lakePolygon.distanceClearFoliageTrees;

                AssetDatabase.SaveAssets();
            }


            if (lakePolygon.currentProfile != null && lakePolygon.currentProfile != lakePolygon.oldProfile)
            {

                ResetToProfile();
                lakePolygon.GeneratePolygon();
                EditorUtility.SetDirty(lakePolygon);

            }

            if (CheckProfileChange())
                EditorGUILayout.HelpBox("Profile data changed.", MessageType.Info);

            if (lakePolygon.currentProfile != null && GUILayout.Button("Reset to profile"))
            {

                ResetToProfile();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();



            lakePolygon.lockHeight = EditorGUILayout.Toggle("Lock height", lakePolygon.lockHeight);

            EditorGUILayout.BeginHorizontal();
            lakePolygon.height = EditorGUILayout.FloatField(lakePolygon.height);
            if (GUILayout.Button("Set heights"))
            {
                for (int i = 0; i < lakePolygon.points.Count; i++)
                {
                    Vector3 point = lakePolygon.points[i];
                    point.y = lakePolygon.height - lakePolygon.transform.position.y;
                    lakePolygon.points[i] = point;
                }
                lakePolygon.GeneratePolygon();
            }

            EditorGUILayout.EndHorizontal();

            lakePolygon.yOffset = EditorGUILayout.FloatField("Y offset mesh", lakePolygon.yOffset);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();


            GUILayout.Label("Mesh settings:", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;
            string meshResolution = "Triangles density" + "(" + lakePolygon.trianglesGenerated + " tris)";

            EditorGUILayout.LabelField(meshResolution);

            if (lakePolygon.vertsGenerated > 65000)
            {
                EditorGUILayout.HelpBox("Too many vertices for 16 bit mesh index buffer.  Mesh switched to 32 bit index buffer.", MessageType.Warning);
            }


            lakePolygon.maximumTriangleSize = EditorGUILayout.DelayedFloatField("Maximum triangle size", lakePolygon.maximumTriangleSize);
            if (lakePolygon.maximumTriangleSize == 0)
                lakePolygon.maximumTriangleSize = 50;
            lakePolygon.traingleDensity = 1 / (float)EditorGUILayout.IntSlider("Spline density", (int)(1 / (float)lakePolygon.traingleDensity), 1, 100);
            lakePolygon.uvScale = EditorGUILayout.FloatField("UV scale", lakePolygon.uvScale);
            EditorGUI.indentLevel--;

            GUILayout.Label("Lightning settings:", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
            lakePolygon.receiveShadows = EditorGUILayout.Toggle("Receive Shadows", lakePolygon.receiveShadows);

            lakePolygon.shadowCastingMode = (ShadowCastingMode)EditorGUILayout.EnumPopup("Shadow Casting Mode", lakePolygon.shadowCastingMode);
            EditorGUI.indentLevel--;

            if (EditorGUI.EndChangeCheck())
            {

                Undo.RecordObject(lakePolygon, "Lake changed");
                lakePolygon.GeneratePolygon();

            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            if (GUILayout.Button("Generate polygon"))
            {
                lakePolygon.GeneratePolygon();

            }
        }


        if (toolbarNew == 1)
        {
            EditorGUILayout.Space();

            PointsUI();
        }
        if (lakePolygon.toolbarInt == 2)
        {

            DrawVertexColorsUI();

        }
        if (lakePolygon.toolbarInt == 3)
        {

            DrawFlowColorsUI();

        }
        if (lakePolygon.toolbarInt == 4)
        {
            EditorGUILayout.HelpBox("\nSet 1 point and R.A.M will generate lake.\n", MessageType.Info);
            EditorGUILayout.Space();
            lakePolygon.angleSimulation = EditorGUILayout.IntSlider("Angle", lakePolygon.angleSimulation, 1, 180);
            lakePolygon.closeDistanceSimulation = EditorGUILayout.FloatField("Point distance", lakePolygon.closeDistanceSimulation);
            lakePolygon.checkDistanceSimulation = EditorGUILayout.FloatField("Check distance", lakePolygon.checkDistanceSimulation);
            lakePolygon.removeFirstPointSimulation = EditorGUILayout.Toggle("Remove first point", lakePolygon.removeFirstPointSimulation);
            if (GUILayout.Button("Simulate"))
            {
                lakePolygon.Simulation();
            }
            if (GUILayout.Button("Remove points except first"))
            {
                lakePolygon.RemovePoints(0);
                lakePolygon.meshfilter.sharedMesh = null;
            }
            if (GUILayout.Button("Remove all points"))
            {
                lakePolygon.RemovePoints();
                lakePolygon.meshfilter.sharedMesh = null;
            }

        }
        if (toolbarNew == 5)
        {
            EditorGUILayout.Space();

            Terrain terrain = Terrain.activeTerrain;

            if (terrain != null && terrain.terrainData != null)
            {

                GUILayout.Label("Terrain carve:", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();

                lakePolygon.terrainCarve = EditorGUILayout.CurveField("Terrain carve", lakePolygon.terrainCarve);
                lakePolygon.terrainSmoothMultiplier = EditorGUILayout.FloatField("Smooth", lakePolygon.terrainSmoothMultiplier);
                lakePolygon.distSmooth = EditorGUILayout.FloatField("Smooth distance", lakePolygon.distSmooth);

                lakePolygon.noiseCarve = EditorGUILayout.Toggle("Add noise", lakePolygon.noiseCarve);
                if (lakePolygon.noiseCarve)
                {
                    EditorGUI.indentLevel++;
                    lakePolygon.noiseMultiplierInside = EditorGUILayout.FloatField("Noise multiplier inside", lakePolygon.noiseMultiplierInside);
                    lakePolygon.noiseMultiplierOutside = EditorGUILayout.FloatField("Noise multiplier outside", lakePolygon.noiseMultiplierOutside);
                    lakePolygon.noiseSizeX = EditorGUILayout.FloatField("Noise scale X", lakePolygon.noiseSizeX);
                    lakePolygon.noiseSizeZ = EditorGUILayout.FloatField("Noise scale Z", lakePolygon.noiseSizeZ);
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.Space();

                if (EditorGUI.EndChangeCheck())
                {
                    if (showCarveTerrain)
                        lakePolygon.TerrainCarve(true);

                    Undo.RecordObject(lakePolygon, "Lake curve changed");
                }

                EditorGUI.indentLevel--;

                if (!showCarveTerrain)
                {
                    if (GUILayout.Button("Show Terrain Carve"))
                    {
                        showCarveTerrain = true;
                        lakePolygon.TerrainCarve(true);
                    }
                }
                else
                {
                    if (GUILayout.Button("Hide Terrain Carve"))
                    {
                        showCarveTerrain = false;

                        if (lakePolygon.meshGOs != null && lakePolygon.meshGOs.Count > 0)
                        {
                            foreach (var item in lakePolygon.meshGOs)
                            {
                                DestroyImmediate(item);
                            }
                            lakePolygon.meshGOs.Clear();
                        }
                    }
                }

                EditorGUI.BeginChangeCheck();

                EditorGUI.indentLevel++;
                lakePolygon.overrideLakeRender = EditorGUILayout.Toggle("Debug Override lake render", lakePolygon.overrideLakeRender);
                EditorGUI.indentLevel--;
                if (GUILayout.Button("Carve Terrain"))
                {
                    showCarveTerrain = false;
                    lakePolygon.TerrainCarve();
                }



                EditorGUILayout.Space();
                GUILayout.Label("Terrain paint:", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                lakePolygon.terrainPaintCarve = EditorGUILayout.CurveField("Terrain paint", lakePolygon.terrainPaintCarve);

                int splatNumber = terrain.terrainData.terrainLayers.Length;
                if (splatNumber > 0)
                {
                    string[] options = new string[splatNumber];
                    for (int i = 0; i < splatNumber; i++)
                    {
                        options[i] = i + " - ";
                        if (terrain.terrainData.terrainLayers[i].diffuseTexture != null)
                        {
                            options[i] += terrain.terrainData.terrainLayers[i].diffuseTexture.name;
                        }
                    }

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("    Splat id:");
                    lakePolygon.currentSplatMap = EditorGUILayout.Popup(lakePolygon.currentSplatMap, options);
                    EditorGUILayout.EndHorizontal();


                    lakePolygon.noisePaint = EditorGUILayout.Toggle("Add noise", lakePolygon.noisePaint);
                    if (lakePolygon.noisePaint)
                    {
                        EditorGUI.indentLevel++;
                        lakePolygon.noiseMultiplierInsidePaint = EditorGUILayout.FloatField("Noise multiplier inside", lakePolygon.noiseMultiplierInsidePaint);
                        lakePolygon.noiseMultiplierOutsidePaint = EditorGUILayout.FloatField("Noise multiplier outside", lakePolygon.noiseMultiplierOutsidePaint);
                        lakePolygon.noiseSizeXPaint = EditorGUILayout.FloatField("Noise scale X", lakePolygon.noiseSizeXPaint);
                        lakePolygon.noiseSizeZPaint = EditorGUILayout.FloatField("Noise scale Z", lakePolygon.noiseSizeZPaint);
                        EditorGUI.indentLevel--;
                    }


                    lakePolygon.mixTwoSplatMaps = EditorGUILayout.Toggle("Mix two splat maps", lakePolygon.mixTwoSplatMaps);
                    if (lakePolygon.mixTwoSplatMaps)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("    Second splat id:");
                        lakePolygon.secondSplatMap = EditorGUILayout.Popup(lakePolygon.secondSplatMap, options);
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.indentLevel--;
                    }

                    lakePolygon.addCliffSplatMap = EditorGUILayout.Toggle("Add cliff splatmap", lakePolygon.addCliffSplatMap);
                    if (lakePolygon.addCliffSplatMap)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("    Cliff splat id:");
                        lakePolygon.cliffSplatMap = EditorGUILayout.Popup(lakePolygon.cliffSplatMap, options);
                        EditorGUILayout.EndHorizontal();
                        lakePolygon.cliffAngle = EditorGUILayout.FloatField("Cliff angle", lakePolygon.cliffAngle);
                        lakePolygon.cliffBlend = EditorGUILayout.FloatField("Cliff blend", lakePolygon.cliffBlend);

                        EditorGUILayout.Space();
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("    Cliff outside splat id:");
                        lakePolygon.cliffSplatMapOutside = EditorGUILayout.Popup(lakePolygon.cliffSplatMapOutside, options);
                        EditorGUILayout.EndHorizontal();
                        lakePolygon.cliffAngleOutside = EditorGUILayout.FloatField("Cliff outside angle", lakePolygon.cliffAngleOutside);
                        lakePolygon.cliffBlendOutside = EditorGUILayout.FloatField("Cliff outside blend", lakePolygon.cliffBlendOutside);
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.Space();

                    EditorGUI.indentLevel--;

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(lakePolygon, "Lake curve changed");
                        foreach (var meshGO in lakePolygon.meshGOs)
                        {
                            if (meshGO != null)
                            {
                                if (lakePolygon.overrideLakeRender)
                                    meshGO.GetComponent<MeshRenderer>().sharedMaterial.renderQueue = 5000;
                                else
                                    meshGO.GetComponent<MeshRenderer>().sharedMaterial.renderQueue = 2980;
                            }
                        }
                    }

                    if (GUILayout.Button("Paint Terrain"))
                    {
                        lakePolygon.TerrainPaint();
                    }
                }
                else
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("Terrain has no splatmaps.", MessageType.Info);

                }
                EditorGUILayout.Space();
                GUILayout.Label("Terrain clear foliage:", EditorStyles.boldLabel);

                EditorGUI.indentLevel++;
                lakePolygon.distanceClearFoliage = EditorGUILayout.FloatField("Remove Details Distance", lakePolygon.distanceClearFoliage);


                if (GUILayout.Button("Remove Details Foliage"))
                {
                    showCarveTerrain = false;
                    lakePolygon.TerrainClearTrees();
                }

                lakePolygon.distanceClearFoliageTrees = EditorGUILayout.FloatField("Remove Trees Distance", lakePolygon.distanceClearFoliageTrees);


                if (GUILayout.Button("Remove Trees"))
                {
                    showCarveTerrain = false;
                    lakePolygon.TerrainClearTrees(false);
                }

                EditorGUI.indentLevel--;


            }
            else
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("No Terrain On Scene.", MessageType.Info);

            }
        }
        if (lakePolygon.toolbarInt == 6)
        {
            FilesManager();
        }
        if (toolbarNew == 7)
        {
            EditorGUILayout.Space();
            Tips();
        }

        if (toolbarNew == 8)
        {
            toolbarNew = lakePolygon.toolbarInt;
            string[] guids1 = AssetDatabase.FindAssets("River Auto and Lava Volcano Environment Manual 2019");
            Application.OpenURL("file:///" + Application.dataPath.Replace("Assets", "") + AssetDatabase.GUIDToAssetPath(guids1[0]));
        }
        if (toolbarNew == 9)
        {
            toolbarNew = lakePolygon.toolbarInt;
            Application.OpenURL("https://www.youtube.com/playlist?list=PLWMxYDHySK5PkIlklmHKLYvRWK2sjDYXX");


        }
        if (toolbarNew == 10)
        {
#if VEGETATION_STUDIO
        EditorGUILayout.Space();
        GUILayout.Label("Vegetation Studio: ", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUI.BeginChangeCheck();
        lakePolygon.vegetationMaskResolution = EditorGUILayout.Slider("Mask Resolution", lakePolygon.vegetationMaskResolution, 0.1f, 1);
        lakePolygon.vegetationMaskPerimeter = EditorGUILayout.FloatField("Vegetation Mask Perimeter", lakePolygon.vegetationMaskPerimeter);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(lakePolygon, "Lake curve changed");
            RegenerateVegetationMask();
        }
        EditorGUI.indentLevel--;
        if (lakePolygon.vegetationMaskArea == null && GUILayout.Button("Add Vegetation Mask Area"))
        {
            lakePolygon.vegetationMaskArea = lakePolygon.gameObject.AddComponent<VegetationMaskArea>();
            RegenerateVegetationMask();
        }
        if (lakePolygon.vegetationMaskArea != null && GUILayout.Button("Calculate hull outline"))
        {

            RegenerateVegetationMask();
        }
#endif

#if VEGETATION_STUDIO_PRO
            EditorGUILayout.Space();
            GUILayout.Label("Vegetation Studio Pro: ", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            lakePolygon.vegetationMaskSize = EditorGUILayout.FloatField("Vegetation Mask Size", lakePolygon.vegetationMaskSize);
            lakePolygon.vegetationBlendDistance = EditorGUILayout.FloatField("Vegetation Blend Distance", lakePolygon.vegetationBlendDistance);
            lakePolygon.biomMaskResolution = EditorGUILayout.Slider("Mask Resolution", lakePolygon.biomMaskResolution, 0.1f, 1);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(lakePolygon, "Lake curve changed");
                RegenerateBiomMask();
            }
            if (lakePolygon.biomeMaskArea != null)
                lakePolygon.refreshMask = EditorGUILayout.Toggle("Auto Refresh Biome Mask", lakePolygon.refreshMask);

            if (GUILayout.Button("Add Vegetation Biome Mask Area"))
            {
                lakePolygon.GeneratePolygon();

                if (lakePolygon.biomeMaskArea == null)
                {
                    GameObject maskObject = new GameObject("MyMask");
                    maskObject.transform.SetParent(lakePolygon.transform);
                    maskObject.transform.localPosition = Vector3.zero;

                    lakePolygon.biomeMaskArea = maskObject.AddComponent<BiomeMaskArea>();
                }

                if (lakePolygon.biomeMaskArea == null)
                    return;

                RegenerateBiomMask(false);
            }

#endif

        }

        lakePolygon.toolbarInt = toolbarNew;


        EditorGUILayout.Space();


    }

    void Tips()
    {
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("\nReflections - Use box projection in reflection probes to get proper render even at river and lake conection.\n", MessageType.Info);
        EditorGUILayout.HelpBox("\nUse low resolution reflection probes, and only around the water. " + "\nFar clip planes also should be short, you probably only need colors from the surounding world.\n", MessageType.Info);
        EditorGUILayout.HelpBox("\nPut reflection probes behind, in and after dark area (tunel, cave) so you will get exelent result in lighting and reflections.\n", MessageType.Info);


        EditorGUILayout.Space();
    }

    void ResetMaterial()
    {
        lakePolygon.showFlowMap = false;
        lakePolygon.showVertexColors = false;
    }

    void DrawVertexColorsUI()
    {
        EditorGUI.BeginChangeCheck();
        lakePolygon.drawOnMesh = true;
        if (lakePolygon.drawOnMesh)
        {


            EditorGUILayout.HelpBox("R - Slow Water G - Small Cascade B - Big Cascade A - Opacity", MessageType.Info);
            EditorGUILayout.Space();
            lakePolygon.drawColor = EditorGUILayout.ColorField("Draw color", lakePolygon.drawColor);

            lakePolygon.opacity = EditorGUILayout.FloatField("Opacity", lakePolygon.opacity);
            lakePolygon.drawSize = EditorGUILayout.FloatField("Size", lakePolygon.drawSize);
            if (lakePolygon.drawSize < 0)
            {
                lakePolygon.drawSize = 0;
            }

            lakePolygon.drawColorR = EditorGUILayout.Toggle("Draw R", lakePolygon.drawColorR);
            lakePolygon.drawColorG = EditorGUILayout.Toggle("Draw G", lakePolygon.drawColorG);
            lakePolygon.drawColorB = EditorGUILayout.Toggle("Draw B", lakePolygon.drawColorB);
            lakePolygon.drawColorA = EditorGUILayout.Toggle("Draw A", lakePolygon.drawColorA);


            EditorGUILayout.Space();
            lakePolygon.drawOnMultiple = EditorGUILayout.Toggle("Draw on multiple rivers", lakePolygon.drawOnMultiple);
        }

        EditorGUILayout.Space();
        if (!lakePolygon.showVertexColors)
        {
            if (GUILayout.Button("Show vertex colors"))
            {

                if (!lakePolygon.showFlowMap && !lakePolygon.showVertexColors)
                    lakePolygon.oldMaterial = lakePolygon.GetComponent<MeshRenderer>().sharedMaterial;
                ResetMaterial();
                lakePolygon.GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("NatureManufacture Shaders/Debug/Vertex color"));
                lakePolygon.showVertexColors = true;
            }
        }
        else
        {
            if (GUILayout.Button("Hide vertex colors"))
            {
                ResetMaterial();
                lakePolygon.GetComponent<MeshRenderer>().sharedMaterial = lakePolygon.oldMaterial;
                lakePolygon.showVertexColors = false;
            }
        }

        if (GUILayout.Button("Reset vertex colors") && EditorUtility.DisplayDialog("Reset vertex colors?",
                "Are you sure you want to reset f vertex colors?", "Yes", "No"))
        {
            lakePolygon.colors = null;
            lakePolygon.GeneratePolygon();
#if VEGETATION_STUDIO_PRO
            RegenerateBiomMask();
#endif
        }

        if (EditorGUI.EndChangeCheck())
        {

            Undo.RecordObject(lakePolygon, "Lake changed");

        }
    }

    void DrawFlowColorsUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Sharp gradient could generate bugged effect. Keep flow changes smooth.", MessageType.Info);

        GUILayout.Label("Flow Map Manual: ", EditorStyles.boldLabel);
        lakePolygon.drawOnMeshFlowMap = true;
        if (lakePolygon.drawOnMeshFlowMap)
        {

            EditorGUILayout.Space();
            lakePolygon.flowSpeed = EditorGUILayout.Slider("Flow U Speed", lakePolygon.flowSpeed, -1, 1);
            lakePolygon.flowDirection = EditorGUILayout.Slider("Flow V Speed", lakePolygon.flowDirection, -1, 1);
            lakePolygon.opacity = EditorGUILayout.FloatField("Opacity", lakePolygon.opacity);
            lakePolygon.drawSize = EditorGUILayout.FloatField("Size", lakePolygon.drawSize);
            if (lakePolygon.drawSize < 0)
            {
                lakePolygon.drawSize = 0;
            }

            EditorGUILayout.Space();
            lakePolygon.drawOnMultiple = EditorGUILayout.Toggle("Draw on multiple rivers", lakePolygon.drawOnMultiple);
        }

        EditorGUILayout.Space();
        if (!lakePolygon.showFlowMap)
        {



            if (GUILayout.Button("Show flow directions"))
            {
                if (!lakePolygon.showFlowMap && !lakePolygon.showVertexColors)
                    lakePolygon.oldMaterial = lakePolygon.GetComponent<MeshRenderer>().sharedMaterial;
                ResetMaterial();
                lakePolygon.GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("NatureManufacture Shaders/Debug/Flowmap Direction"));
                lakePolygon.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_Direction", Resources.Load<Texture2D>("Debug_Arrow"));



                lakePolygon.showFlowMap = true;
            }
            if (GUILayout.Button("Show flow smoothness"))
            {
                if (!lakePolygon.showFlowMap && !lakePolygon.showVertexColors)
                    lakePolygon.oldMaterial = lakePolygon.GetComponent<MeshRenderer>().sharedMaterial;
                ResetMaterial();
                lakePolygon.GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("NatureManufacture Shaders/Debug/FlowMapUV4"));
                lakePolygon.showFlowMap = true;
            }
        }

        if (lakePolygon.showFlowMap)
        {

            if (GUILayout.Button("Hide flow"))
            {
                ResetMaterial();
                lakePolygon.GetComponent<MeshRenderer>().sharedMaterial = lakePolygon.oldMaterial;
            }
        }

        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();
        GUILayout.Label("Flow Map Automatic: ", EditorStyles.boldLabel);
        lakePolygon.automaticFlowMapScale = EditorGUILayout.FloatField("Automatic speed", lakePolygon.automaticFlowMapScale);
        lakePolygon.noiseflowMap = EditorGUILayout.Toggle("Add noise", lakePolygon.noiseflowMap);
        if (lakePolygon.noiseflowMap)
        {
            EditorGUI.indentLevel++;
            lakePolygon.noiseMultiplierflowMap = EditorGUILayout.FloatField("Noise multiplier inside", lakePolygon.noiseMultiplierflowMap);
            lakePolygon.noiseSizeXflowMap = EditorGUILayout.FloatField("Noise scale X", lakePolygon.noiseSizeXflowMap);
            lakePolygon.noiseSizeZflowMap = EditorGUILayout.FloatField("Noise scale Z", lakePolygon.noiseSizeZflowMap);
            EditorGUI.indentLevel--;
        }
        if (GUILayout.Button("Reset flow to automatic") && EditorUtility.DisplayDialog("Reset flow to automatic?",
                "Are you sure you want to reset flow to automatic?", "Yes", "No"))
        {
            lakePolygon.overrideFlowMap = false;
            lakePolygon.GeneratePolygon();
#if VEGETATION_STUDIO_PRO
            RegenerateBiomMask();
#endif
        }
        EditorGUILayout.Space();
        GUILayout.Label("Flow Map Physic: ", EditorStyles.boldLabel);
        lakePolygon.floatSpeed = EditorGUILayout.FloatField("Float speed", lakePolygon.floatSpeed);

        if (EditorGUI.EndChangeCheck())
        {

            Undo.RecordObject(lakePolygon, "Lake changed");
            if (!lakePolygon.overrideFlowMap)
                lakePolygon.GeneratePolygon();

        }
    }


    void PointsUI()
    {
        EditorGUI.BeginChangeCheck();
        if (GUILayout.Button(new GUIContent("Remove all points", "Removes all points")))
        {
            lakePolygon.RemovePoints();
            lakePolygon.meshfilter.sharedMesh = null;

        }

        for (int i = 0; i < lakePolygon.points.Count; i++)
        {

            GUILayout.Label("Point: " + i.ToString(), EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            lakePolygon.points[i] = EditorGUILayout.Vector3Field("", lakePolygon.points[i]);
            if (GUILayout.Button(new GUIContent("A", "Add point after this point"), GUILayout.MaxWidth(20)))
            {

                lakePolygon.AddPointAfter(i);
                lakePolygon.GeneratePolygon();
            }
            if (GUILayout.Button(new GUIContent("R", "Remove this Point"), GUILayout.MaxWidth(20)))
            {

                lakePolygon.RemovePoint(i);
                lakePolygon.GeneratePolygon();
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


            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
        }

        if (EditorGUI.EndChangeCheck())
        {

            Undo.RecordObject(lakePolygon, "Lake changed");
            lakePolygon.GeneratePolygon();

        }

    }

#if VEGETATION_STUDIO_PRO
    void RegenerateBiomMask(bool checkAuto = true)
    {
        if (checkAuto && !lakePolygon.refreshMask)
            return;

        if (lakePolygon.biomeMaskArea == null)
            return;

        lakePolygon.biomeMaskArea.BiomeType = BiomeType.Underwater;

        List<Vector3> worldspacePointList = new List<Vector3>();
        for (int i = 0; i < lakePolygon.splinePoints.Count; i += (int)(1 / (float)lakePolygon.biomMaskResolution))
        {
            Vector3 position = lakePolygon.transform.TransformPoint(lakePolygon.splinePoints[i])
    + (lakePolygon.transform.TransformPoint(lakePolygon.splinePoints[i]) - lakePolygon.transform.position).normalized * lakePolygon.vegetationMaskSize;

            worldspacePointList.Add(position);
        }


        lakePolygon.biomeMaskArea.ClearNodes();

        for (var i = 0; i <= worldspacePointList.Count - 1; i++)
        {
            lakePolygon.biomeMaskArea.AddNodeToEnd(worldspacePointList[i]);
        }

        //these have default values but you can set them if you want a different default setting
        lakePolygon.biomeMaskArea.BlendDistance = lakePolygon.vegetationBlendDistance;
        lakePolygon.biomeMaskArea.NoiseScale = 5;
        lakePolygon.biomeMaskArea.UseNoise = true;

        //These 3 curves holds the blend curves for vegetation and textures. they have default values;
        //biomeMaskArea.BlendCurve;
        //biomeMaskArea.InverseBlendCurve;
        //biomeMaskArea.TextureBlendCurve;

        if (lakePolygon.currentProfile != null)
        {
            lakePolygon.biomeMaskArea.BiomeType = (BiomeType)lakePolygon.currentProfile.biomeType;

        }
        else
            lakePolygon.biomeMaskArea.BiomeType = BiomeType.River;

        lakePolygon.biomeMaskArea.UpdateBiomeMask();

    }
#endif

#if VEGETATION_STUDIO
    private void RegenerateVegetationMask()
    {
        if (lakePolygon.vegetationMaskArea == null)
            return;

        lakePolygon.vegetationMaskArea.AdditionalGrassPerimiterMax = lakePolygon.vegetationMaskPerimeter;
        lakePolygon.vegetationMaskArea.AdditionalLargeObjectPerimiterMax = lakePolygon.vegetationMaskPerimeter;
        lakePolygon.vegetationMaskArea.AdditionalObjectPerimiterMax = lakePolygon.vegetationMaskPerimeter;
        lakePolygon.vegetationMaskArea.AdditionalPlantPerimiterMax = lakePolygon.vegetationMaskPerimeter;
        lakePolygon.vegetationMaskArea.AdditionalTreePerimiterMax = lakePolygon.vegetationMaskPerimeter;
        lakePolygon.vegetationMaskArea.GenerateHullNodes(lakePolygon.vegetationMaskArea.ReductionTolerance);

        lakePolygon.GeneratePolygon();
        List<Vector3> worldspacePointList = new List<Vector3>();
        for (int i = 0; i < lakePolygon.splinePoints.Count; i += (int)(1 / (float)lakePolygon.vegetationMaskResolution))
        {
            Vector3 position = lakePolygon.transform.TransformPoint(lakePolygon.splinePoints[i])
        + (lakePolygon.transform.TransformPoint(lakePolygon.splinePoints[i]) - lakePolygon.transform.position).normalized * lakePolygon.vegetationMaskPerimeter;

            worldspacePointList.Add(position);
        }


        lakePolygon.vegetationMaskArea.ClearNodes();

        for (var i = 0; i <= worldspacePointList.Count - 1; i++)
        {
            lakePolygon.vegetationMaskArea.AddNodeToEnd(worldspacePointList[i]);
        }
        lakePolygon.vegetationMaskArea.UpdateVegetationMask();
    }
#endif

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





    void DrawOnVertexColors()
    {

        if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
        {
            Undo.RegisterCompleteObjectUndo(lakePolygon, "Painted");
        }

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        //  Camera sceneCamera = SceneView.lastActiveSceneView.camera;
        HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        // Vector2 mousePos = Event.current.mousePosition;
        // mousePos.y = Screen.height - mousePos.y - 40;
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);// sceneCamera.ScreenPointToRay(mousePos);



        List<MeshCollider> meshColliders = new List<MeshCollider>();
        foreach (var item in lakes)
        {
            meshColliders.Add(item.gameObject.AddComponent<MeshCollider>());
        }


        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

        GameObject go = null;
        Vector3 hitPosition = Vector3.zero;
        Vector3 hitNormal = Vector3.zero;
        LakePolygon hitedSpline = null;
        if (hits.Length > 0)
        {

            foreach (var hit in hits)
            {
                if (hit.collider is MeshCollider)
                {
                    go = hit.collider.gameObject;
                    hitedSpline = go.GetComponent<LakePolygon>();

                    if (hitedSpline != null && (lakePolygon.drawOnMultiple || hitedSpline == lakePolygon))
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
            Handles.color = new Color(lakePolygon.drawColor.r, lakePolygon.drawColor.g, lakePolygon.drawColor.b, 1);
            Handles.DrawLine(hitPosition, hitPosition + hitNormal * 2);
            Handles.CircleHandleCap(
                0,
                hitPosition,
                Quaternion.LookRotation(hitNormal),
                lakePolygon.drawSize,
                EventType.Repaint
            );
            Handles.color = Color.black;
            Handles.CircleHandleCap(
                0,
                hitPosition,
                Quaternion.LookRotation(hitNormal),
                lakePolygon.drawSize - 0.1f,
                EventType.Repaint
            );


            if (!(Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) || Event.current.button != 0)
                return;


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
                            if (lakePolygon.drawColorR)
                                colors[i].r = Mathf.Lerp(colors[i].r, 0, lakePolygon.opacity);
                            if (lakePolygon.drawColorG)
                                colors[i].g = Mathf.Lerp(colors[i].g, 0, lakePolygon.opacity);
                            if (lakePolygon.drawColorB)
                                colors[i].b = Mathf.Lerp(colors[i].b, 0, lakePolygon.opacity);
                            if (lakePolygon.drawColorA)
                                colors[i].a = Mathf.Lerp(colors[i].a, 1, lakePolygon.opacity);
                        }
                        else
                        {
                            if (lakePolygon.drawColorR)
                                colors[i].r = Mathf.Lerp(colors[i].r, lakePolygon.drawColor.r, lakePolygon.opacity);
                            if (lakePolygon.drawColorG)
                                colors[i].g = Mathf.Lerp(colors[i].g, lakePolygon.drawColor.g, lakePolygon.opacity);
                            if (lakePolygon.drawColorB)
                                colors[i].b = Mathf.Lerp(colors[i].b, lakePolygon.drawColor.b, lakePolygon.opacity);
                            if (lakePolygon.drawColorA)
                                colors[i].a = Mathf.Lerp(colors[i].a, lakePolygon.drawColor.a, lakePolygon.opacity);
                        }

                    }
                }

                mesh.colors = colors;
                meshFilter.sharedMesh = mesh;
            }
        }
    }

    void DrawOnFlowMap()
    {
        if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
        {
            Undo.RegisterCompleteObjectUndo(lakePolygon, "Painted");
        }
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        //Camera sceneCamera = SceneView.lastActiveSceneView.camera;
        // Vector2 mousePos = Event.current.mousePosition;
        // mousePos.y = Screen.height - mousePos.y - 40;

        // Ray ray = sceneCamera.ScreenPointToRay(mousePos);

        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        List<MeshCollider> meshColliders = new List<MeshCollider>();
        foreach (var item in lakes)
        {
            meshColliders.Add(item.gameObject.AddComponent<MeshCollider>());
        }


        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

        GameObject go = null;
        Vector3 hitPosition = Vector3.zero;
        Vector3 hitNormal = Vector3.zero;
        LakePolygon hitedSpline = null;
        if (hits.Length > 0)
        {

            foreach (var hit in hits)
            {
                if (hit.collider is MeshCollider)
                {
                    go = hit.collider.gameObject;
                    hitedSpline = go.GetComponent<LakePolygon>();


                    if (hitedSpline != null && (lakePolygon.drawOnMultiple || hitedSpline == lakePolygon))
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

            Handles.color = new Color(lakePolygon.flowDirection, lakePolygon.flowSpeed, 0, 1);
            Handles.DrawLine(hitPosition, hitPosition + hitNormal * 2);
            Handles.CircleHandleCap(
                0,
                hitPosition,
                Quaternion.LookRotation(hitNormal),
                lakePolygon.drawSize,
                EventType.Repaint
            );
            Handles.color = Color.black;
            Handles.CircleHandleCap(
                0,
                hitPosition,
                Quaternion.LookRotation(hitNormal),
                lakePolygon.drawSize - 0.1f,
                EventType.Repaint
            );

            if (!(Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) || Event.current.button != 0)
                return;


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
                    if (dist < lakePolygon.drawSize)
                    {
                        distValue = (lakePolygon.drawSize - dist) / (float)lakePolygon.drawSize;
                        if (Event.current.shift)
                        {
                            colorsFlowMap[i] = Vector2.Lerp(colorsFlowMap[i], new Vector2(0, 0), lakePolygon.opacity);

                        }
                        else
                        {
                            colorsFlowMap[i] = Vector2.Lerp(colorsFlowMap[i], new Vector2(lakePolygon.flowDirection, lakePolygon.flowSpeed), lakePolygon.opacity * distValue);

                        }

                    }
                }

                mesh.uv4 = colorsFlowMap.ToArray();
                hitedSpline.colorsFlowMap = colorsFlowMap;
                meshFilter.sharedMesh = mesh;
            }

        }
    }



    protected virtual void OnSceneGUI()
    {
        if (lakePolygon == null)
            lakePolygon = (LakePolygon)target;

        Color baseColor = Handles.color;
        int controlId = GUIUtility.GetControlID(FocusType.Passive);



        if (lakePolygon != null)


            if (lakePolygon.drawOnMesh || lakePolygon.drawOnMeshFlowMap)
            {
                Tools.current = Tool.None;
                if (lakePolygon.meshfilter != null)
                {
                    Handles.color = Color.magenta;
                    Vector3[] vertices = lakePolygon.meshfilter.sharedMesh.vertices;
                    Vector2[] uv4 = lakePolygon.meshfilter.sharedMesh.uv4;
                    Vector3[] normals = lakePolygon.meshfilter.sharedMesh.normals;
                    Quaternion up = Quaternion.Euler(90, 0, 0);
                    for (int i = 0; i < vertices.Length; i += 5)
                    {
                        Vector3 item = vertices[i];
                        Vector3 handlePos = lakePolygon.transform.TransformPoint(item);

                        if (lakePolygon.drawOnMesh)
                            Handles.RectangleHandleCap(0, handlePos, up, 0.05f, EventType.Repaint);
                    }

                }
                if (lakePolygon.drawOnMesh)
                    DrawOnVertexColors();
                else
                    DrawOnFlowMap();
                return;
            }

        if (lakePolygon.lockHeight && lakePolygon.points.Count > 1)
        {
            for (int i = 1; i < lakePolygon.points.Count; i++)
            {
                Vector3 vec = lakePolygon.points[i];
                vec.y = lakePolygon.points[0].y;
                lakePolygon.points[i] = vec;
            }

        }
        {

            Vector3[] points = new Vector3[lakePolygon.splinePoints.Count];


            for (int i = 0; i < lakePolygon.splinePoints.Count; i++)
            {
                points[i] = lakePolygon.splinePoints[i] + lakePolygon.transform.position;
            }


            Handles.color = Color.white;
            if (lakePolygon.points.Count > 1)
                Handles.DrawPolyLine(points);

            if (Event.current.commandName == "UndoRedoPerformed")
            {

                lakePolygon.GeneratePolygon();
                return;
            }

            if (selectedPosition >= 0 && selectedPosition < lakePolygon.points.Count)
            {
                Handles.color = Color.red;
                Handles.SphereHandleCap(0, (Vector3)lakePolygon.points[selectedPosition] + lakePolygon.transform.position, Quaternion.identity, 1, EventType.Repaint);

            }


            int controlPointToDelete = -1;

            for (int j = 0; j < lakePolygon.points.Count; j++)
            {

                EditorGUI.BeginChangeCheck();

                Vector3 handlePos = (Vector3)lakePolygon.points[j] + lakePolygon.transform.position;

                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.red;

                Vector3 screenPoint = Camera.current.WorldToScreenPoint(handlePos);

                if (screenPoint.z > 0)
                {

                    Handles.Label(handlePos + Vector3.up * HandleUtility.GetHandleSize(handlePos), "Point: " + j.ToString(), style);

                }

                if (Event.current.control && Event.current.shift)
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
                        Handles.SphereHandleCap(id, (Vector3)lakePolygon.points[j] + lakePolygon.transform.position, Quaternion.identity, size, EventType.Repaint);
                    }
                    else if (Event.current.type == EventType.Layout)
                    {
                        Handles.SphereHandleCap(id, (Vector3)lakePolygon.points[j] + lakePolygon.transform.position, Quaternion.identity, size, EventType.Layout);
                    }

                }
                else if (Tools.current == Tool.Move)
                {

                    float size = 0.6f;
                    size = HandleUtility.GetHandleSize(handlePos) * size;

                    Handles.color = Handles.xAxisColor;
                    Vector3 pos = Handles.Slider((Vector3)lakePolygon.points[j] + lakePolygon.transform.position, Vector3.right, size, Handles.ArrowHandleCap, 0.01f) - lakePolygon.transform.position;
                    if (!lakePolygon.lockHeight || lakePolygon.points.Count == 1)
                    {
                        Handles.color = Handles.yAxisColor;

                        pos = Handles.Slider((Vector3)pos + lakePolygon.transform.position, Vector3.up, size, Handles.ArrowHandleCap, 0.01f) - lakePolygon.transform.position;
                    }
                    Handles.color = Handles.zAxisColor;
                    pos = Handles.Slider((Vector3)pos + lakePolygon.transform.position, Vector3.forward, size, Handles.ArrowHandleCap, 0.01f) - lakePolygon.transform.position;

                    Vector3 halfPos = (Vector3.right + Vector3.forward) * size * 0.3f;
                    Handles.color = Handles.yAxisColor;
                    pos = Handles.Slider2D((Vector3)pos + lakePolygon.transform.position + halfPos, Vector3.up, Vector3.right, Vector3.forward, size * 0.3f, Handles.RectangleHandleCap, 0.01f) - lakePolygon.transform.position - halfPos;
                    halfPos = (Vector3.right + Vector3.up) * size * 0.3f;

                    if (!lakePolygon.lockHeight || lakePolygon.points.Count == 1)
                    {
                        Handles.color = Handles.zAxisColor;
                        pos = Handles.Slider2D((Vector3)pos + lakePolygon.transform.position + halfPos, Vector3.forward, Vector3.right, Vector3.up, size * 0.3f, Handles.RectangleHandleCap, 0.01f) - lakePolygon.transform.position - halfPos;
                        halfPos = (Vector3.up + Vector3.forward) * size * 0.3f;
                        Handles.color = Handles.xAxisColor;
                        pos = Handles.Slider2D((Vector3)pos + lakePolygon.transform.position + halfPos, Vector3.right, Vector3.up, Vector3.forward, size * 0.3f, Handles.RectangleHandleCap, 0.01f) - lakePolygon.transform.position - halfPos;
                    }

                    lakePolygon.points[j] = pos;


                }


                if (EditorGUI.EndChangeCheck())
                {

                    Undo.RecordObject(lakePolygon, "Change Position");
                    lakePolygon.GeneratePolygon();
#if VEGETATION_STUDIO
                    RegenerateVegetationMask();
#endif
#if VEGETATION_STUDIO_PRO
                    RegenerateBiomMask();
#endif

                }

            }

            if (controlPointToDelete >= 0)
            {
                Undo.RecordObject(lakePolygon, "Remove point");
                Undo.RecordObject(lakePolygon.transform, "Remove point");


                lakePolygon.RemovePoint(controlPointToDelete);

                lakePolygon.GeneratePolygon();

                GUIUtility.hotControl = controlId;
                Event.current.Use();
                HandleUtility.Repaint();
                controlPointToDelete = -1;
            }

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.control)
            {


                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Undo.RecordObject(lakePolygon, "Add point");
                    Undo.RecordObject(lakePolygon.transform, "Add point");

                    Vector3 position = hit.point - lakePolygon.transform.position;
                    lakePolygon.AddPoint(position);

                    lakePolygon.GeneratePolygon();

#if VEGETATION_STUDIO
                    RegenerateVegetationMask();
#endif
#if VEGETATION_STUDIO_PRO
                    RegenerateBiomMask();
#endif

                    GUIUtility.hotControl = controlId;
                    Event.current.Use();
                    HandleUtility.Repaint();
                }
            }

            if (!Event.current.control && Event.current.shift && lakePolygon.points.Count > 1)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    int idMin = -1;
                    float distanceMin = float.MaxValue;

                    for (int j = 0; j < lakePolygon.points.Count; j++)
                    {
                        Vector3 handlePos = (Vector3)lakePolygon.points[j] + lakePolygon.transform.position;

                        float pointDist = Vector3.Distance(hit.point, handlePos);
                        if (pointDist < distanceMin)
                        {
                            distanceMin = pointDist;
                            idMin = j;
                        }
                    }

                    Vector3 posOne = (Vector3)lakePolygon.points[idMin] + lakePolygon.transform.position;
                    Vector3 posTwo;




                    Vector3 posPrev = (Vector3)lakePolygon.points[lakePolygon.ClampListPos(idMin - 1)] + lakePolygon.transform.position;
                    Vector3 posNext = (Vector3)lakePolygon.points[lakePolygon.ClampListPos(idMin + 1)] + lakePolygon.transform.position;

                    if (Vector3.Distance(hit.point, posPrev) > Vector3.Distance(hit.point, posNext))
                        posTwo = posNext;
                    else
                    {
                        posTwo = posPrev;
                        idMin = lakePolygon.ClampListPos(idMin - 1);
                    }




                    Handles.color = Handles.xAxisColor;
                    Handles.DrawLine(hit.point, posOne);
                    Handles.DrawLine(hit.point, posTwo);

                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {

                        Undo.RecordObject(lakePolygon, "Add point");
                        Undo.RecordObject(lakePolygon.transform, "Add point");

                        Vector4 position = hit.point - lakePolygon.transform.position;
                        lakePolygon.AddPointAfter(idMin);
                        lakePolygon.ChangePointPosition(idMin + 1, position);

                        lakePolygon.GeneratePolygon();

                        GUIUtility.hotControl = controlId;
                        Event.current.Use();
                        HandleUtility.Repaint();
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

    bool CheckProfileChange()
    {
        if (lakePolygon.currentProfile == null)
            return false;

        //if (lakePolygon.terrainCarve != lakePolygon.currentProfile.terrainCarve)
        //    return true;
        if (lakePolygon.distSmooth != lakePolygon.currentProfile.distSmooth)
            return true;
        if (lakePolygon.uvScale != lakePolygon.currentProfile.uvScale)
            return true;
        if (lakePolygon.terrainSmoothMultiplier != lakePolygon.currentProfile.terrainSmoothMultiplier)
            return true;
        //if (lakePolygon.terrainPaintCarve != lakePolygon.currentProfile.terrainPaintCarve)
        //    return true;
        if (lakePolygon.currentSplatMap != lakePolygon.currentProfile.currentSplatMap)
            return true;

        if (lakePolygon.maximumTriangleSize != lakePolygon.currentProfile.maximumTriangleSize)
            return true;
        if (lakePolygon.traingleDensity != lakePolygon.currentProfile.traingleDensity)
            return true;

        if (lakePolygon.currentProfile.receiveShadows != lakePolygon.receiveShadows)
            return true;
        if (lakePolygon.currentProfile.shadowCastingMode != lakePolygon.shadowCastingMode)
            return true;



        if (lakePolygon.automaticFlowMapScale != lakePolygon.currentProfile.automaticFlowMapScale)
            return true;
        if (lakePolygon.noiseflowMap != lakePolygon.currentProfile.noiseflowMap)
            return true;
        if (lakePolygon.noiseMultiplierflowMap != lakePolygon.currentProfile.noiseMultiplierflowMap)
            return true;
        if (lakePolygon.noiseSizeXflowMap != lakePolygon.currentProfile.noiseSizeXflowMap)
            return true;
        if (lakePolygon.noiseSizeZflowMap != lakePolygon.currentProfile.noiseSizeZflowMap)
            return true;


        if (lakePolygon.currentProfile.noisePaint != lakePolygon.noisePaint)
            return true;
        if (lakePolygon.currentProfile.noiseMultiplierInsidePaint != lakePolygon.noiseMultiplierInsidePaint)
            return true;
        if (lakePolygon.currentProfile.noiseMultiplierOutsidePaint != lakePolygon.noiseMultiplierOutsidePaint)
            return true;
        if (lakePolygon.currentProfile.noiseSizeXPaint != lakePolygon.noiseSizeXPaint)
            return true;
        if (lakePolygon.currentProfile.noiseSizeZPaint != lakePolygon.noiseSizeZPaint)
            return true;
        if (lakePolygon.currentProfile.mixTwoSplatMaps != lakePolygon.mixTwoSplatMaps)
            return true;
        if (lakePolygon.currentProfile.secondSplatMap != lakePolygon.secondSplatMap)
            return true;
        if (lakePolygon.currentProfile.addCliffSplatMap != lakePolygon.addCliffSplatMap)
            return true;
        if (lakePolygon.currentProfile.cliffSplatMap != lakePolygon.cliffSplatMap)
            return true;
        if (lakePolygon.currentProfile.cliffAngle != lakePolygon.cliffAngle)
            return true;
        if (lakePolygon.currentProfile.cliffBlend != lakePolygon.cliffBlend)
            return true;

        if (lakePolygon.currentProfile.cliffSplatMapOutside != lakePolygon.cliffSplatMapOutside)
            return true;
        if (lakePolygon.currentProfile.cliffAngleOutside != lakePolygon.cliffAngleOutside)
            return true;
        if (lakePolygon.currentProfile.cliffBlendOutside != lakePolygon.cliffBlendOutside)
            return true;


        if (lakePolygon.currentProfile.distanceClearFoliage != lakePolygon.distanceClearFoliage)
            return true;
        if (lakePolygon.currentProfile.distanceClearFoliageTrees != lakePolygon.distanceClearFoliageTrees)
            return true;


        return false;
    }

    public void ResetToProfile()
    {
        if (lakePolygon == null)
            lakePolygon = (LakePolygon)target;

        MeshRenderer ren = lakePolygon.GetComponent<MeshRenderer>();
        ren.sharedMaterial = lakePolygon.currentProfile.lakeMaterial;

        lakePolygon.terrainCarve = new AnimationCurve(lakePolygon.currentProfile.terrainCarve.keys);
        lakePolygon.terrainPaintCarve = new AnimationCurve(lakePolygon.currentProfile.terrainPaintCarve.keys);

        // lakePolygon.terrainCarve = lakePolygon.currentProfile.terrainCarve;
        lakePolygon.distSmooth = lakePolygon.currentProfile.distSmooth;
        lakePolygon.uvScale = lakePolygon.currentProfile.uvScale;
        lakePolygon.terrainSmoothMultiplier = lakePolygon.currentProfile.terrainSmoothMultiplier;
        // lakePolygon.terrainPaintCarve = lakePolygon.currentProfile.terrainPaintCarve;
        lakePolygon.currentSplatMap = lakePolygon.currentProfile.currentSplatMap;

        lakePolygon.maximumTriangleSize = lakePolygon.currentProfile.maximumTriangleSize;
        lakePolygon.traingleDensity = lakePolygon.currentProfile.traingleDensity;

        lakePolygon.receiveShadows = lakePolygon.currentProfile.receiveShadows;
        lakePolygon.shadowCastingMode = lakePolygon.currentProfile.shadowCastingMode;

        lakePolygon.automaticFlowMapScale = lakePolygon.currentProfile.automaticFlowMapScale;

        lakePolygon.noiseflowMap = lakePolygon.currentProfile.noiseflowMap;
        lakePolygon.noiseMultiplierflowMap = lakePolygon.currentProfile.noiseMultiplierflowMap;
        lakePolygon.noiseSizeXflowMap = lakePolygon.currentProfile.noiseSizeXflowMap;
        lakePolygon.noiseSizeZflowMap = lakePolygon.currentProfile.noiseSizeZflowMap;



        lakePolygon.noiseCarve = lakePolygon.currentProfile.noiseCarve;
        lakePolygon.noiseMultiplierInside = lakePolygon.currentProfile.noiseMultiplierInside;
        lakePolygon.noiseMultiplierOutside = lakePolygon.currentProfile.noiseMultiplierOutside;
        lakePolygon.noiseSizeX = lakePolygon.currentProfile.noiseSizeX;
        lakePolygon.noiseSizeZ = lakePolygon.currentProfile.noiseSizeZ;


        lakePolygon.noisePaint = lakePolygon.currentProfile.noisePaint;
        lakePolygon.noiseMultiplierInsidePaint = lakePolygon.currentProfile.noiseMultiplierInsidePaint;
        lakePolygon.noiseMultiplierOutsidePaint = lakePolygon.currentProfile.noiseMultiplierOutsidePaint;
        lakePolygon.noiseSizeXPaint = lakePolygon.currentProfile.noiseSizeXPaint;
        lakePolygon.noiseSizeZPaint = lakePolygon.currentProfile.noiseSizeZPaint;
        lakePolygon.mixTwoSplatMaps = lakePolygon.currentProfile.mixTwoSplatMaps;
        lakePolygon.secondSplatMap = lakePolygon.currentProfile.secondSplatMap;
        lakePolygon.addCliffSplatMap = lakePolygon.currentProfile.addCliffSplatMap;
        lakePolygon.cliffSplatMap = lakePolygon.currentProfile.cliffSplatMap;
        lakePolygon.cliffAngle = lakePolygon.currentProfile.cliffAngle; ;
        lakePolygon.cliffBlend = lakePolygon.currentProfile.cliffBlend;

        lakePolygon.cliffSplatMapOutside = lakePolygon.currentProfile.cliffSplatMapOutside;
        lakePolygon.cliffAngleOutside = lakePolygon.currentProfile.cliffAngleOutside;
        lakePolygon.cliffBlendOutside = lakePolygon.currentProfile.cliffBlendOutside;

        lakePolygon.distanceClearFoliage = lakePolygon.currentProfile.distanceClearFoliage;
        lakePolygon.distanceClearFoliageTrees = lakePolygon.currentProfile.distanceClearFoliageTrees;

        lakePolygon.oldProfile = lakePolygon.currentProfile;

    }

    public void PointsToFile()
    {
        var path = EditorUtility.SaveFilePanelInProject(
            "Save Lake Points",
            lakePolygon.name + "Points.csv",
            "csv",
            "Save Spline " + lakePolygon.name + " Points in CSV");

        if (string.IsNullOrEmpty(path))
            return;

        string fileData = "";

        foreach (Vector4 v in lakePolygon.points)
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
        string path = EditorUtility.OpenFilePanel("Read Lake Points from CSV", Application.dataPath, "csv");

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
        Undo.RecordObject(lakePolygon, "Lake changed");
        if (vectors.Length > 0)
        {
            foreach (var item in vectors)
            {
                lakePolygon.AddPoint(item);
            }

        }

    }

}

