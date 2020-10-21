using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Contains UI logic for each node
/// Instances are attached to the gameobjects generated under UI->ColliderContainer
/// Responsible for the Mouseover text popup
/// </summary>
public class ColliderExtended : MonoBehaviour {
    public MapGraph.MapNode node; //refers to the node that the collider represents
    public TextMesh text; //TextMesh Object for showing the node's properties
    private bool mouseOver;
    private ColliderManager cManager;

    public void SetData(MapGraph.MapNode node, ColliderManager cManager) {
        SphereCollider collider = gameObject.AddComponent(typeof(SphereCollider)) as SphereCollider;
        collider.radius = 4;
        gameObject.layer = 5;
        TextMesh text = gameObject.AddComponent(typeof(TextMesh)) as TextMesh;      
        this.node = node;
        this.cManager = cManager;
        gameObject.transform.position = node.centerPoint;
        switch (node.nodeType) {
            case MapGraph.MapNodeType.Mountain:
                //Debug.Log("IS MOUNTAIN");
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, 2, gameObject.transform.position.z);
                break;
            case MapGraph.MapNodeType.Snow:
                //Debug.Log("RAISING"); 
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, 5, gameObject.transform.position.z);
                break;
            case MapGraph.MapNodeType.Highland:
                //Debug.Log("RAISING"); 
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, 5, gameObject.transform.position.z);
                break;
            case MapGraph.MapNodeType.SaltWater:
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, -7, gameObject.transform.position.z);
                break;
            case MapGraph.MapNodeType.FreshWater:
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, -7, gameObject.transform.position.z);
                break;
        }             
        this.text = text; //setting all properties of TextMesh vvv
        this.text.alignment = TextAlignment.Center;
        this.text.anchor = TextAnchor.LowerCenter;
        this.text.fontStyle = FontStyle.Bold;
        this.text.characterSize = 5;
        mouseOver = false;            
    }
    void Update() {
        if(mouseOver) { //makes the TextMesh face the camera
            text.transform.rotation = Quaternion.LookRotation(text.transform.position - Camera.main.transform.position);
        }
    }
    private void OnMouseEnter() {
        mouseOver = true;       
        cManager.SetActiveNode(node);     
        if(this.text != null) { //preventing errors during loading
            this.text.text = node.nodeType.ToString(); //display type
            if(node.secondType != MapGraph.SecondType.nothing) { //if a second type is set, display it
                this.text.text += "\n" + node.secondType.ToString();
            }
            this.text.text += "\nCost: " + node.cost; //Display cost
        }       
    }
    private void OnMouseExit() {
        mouseOver = false;
        if (this.text != null) {
            this.text.text = ""; //hide the inactive TextMeshes by setting their text to empty
        }
    }
}
