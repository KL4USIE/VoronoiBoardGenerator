using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;


#if VEGETATION_STUDIO
using AwesomeTechnologies;
using AwesomeTechnologies.VegetationStudio;
#endif
#if VEGETATION_STUDIO_PRO
using AwesomeTechnologies.VegetationSystem.Biomes;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif


[RequireComponent(typeof(MeshFilter))]
public class RamSpline : MonoBehaviour
{
    public SplineProfile currentProfile;
    public SplineProfile oldProfile;

    public List<RamSpline> beginnigChildSplines = new List<RamSpline>();
    public List<RamSpline> endingChildSplines = new List<RamSpline>();
    public RamSpline beginningSpline;
    public RamSpline endingSpline;
    public int beginningConnectionID;
    public int endingConnectionID;
    public float beginningMinWidth = 0.5f;
    public float beginningMaxWidth = 1f;
    public float endingMinWidth = 0.5f;
    public float endingMaxWidth = 1f;

    public int toolbarInt = 0;

    public bool invertUVDirection = false;
    public bool uvRotation = true;

    public MeshFilter meshfilter;
    public List<Vector4> controlPoints = new List<Vector4>();

    public List<Quaternion> controlPointsRotations = new List<Quaternion>();

    public List<Quaternion> controlPointsOrientation = new List<Quaternion>();

    public List<Vector3> controlPointsUp = new List<Vector3>();
    public List<Vector3> controlPointsDown = new List<Vector3>();
    public List<float> controlPointsSnap = new List<float>();

    public AnimationCurve meshCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 0) });
    public List<AnimationCurve> controlPointsMeshCurves = new List<AnimationCurve>();

    public bool normalFromRaycast = false;
    public bool snapToTerrain = false;
    public LayerMask snapMask = 1;

    public List<Vector3> points = new List<Vector3>();

    public List<Vector3> pointsUp = new List<Vector3>();
    public List<Vector3> pointsDown = new List<Vector3>();



    public List<Vector3> points2 = new List<Vector3>();


    public List<Vector3> verticesBeginning = new List<Vector3>();
    public List<Vector3> verticesEnding = new List<Vector3>();

    public List<Vector3> normalsBeginning = new List<Vector3>();
    public List<Vector3> normalsEnding = new List<Vector3>();

    public List<float> widths = new List<float>();
    public List<float> snaps = new List<float>();
    public List<float> lerpValues = new List<float>();
    public List<Quaternion> orientations = new List<Quaternion>();
    public List<Vector3> tangents = new List<Vector3>();
    public List<Vector3> normalsList = new List<Vector3>();
    public Color[] colors;
    public List<Vector2> colorsFlowMap = new List<Vector2>();
    public List<Vector3> verticeDirection = new List<Vector3>();
    public float floatSpeed = 10;

    public bool generateOnStart = false;

    public float minVal = 0.5f;
    public float maxVal = 0.5f;

    public float width = 4;

    public int vertsInShape = 3;
    public float traingleDensity = 0.2f;
    public float uvScale = 3;

    public Material oldMaterial;
    public bool showVertexColors;
    public bool showFlowMap;
    public bool overrideFlowMap = false;

    public bool drawOnMesh = false;
    public bool drawOnMeshFlowMap = false;
    public bool uvScaleOverride = false;

    public bool debug = false;
    public bool debugNormals = false;
    public bool debugTangents = false;
    public bool debugBitangent = false;
    public bool debugFlowmap = false;
    public bool debugPoints = false;
    public bool debugPointsConnect = false;
    public bool debugMesh = true;
    public float distanceToDebug = 5;

    public Color drawColor = Color.black;
    public bool drawColorR = true;
    public bool drawColorG = true;
    public bool drawColorB = true;
    public bool drawColorA = true;

    public bool drawOnMultiple = false;

    public float flowSpeed = 1f;
    public float flowDirection = 0f;
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


    public float opacity = 0.1f;
    public float drawSize = 1f;

    public float length = 0;
    public float fulllength = 0;

    public float minMaxWidth;
    public float uvWidth;

    public float uvBeginning;


    public bool receiveShadows = false;
    public ShadowCastingMode shadowCastingMode = ShadowCastingMode.Off;

    //Part meshes
    public bool generateMeshParts = false;
    public int meshPartsCount = 3;
    public List<Transform> meshesPartTransforms = new List<Transform>();

    //Simulate mesh
    public float simulatedRiverLength = 100;
    public int simulatedRiverPoints = 10;
    public float simulatedMinStepSize = 1f;
    public bool simulatedNoUp = false;
    public bool simulatedBreakOnUp = true;

    //Terrain Change
    public int detailTerrain = 100;
    public int detailTerrainForward = 100;

    public float terrainAdditionalWidth = 2;
    public float terrainSmoothMultiplier = 5;

    public bool overrideRiverRender = false;

    public bool noiseWidth = false;
    public float noiseMultiplierWidth = 4f;
    public float noiseSizeWidth = 0.5f;

    public bool noiseCarve = false;
    public float noiseMultiplierInside = 1f;
    public float noiseMultiplierOutside = 0.25f;
    public float noiseSizeX = 0.2f;
    public float noiseSizeZ = 0.2f;

    public bool noisePaint = false;
    public float noiseMultiplierInsidePaint = 0.25f;
    public float noiseMultiplierOutsidePaint = 0.25f;
    public float noiseSizeXPaint = 0.2f;
    public float noiseSizeZPaint = 0.2f;

    public AnimationCurve terrainCarve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0.5f), new Keyframe(10, -4) });
    public float distSmooth = 5;
    public float distSmoothStart = 1;
    public AnimationCurve terrainPaintCarve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });
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

#if VEGETATION_STUDIO_PRO
    public float biomMaskResolution = 0.5f;
    public float vegetationMaskSize = 3;
    public float vegetationBlendDistance = 1f;
    public BiomeMaskArea biomeMaskArea;
    public bool refreshMask = false;
#endif
#if VEGETATION_STUDIO

    public float vegetationMaskPerimeter = 5;
    public VegetationMaskArea vegetationMaskArea;

