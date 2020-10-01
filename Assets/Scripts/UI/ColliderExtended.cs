using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//--Class purpose-- 
//Contains UI logic for each node
//Instances are attached to the gameobjects generated under UI->ColliderContainer
//Responsible for the Mouseover text popup
public class ColliderExtended : MonoBehaviour {
    public MapGraph.MapNode node; //refers to the node that the collider represents
    public TextMesh text; //TextMesh Object for showing the node's properties
    bool mouseOver;
    ColliderManager cManager;

    public void SetData(MapGraph.MapNode node, SphereCollider collider, TextMesh text, ColliderManager cManager) {
        this.node = node;
        this.cManager = cManager;
        gameObject.transform.position = node.centerPoint;
        switch (node.nodeType) { //broken, never triggered
            case MapGraph.MapNodeType.Mountain:
                collider.transform.position = new Vector3(collider.transform.position.x, 3, collider.transform.position.z);
                break;
            case MapGraph.MapNodeType.Snow:
                Debug.Log("RAISING"); 
                collider.transform.position = new Vector3(collider.transform.position.x, 7, collider.transform.position.z);
                break;
            case MapGraph.MapNodeType.SaltWater:

                break;
            case MapGraph.MapNodeType.FreshWater:

                break;
        }      
        collider.radius = 4;
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
        }       
    }
    void OnMouseExit() {
        mouseOver = false;
        if (this.text != null) {
            this.text.text = ""; //hide the inactive TextMeshes by setting their text to empty
        }
    }
}
