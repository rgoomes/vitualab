using UnityEngine;
using System.Collections;

public class LightAnimation : MonoBehaviour {

	const float minIntensity = 1.75f;
	const float maxIntensity = 2.25f;
	const float minAngle 	 = 0.00f;
	const float halfAngle 	 = 180.0f;
	const float maxAngle 	 = 360.0f;
	float random, mm, hh;

	// Use this for initialization
	void Start () {
		random = Random.Range(0.0f, 65535.0f);
	}
	
	// Update is called once per frame
	void Update () {
		float noise = Mathf.PerlinNoise(random, Time.time);
		this.GetComponent<Light>().intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);

		mm = Random.Range(minAngle, halfAngle);
		hh = Random.Range(halfAngle, maxAngle);
		this.transform.Rotate(Vector3.up, Mathf.Lerp(mm, hh, noise) / maxAngle);

		mm = Random.Range(minAngle, halfAngle);
		hh = Random.Range(halfAngle, maxAngle);
		this.transform.Rotate(Vector3.right, Mathf.Lerp(mm, hh, noise) / maxAngle);

		mm = Random.Range(minAngle, halfAngle);
		hh = Random.Range(halfAngle, maxAngle);
		this.transform.Rotate(Vector3.back, Mathf.Lerp(mm, hh, noise) / maxAngle);
	}
}