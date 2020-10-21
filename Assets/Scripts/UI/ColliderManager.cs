using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//--Class purpose-- 
//Manages instances of ColliderExtended.cs 
//Singular instance is attached to ColliderContainer
//Responsible for Bottom-Left UI
public class ColliderManager : MonoBehaviour {    
    private List<GameObject> colliderObjects = new List<GameObject>(); //list of all generated GameObjects, to delete them later
    public MapGeneratorPreview mapGenerator; //Reference so that the UI button can trigger generation
    private MapGraph.MapNode activeNode;

    private void Start() {
        //ClearColliders();
    }
    public void SetActiveNode(MapGraph.MapNode node) {
        this.activeNode = node;
    }
    public MapGraph.MapNode GetActiveNode() {
        return activeNode;
    }
    public void AddCollider(MapGraph.MapNode node) { //for generating a collider for each node
        GameObject colliderContainer = new GameObject();
        colliderContainer.transform.parent = this.transform; //sets as child in scene hierarchy
        ColliderExtended cExt = colliderContainer.AddComponent(typeof(ColliderExtended)) as ColliderExtended;
        cExt.SetData(node, this);                      
        colliderObjects.Add(colliderContainer);
    }
    public void ClearColliders() { //deletes all generated colliders by destroying their gameobjects; used for cleanup of old generation
        foreach(GameObject obj in colliderObjects) {
            DestroyImmediate(obj);
        }
        colliderObjects.Clear();      
    }
    private void OnGUI() {
        GUI.skin.label.fontSize = 16;
        if (GUI.Button(new Rect(10, 10, 160, 30), "Generate Random Seed")) { //Button that trigger random seed generation
            mapGenerator.seed = (int)Random.Range(0, 100);
            mapGenerator.Start();
        }
    }
}
