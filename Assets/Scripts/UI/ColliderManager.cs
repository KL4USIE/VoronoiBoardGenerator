using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderManager : MonoBehaviour {    
    public List<GameObject> colliderObjects = new List<GameObject>(); //list of all generated GameObjects, to delete them later
    public MapGeneratorPreview mapGenerator; //Reference so that the UI button can trigger generation

    public void AddCollider(MapGraph.MapNode node) { //for generating a collider for each node
        GameObject colliderContainer = new GameObject();
        colliderContainer.transform.parent = this.transform; //sets as child in scene hierarchy
        SphereCollider collider = new SphereCollider();
        colliderContainer.AddComponent<SphereCollider>();
        colliderContainer.AddComponent<ColliderExtended>();
        colliderContainer.AddComponent<TextMesh>();
        colliderContainer.GetComponent<ColliderExtended>().SetData(node, colliderContainer.GetComponent<SphereCollider>(), colliderContainer.GetComponent<TextMesh>());
        colliderObjects.Add(colliderContainer);
    }
    public void ClearColliders() { //deletes all generated colliders by destroying their gameobjects; used for cleanup of old generation
        foreach(GameObject obj in colliderObjects) {
            DestroyImmediate(obj);
        }
        colliderObjects.Clear();      
    }
    private void OnGUI() {
        if (GUI.Button(new Rect(10, 10, 160, 30), "Generate Random Seed")) { //Button that trigger random seed generation
            mapGenerator.seed = (int)Random.Range(0, 100);
            mapGenerator.Start();
        }
    }
}
