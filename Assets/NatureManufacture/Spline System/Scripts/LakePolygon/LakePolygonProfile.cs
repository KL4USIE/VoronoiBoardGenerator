using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "LakePolygonProfile", menuName = "LakePolygonProfile", order = 1)]
public class LakePolygonProfile : ScriptableObject
{
    #region basic
    public Material lakeMaterial;
    public float distSmooth = 5;
    public float uvScale = 1;
    public float maximumTriangleSize = 50;
    public float traingleDensity = 0.2f;
    public bool receiveShadows = false;
    public ShadowCastingMode shadowCastingMode = ShadowCastingMode.Off;
    #endregion


    #region flowmap

    public float automaticFlowMapScale = 0.2f;

    public bool noiseflowMap = false;
    public float noiseMultiplierflowMap = 1f;
    public float noiseSizeXflowMap = 0.2f;
    public float noiseSizeZflowMap = 0.2f;

    #endregion

    #region terrain
    public AnimationCurve terrainCarve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(10, -2) });
    public float terrainSmoothMultiplier = 1;
    public AnimationCurve terrainPaintCarve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });

    public bool noiseCarve = false;
    public float noiseMultiplierInside = 1f;
    public float noiseMultiplierOutside = 0.25f;
    public float noiseSizeX = 0.2f;
    public float noiseSizeZ = 0.2f;

    public int currentSplatMap = 1;


    public bool noisePaint = false;
    public float noiseMultiplierInsidePaint = 1f;
    public float noiseMultiplierOutsidePaint = 0.5f;
    public float noiseSizeXPaint = 0.2f;
    public float noiseSizeZPaint = 0.2f;

    public bool mixTwoSplatMaps = false;
    public int secondSplatMap = 1;

    public bool addCliffSplatMap = false;
    public int cliffSplatMap = 1;
    public float cliffAngle = 25;
    public float cliffBlend = 1;

    public int cliffSplatMapOutside = 1;
    public float cliffAngleOutside = 25;
    public float cliffBlendOutside = 1;

    public float distanceClearFoliage = 1;
    public float distanceClearFoliageTrees = 1;

    #endregion


    public int biomeType;
}
