using UnityEngine;
using System.Collections;

public class TextAnimation : MonoBehaviour {

	float x; /* new x of animated text */
	float curx; /* initial x of text */
	float acc; /* accelaration */
	float radius; /* bound of animation */
	float speed; /* animation speed */

	int pos; /* object position left or right */

	// Use this for initialization
	void Start () {
		x = curx = this.transform.position.x;
		pos = (x > Screen.width/2) ? 1 : -1;

		acc = 0; /* initial accelaration */
		speed = 70; /* speed is between 1 and 100 */
		radius = 20; /* 40 world total */
	}
	
	// Update is called once per frame
	void Update () {
		float delta = Time.deltaTime * 100;
		x = curx + Mathf.Cos(acc) * pos*radius*delta;
		acc += Mathf.PI / (100 - speed);

		this.transform.position = new Vector3(x, transform.position.y, transform.position.z);
	}
}
