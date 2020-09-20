using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LakePolygonSwitch : MonoBehaviour
{
    public LakePolygon Polygon;
    public LakePolygonProfile Profile;
    public void Switch()
    {
        SetProfile(Profile);
        Polygon.GeneratePolygon();
    }

    public void SetProfile(LakePolygonProfile lakeProfile)
    {

        Polygon.currentProfile = lakeProfile;
        MeshRenderer ren = Polygon.GetComponent<MeshRenderer>();
        ren.sharedMaterial = Polygon.currentProfile.lakeMaterial;

        Polygon.terrainCarve = new AnimationCurve(Polygon.currentProfile.terrainCarve.keys);
        Polygon.terrainPaintCarve = new AnimationCurve(Polygon.currentProfile.terrainPaintCarve.keys);

        // Polygon.terrainCarve = Polygon.currentProfile.terrainCarve;
        Polygon.distSmooth = Polygon.currentProfile.distSmooth;
        Polygon.uvScale = Polygon.currentProfile.uvScale;
        Polygon.terrainSmoothMultiplier = Polygon.currentProfile.terrainSmoothMultiplier;
        // Polygon.terrainPaintCarve = Polygon.currentProfile.terrainPaintCarve;
        Polygon.currentSplatMap = Polygon.currentProfile.currentSplatMap;

        Polygon.maximumTriangleSize = Polygon.currentProfile.maximumTriangleSize;
        Polygon.traingleDensity = Polygon.currentProfile.traingleDensity;

        Polygon.receiveShadows = Polygon.currentProfile.receiveShadows;
        Polygon.shadowCastingMode = Polygon.currentProfile.shadowCastingMode;

        Polygon.automaticFlowMapScale = Polygon.currentProfile.automaticFlowMapScale;

        Polygon.noiseflowMap = Polygon.currentProfile.noiseflowMap;
        Polygon.noiseMultiplierflowMap = Polygon.currentProfile.noiseMultiplierflowMap;
        Polygon.noiseSizeXflowMap = Polygon.currentProfile.noiseSizeXflowMap;
        Polygon.noiseSizeZflowMap = Polygon.currentProfile.noiseSizeZflowMap;



        Polygon.noiseCarve = Polygon.currentProfile.noiseCarve;
        Polygon.noiseMultiplierInside = Polygon.currentProfile.noiseMultiplierInside;
        Polygon.noiseMultiplierOutside = Polygon.currentProfile.noiseMultiplierOutside;
        Polygon.noiseSizeX = Polygon.currentProfile.noiseSizeX;
        Polygon.noiseSizeZ = Polygon.currentProfile.noiseSizeZ;


        Polygon.noisePaint = Polygon.currentProfile.noisePaint;
        Polygon.noiseMultiplierInsidePaint = Polygon.currentProfile.noiseMultiplierInsidePaint;
        Polygon.noiseMultiplierOutsidePaint = Polygon.currentProfile.noiseMultiplierOutsidePaint;
        Polygon.noiseSizeXPaint = Polygon.currentProfile.noiseSizeXPaint;
        Polygon.noiseSizeZPaint = Polygon.currentProfile.noiseSizeZPaint;
        Polygon.mixTwoSplatMaps = Polygon.currentProfile.mixTwoSplatMaps;
        Polygon.secondSplatMap = Polygon.currentProfile.secondSplatMap;
        Polygon.addCliffSplatMap = Polygon.currentProfile.addCliffSplatMap;
        Polygon.cliffSplatMap = Polygon.currentProfile.cliffSplatMap;
        Polygon.cliffAngle = Polygon.currentProfile.cliffAngle; ;
        Polygon.cliffBlend = Polygon.currentProfile.cliffBlend;

        Polygon.cliffSplatMapOutside = Polygon.currentProfile.cliffSplatMapOutside;
        Polygon.cliffAngleOutside = Polygon.currentProfile.cliffAngleOutside;
        Polygon.cliffBlendOutside = Polygon.currentProfile.cliffBlendOutside;

        Polygon.distanceClearFoliage = Polygon.currentProfile.distanceClearFoliage;
        Polygon.distanceClearFoliageTrees = Polygon.currentProfile.distanceClearFoliageTrees;

        Polygon.oldProfile = Polygon.currentProfile;

    }

}