#endif

    public GameObject meshGO;

    public void Start()
    {
        if (generateOnStart)
            GenerateSpline();

    }

    /// <summary>
    /// Creates spline
    /// </summary>
    /// <param name="splineMaterial">Material of the spline</param>
    /// <param name="positions">Positions to add to the spline</param>
    /// <returns></returns>
    public static RamSpline CreateSpline(Material splineMaterial = null, List<Vector4> positions = null, string name = "RamSpline")
    {
        GameObject gameobject = new GameObject(name);
        gameobject.layer = LayerMask.NameToLayer("Water");
        RamSpline spline = gameobject.AddComponent<RamSpline>();
        MeshRenderer meshRenderer = gameobject.AddComponent<MeshRenderer>();
        meshRenderer.receiveShadows = false;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        if (splineMaterial != null)
            meshRenderer.sharedMaterial = splineMaterial;

        if (positions != null)
            for (int i = 0; i < positions.Count; i++)
            {
                spline.AddPoint(positions[i]);
            }
#if UNITY_EDITOR
        Undo.RegisterCreatedObjectUndo(gameobject, "Create river");
#endif

        return spline;

    }

    /// <summary>
    /// Add point at end of spline
    /// </summary>
    /// <param name="position">New point position</param>
    public void AddPoint(Vector4 position)
    {

        if (position.w == 0)
        {
            if (controlPoints.Count > 0)
                position.w = controlPoints[controlPoints.Count - 1].w;
            else
                position.w = width;
        }


        controlPointsRotations.Add(Quaternion.identity);
        controlPoints.Add(position);
        controlPointsSnap.Add(0);
        controlPointsMeshCurves.Add(new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 0) }));
    }

    /// <summary>
    /// Add point in the middle of the spline
    /// </summary>
    /// <param name="i">Point id</param>
    public void AddPointAfter(int i)
    {
        Vector4 position = controlPoints[i];
        if (i < controlPoints.Count - 1 && controlPoints.Count > i + 1)
        {
            Vector4 positionSecond = controlPoints[i + 1];
            if (Vector3.Distance((Vector3)positionSecond, (Vector3)position) > 0)
                position = (position + positionSecond) * 0.5f;
            else
                position.x += 1;
        }
        else if (controlPoints.Count > 1 && i == controlPoints.Count - 1)
        {
            Vector4 positionSecond = controlPoints[i - 1];
            if (Vector3.Distance((Vector3)positionSecond, (Vector3)position) > 0)
                position = position + (position - positionSecond);
            else
                position.x += 1;
        }
        else
        {
            position.x += 1;
        }
        controlPoints.Insert(i + 1, position);
        controlPointsRotations.Insert(i + 1, Quaternion.identity);
        controlPointsSnap.Insert(i + 1, 0);
        controlPointsMeshCurves.Insert(i + 1, new AnimationCurve(new Keyframe[] {
                    new Keyframe (0, 0),
                    new Keyframe (1, 0)
                }));
    }

    /// <summary>
    /// Changes point position, if new position doesn't have width old width will be taken
    /// </summary>
    /// <param name="i">Point id</param>
    /// <param name="position">New position</param>
    public void ChangePointPosition(int i, Vector3 position)
    {
        ChangePointPosition(i, new Vector4(position.x, position.y, position.z, 0));
    }

    /// <summary>
    /// Changes point position, if new position doesn't have width old width will be taken
    /// </summary>
    /// <param name="i">Point id</param>
    /// <param name="position">New position</param>
    public void ChangePointPosition(int i, Vector4 position)
    {
        Vector4 oldPos = controlPoints[i];

        if (position.w == 0)
            position.w = oldPos.w;

        controlPoints[i] = position;
    }

    /// <summary>
    /// Removes point in spline
    /// </summary>
    /// <param name="i"></param>
    public void RemovePoint(int i)
    {
        if (i < controlPoints.Count)
        {
            controlPoints.RemoveAt(i);
            controlPointsRotations.RemoveAt(i);
            controlPointsMeshCurves.RemoveAt(i);
            controlPointsSnap.RemoveAt(i);
        }
    }

    /// <summary>
    /// Removes points from point id forward
    /// </summary>
    /// <param name="fromID">Point id</param>
    public void RemovePoints(int fromID = -1)
    {
        int pointsCount = controlPoints.Count - 1;
        for (int i = pointsCount; i > fromID; i--)
        {
            RemovePoint(i);
        }

    }

    public void GenerateBeginningParentBased()
    {

        vertsInShape = (int)Mathf.Round((beginningSpline.vertsInShape - 1) * (beginningMaxWidth - beginningMinWidth) + 1);
        if (vertsInShape < 1)
            vertsInShape = 1;

        beginningConnectionID = beginningSpline.points.Count - 1;
        Vector4 pos = beginningSpline.controlPoints[beginningSpline.controlPoints.Count - 1];
        float width = pos.w;
        width *= beginningMaxWidth - beginningMinWidth;
        pos = Vector3.Lerp(beginningSpline.pointsDown[beginningConnectionID], beginningSpline.pointsUp[beginningConnectionID], beginningMinWidth + (beginningMaxWidth - beginningMinWidth) * 0.5f)
        + beginningSpline.transform.position - transform.position;
        pos.w = width;
        controlPoints[0] = pos;

        if (!uvScaleOverride)
            uvScale = beginningSpline.uvScale;
    }

    public void GenerateEndingParentBased()
    {

        if (beginningSpline == null)
        {
            vertsInShape = (int)Mathf.Round((endingSpline.vertsInShape - 1) * (endingMaxWidth - endingMinWidth) + 1);
            if (vertsInShape < 1)
                vertsInShape = 1;
        }

        endingConnectionID = 0;
        Vector4 pos = endingSpline.controlPoints[0];
        float width = pos.w;
        width *= endingMaxWidth - endingMinWidth;
        pos = Vector3.Lerp(endingSpline.pointsDown[endingConnectionID], endingSpline.pointsUp[endingConnectionID], endingMinWidth + (endingMaxWidth - endingMinWidth) * 0.5f) + endingSpline.transform.position - transform.position;
        pos.w = width;
        controlPoints[controlPoints.Count - 1] = pos;
    }


    public void GenerateSpline(List<RamSpline> generatedSplines = null)
    {
        generatedSplines = new List<RamSpline>();

        if (beginningSpline)
        {
            GenerateBeginningParentBased();
        }
        if (endingSpline)
        {
            GenerateEndingParentBased();
        }




        List<Vector4> pointsChecked = new List<Vector4>();
        for (int i = 0; i < controlPoints.Count; i++)
        {
            if (i > 0)
            {
                if (Vector3.Distance((Vector3)controlPoints[i], (Vector3)controlPoints[i - 1]) > 0)
                    pointsChecked.Add(controlPoints[i]);

            }
            else
                pointsChecked.Add(controlPoints[i]);
        }

        Mesh mesh = new Mesh();
        meshfilter = GetComponent<MeshFilter>();
        if (pointsChecked.Count < 2)
        {
            mesh.Clear();

            meshfilter.mesh = mesh;
            return;

        }

        controlPointsOrientation = new List<Quaternion>();
        lerpValues.Clear();
        snaps.Clear();
        points.Clear();
        pointsUp.Clear();
        pointsDown.Clear();
        orientations.Clear();
        tangents.Clear();
        normalsList.Clear();
        widths.Clear();
        controlPointsUp.Clear();
        controlPointsDown.Clear();
        verticesBeginning.Clear();
        verticesEnding.Clear();
        normalsBeginning.Clear();
        normalsEnding.Clear();

        if (beginningSpline != null && beginningSpline.controlPointsRotations.Count > 0)
            controlPointsRotations[0] = Quaternion.identity;
        if (endingSpline != null && endingSpline.controlPointsRotations.Count > 0)
            controlPointsRotations[controlPointsRotations.Count - 1] = Quaternion.identity;

        for (int i = 0; i < pointsChecked.Count; i++)
        {

            if (i > pointsChecked.Count - 2)
            {
                continue;
            }

            CalculateCatmullRomSideSplines(pointsChecked, i);
        }

        if (beginningSpline != null && beginningSpline.controlPointsRotations.Count > 0)
            controlPointsRotations[0] = Quaternion.Inverse(controlPointsOrientation[0]) * (beginningSpline.controlPointsOrientation[beginningSpline.controlPointsOrientation.Count - 1]);

        if (endingSpline != null && endingSpline.controlPointsRotations.Count > 0)
            controlPointsRotations[controlPointsRotations.Count - 1] = Quaternion.Inverse(controlPointsOrientation[controlPointsOrientation.Count - 1]) * (endingSpline.controlPointsOrientation[0]);// * endingSpline.controlPointsRotations [0]);

        controlPointsOrientation = new List<Quaternion>();
        controlPointsUp.Clear();
        controlPointsDown.Clear();


        for (int i = 0; i < pointsChecked.Count; i++)
        {

            if (i > pointsChecked.Count - 2)
            {
                continue;
            }

            CalculateCatmullRomSideSplines(pointsChecked, i);
        }




        for (int i = 0; i < pointsChecked.Count; i++)
        {

            if (i > pointsChecked.Count - 2)
            {
                continue;
            }

            CalculateCatmullRomSplineParameters(pointsChecked, i);
        }

        for (int i = 0; i < controlPointsUp.Count; i++)
        {

            if (i > controlPointsUp.Count - 2)
            {
                continue;
            }

            CalculateCatmullRomSpline(controlPointsUp, i, ref pointsUp);
        }
        for (int i = 0; i < controlPointsDown.Count; i++)
        {

            if (i > controlPointsDown.Count - 2)
            {
                continue;
            }

            CalculateCatmullRomSpline(controlPointsDown, i, ref pointsDown);
        }

        GenerateMesh(ref mesh);

        if (generatedSplines != null)
        {

            generatedSplines.Add(this);
            foreach (var item in beginnigChildSplines)
            {
                if (item != null && !generatedSplines.Contains(item))
                {
                    if (item.beginningSpline == this || item.endingSpline == this)
                    {
                        item.GenerateSpline(generatedSplines);
                    }
                }
            }

            foreach (var item in endingChildSplines)
            {
                if (item != null && !generatedSplines.Contains(item))
                {
                    if (item.beginningSpline == this || item.endingSpline == this)
                    {
                        item.GenerateSpline(generatedSplines);
                    }
                }
            }
        }
    }


    void CalculateCatmullRomSideSplines(List<Vector4> controlPoints, int pos)
    {
        Vector3 p0 = controlPoints[pos];
        Vector3 p1 = controlPoints[pos];
        Vector3 p2 = controlPoints[ClampListPos(pos + 1)];
        Vector3 p3 = controlPoints[ClampListPos(pos + 1)];

        if (pos > 0)
            p0 = controlPoints[ClampListPos(pos - 1)];

        if (pos < controlPoints.Count - 2)
            p3 = controlPoints[ClampListPos(pos + 2)];


        int tValueMax = 0;
        if (pos == controlPoints.Count - 2)
        {
            tValueMax = 1;
        }

        for (int tValue = 0; tValue <= tValueMax; tValue++)
        {

            Vector3 newPos = GetCatmullRomPosition(tValue, p0, p1, p2, p3);
            Vector3 tangent = GetCatmullRomTangent(tValue, p0, p1, p2, p3).normalized;
            Vector3 normal = CalculateNormal(tangent, Vector3.up).normalized;

            Quaternion orientation;
            if (normal == tangent && normal == Vector3.zero)
                orientation = Quaternion.identity;
            else
                orientation = Quaternion.LookRotation(tangent, normal);

            orientation *= Quaternion.Lerp(controlPointsRotations[pos], controlPointsRotations[ClampListPos(pos + 1)], tValue);

            //			if (beginningSpline && pos == 0) {
            //				
            //				int lastId = beginningSpline.controlPointsOrientation.Count - 1;
            //				//orientation = beginningSpline.controlPointsOrientation [lastId];
            //
            //			} 
            //		
            //			if (endingSpline && pos == controlPoints.Count - 2 && tValue == 1) {
            //				
            //				//orientation = endingSpline.controlPointsOrientation [0];
            //
            //			}

            controlPointsOrientation.Add(orientation);

            Vector3 posUp = newPos + orientation * (0.5f * controlPoints[pos + tValue].w * Vector3.right);
            Vector3 posDown = newPos + orientation * (0.5f * controlPoints[pos + tValue].w * Vector3.left);

            controlPointsUp.Add(posUp);
            controlPointsDown.Add(posDown);
        }

    }


    void CalculateCatmullRomSplineParameters(List<Vector4> controlPoints, int pos, bool initialPoints = false)
    {


        Vector3 p0 = controlPoints[pos];
        Vector3 p1 = controlPoints[pos];
        Vector3 p2 = controlPoints[ClampListPos(pos + 1)];
        Vector3 p3 = controlPoints[ClampListPos(pos + 1)];

        if (pos > 0)
            p0 = controlPoints[ClampListPos(pos - 1)];

        if (pos < controlPoints.Count - 2)
            p3 = controlPoints[ClampListPos(pos + 2)];


        int loops = Mathf.FloorToInt(1f / traingleDensity);

        float i = 1;

        float start = 0;
        if (pos > 0)
            start = 1;

        for (i = start; i <= loops; i++)
        {
            float t = i * traingleDensity;
            CalculatePointParameters(controlPoints, pos, p0, p1, p2, p3, t);
        }

        if (i < loops)
        {
            i = loops;
            float t = i * traingleDensity;
            CalculatePointParameters(controlPoints, pos, p0, p1, p2, p3, t);
        }

    }

    void CalculateCatmullRomSpline(List<Vector3> controlPoints, int pos, ref List<Vector3> points)
    {


        Vector3 p0 = controlPoints[pos];
        Vector3 p1 = controlPoints[pos];
        Vector3 p2 = controlPoints[ClampListPos(pos + 1)];
        Vector3 p3 = controlPoints[ClampListPos(pos + 1)];

        if (pos > 0)
            p0 = controlPoints[ClampListPos(pos - 1)];

        if (pos < controlPoints.Count - 2)
            p3 = controlPoints[ClampListPos(pos + 2)];

        int loops = Mathf.FloorToInt(1f / traingleDensity);

        float i = 1;

        float start = 0;
        if (pos > 0)
            start = 1;

        for (i = start; i <= loops; i++)
        {
            float t = i * traingleDensity;
            CalculatePointPosition(controlPoints, pos, p0, p1, p2, p3, t, ref points);
        }

        if (i < loops)
        {
            i = loops;
            float t = i * traingleDensity;
            CalculatePointPosition(controlPoints, pos, p0, p1, p2, p3, t, ref points);
        }

    }

    void CalculatePointPosition(List<Vector3> controlPoints, int pos, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t, ref List<Vector3> points)
    {

        Vector3 newPos = GetCatmullRomPosition(t, p0, p1, p2, p3);
        points.Add(newPos);

        Vector3 tangent = GetCatmullRomTangent(t, p0, p1, p2, p3).normalized;
        Vector3 normal = CalculateNormal(tangent, Vector3.up).normalized;

    }

    void CalculatePointParameters(List<Vector4> controlPoints, int pos, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {

        Vector3 newPos = GetCatmullRomPosition(t, p0, p1, p2, p3);



        widths.Add(Mathf.Lerp(controlPoints[pos].w, controlPoints[ClampListPos(pos + 1)].w, t));

        if (controlPointsSnap.Count > pos + 1)
            snaps.Add(Mathf.Lerp(controlPointsSnap[pos], controlPointsSnap[ClampListPos(pos + 1)], t));
        else
            snaps.Add(0);

        lerpValues.Add(pos + t);


        points.Add(newPos);

        Vector3 tangent = GetCatmullRomTangent(t, p0, p1, p2, p3).normalized;
        Vector3 normal = CalculateNormal(tangent, Vector3.up).normalized;
        // Debug.Log(tangent + " CalculatePointParameters: " + normal);

        Quaternion orientation;
        if (normal == tangent && normal == Vector3.zero)
            orientation = Quaternion.identity;
        else
            orientation = Quaternion.LookRotation(tangent, normal);


        orientation *= Quaternion.Lerp(controlPointsRotations[pos], controlPointsRotations[ClampListPos(pos + 1)], t);
        orientations.Add(orientation);

        tangents.Add(tangent);
        if (normalsList.Count > 0 && Vector3.Angle(normalsList[normalsList.Count - 1], normal) > 90)
        {
            normal *= -1;
        }

        normalsList.Add(normal);

    }

    int ClampListPos(int pos)
    {
        if (pos < 0)
        {
            pos = controlPoints.Count - 1;
        }

        if (pos > controlPoints.Count)
        {
            pos = 1;
        }
        else if (pos > controlPoints.Count - 1)
        {
            pos = 0;
        }

        return pos;
    }

    Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {

        Vector3 a = 2f * p1;
        Vector3 b = p2 - p0;
        Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
        Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

        Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

        return pos;
    }

    Vector3 GetCatmullRomTangent(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return 0.5f * ((-p0 + p2) + 2f * (2f * p0 - 5f * p1 + 4f * p2 - p3) * t + 3f * (-p0 + 3f * p1 - 3f * p2 + p3) * t * t);
    }

    Vector3 CalculateNormal(Vector3 tangent, Vector3 up)
    {
        Vector3 binormal = Vector3.Cross(up, tangent);
        return Vector3.Cross(tangent, binormal);
    }


    void GenerateMesh(ref Mesh mesh)
    {

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.receiveShadows = receiveShadows;
            meshRenderer.shadowCastingMode = shadowCastingMode;
        }

        foreach (var item in meshesPartTransforms)
        {
            if (item != null)
            {

#if UNITY_EDITOR
                if (EditorApplication.isPlaying)
                    Destroy(item.gameObject);
                else
                    DestroyImmediate(item.gameObject);
#else
                    Destroy(item.gameObject);

#endif

            }
        }


        int segments = points.Count - 1;
        int edgeLoops = points.Count;
        int vertCount = vertsInShape * edgeLoops;

        List<int> triangleIndices = new List<int>();
        Vector3[] vertices = new Vector3[vertCount];
        Vector3[] normals = new Vector3[vertCount];
        Vector2[] uvs = new Vector2[vertCount];
        Vector2[] uvs3 = new Vector2[vertCount];
        Vector2[] uvs4 = new Vector2[vertCount];

        if (colors == null || colors.Length != vertCount)
        {
            colors = new Color[vertCount];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = Color.black;
            }
        }

        if (colorsFlowMap.Count != vertCount)
            colorsFlowMap.Clear();


        length = 0;
        fulllength = 0;

        if (beginningSpline != null)
            length = beginningSpline.length;

        minMaxWidth = 1;
        uvWidth = 1;
        uvBeginning = 0;

        if (beginningSpline != null)
        {

            minMaxWidth = beginningMaxWidth - beginningMinWidth;


            uvWidth = minMaxWidth * beginningSpline.uvWidth;

            uvBeginning = beginningSpline.uvWidth * beginningMinWidth + beginningSpline.uvBeginning;

        }
        else if (endingSpline != null)
        {

            minMaxWidth = endingMaxWidth - endingMinWidth;

            uvWidth = minMaxWidth * endingSpline.uvWidth;

            uvBeginning = endingSpline.uvWidth * endingMinWidth + endingSpline.uvBeginning;
        }



        for (int i = 0; i < pointsDown.Count; i++)
        {
            float width = widths[i];
            if (i > 0)
                fulllength += uvWidth * Vector3.Distance(pointsDown[i], pointsDown[i - 1]) / (float)(uvScale * width);
        }



        float roundEnding = Mathf.Round(fulllength);

        for (int i = 0; i < pointsDown.Count; i++)
        {

            float width = widths[i];

            int offset = i * vertsInShape;

            if (i > 0)
            {
                length += (uvWidth * Vector3.Distance(pointsDown[i], pointsDown[i - 1]) / (float)(uvScale * width)) / fulllength * roundEnding;
            }

            float u = 0;
            float u3 = 0;






            for (int j = 0; j < vertsInShape; j++)
            {
                int id = offset + j;

                //VERTICES
                float pos = j / (float)(vertsInShape - 1);

                if (pos < 0.5f)
                    pos *= minVal * 2;
                else
                    pos = ((pos - 0.5f) * (1 - maxVal) + 0.5f * maxVal) * 2;



                if (i == 0 && beginningSpline != null && beginningSpline.verticesEnding != null && beginningSpline.normalsEnding != null)
                {


                    int pos2 = (int)(beginningSpline.vertsInShape * beginningMinWidth);

                    vertices[id] = beginningSpline.verticesEnding[Mathf.Clamp(j + pos2, 0, beginningSpline.verticesEnding.Count - 1)] + beginningSpline.transform.position - transform.position;
                    //if (beginningSpline.normalsEnding.Count > 0)
                    //	normals [id] = beginningSpline.normalsEnding [Mathf.Clamp (j + pos2, 0, beginningSpline.normalsEnding.Count - 1)];

                }
                else if (i == pointsDown.Count - 1 && endingSpline != null && endingSpline.verticesBeginning != null && endingSpline.verticesBeginning.Count > 0 && endingSpline.normalsBeginning != null)
                {

                    int pos2 = (int)(endingSpline.vertsInShape * endingMinWidth);

                    vertices[id] = endingSpline.verticesBeginning[Mathf.Clamp(j + pos2, 0, endingSpline.verticesBeginning.Count - 1)] + endingSpline.transform.position - transform.position;
                    //if (endingSpline.normalsBeginning.Count > 0)
                    //	normals [id] = endingSpline.normalsBeginning [Mathf.Clamp (j + pos2, 0, endingSpline.normalsBeginning.Count - 1)];


                }
                else
                {
                    vertices[id] = Vector3.Lerp(pointsDown[i], pointsUp[i], pos);

                    RaycastHit hit;
                    if (Physics.Raycast(vertices[id] + transform.position + Vector3.up * 5, Vector3.down, out hit, 1000, snapMask.value))
                    {

                        vertices[id] = Vector3.Lerp(vertices[id], hit.point - transform.position + new Vector3(0, 0.1f, 0), (Mathf.Sin(Mathf.PI * snaps[i] - Mathf.PI * 0.5f) + 1) * 0.5f);

                    }


                    if (normalFromRaycast)
                    {
                        RaycastHit hit2;
                        if (Physics.Raycast(points[i] + transform.position + Vector3.up * 5, Vector3.down, out hit2, 1000, snapMask.value))
                        {
                            normals[id] = hit2.normal;
                        }
                    }


                    vertices[id].y += Mathf.Lerp(controlPointsMeshCurves[Mathf.FloorToInt(lerpValues[i])].Evaluate(pos),
                        controlPointsMeshCurves[Mathf.CeilToInt(lerpValues[i])].Evaluate(pos),
                        lerpValues[i] - Mathf.Floor(lerpValues[i]));

                }

                if (i > 0 && i < 5 && beginningSpline != null && beginningSpline.verticesEnding != null)
                {
                    vertices[id].y = (vertices[id].y + vertices[id - vertsInShape].y) * 0.5f;
                }

                if (i == pointsDown.Count - 1 && endingSpline != null && endingSpline.verticesBeginning != null)
                {
                    for (int k = 1; k < 5; k++)
                    {
                        vertices[id - vertsInShape * k].y = (vertices[id - vertsInShape * (k - 1)].y + vertices[id - vertsInShape * k].y) * 0.5f;
                    }

                }


                if (i == 0)
                    verticesBeginning.Add(vertices[id]);

                if (i == pointsDown.Count - 1)
                    verticesEnding.Add(vertices[id]);


                //NORMALS
                if (!normalFromRaycast)
                    //	if ((i > 0 || beginningSpline == null) && (i < pointsDown.Count - 1 || endingSpline == null))
                    normals[id] = orientations[i] * Vector3.up;


                //if (beginningSpline != null && i == 1)
                //	normals [id] = (normals [id] + normals [id - vertsInShape]) * 0.5f;
                //
                //if (i == pointsDown.Count - 2 && endingSpline != null && endingSpline.normalsBeginning != null && endingSpline.normalsBeginning.Count > 0) {
                //
                //	int pos2 = (int)(endingSpline.vertsInShape * endingMinWidth);
                //	normals [id] = (normals [id] + endingSpline.normalsBeginning [Mathf.Clamp (j + pos2, 0, endingSpline.normalsBeginning.Count - 1)]) * 0.5f;
                //
                //}




                if (i == 0)
                    normalsBeginning.Add(normals[id]);

                if (i == pointsDown.Count - 1)
                    normalsEnding.Add(normals[id]);



                //UVS
                if (j > 0)
                {
                    u = (pos) * uvWidth;
                    u3 = pos;
                }



                if (beginningSpline != null || endingSpline != null)
                {
                    u += uvBeginning;
                }
                u = u / uvScale;



                float uv4u = FlowCalculate(u3, normals[id].y, vertices[id]);



                int lerpDistance = 10;

                if (beginnigChildSplines.Count > 0 && i <= lerpDistance)
                {


                    float lerpUv4u = 0;
                    foreach (var item in beginnigChildSplines)
                    {
                        if (item == null)
                            continue;
                        if (Mathf.CeilToInt(item.endingMaxWidth * (vertsInShape - 1)) >= j && j >= Mathf.CeilToInt(item.endingMinWidth * (vertsInShape - 1)))
                        {

                            lerpUv4u = (j - Mathf.CeilToInt(item.endingMinWidth * (vertsInShape - 1)))
                            / (float)(Mathf.CeilToInt(item.endingMaxWidth * (vertsInShape - 1)) - Mathf.CeilToInt(item.endingMinWidth * (vertsInShape - 1)));

                            lerpUv4u = FlowCalculate(lerpUv4u, normals[id].y, vertices[id]);

                        }
                    }
                    if (i > 0)
                        uv4u = Mathf.Lerp(uv4u, lerpUv4u, 1 - (i / (float)lerpDistance));
                    else
                        uv4u = lerpUv4u;

                }


                if (i >= pointsDown.Count - lerpDistance - 1 && endingChildSplines.Count > 0)
                {

                    float lerpUv4u = 0;

                    foreach (var item in endingChildSplines)
                    {
                        if (item == null)
                            continue;
                        if (Mathf.CeilToInt(item.beginningMaxWidth * (vertsInShape - 1)) >= j && j >= Mathf.CeilToInt(item.beginningMinWidth * (vertsInShape - 1)))
                        {

                            lerpUv4u = (j - Mathf.CeilToInt(item.beginningMinWidth * (vertsInShape - 1)))
                            / (float)(Mathf.CeilToInt(item.beginningMaxWidth * (vertsInShape - 1)) - Mathf.CeilToInt(item.beginningMinWidth * (vertsInShape - 1)));

                            lerpUv4u = FlowCalculate(lerpUv4u, normals[id].y, vertices[id]);

                        }

                    }
                    if (i < pointsDown.Count - 1)
                        uv4u = Mathf.Lerp(uv4u, lerpUv4u, (i - (pointsDown.Count - lerpDistance - 1)) / (float)lerpDistance);
                    else
                        uv4u = lerpUv4u;

                }

                float uv4v = -(u3 - 0.5f) * 0.01f;

                if (uvRotation)
                {

                    if (!invertUVDirection)
                    {

                        uvs[id] = new Vector2(1 - length, u);
                        uvs3[id] = new Vector2(1 - length / (float)fulllength, u3);
                        uvs4[id] = new Vector2(uv4u, uv4v);

                    }
                    else
                    {

                        uvs[id] = new Vector2(1 + length, u);
                        uvs3[id] = new Vector2(1 + length / (float)fulllength, u3);
                        uvs4[id] = new Vector2(uv4u, uv4v);

                    }
                }
                else
                {
                    if (!invertUVDirection)
                    {

                        uvs[id] = new Vector2(u, 1 - length);
                        uvs3[id] = new Vector2(u3, 1 - length / (float)fulllength);
                        uvs4[id] = new Vector2(uv4v, uv4u);
                    }
                    else
                    {

                        uvs[id] = new Vector2(u, 1 + length);
                        uvs3[id] = new Vector2(u3, 1 + length / (float)fulllength);
                        uvs4[id] = new Vector2(uv4v, uv4u);

                    }
                }

                float tempRound = (int)(uvs4[id].x * 100);
                uvs4[id].x = tempRound * 0.01f;
                tempRound = (int)(uvs4[id].y * 100);
                uvs4[id].y = tempRound * 0.01f;


                if (colorsFlowMap.Count <= id)
                    colorsFlowMap.Add(uvs4[id]);
                else if (!overrideFlowMap)
                    colorsFlowMap[id] = uvs4[id];




            }

        }

        //TRIANGLES
        for (int i = 0; i < segments; i++)
        {
            int offset = i * vertsInShape;
            for (int l = 0; l < vertsInShape - 1; l += 1)
            {
                int a = offset + l;
                int b = offset + l + vertsInShape;
                int c = offset + l + 1 + vertsInShape;
                int d = offset + l + 1;
                triangleIndices.Add(a);
                triangleIndices.Add(b);
                triangleIndices.Add(c);
                triangleIndices.Add(c);
                triangleIndices.Add(d);
                triangleIndices.Add(a);
            }
        }
        verticeDirection.Clear();
        for (int i = 0; i < vertices.Length - vertsInShape; i++)
        {
            Vector3 dir = (vertices[i + vertsInShape] - vertices[i]).normalized;

            if (uvRotation)
                dir = new Vector3(dir.z, 0, -dir.x);


            verticeDirection.Add(dir);
        }

        for (int i = vertices.Length - vertsInShape; i < vertices.Length; i++)
        {
            Vector3 dir = (vertices[i] - vertices[i - vertsInShape]).normalized;

            if (uvRotation)
                dir = new Vector3(dir.z, 0, -dir.x);
            verticeDirection.Add(dir);
        }

        mesh = new Mesh();
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.uv3 = uvs3;
        mesh.uv4 = colorsFlowMap.ToArray();

        mesh.triangles = triangleIndices.ToArray();
        mesh.colors = colors;
        mesh.RecalculateTangents();
        meshfilter.mesh = mesh;
        GetComponent<MeshRenderer>().enabled = true;

        if (generateMeshParts)
            GenerateMeshParts(mesh);
    }

    public void GenerateMeshParts(Mesh baseMesh)
    {
        foreach (var item in meshesPartTransforms)
        {
            if (item != null)
                DestroyImmediate(item.gameObject);
        }
        Vector3[] vertices = baseMesh.vertices;
        Vector3[] normals = baseMesh.normals;
        Vector2[] uvs = baseMesh.uv;
        Vector2[] uvs3 = baseMesh.uv3;


        GetComponent<MeshRenderer>().enabled = false;
        int verticesLinesPart = Mathf.RoundToInt((vertices.Length / vertsInShape) / (float)meshPartsCount);

        int verticesInPart = verticesLinesPart * vertsInShape;


        for (int i = 0; i < meshPartsCount; i++)
        {
            GameObject go = new GameObject(gameObject.name + "- Mesh part " + i);
            go.transform.SetParent(gameObject.transform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = Vector3.zero;
            go.transform.localScale = Vector3.one;

            meshesPartTransforms.Add(go.transform);

            MeshRenderer meshRendererPart = go.AddComponent<MeshRenderer>();

            meshRendererPart.sharedMaterial = GetComponent<MeshRenderer>().sharedMaterial;
            meshRendererPart.receiveShadows = receiveShadows;
            meshRendererPart.shadowCastingMode = shadowCastingMode;



            MeshFilter mf = go.AddComponent<MeshFilter>();
            Mesh meshPart = new Mesh();
            meshPart.Clear();

            List<Vector3> verticesPart = new List<Vector3>();
            List<Vector3> normalsPart = new List<Vector3>();
            List<Vector2> uvPart = new List<Vector2>();
            List<Vector2> uv3Part = new List<Vector2>();
            List<Vector2> uv4Part = new List<Vector2>();
            List<Color> colorsPart = new List<Color>();
            List<int> trianglesPart = new List<int>();


            for (int j = verticesInPart * i + (i > 0 ? -vertsInShape : 0); (j < verticesInPart * (i + 1) && j < vertices.Length) || (i == meshPartsCount - 1 && j < vertices.Length); j++)
            {

                verticesPart.Add(vertices[j]);
                normalsPart.Add(normals[j]);
                uvPart.Add(uvs[j]);
                uv3Part.Add(uvs3[j]);
                uv4Part.Add(colorsFlowMap[j]);
                colorsPart.Add(colors[j]);
            }
            if (verticesPart.Count > 0)
            {

                Vector3 pivotChange = verticesPart[0];
                for (int j = 0; j < verticesPart.Count; j++)
                {
                    verticesPart[j] = verticesPart[j] - pivotChange;
                }

                for (int k = 0; k < verticesPart.Count / vertsInShape - 1; k++)
                {
                    int offset = k * vertsInShape;

                    for (int l = 0; l < vertsInShape - 1; l += 1)
                    {
                        int a = offset + l;
                        int b = offset + l + vertsInShape;
                        int c = offset + l + 1 + vertsInShape;
                        int d = offset + l + 1;
                        trianglesPart.Add(a);
                        trianglesPart.Add(b);
                        trianglesPart.Add(c);
                        trianglesPart.Add(c);
                        trianglesPart.Add(d);
                        trianglesPart.Add(a);
                    }
                }
                go.transform.position += pivotChange;

                meshPart.vertices = verticesPart.ToArray();
                meshPart.triangles = trianglesPart.ToArray();
                meshPart.normals = normalsPart.ToArray();
                meshPart.uv = uvPart.ToArray();
                meshPart.uv3 = uv3Part.ToArray();
                meshPart.uv4 = uv4Part.ToArray();
                meshPart.colors = colorsPart.ToArray();


                meshPart.RecalculateTangents();
                mf.mesh = meshPart;

                //MeshCollider meshCollider = go.AddComponent<MeshCollider>();

                //meshCollider.cookingOptions = MeshColliderCookingOptions.InflateConvexMesh | MeshColliderCookingOptions.CookForFasterSimulation | MeshColliderCookingOptions.EnableMeshCleaning | MeshColliderCookingOptions.WeldColocatedVertices;
                //meshCollider.skinWidth = 0.1f;
                //meshCollider.convex = true;
                //meshCollider.isTrigger = true;
            }
        }
    }

    public void AddNoiseToWidths()
    {
        for (int i = 0; i < controlPoints.Count; i++)
        {
            Vector4 controlPoint = controlPoints[i];

            controlPoint.w = controlPoint.w + (noiseWidth ? noiseMultiplierWidth * (Mathf.PerlinNoise(noiseSizeWidth * i, 0) - 0.5f) : 0);

            if (controlPoint.w < 0)
            {
                controlPoint.w = 0;
            }
            controlPoints[i] = controlPoint;
        }

    }

    public void SimulateRiver(bool generate = true)
    {

        if (meshGO != null)
        {
            if (Application.isEditor)
                DestroyImmediate(meshGO);
            else
                Destroy(meshGO);
        }

        if (controlPoints.Count == 0)
        {
            Debug.Log("Add one point to start Simulating River");
            return;
        }


        Ray ray = new Ray();
        RaycastHit hit;


        Vector3 lastPosition = transform.TransformPoint((Vector3)controlPoints[controlPoints.Count - 1]);

        List<Vector3> positionsGenerated = new List<Vector3>();
        if (controlPoints.Count > 1)
        {
            positionsGenerated.Add(transform.TransformPoint((Vector3)controlPoints[controlPoints.Count - 2]));
            positionsGenerated.Add(lastPosition);
        }


        List<Vector3> samplePositionsGenerated = new List<Vector3>();
        samplePositionsGenerated.Add(lastPosition);

        //Debug.DrawRay(lastPosition + new Vector3(0, 3, 0), Vector3.down * 20, Color.white, 3);

        float length = 0;
        int i = -1;
        int added = 0;
        bool end = false;

        float widthNew = 0;
        if (controlPoints.Count > 0)
            widthNew = controlPoints[controlPoints.Count - 1].w;
        else
            widthNew = width;

        do
        {
            i++;
            if (i > 0)
            {
                Vector3 maxPosition = Vector3.zero;
                float max = float.MinValue;
                bool foundNextPositon = false;
                for (float j = simulatedMinStepSize; j < 10; j += 0.1f)
                {
                    for (int angle = 0; angle < 36; angle++)
                    {
                        float x = j * Mathf.Cos(angle);
                        float z = j * Mathf.Sin(angle);

                        ray.origin = lastPosition + new Vector3(0, 1000, 0) + new Vector3(x, 0, z);
                        ray.direction = Vector3.down;

                        if (Physics.Raycast(ray, out hit, 10000))
                        {
                            if (hit.distance > max)
                            {
                                bool goodPoint = true;


                                foreach (var item in positionsGenerated)
                                {
                                    if (Vector3.Distance(item, lastPosition) > Vector3.Distance(item, hit.point) + 0.5f)
                                    {
                                        goodPoint = false;
                                        break;
                                    }
                                }
                                if (goodPoint)
                                {
                                    foundNextPositon = true;
                                    max = hit.distance;
                                    maxPosition = hit.point;
                                }
                            }
                            //else
                            //    Debug.DrawRay(ray.origin, ray.direction * 10000, Color.red, 3);

                        }

                    }
                    if (foundNextPositon)
                        break;
                }
                if (!foundNextPositon)
                    break;

                if (maxPosition.y > lastPosition.y)
                {
                    if (simulatedNoUp)
                        maxPosition.y = lastPosition.y;
                    if (simulatedBreakOnUp)
                        end = true;

                    //Debug.DrawRay(maxPosition + new Vector3(0, 5, 0), ray.direction * 10, Color.red, 3);
                }
                // else
                //    Debug.DrawRay(maxPosition + new Vector3(0, 5, 0), ray.direction * 10, Color.blue, 3);


                length += Vector3.Distance(maxPosition, lastPosition);
                if (i % simulatedRiverPoints == 0 || simulatedRiverLength <= length || end)
                {
                    //Debug.DrawRay(maxPosition + new Vector3(0, 5, 0), ray.direction * 20, Color.white, 3);

                    samplePositionsGenerated.Add(maxPosition);

                    if (generate)
                    {
                        added++;

                        Vector4 newPosition = maxPosition - transform.position;

                        newPosition.w = widthNew + (noiseWidth ? noiseMultiplierWidth * (Mathf.PerlinNoise(noiseSizeWidth * added, 0) - 0.5f) : 0);

                        controlPointsRotations.Add(Quaternion.identity);
                        controlPoints.Add(newPosition);
                        controlPointsSnap.Add(0);
                        controlPointsMeshCurves.Add(new AnimationCurve(meshCurve.keys));

                    }
                }
                else
                {
                    //samplePositionsGenerated.Add(maxPosition);
                    //Debug.DrawRay(maxPosition + new Vector3(0, 3, 0), ray.direction * 20, Color.Lerp(Color.red, Color.green, i / (float)spline.simulatedRiverLength), 3);
                }


                positionsGenerated.Add(lastPosition);
                lastPosition = maxPosition;

            }


        } while (simulatedRiverLength > length && !end);

        if (!generate)
        {

            if (controlPoints.Count > 0)
                widthNew = controlPoints[controlPoints.Count - 1].w;
            else
                widthNew = width;
            float widthNoise = 0;

            List<List<Vector4>> positionArray = new List<List<Vector4>>();
            Vector3 v1 = new Vector3();
            for (i = 0; i < samplePositionsGenerated.Count - 1; i++)
            {
                widthNoise = widthNew + (noiseWidth ? noiseMultiplierWidth * (Mathf.PerlinNoise(noiseSizeWidth * i, 0) - 0.5f) : 0);


                //Debug.DrawLine(samplePositionsGenerated[i], samplePositionsGenerated[i + 1], Color.white, 3);

                v1 = Vector3.Cross(samplePositionsGenerated[i + 1] - samplePositionsGenerated[i], Vector3.up).normalized;

                if (i > 0)
                {
                    Vector3 v2 = Vector3.Cross(samplePositionsGenerated[i] - samplePositionsGenerated[i - 1], Vector3.up).normalized;
                    v1 = (v1 + v2).normalized;
                }

                //Vector3 v2 = Vector3.Cross(samplePositionsGenerated[i + 1] - samplePositionsGenerated[i], v1).normalized;

                //Debug.DrawLine(samplePositionsGenerated[i] - v1 * widthNew * 0.5f, samplePositionsGenerated[i] + v1 * widthNew * 0.5f, Color.blue, 3);

                List<Vector4> positionRow = new List<Vector4>();

                positionRow.Add(samplePositionsGenerated[i] + v1 * widthNoise * 0.5f);
                positionRow.Add(samplePositionsGenerated[i] - v1 * widthNoise * 0.5f);
                positionArray.Add(positionRow);
            }

            widthNoise = widthNew + (noiseWidth ? noiseMultiplierWidth * (Mathf.PerlinNoise(noiseSizeWidth * i, 0) - 0.5f) : 0);
            List<Vector4> positionRowLast = new List<Vector4>();

            positionRowLast.Add(samplePositionsGenerated[i] + v1 * widthNoise * 0.5f);
            positionRowLast.Add(samplePositionsGenerated[i] - v1 * widthNoise * 0.5f);
            positionArray.Add(positionRowLast);


            Mesh meshTerrain = new Mesh();
            meshTerrain.indexFormat = IndexFormat.UInt32;
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            // List<Vector2> uv = new List<Vector2>();

            foreach (var positionRow in positionArray)
            {
                foreach (var vert in positionRow)
                {
                    vertices.Add(vert);
                }
            }

            for (i = 0; i < positionArray.Count - 1; i++)
            {
                int count = positionArray[i].Count;
                for (int j = 0; j < count - 1; j++)
                {
                    triangles.Add(j + i * count);
                    triangles.Add(j + (i + 1) * count);
                    triangles.Add((j + 1) + i * count);

                    triangles.Add((j + 1) + i * count);
                    triangles.Add(j + (i + 1) * count);
                    triangles.Add((j + 1) + (i + 1) * count);

                }
            }


            meshTerrain.SetVertices(vertices);
            meshTerrain.SetTriangles(triangles, 0);
            // meshTerrain.SetUVs(0, uv);

            meshTerrain.RecalculateNormals();
            meshTerrain.RecalculateTangents();
            meshTerrain.RecalculateBounds();

            meshGO = new GameObject("TerrainMesh");
            meshGO.hideFlags = HideFlags.HideAndDontSave;
            meshGO.AddComponent<MeshFilter>();
            meshGO.transform.parent = transform;
            MeshRenderer meshRenderer = meshGO.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = new Material(Shader.Find("Debug Terrain Carve"));
            meshRenderer.sharedMaterial.color = new Color(0, 0.5f, 0);


            meshGO.transform.position = Vector3.zero;
            meshGO.GetComponent<MeshFilter>().sharedMesh = meshTerrain;

        }
    }

    #region Terrain

    public void ShowTerrainCarve(float differentSize = 0)
    {
        if (Application.isEditor && meshGO == null)
        {

            Transform meshGoTrans = transform.Find("TerrainMesh");
            if (meshGoTrans != null)
                meshGO = meshGoTrans.gameObject;


        }

        if (meshGO != null)
        {
            if (Application.isEditor)
                DestroyImmediate(meshGO);
            else
                Destroy(meshGO);
        }

        Mesh mesh = meshfilter.sharedMesh;
        RaycastHit hit;
        Vector3 rayPointDown;
        Vector3 rayPointUp;
        Vector3 point;

        detailTerrainForward = 2;
        detailTerrain = 10;

        if (differentSize == 0)
            terrainAdditionalWidth = distSmooth + distSmoothStart;
        else
            terrainAdditionalWidth = differentSize;

        List<List<Vector4>> positionArray = new List<List<Vector4>>();

        float noise = 0;
        for (int i = 0; i < pointsDown.Count - 1; i++)
        {

            for (int tf = 0; tf <= detailTerrainForward; tf++)
            {
                List<Vector4> positionArrayRow = new List<Vector4>();

                rayPointDown = Vector3.Lerp(pointsDown[i], pointsDown[i + 1], tf / (float)detailTerrainForward);
                rayPointUp = Vector3.Lerp(pointsUp[i], pointsUp[i + 1], tf / (float)detailTerrainForward);


                Vector3 diff = rayPointDown - rayPointUp;
                float diffMagintude = diff.magnitude;
                rayPointDown += diff * 0.05f;
                rayPointUp -= diff * 0.05f;

                diff.Normalize();
                Vector3 rayPointDownNew = rayPointDown + diff * terrainAdditionalWidth * 0.5f;
                Vector3 rayPointUpNew = rayPointUp - diff * terrainAdditionalWidth * 0.5f;


                if (terrainAdditionalWidth > 0)
                {
                    for (int t = 0; t < detailTerrain; t++)
                    {

                        point = Vector3.Lerp(rayPointDownNew, rayPointDown, t / (float)detailTerrain) + transform.position;
                        if (Physics.Raycast(point + Vector3.up * 500, Vector3.down, out hit))
                        {
                            if (noiseCarve)
                                noise = Mathf.PerlinNoise(point.x * noiseSizeX, point.z * noiseSizeZ) * noiseMultiplierOutside - noiseMultiplierOutside * 0.5f;
                            else
                                noise = 0;

                            float evaluate = 1 - t / (float)detailTerrain;
                            evaluate *= terrainAdditionalWidth;
                            float height = point.y + terrainCarve.Evaluate(-evaluate) + terrainCarve.Evaluate(-evaluate) * noise;

                            float smoothValue = t / (float)detailTerrain;
                            smoothValue = Mathf.Pow(smoothValue, terrainSmoothMultiplier);



                            height = Mathf.Lerp(hit.point.y, height, smoothValue);

                            Vector4 newPos = new Vector4(hit.point.x, height, hit.point.z, -evaluate);
                            positionArrayRow.Add(newPos);



                        }
                        else
                            positionArrayRow.Add(point);
                    }
                }
                for (int t = 0; t <= detailTerrain; t++)
                {

                    point = Vector3.Lerp(rayPointDown, rayPointUp, t / (float)detailTerrain) + transform.position;
                    if (Physics.Raycast(point + Vector3.up * 500, Vector3.down, out hit))
                    {

                        if (noiseCarve)
                            noise = Mathf.PerlinNoise(point.x * noiseSizeX, point.z * noiseSizeZ) * noiseMultiplierInside - noiseMultiplierInside * 0.5f;
                        else
                            noise = 0;

                        float evaluate = diffMagintude * (0.5f - Mathf.Abs(0.5f - t / (float)detailTerrain));
                        float height = point.y + terrainCarve.Evaluate(evaluate) + terrainCarve.Evaluate(evaluate) * noise;

                        float smoothValue = 1 - 2 * Mathf.Abs(t / (float)detailTerrain - 0.5f);
                        smoothValue = Mathf.Pow(smoothValue, terrainSmoothMultiplier);

                        height = Mathf.Lerp(hit.point.y, height, 1);

                        Vector4 newPos = new Vector4(hit.point.x, height, hit.point.z, evaluate);

                        positionArrayRow.Add(newPos);


                    }
                    else
                        positionArrayRow.Add(point);
                }



                if (terrainAdditionalWidth > 0)
                {
                    for (int t = 1; t <= detailTerrain; t++)
                    {
                        point = Vector3.Lerp(rayPointUp, rayPointUpNew, t / (float)detailTerrain) + transform.position;
                        if (Physics.Raycast(point + Vector3.up * 50, Vector3.down, out hit))
                        {
                            if (noiseCarve)
                                noise = Mathf.PerlinNoise(point.x * noiseSizeX, point.z * noiseSizeZ) * noiseMultiplierOutside - noiseMultiplierOutside * 0.5f;
                            else
                                noise = 0;

                            float evaluate = t / (float)detailTerrain;
                            evaluate *= terrainAdditionalWidth;

                            float height = point.y + terrainCarve.Evaluate(-evaluate) + terrainCarve.Evaluate(-evaluate) * noise;

                            float smoothValue = 1 - t / (float)detailTerrain;
                            smoothValue = Mathf.Pow(smoothValue, terrainSmoothMultiplier);

                            height = Mathf.Lerp(hit.point.y, height, smoothValue);
                            Vector4 newPos = new Vector4(hit.point.x, height, hit.point.z, -evaluate);
                            positionArrayRow.Add(newPos);

                        }
                        else
                            positionArrayRow.Add(point);
                    }


                }
                positionArray.Add(positionArrayRow);
            }
        }


        Mesh meshTerrain = new Mesh();
        meshTerrain.indexFormat = IndexFormat.UInt32;
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();

        foreach (var positionRow in positionArray)
        {
            foreach (var vert in positionRow)
            {
                vertices.Add(vert);
            }
        }

        for (int i = 0; i < positionArray.Count - 1; i++)
        {
            int count = positionArray[i].Count;
            for (int j = 0; j < count - 1; j++)
            {
                triangles.Add(j + i * count);
                triangles.Add(j + (i + 1) * count);
                triangles.Add((j + 1) + i * count);

                triangles.Add((j + 1) + i * count);
                triangles.Add(j + (i + 1) * count);
                triangles.Add((j + 1) + (i + 1) * count);

            }
        }

        foreach (var positionRow in positionArray)
        {
            foreach (var vert in positionRow)
            {
                uv.Add(new Vector2(vert.w, 0));
            }
        }


        meshTerrain.SetVertices(vertices);
        meshTerrain.SetTriangles(triangles, 0);
        meshTerrain.SetUVs(0, uv);

        meshTerrain.RecalculateNormals();
        meshTerrain.RecalculateTangents();
        meshTerrain.RecalculateBounds();

        //if (meshGO == null)
        //{
        meshGO = new GameObject("TerrainMesh");
        meshGO.transform.parent = transform;
        meshGO.hideFlags = HideFlags.HideAndDontSave;
        meshGO.AddComponent<MeshFilter>();
        meshGO.transform.parent = transform;
        MeshRenderer meshRenderer = meshGO.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Debug Terrain Carve"));
        meshRenderer.sharedMaterial.color = new Color(0, 0.5f, 0);


        // }
        meshGO.transform.position = Vector3.zero;
        meshGO.GetComponent<MeshFilter>().sharedMesh = meshTerrain;

        if (overrideRiverRender)
            meshGO.GetComponent<MeshRenderer>().sharedMaterial.renderQueue = 5000;
        else
            meshGO.GetComponent<MeshRenderer>().sharedMaterial.renderQueue = 2980;

    }

    public void TerrainCarve()
    {
        bool debugLines = false;

        bool savedAutoSyncTransforms = Physics.autoSyncTransforms;
        Physics.autoSyncTransforms = false;
        foreach (Terrain terrain in Terrain.activeTerrains)
        {

            TerrainData terrainData = terrain.terrainData;
            float posY = terrain.transform.position.y;
            float sizeX = terrain.terrainData.size.x;
            float sizeY = terrain.terrainData.size.y;
            float sizeZ = terrain.terrainData.size.z;
            float terrainTowidth = (1 / (float)sizeZ * (terrainData.heightmapResolution - 1));
            float terrainToheight = (1 / (float)sizeX * (terrainData.heightmapResolution - 1));

            float minX;
            float maxX;
            float minZ;
            float maxZ;

#if UNITY_EDITOR
            Undo.RegisterCompleteObjectUndo(terrainData, "River curve");
#endif

            MeshCollider meshCollider = meshGO.gameObject.AddComponent<MeshCollider>();

            List<Vector3> transformPointUp = new List<Vector3>();
            List<Vector3> transformPointDown = new List<Vector3>();

            int pointsCount = 5;
            int pointsStart = 0;//pointsUp.Count

            //List<Vector2> done = new List<Vector2>();


            //List<Vector3> positionArray = new List<Vector3>();

            Vector3 pointOne = Vector3.zero;
            Vector3 pointTwo = Vector3.zero;

            for (pointsStart = 0; pointsStart < pointsUp.Count; pointsStart = Mathf.Clamp(pointsStart + pointsCount - 1, 0, pointsUp.Count))
            {
                int end = Mathf.Min(pointsStart + pointsCount, pointsUp.Count);
                //int currentCount = Mathf.Min(pointsCount, pointsUp.Count - pointsStart);

                transformPointUp.Clear();
                transformPointDown.Clear();

                for (int i = pointsStart; i < end; i++)
                {
                    transformPointUp.Add(transform.TransformPoint(pointsUp[i]));
                    transformPointDown.Add(transform.TransformPoint(pointsDown[i]));
                }

                minX = float.MaxValue;
                maxX = float.MinValue;
                minZ = float.MaxValue;
                maxZ = float.MinValue;



                for (int i = 0; i < transformPointUp.Count; i++)
                {
                    Vector3 point = transformPointUp[i];


                    if (minX > point.x)
                        minX = point.x;

                    if (maxX < point.x)
                        maxX = point.x;

                    if (minZ > point.z)
                        minZ = point.z;

                    if (maxZ < point.z)
                        maxZ = point.z;
                }

                for (int i = 0; i < transformPointDown.Count; i++)
                {
                    Vector3 point = transformPointDown[i];


                    if (minX > point.x)
                        minX = point.x;

                    if (maxX < point.x)
                        maxX = point.x;

                    if (minZ > point.z)
                        minZ = point.z;

                    if (maxZ < point.z)
                        maxZ = point.z;
                }

                minX -= terrain.transform.position.x + distSmooth;
                maxX -= terrain.transform.position.x - distSmooth;

                minZ -= terrain.transform.position.z + distSmooth;
                maxZ -= terrain.transform.position.z - distSmooth;


                minX = minX * terrainToheight;
                maxX = maxX * terrainToheight;

                minZ = minZ * terrainTowidth;
                maxZ = maxZ * terrainTowidth;

                maxX = Mathf.Ceil(Mathf.Clamp(maxX + 1, 0, (terrainData.heightmapResolution)));
                minZ = Mathf.Floor(Mathf.Clamp(minZ, 0, (terrainData.heightmapResolution)));
                maxZ = Mathf.Ceil(Mathf.Clamp(maxZ + 1, 0, (terrainData.heightmapResolution)));
                minX = Mathf.Floor(Mathf.Clamp(minX, 0, (terrainData.heightmapResolution)));

                float[,] heightmapData = terrainData.GetHeights((int)minX, (int)minZ, (int)(maxX - minX), (int)(maxZ - minZ));

                Vector3 position = Vector3.zero;
                Vector3 pointMin = Vector3.zero;

                for (int x = 0; x < heightmapData.GetLength(0); x++)
                {


                    for (int z = 0; z < heightmapData.GetLength(1); z++)
                    {


                        position.x = (z + minX) / (float)terrainToheight + terrain.transform.position.x;
                        position.z = (x + minZ) / (float)terrainTowidth + terrain.transform.position.z;


                        Ray ray = new Ray(position + Vector3.up * 3000, Vector3.down);
                        RaycastHit hit;
                        if (meshCollider.Raycast(ray, out hit, 10000))
                        {

                            float height = hit.point.y - posY;
                            heightmapData[x, z] = height / (float)sizeY;

                            if (debugLines)
                            {
                                Debug.DrawLine(hit.point, hit.point + Vector3.up * 0.5f, Color.magenta, 10);

                            }


                        }

                    }
                }

                terrainData.SetHeights((int)minX, (int)minZ, heightmapData);


            }
            DestroyImmediate(meshCollider);
            terrain.Flush();
        }

        Physics.autoSyncTransforms = savedAutoSyncTransforms;





        if (meshGO != null)
            DestroyImmediate(meshGO);


    }

    public void TerrainPaintMeshBased()
    {

        bool savedAutoSyncTransforms = Physics.autoSyncTransforms;
        Physics.autoSyncTransforms = false;
        foreach (Terrain terrain in Terrain.activeTerrains)
        {

            TerrainData terrainData = terrain.terrainData;

            float sizeX = terrain.terrainData.size.x;
            float sizeY = terrain.terrainData.size.y;
            float sizeZ = terrain.terrainData.size.z;
            float terrainTowidth = (1 / (float)sizeZ * (terrainData.alphamapWidth - 1));
            float terrainToheight = (1 / (float)sizeX * (terrainData.alphamapHeight - 1));


#if UNITY_EDITOR
            Undo.RegisterCompleteObjectUndo(terrainData, "Paint river");
            Undo.RegisterCompleteObjectUndo(terrain, "Terrain draw texture");
            Undo.RegisterCompleteObjectUndo(terrainData.alphamapTextures, "alpha");
#endif

            float minX;
            float maxX;
            float minZ;
            float maxZ;

            MeshCollider meshCollider = meshGO.gameObject.AddComponent<MeshCollider>();

            List<Vector3> transformPointUp = new List<Vector3>();
            List<Vector3> transformPointDown = new List<Vector3>();

            int pointsCount = 5;
            int pointsStart = 0;//pointsUp.Count

            //List<Vector2> done = new List<Vector2>();


            //List<Vector3> positionArray = new List<Vector3>();

            Vector3 pointOne = Vector3.zero;
            Vector3 pointTwo = Vector3.zero;

            for (pointsStart = 0; pointsStart < pointsUp.Count; pointsStart = Mathf.Clamp(pointsStart + pointsCount - 1, 0, pointsUp.Count))
            {
                int end = Mathf.Min(pointsStart + pointsCount, pointsUp.Count);
                //int currentCount = Mathf.Min(pointsCount, pointsUp.Count - pointsStart);

                transformPointUp.Clear();
                transformPointDown.Clear();

                for (int i = pointsStart; i < end; i++)
                {
                    transformPointUp.Add(transform.TransformPoint(pointsUp[i]));
                    transformPointDown.Add(transform.TransformPoint(pointsDown[i]));
                }

                minX = float.MaxValue;
                maxX = float.MinValue;
                minZ = float.MaxValue;
                maxZ = float.MinValue;



                for (int i = 0; i < transformPointUp.Count; i++)
                {
                    Vector3 point = transformPointUp[i];


                    if (minX > point.x)
                        minX = point.x;

                    if (maxX < point.x)
                        maxX = point.x;

                    if (minZ > point.z)
                        minZ = point.z;

                    if (maxZ < point.z)
                        maxZ = point.z;
                }

                for (int i = 0; i < transformPointDown.Count; i++)
                {
                    Vector3 point = transformPointDown[i];


                    if (minX > point.x)
                        minX = point.x;

                    if (maxX < point.x)
                        maxX = point.x;

                    if (minZ > point.z)
                        minZ = point.z;

                    if (maxZ < point.z)
                        maxZ = point.z;
                }

                minX -= terrain.transform.position.x + distSmooth;
                maxX -= terrain.transform.position.x - distSmooth;

                minZ -= terrain.transform.position.z + distSmooth;
                maxZ -= terrain.transform.position.z - distSmooth;


                minX = minX * terrainToheight;
                maxX = maxX * terrainToheight;

                minZ = minZ * terrainTowidth;
                maxZ = maxZ * terrainTowidth;

                minX = Mathf.Floor(Mathf.Clamp(minX, 0, (terrainData.alphamapWidth)));
                maxX = Mathf.Ceil(Mathf.Clamp(maxX + 1, 0, (terrainData.alphamapWidth)));
                minZ = Mathf.Floor(Mathf.Clamp(minZ, 0, (terrainData.alphamapHeight)));
                maxZ = Mathf.Ceil(Mathf.Clamp(maxZ + 1, 0, (terrainData.alphamapHeight)));

                float[,,] alphamapData = terrainData.GetAlphamaps((int)minX, (int)minZ, (int)(maxX - minX), (int)(maxZ - minZ));

                Vector3 position = Vector3.zero;
                Vector3 pointMin = Vector3.zero;
                float noise = 0;
                for (int x = 0; x < alphamapData.GetLength(0); x++)
                {


                    for (int z = 0; z < alphamapData.GetLength(1); z++)
                    {


                        position.x = (z + minX) / (float)terrainToheight + terrain.transform.position.x;
                        position.z = (x + minZ) / (float)terrainTowidth + terrain.transform.position.z;


                        Ray ray = new Ray(position + Vector3.up * 3000, Vector3.down);
                        RaycastHit hit;
                        if (meshCollider.Raycast(ray, out hit, 10000))
                        {
                            float minDist = hit.textureCoord.x;
                            if (!mixTwoSplatMaps)
                            {


                                if (noisePaint)
                                {
                                    if (minDist >= 0)
                                        noise = Mathf.PerlinNoise(hit.point.x * noiseSizeXPaint, hit.point.z * noiseSizeZPaint) * noiseMultiplierInsidePaint - noiseMultiplierInsidePaint * 0.5f;
                                    else
                                        noise = Mathf.PerlinNoise(hit.point.x * noiseSizeXPaint, hit.point.z * noiseSizeZPaint) * noiseMultiplierOutsidePaint - noiseMultiplierOutsidePaint * 0.5f;
                                }
                                else
                                    noise = 0;
                                float oldValue = alphamapData[x, z, currentSplatMap];

                                alphamapData[x, z, currentSplatMap] = Mathf.Clamp01(Mathf.Lerp(alphamapData[x, z, currentSplatMap], 1, terrainPaintCarve.Evaluate(minDist) + terrainPaintCarve.Evaluate(minDist) * noise));


                                for (int l = 0; l < terrainData.terrainLayers.Length; l++)
                                {
                                    if (l != currentSplatMap)
                                    {
                                        alphamapData[x, z, l] = oldValue == 1 ? 0 : Mathf.Clamp01(alphamapData[x, z, l] * ((1 - alphamapData[x, z, currentSplatMap]) / (1 - oldValue)));

                                    }
                                }
                            }
                            else
                            {


                                if (minDist >= 0)
                                    noise = Mathf.PerlinNoise(hit.point.x * noiseSizeXPaint, hit.point.z * noiseSizeZPaint) * noiseMultiplierInsidePaint - noiseMultiplierInsidePaint * 0.5f;
                                else
                                    noise = Mathf.PerlinNoise(hit.point.x * noiseSizeXPaint, hit.point.z * noiseSizeZPaint) * noiseMultiplierOutsidePaint - noiseMultiplierOutsidePaint * 0.5f;


                                float oldValue = alphamapData[x, z, currentSplatMap];

                                alphamapData[x, z, currentSplatMap] = Mathf.Clamp01(Mathf.Lerp(alphamapData[x, z, currentSplatMap], 1, terrainPaintCarve.Evaluate(minDist)));


                                for (int l = 0; l < terrainData.terrainLayers.Length; l++)
                                {
                                    if (l != currentSplatMap)
                                    {
                                        alphamapData[x, z, l] = oldValue == 1 ? 0 : Mathf.Clamp01(alphamapData[x, z, l] * ((1 - alphamapData[x, z, currentSplatMap]) / (1 - oldValue)));

                                    }
                                }

                                if (noise > 0)
                                {

                                    oldValue = alphamapData[x, z, secondSplatMap];
                                    alphamapData[x, z, secondSplatMap] = Mathf.Clamp01(Mathf.Lerp(alphamapData[x, z, secondSplatMap], 1, noise));


                                    for (int l = 0; l < terrainData.terrainLayers.Length; l++)
                                    {
                                        if (l != secondSplatMap)
                                        {
                                            alphamapData[x, z, l] = oldValue == 1 ? 0 : Mathf.Clamp01(alphamapData[x, z, l] * ((1 - alphamapData[x, z, secondSplatMap]) / (1 - oldValue)));

                                        }
                                    }
                                }

                            }


                            if (addCliffSplatMap)
                            {
                                if (minDist >= 0)
                                {
                                    float angle = Vector3.Angle(hit.normal, Vector3.up);
                                    if (angle > cliffAngle)
                                    {
                                        float oldValue = alphamapData[x, z, cliffSplatMap];
                                        alphamapData[x, z, cliffSplatMap] = cliffBlend;


                                        for (int l = 0; l < terrainData.terrainLayers.Length; l++)
                                        {
                                            if (l != cliffSplatMap)
                                            {
                                                alphamapData[x, z, l] = oldValue == 1 ? 0 : Mathf.Clamp01(alphamapData[x, z, l] * ((1 - alphamapData[x, z, cliffSplatMap]) / (1 - oldValue)));

                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    float angle = Vector3.Angle(hit.normal, Vector3.up);
                                    if (angle > cliffAngleOutside)
                                    {
                                        float oldValue = alphamapData[x, z, cliffSplatMapOutside];
                                        alphamapData[x, z, cliffSplatMapOutside] = cliffBlendOutside;


                                        for (int l = 0; l < terrainData.terrainLayers.Length; l++)
                                        {
                                            if (l != cliffSplatMapOutside)
                                            {
                                                alphamapData[x, z, l] = oldValue == 1 ? 0 : Mathf.Clamp01(alphamapData[x, z, l] * ((1 - alphamapData[x, z, cliffSplatMapOutside]) / (1 - oldValue)));

                                            }
                                        }
                                    }

                                }
                            }
                        }

                    }
                }


                terrainData.SetAlphamaps((int)minX, (int)minZ, alphamapData);


            }
            DestroyImmediate(meshCollider);
            terrain.Flush();
        }

        Physics.autoSyncTransforms = savedAutoSyncTransforms;





        if (meshGO != null)
            DestroyImmediate(meshGO);


    }
    public void TerrainClearFoliage(bool details = true)
    {

        bool savedAutoSyncTransforms = Physics.autoSyncTransforms;
        Physics.autoSyncTransforms = false;
        foreach (Terrain terrain in Terrain.activeTerrains)
        {

            TerrainData terrainData = terrain.terrainData;
            Transform transformTerrain = terrain.transform;
            float posY = terrain.transform.position.y;
            float sizeX = terrain.terrainData.size.x;
            float sizeY = terrain.terrainData.size.y;
            float sizeZ = terrain.terrainData.size.z;
            float terrainTowidth = (1 / (float)sizeX * (terrainData.detailWidth - 1));
            float terrainToheight = (1 / (float)sizeZ * (terrainData.detailHeight - 1));


#if UNITY_EDITOR
            Undo.RegisterCompleteObjectUndo(terrainData, "Paint river");
            Undo.RegisterCompleteObjectUndo(terrain, "Terrain draw texture");
#endif

            float minX;
            float maxX;
            float minZ;
            float maxZ;

            MeshCollider meshCollider = meshGO.gameObject.AddComponent<MeshCollider>();

            List<Vector3> transformPointUp = new List<Vector3>();
            List<Vector3> transformPointDown = new List<Vector3>();

            int pointsCount = 5;
            int pointsStart = 0;//pointsUp.Count

            //List<Vector2> done = new List<Vector2>();


            //List<Vector3> positionArray = new List<Vector3>();

            Vector3 pointOne = Vector3.zero;
            Vector3 pointTwo = Vector3.zero;
            Vector3 position = Vector3.zero;
            if (details)
            {
                for (pointsStart = 0; pointsStart < pointsUp.Count; pointsStart = Mathf.Clamp(pointsStart + pointsCount - 1, 0, pointsUp.Count))
                {
                    int end = Mathf.Min(pointsStart + pointsCount, pointsUp.Count);
                    //int currentCount = Mathf.Min(pointsCount, pointsUp.Count - pointsStart);

                    transformPointUp.Clear();
                    transformPointDown.Clear();

                    for (int i = pointsStart; i < end; i++)
                    {
                        transformPointUp.Add(transform.TransformPoint(pointsUp[i]));
                        transformPointDown.Add(transform.TransformPoint(pointsDown[i]));
                    }

                    minX = float.MaxValue;
                    maxX = float.MinValue;
                    minZ = float.MaxValue;
                    maxZ = float.MinValue;



                    for (int i = 0; i < transformPointUp.Count; i++)
                    {
                        Vector3 point = transformPointUp[i];


                        if (minX > point.x)
                            minX = point.x;

                        if (maxX < point.x)
                            maxX = point.x;

                        if (minZ > point.z)
                            minZ = point.z;

                        if (maxZ < point.z)
                            maxZ = point.z;
                    }

                    for (int i = 0; i < transformPointDown.Count; i++)
                    {
                        Vector3 point = transformPointDown[i];


                        if (minX > point.x)
                            minX = point.x;

                        if (maxX < point.x)
                            maxX = point.x;

                        if (minZ > point.z)
                            minZ = point.z;

                        if (maxZ < point.z)
                            maxZ = point.z;
                    }

                    minX -= terrain.transform.position.x + distSmooth;
                    maxX -= terrain.transform.position.x - distSmooth;

                    minZ -= terrain.transform.position.z + distSmooth;
                    maxZ -= terrain.transform.position.z - distSmooth;


                    minX = minX * terrainToheight;
                    maxX = maxX * terrainToheight;

                    minZ = minZ * terrainTowidth;
                    maxZ = maxZ * terrainTowidth;

                    minX = Mathf.Floor(Mathf.Clamp(minX, 0, (terrainData.alphamapWidth)));
                    maxX = Mathf.Ceil(Mathf.Clamp(maxX + 1, 0, (terrainData.alphamapWidth)));
                    minZ = Mathf.Floor(Mathf.Clamp(minZ, 0, (terrainData.alphamapHeight)));
                    maxZ = Mathf.Ceil(Mathf.Clamp(maxZ + 1, 0, (terrainData.alphamapHeight)));

                    int[,] detailLayer;// = terrainData.GetAlphamaps((int)minX, (int)minZ, (int)(maxX - minX), (int)(maxZ - minZ));


                    for (int l = 0; l < terrainData.detailPrototypes.Length; l++)
                    {
                        detailLayer = terrainData.GetDetailLayer((int)minX, (int)minZ, (int)(maxX - minX), (int)(maxZ - minZ), l);

                        for (int x = 0; x < detailLayer.GetLength(0); x++)
                        {

                            for (int z = 0; z < detailLayer.GetLength(1); z++)
                            {


                                position.x = (z + minX) / (float)terrainToheight + terrain.transform.position.x;
                                position.z = (x + minZ) / (float)terrainTowidth + terrain.transform.position.z;


                                Ray ray = new Ray(position + Vector3.up * 3000, Vector3.down);
                                RaycastHit hit;
                                if (meshCollider.Raycast(ray, out hit, 10000))
                                {
                                    detailLayer[x, z] = 0;


                                }

                            }
                        }


                        terrainData.SetDetailLayer((int)minX, (int)minZ, l, detailLayer);
                    }



                }
            }
            else
            {
                List<TreeInstance> newTrees = new List<TreeInstance>();
                TreeInstance[] oldTrees = terrainData.treeInstances;
                foreach (var tree in oldTrees)
                {
                    //Debug.DrawRay(new Vector3(, 0, tree.position.z * sizeZ) + terrain.transform.position, Vector3.up * 5, Color.red, 3);

                    position.x = tree.position.x * sizeX + transformTerrain.position.x;//, polygonHeight
                    position.z = tree.position.z * sizeZ + transformTerrain.position.z;

                    Ray ray = new Ray(position + Vector3.up * 3000, Vector3.down);
                    RaycastHit hit;

                    if (!meshCollider.Raycast(ray, out hit, 10000))
                    {
                        newTrees.Add(tree);
                    }
                }
                terrainData.treeInstances = newTrees.ToArray();
            }

            DestroyImmediate(meshCollider);
            terrain.Flush();
        }

        Physics.autoSyncTransforms = savedAutoSyncTransforms;





        if (meshGO != null)
            DestroyImmediate(meshGO);


    }

    #endregion

    float FlowCalculate(float u, float normalY, Vector3 vertice)
    {
        float noise = (noiseflowMap ? Mathf.PerlinNoise(vertice.x * noiseSizeXflowMap, vertice.z * noiseSizeZflowMap) * noiseMultiplierflowMap - noiseMultiplierflowMap * 0.5f : 0) * Mathf.Pow(Mathf.Clamp(normalY, 0, 1), 5);
        return Mathf.Lerp(flowWaterfall.Evaluate(u), flowFlat.Evaluate(u) + noise, Mathf.Clamp(normalY, 0, 1));
    }


}