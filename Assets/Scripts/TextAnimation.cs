using UnityEngine;
using System.Collections;

public class TextAnimation : MonoBehaviour {

	public float speed; /* animation speed */
	public float radius; /* bound of animation */
	public int pos; /* object position left or right */

	float x, y; /* new x of animated text */
	float curx, cury; /* initial x and y of text */
	float acc; /* accelaration */

	// Use this for initialization
	void Start () {
		x = curx = this.transform.position.x;
		y = cury = this.transform.position.y;

		acc = 0; /* initial accelaration */
	}
	
	// Update is called once per frame
	void Update () {
		float delta = Time.deltaTime * 100;
		acc += Mathf.PI / (100 - speed);

		if(pos != 0) { /* if in left or right position */
			x = curx + Mathf.Cos(acc) * pos*radius*delta;
			this.transform.position = new Vector3(x, transform.position.y, transform.position.z);
		} else { /* in middle */
			y = cury + Mathf.Cos(acc) * radius*delta;
			this.transform.position = new Vector3(transform.position.x, y, transform.position.z);
		}
	}
}
