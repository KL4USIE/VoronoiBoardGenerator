using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "SplineProfile", menuName = "SplineProfile", order = 1)]
public class SplineProfile : ScriptableObject
{
    #region basic
    public Material splineMaterial;
    public AnimationCurve meshCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 0) });


    public float minVal = 0.5f;
    public float maxVal = 0.5f;

    public int vertsInShape = 3;
    public float traingleDensity = 0.2f;

    public float uvScale = 3;
    public bool uvRotation = true;


    public bool receiveShadows = false;
    public ShadowCastingMode shadowCastingMode = ShadowCastingMode.Off;

    #endregion

    #region flowmap
    public AnimationCurve flowFlat = new AnimationCurve(new Keyframe[] {
        new Keyframe (0, 0.025f),
        new Keyframe (0.5f, 0.05f),
        new Keyframe (1, 0.025f)
    });
    public AnimationCurve flowWaterfall = new AnimationCurve(new Keyframe[] {
        new Keyframe (0, 0.25f),
        new Keyframe (1, 0.25f)
    });

    public bool noiseflowMap = false;
    public float noiseMultiplierflowMap = 0.1f;
    public float noiseSizeXflowMap = 2f;
    public float noiseSizeZflowMap = 2f;

    public float floatSpeed = 10;
    #endregion

    #region terrrain
    public AnimationCurve terrainCarve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(10, -2) });
    public float distSmooth = 5;
    public float distSmoothStart = 1;
    public AnimationCurve terrainPaintCarve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });


    public bool noiseCarve = false;
    public float noiseMultiplierInside = 1f;
    public float noiseMultiplierOutside = 0.25f;
    public float noiseSizeX = 0.2f;
    public float noiseSizeZ = 0.2f;

    public float terrainSmoothMultiplier = 5;

    public int currentSplatMap = 1;

    public bool mixTwoSplatMaps = false;
    public int secondSplatMap = 1;

    public bool addCliffSplatMap = false;
    public int cliffSplatMap = 1;
    public float cliffAngle = 45;
    public float cliffBlend = 1;

    public int cliffSplatMapOutside = 1;
    public float cliffAngleOutside = 45;
    public float cliffBlendOutside = 1;

    public float distanceClearFoliage = 1;
    public float distanceClearFoliageTrees = 1;

    public bool noisePaint = false;
    public float noiseMultiplierInsidePaint = 0.25f;
    public float noiseMultiplierOutsidePaint = 0.25f;
    public float noiseSizeXPaint = 0.2f;
    public float noiseSizeZPaint = 0.2f;
    #endregion

    #region simulation
    public float simulatedRiverLength = 100;
    public int simulatedRiverPoints = 10;
    public float simulatedMinStepSize = 1f;
    public bool simulatedNoUp = false;
    public bool simulatedBreakOnUp = true;

    public bool noiseWidth = false;
    public float noiseMultiplierWidth = 4f;
    public float noiseSizeWidth = 0.5f;
    #endregion


    public int biomeType;


}
