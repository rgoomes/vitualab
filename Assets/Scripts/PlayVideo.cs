using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayVideo : MonoBehaviour {

	public GameObject scriptController;
	public MovieTexture movie;
	public int place;

	float elapsed = 0;

	// Use this for initialization
	void Start () {
		GetComponent<RawImage>().texture = movie as MovieTexture;
		movie.loop = true;
		movie.Play();
	}
	
	// Update is called once per frame
	void Update () {

		/* stop all movies after 1 second */
		if(elapsed > 1){

			/* this fixs low fps */
			StateController sc = scriptController.GetComponent<MainController>().getStateController();

			if(sc.getPlace() != place){
				movie.Stop();
			} else if(!movie.isPlaying)
				movie.Play();
		}

		elapsed += Time.deltaTime;
	}
}