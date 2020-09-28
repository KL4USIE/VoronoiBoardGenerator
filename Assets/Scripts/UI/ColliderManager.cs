using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderManager : MonoBehaviour {
    
    public List<GameObject> colliderObjects = new List<GameObject>();
    public MapGeneratorPreview mapGenerator;

    void Start()
    {
        Debug.Log(colliderObjects);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddCollider(MapGraph.MapNode node) {
        GameObject colliderContainer = new GameObject();
        colliderContainer.transform.parent = this.transform;
        /*
                PolygonCollider2D collider = new PolygonCollider2D();
                List<Vector2> vecList = new List<Vector2>();
                foreach (var point in node.GetCorners()) {
                    vecList.Add(new Vector2(point.position.x, point.position.z));
                }
                Vector2[] pointArray = vecList.ToArray();
                //collider.points = pointArray;
                //graph.colliderList.Add(new ColliderExtended(node, collider));

                colliderContainer.AddComponent<PolygonCollider2D>();
                colliderContainer.GetComponent<PolygonCollider2D>().points = pointArray;

                colliderContainer.AddComponent<ColliderExtended>();
                colliderContainer.transform.parent = this.transform;
                */
        SphereCollider collider = new SphereCollider();
        
        //collider.points = pointArray;
        //graph.colliderList.Add(new ColliderExtended(node, collider));

        colliderContainer.AddComponent<SphereCollider>();
        //colliderContainer.GetComponent<SphereCollider>().center = new Vector3(node.centerPoint.x, 0, node.centerPoint.z);
        //colliderContainer.GetComponent<SphereCollider>().radius = 4;
        colliderContainer.AddComponent<ColliderExtended>();
        //colliderContainer.GetComponent<ColliderExtended>().SetData(node, colliderContainer.GetComponent<SphereCollider>());
        colliderContainer.AddComponent<TextMesh>();
        colliderContainer.GetComponent<ColliderExtended>().SetData2(node, colliderContainer.GetComponent<SphereCollider>(), colliderContainer.GetComponent<TextMesh>());
        //ColliderExtended cExt = new ColliderExtended();
        //cExt.GetCollider().center = node.centerPoint;
        //cExt.GetCollider().radius = 4;
        //colliderContainer.AddComponent<ColliderExtended>();
        colliderObjects.Add(colliderContainer);
    }
    public void ClearColliders() {
        foreach(GameObject obj in colliderObjects) {
            DestroyImmediate(obj);
        }
        colliderObjects.Clear();
        
    }
    private void OnGUI() {
        if (GUI.Button(new Rect(10, 10, 160, 30), "Generate Random Seed")) {
            mapGenerator.seed = (int)Random.Range(0, 100);
            mapGenerator.Start();
        }
    }
}
