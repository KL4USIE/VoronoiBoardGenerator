using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
#if VEGETATION_STUDIO_PRO
using AwesomeTechnologies.VegetationSystem;
#endif

[CustomEditor(typeof(SplineProfile)), CanEditMultipleObjects]
public class SplineProfileEditor : Editor
{


    public override void OnInspectorGUI()
    {
        SplineProfile spline = (SplineProfile)target;

        GUILayout.Label("Basic: ", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        spline.splineMaterial = (Material)EditorGUILayout.ObjectField("Material", spline.splineMaterial, typeof(Material), false);

        spline.meshCurve = EditorGUILayout.CurveField("Mesh curve", spline.meshCurve);

        EditorGUILayout.LabelField("Vertice distribution: " + spline.minVal.ToString() + " " + spline.maxVal.ToString());
        EditorGUILayout.MinMaxSlider(ref spline.minVal, ref spline.maxVal, 0, 1);
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

        spline.traingleDensity = 1 / (float)EditorGUILayout.IntSlider("U", (int)(1 / (float)spline.traingleDensity), 1, 100);
        spline.vertsInShape = EditorGUILayout.IntSlider("V", spline.vertsInShape - 1, 1, 20) + 1;

        spline.uvScale = EditorGUILayout.FloatField("UV scale (texture tiling)", spline.uvScale);
        spline.uvRotation = EditorGUILayout.Toggle("Rotate UV", spline.uvRotation);
        EditorGUI.indentLevel--;

        GUILayout.Label("Flow Map Automatic: ", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
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
        EditorGUI.indentLevel--;

        GUILayout.Label("Flow Map Physic: ", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        spline.floatSpeed = EditorGUILayout.FloatField("River float speed", spline.floatSpeed);
        EditorGUI.indentLevel--;

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

        GUILayout.Label("Terrain paint:", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        EditorGUI.BeginChangeCheck();
        spline.terrainPaintCarve = EditorGUILayout.CurveField("Terrain paint", spline.terrainPaintCarve);


        Terrain terrain = Terrain.activeTerrain;
        if (terrain != null)
        {
            int splatNumber = terrain.terrainData.terrainLayers.Length;
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
                GUILayout.Label("    Cliff oustide splat id:");
                spline.cliffSplatMapOutside = EditorGUILayout.Popup(spline.cliffSplatMapOutside, options);
                EditorGUILayout.EndHorizontal();
                spline.cliffAngleOutside = EditorGUILayout.FloatField("Cliff oustide angle", spline.cliffAngleOutside);
                spline.cliffBlendOutside = EditorGUILayout.FloatField("Cliff oustide blend", spline.cliffBlendOutside);
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
        }

        GUILayout.Label("Terrain clear foliage:", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        spline.distanceClearFoliage = EditorGUILayout.FloatField("Remove Details Distance", spline.distanceClearFoliage);
        spline.distanceClearFoliageTrees = EditorGUILayout.FloatField("Remove Trees Distance", spline.distanceClearFoliageTrees);
        EditorGUI.indentLevel--;

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
        EditorGUI.indentLevel--;

        GUILayout.Label("Lightning settings:", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        spline.receiveShadows = EditorGUILayout.Toggle("Receive Shadows", spline.receiveShadows);

        spline.shadowCastingMode = (ShadowCastingMode)EditorGUILayout.EnumPopup("Shadow Casting Mode", spline.shadowCastingMode);
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();
        EditorUtility.SetDirty(target);
#if VEGETATION_STUDIO_PRO
        GUILayout.Label("Vegetation stuio pro:", EditorStyles.boldLabel);
        spline.biomeType = System.Convert.ToInt32(EditorGUILayout.EnumPopup("Select biome", (BiomeType)spline.biomeType));
#else
        GUILayout.Label("Vegetation studio pro:", EditorStyles.boldLabel);
        spline.biomeType = EditorGUILayout.IntField("Select biome", spline.biomeType);
#endif

    }
}
