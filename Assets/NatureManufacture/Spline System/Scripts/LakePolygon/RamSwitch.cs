using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RamSwitch : MonoBehaviour
{
    public RamSpline spline;
    public SplineProfile Profile;

    public void Switch()
    {
        SetProfile(Profile);
        spline.GenerateSpline();
    }

    public void SetProfile(SplineProfile splineProfile)
    {
        spline.currentProfile = splineProfile;

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

        spline.oldProfile = spline.currentProfile;


    }

}
