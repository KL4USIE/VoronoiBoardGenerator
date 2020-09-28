using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderExtended : MonoBehaviour
{
    public MapGraph.MapNode node;
    public SphereCollider collider;
    public TextMesh text;
    bool mouseOver;

    public void SetData1(MapGraph.MapNode node, SphereCollider collider) {
        this.node = node;
        this.collider = collider;
        this.collider.center = node.centerPoint;
        this.collider.radius = 4;
       
        mouseOver = false;
    }
    public void SetData2(MapGraph.MapNode node, SphereCollider collider, TextMesh text) {
        this.node = node;
        gameObject.transform.position = node.centerPoint;
        this.collider = collider;       
        this.collider.radius = 4;
        this.text = text;
        this.text.alignment = TextAlignment.Center;
        this.text.anchor = TextAnchor.LowerCenter;
        this.text.fontStyle = FontStyle.Bold;
        this.text.characterSize = 5;
        mouseOver = false;       
    }
    public SphereCollider GetCollider() {
        return this.collider;
    }   

    // Update is called once per frame
    void Update() {
        if(mouseOver) {
            //Debug.Log("UPDATE");
            text.transform.rotation = Quaternion.LookRotation(text.transform.position - Camera.main.transform.position);
        }
    }
    private void OnMouseEnter() {
        mouseOver = true;
        this.text.text = node.nodeType.ToString();
    }
    void OnMouseExit() {
        mouseOver = false;
        this.text.text = "";
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
