using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//--Class purpose-- 
//Manages instances of ColliderExtended.cs 
//Singular instance is attached to ColliderContainer
//Responsible for Bottom-Left UI
public class ColliderManager : MonoBehaviour {    
    List<GameObject> colliderObjects = new List<GameObject>(); //list of all generated GameObjects, to delete them later
    public MapGeneratorPreview mapGenerator; //Reference so that the UI button can trigger generation
    MapGraph.MapNode activeNode;

    public void SetActiveNode(MapGraph.MapNode node) {
        this.activeNode = node;
    }
    public MapGraph.MapNode GetActiveNode() {
        return activeNode;
    }
    public void AddCollider(MapGraph.MapNode node) { //for generating a collider for each node
        GameObject colliderContainer = new GameObject();
        colliderContainer.transform.parent = this.transform; //sets as child in scene hierarchy
        SphereCollider collider = new SphereCollider();
        colliderContainer.AddComponent<SphereCollider>();
        colliderContainer.AddComponent<ColliderExtended>();
        colliderContainer.AddComponent<TextMesh>();
        colliderContainer.GetComponent<ColliderExtended>().SetData(node, colliderContainer.GetComponent<SphereCollider>(), colliderContainer.GetComponent<TextMesh>(), this);
        colliderContainer.layer = 5;
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
