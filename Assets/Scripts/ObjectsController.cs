using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectsController {

	const float MIN_SCALE_FACTOR =  50.0f;
	const float MAX_SCALE_FACTOR = 150.0f;

	List<GameObject> labObjects;
	int cur_obj;
	
	bool setObject;

	public ObjectsController(){
		setObject = false;
		labObjects = new List<GameObject>();
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

	public void addObject(GameObject go){
		this.labObjects.Add(go);
	}

	public GameObject getCurObject(){
		return this.labObjects[cur_obj];
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

	public void rotateObject(float radius, int clockwise){
		float delta = Time.deltaTime*100;
		getCurObject().transform.Rotate(clockwise*Vector3.up, radius*delta);
	}

	public float getCurObjScaleFactor(){
		return getCurObject().transform.localScale.x;
	}

	public void scaleObject(float scaleFactor){
		float delta = Time.deltaTime*100;

		if(getCurObjScaleFactor()+scaleFactor*delta < MIN_SCALE_FACTOR)
			return;
		if(getCurObjScaleFactor()+scaleFactor*delta > MAX_SCALE_FACTOR)
			return;

		getCurObject().transform.localScale += new Vector3(scaleFactor*delta, scaleFactor*delta, scaleFactor*delta);
	}
}
