using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
#if VEGETATION_STUDIO_PRO
using AwesomeTechnologies.VegetationSystem;
#endif

[CustomEditor(typeof(LakePolygonProfile)), CanEditMultipleObjects]
public class LakePolygonProfileEditor : Editor
{


    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        LakePolygonProfile lakePolygon = (LakePolygonProfile)target;
        GUILayout.Label("Basic: ", EditorStyles.boldLabel);
        lakePolygon.lakeMaterial = (Material)EditorGUILayout.ObjectField("Material", lakePolygon.lakeMaterial, typeof(Material), false);

        GUILayout.Label("Mesh settings:", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        lakePolygon.maximumTriangleSize = EditorGUILayout.FloatField("Maximum triangle size", lakePolygon.maximumTriangleSize);
        lakePolygon.traingleDensity = 1 / (float)EditorGUILayout.IntSlider("Spline density", (int)(1 / (float)lakePolygon.traingleDensity), 1, 100);
        lakePolygon.uvScale = EditorGUILayout.FloatField("UV scale", lakePolygon.uvScale);



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



        GUILayout.Label("Terrain carve:", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        lakePolygon.terrainCarve = EditorGUILayout.CurveField("Terrain carve", lakePolygon.terrainCarve);
        lakePolygon.distSmooth = EditorGUILayout.FloatField("Smooth distance", lakePolygon.distSmooth);
        lakePolygon.terrainSmoothMultiplier = EditorGUILayout.FloatField("Smooth", lakePolygon.terrainSmoothMultiplier);

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

        EditorGUI.indentLevel--;

        GUILayout.Label("Terrain paint:", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        lakePolygon.terrainPaintCarve = EditorGUILayout.CurveField("Terrain paint", lakePolygon.terrainPaintCarve);

        Terrain terrain = Terrain.activeTerrain;

        if (terrain != null && terrain.terrainData != null)
        {
            int splatNumber = terrain.terrainData.terrainLayers.Length;
            if (splatNumber > 0)
            {
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

            }
        }
        EditorGUILayout.Space();
        GUILayout.Label("Terrain clear foliage:", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;
        lakePolygon.distanceClearFoliage = EditorGUILayout.FloatField("Remove Details Distance", lakePolygon.distanceClearFoliage);


        lakePolygon.distanceClearFoliageTrees = EditorGUILayout.FloatField("Remove Trees Distance", lakePolygon.distanceClearFoliageTrees);


        EditorGUI.indentLevel--;

        GUILayout.Label("Lightning settings:", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;
        lakePolygon.receiveShadows = EditorGUILayout.Toggle("Receive Shadows", lakePolygon.receiveShadows);

        lakePolygon.shadowCastingMode = (ShadowCastingMode)EditorGUILayout.EnumPopup("Shadow Casting Mode", lakePolygon.shadowCastingMode);
        EditorGUI.indentLevel--;


#if VEGETATION_STUDIO_PRO
        GUILayout.Label("Vegetation stuio pro:", EditorStyles.boldLabel);
        lakePolygon.biomeType = System.Convert.ToInt32(EditorGUILayout.EnumPopup("Select biome", (BiomeType)lakePolygon.biomeType));
#else
        GUILayout.Label("Vegetation stuio:", EditorStyles.boldLabel);
        lakePolygon.biomeType = EditorGUILayout.IntField("Select biome", lakePolygon.biomeType);
#endif

        if(EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(lakePolygon);
           // AssetDatabase.Refresh();
        }
    }
}
