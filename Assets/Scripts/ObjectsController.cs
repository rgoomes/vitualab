using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectsController {

	const float MIN_SCALE_FACTOR =  50.0f;
	const float MAX_SCALE_FACTOR = 150.0f;

	List<GameObject> labObjects;
	List<string> objectsDecription;
	int cur_obj;
	
	bool setObject;

	public ObjectsController(){
		setObject = false;
		labObjects = new List<GameObject>();
		objectsDecription = new List<string>();
		cur_obj = 0;
	}

	public int objectsNumber(){
		return labObjects.Count;
	}

	public int getCurObjPos(){
		return cur_obj;
	}

	public void setCurObjPos(int new_pos){
		cur_obj = new_pos;
	}

	public void addObject(GameObject go, string description){
		this.labObjects.Add(go);
		this.objectsDecription.Add(description);
	}

	public GameObject getCurObject(){
		return this.labObjects[cur_obj];
	}

	public string getCurObjDescription(){
		return objectsDecription[getCurObjPos()];
	}

	public GameObject getObject(int pos){
		return this.labObjects[pos];
	}

	public bool getSetObject(){
		return setObject;
	}

	public void setSetObject(bool value){
		setObject = value;
	}
	
	public void showLabObject(){
		getCurObject().transform.position = new Vector3(-1000, 0, getCurObject().transform.position.z);
	}
	
	public void hideLabObject(){
		getCurObject().transform.position = new Vector3(-1000, 1000, getCurObject().transform.position.z);
	}

	public void rotateObject(float angle, int clockwise){
		float delta = Time.deltaTime * 10;
		getCurObject().transform.Rotate(clockwise*Vector3.up, angle*delta);
	}

	public float getCurObjScaleFactor(){
		return getCurObject().transform.localScale.x;
	}

	public void scaleObject(float scaleFactor){
		float delta = Time.deltaTime * 10;

		int isZoomIn   = (scaleFactor < 1) ? -1 : 1;
		float scaleVal = scaleFactor * delta;

		/* reached lower and upper bounds */
		if(getCurObjScaleFactor() + isZoomIn*scaleVal < MIN_SCALE_FACTOR)
			return;
		if(getCurObjScaleFactor() + isZoomIn*scaleVal > MAX_SCALE_FACTOR)
			return;

		getCurObject().transform.localScale += isZoomIn * new Vector3(scaleVal, scaleVal, scaleVal);
	}
}
