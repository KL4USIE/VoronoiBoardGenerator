using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderExtended : MonoBehaviour
{
    public MapGraph.MapNode node; //refers to the node that the collider represents
    //public SphereCollider collider;
    public TextMesh text; //TextMesh Object for showing the node's properties
    bool mouseOver;

    public void SetData(MapGraph.MapNode node, SphereCollider collider, TextMesh text) {
        this.node = node;       
        gameObject.transform.position = node.centerPoint;
        switch (node.nodeType) {
            case MapGraph.MapNodeType.Mountain:
                //gameObject.transform. = 7;
                break;
            case MapGraph.MapNodeType.Snow:
                Debug.Log("RAISING"); //broken, never triggered
                collider.transform.position = new Vector3(collider.transform.position.x, 7, collider.transform.position.z);
                break;
            case MapGraph.MapNodeType.SaltWater:

                break;
            case MapGraph.MapNodeType.FreshWater:

                break;
        }
        //this.collider = collider;       
        collider.radius = 4;
        //this.collider.radius = 4;
        this.text = text; //setting all properties of TextMesh vvv
        this.text.alignment = TextAlignment.Center;
        this.text.anchor = TextAnchor.LowerCenter;
        this.text.fontStyle = FontStyle.Bold;
        this.text.characterSize = 5;
        mouseOver = false;
        
    }
    /*
    public SphereCollider GetCollider() {
        return this.collider;
    }   */
    void Update() {
        if(mouseOver) { //makes the TextMesh face the camera
            text.transform.rotation = Quaternion.LookRotation(text.transform.position - Camera.main.transform.position);
        }
    }
    private void OnMouseEnter() {
        mouseOver = true;
        if(this.text != null) {
            this.text.text = node.nodeType.ToString();
        }
        
    }
    void OnMouseExit() {
        mouseOver = false;
        if (this.text != null) {
            this.text.text = ""; //hide the inactive TextMeshes by setting their text to empty
        }
    }
    /*
    void OnGUI() {

        if(mouseOver) {
            Debug.Log("NODETYPE: " + node.nodeType);
            Vector3 mousePos = Input.mousePosition;
            GUI.Label(new Rect(mousePos.x-50, mousePos.y-10, 100, 20), node.nodeType.ToString());
        }
    }
    */
}
