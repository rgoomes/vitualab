using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

public class StateController {
	const int SUCCESS_SCREEN = -2;
	const int OPTIONS_SCREEN = -1;
	const int MAIN_SCREEN    =  0;
	const int TUT0_SCREEN    =  1; // ZOOM
	const int TUT1_SCREEN    =  2; // ROTATE
	const int TUT2_SCREEN    =  3; // CHANGE
	const int TUT3_SCREEN    =  4; // TAP
	const int TUT4_SCREEN    =  5; // CIRCULAR
	const int LABORATORY     =  6;

	int place, last_place;
	
	const int RIGHT = -1; /* right direction */
	const int LEFT	=  1; /* left direction */

	GameObject controlPanel;
	GameObject optionsPanel;
	GameObject successPanel;
	GameObject congratzPanel;
	GameObject soundIcon;
	GameObject successSound;
	GameObject descriptionText;
	GameObject previewImages;
	GameObject leapObject;
	GameObject discoCamera;
	GameObject bellSound;
	GameObject backgroundImage;

	List<Animation> animations;
	List<string> tutorialAnimations, toMainAnimations;

	ObjectsController oc;

	int volume;
	float bloom;

	public StateController(GameObject cp, GameObject op, GameObject sp, GameObject gp, GameObject si,
	                       GameObject ss, GameObject dt, GameObject pp, GameObject lm, GameObject dc,
	                       GameObject bs, GameObject bi){

		this.place = last_place = MAIN_SCREEN;
		this.controlPanel = cp;
		this.optionsPanel = op;
		this.successPanel = sp;
		this.congratzPanel = gp;
		this.soundIcon = si;
		this.successSound = ss;
		this.descriptionText = dt;
		this.previewImages = pp;
		this.leapObject = lm;
		this.discoCamera = dc;
		this.bellSound = bs;
		this.backgroundImage = bi;

		volume = 100;
		bloom  = 0;

		oc = new ObjectsController();

		animations = new List<Animation>();
		this.animations.Add(op.GetComponent<Animation>());
		this.animations.Add(cp.GetComponent<Animation>());
		this.animations.Add(sp.GetComponent<Animation>());
		this.animations.Add(gp.GetComponent<Animation>());

		tutorialAnimations = new List<string>(new string[] {
			"goto_rotate", "goto_switch", "goto_pick", "goto_circular"
		});

		toMainAnimations = new List<string>(new string[] {
			"",
			"main_from_zoom", "main_from_rotate",   "main_from_switch",
			"main_from_pick", "main_from_circular", "main_from_lab"
		});
	}

	public ObjectsController getObjectController(){
		return this.oc;
	}

	public void addLabObject(GameObject go, string description){
		oc.addObject(go, description);
		animations.Add(go.GetComponent<Animation>());
	}
	
	public int getLastPlace(){
		return this.last_place;
	}
	
	public void setLastPlace(int new_last_place){
		this.last_place = new_last_place;
	}
	
	public int getPlace(){
		return this.place;
	}
	
	public void setPlace(int new_place){
		this.place = new_place;
	}
	
	public bool canAnimate(){
		for(int i = 0; i < animations.Count; i++)
			if(animations[i].isPlaying)
				return false;
		
		return true;
	}
	
	public void showOptions(){
		if(!this.canAnimate())
			return;
		if(getPlace() == OPTIONS_SCREEN)
			return;

		if(getPlace() == MAIN_SCREEN)
			uphideLeapmotion();
		
		this.setLastPlace(getPlace());
		this.setPlace(OPTIONS_SCREEN);
		optionsPanel.GetComponent<Animation>().Play("show_options");
	}
	
	public void hideOptions(){
		if(!this.canAnimate())
			return;
		if(getPlace() != OPTIONS_SCREEN)
			return;

		if(getLastPlace() == MAIN_SCREEN)
			downshowLeapmotion();
		
		this.setPlace(this.getLastPlace());
		this.setLastPlace(OPTIONS_SCREEN);
		optionsPanel.GetComponent<Animation>().Play("hide_options");
	}

	public void skipTutorial(){
		if(!this.canAnimate())
			return;
		if(getPlace() != MAIN_SCREEN)
			return;

		oc.setSetObject(true);
		hideLeapmotion();

		backgroundImage.GetComponent<Animation>().Play("backgroundskip");

		this.setPlace(LABORATORY);
		this.setLastPlace(MAIN_SCREEN);
		controlPanel.GetComponent<Animation>().Play("skip_tutorial");
	}

	public void enterTutorial(){
		if(!this.canAnimate())
			return;
		if(getPlace() != MAIN_SCREEN)
			return;

		leapObject.GetComponent<Animation>().Play("lefthide");
		backgroundImage.GetComponent<Animation>().Play("backgroundenter");

		this.setPlace(TUT0_SCREEN);
		this.setLastPlace(MAIN_SCREEN);
		controlPanel.GetComponent<Animation>().Play("enter_tutorial");
	}

	public void nextTutorial(){
		if(!this.canAnimate())
			return;
		if(getPlace() < TUT0_SCREEN || getPlace() > TUT3_SCREEN)
			return;

		this.setLastPlace(getPlace());
		this.setPlace(getPlace()+1);
		controlPanel.GetComponent<Animation>().Play(tutorialAnimations[getPlace() - 2]);
	}

	public void playSuccessSound(){
		AudioSource audio = successSound.GetComponent<AudioSource>();
		audio.volume = getVolume() / 100.0f;
		audio.Play();
	}

	public void successTutorial(){
		if(!this.canAnimate())
			return;
		if(getPlace() < TUT0_SCREEN || getPlace() > TUT3_SCREEN)
			return;

		this.setLastPlace(getPlace());
		this.setPlace(getPlace()+1);
		successPanel.GetComponent<Animation>().Play("success");
		controlPanel.GetComponent<Animation>().Play(tutorialAnimations[getPlace() - 2]);

		playSuccessSound();
	}

	public void endedTutorial(){
		if(!this.canAnimate())
			return;
		if(getPlace() != TUT4_SCREEN)
			return;

		oc.setSetObject(true);

		this.setLastPlace(TUT4_SCREEN);
		this.setPlace(LABORATORY);

		congratzPanel.GetComponent<Animation>().Play("success");
		controlPanel.GetComponent<Animation>().Play("goto_lab");

		playSuccessSound();
	}

	public void backToMainMenu(){
		if(!this.canAnimate())
			return;
		if(getPlace() != OPTIONS_SCREEN)
			return;
		if(getLastPlace() < MAIN_SCREEN || getLastPlace() > LABORATORY)
			return;

		if(getLastPlace() != MAIN_SCREEN) {
			showLeapmotion();
			backgroundImage.GetComponent<Animation>().Play("backgroundmain");
		} else
			downshowLeapmotion();

		if(getLastPlace() == LABORATORY)
			oc.hideLabObject();

		int old_last = getLastPlace();
		this.setLastPlace(getPlace());
		this.setPlace(MAIN_SCREEN);

		optionsPanel.GetComponent<Animation>().Play("hide_options");

		if(old_last == MAIN_SCREEN)
			return;

		controlPanel.GetComponent<Animation>().Play(toMainAnimations[old_last]);
	}

	public void changeDescription(){
		Text desc = descriptionText.GetComponent<Text>();
		desc.text = oc.getCurObjDescription();
	}

	public void updatePreviewPanel(int dir){
		const float default_width  = 1920.0f;
		const float images_between = 150.0f;

		float desvio = Screen.width*images_between / default_width * -dir;

		previewImages.transform.position = new Vector3(
			previewImages.transform.position.x + desvio,
			previewImages.transform.position.y,
			previewImages.transform.position.z
		);
	}

	public void changeObject(int dir /* direction of swipe */ ){
		if(!this.canAnimate())
			return;
		if(getPlace() != LABORATORY)
			return;

		/* invalid direction */
		if(dir != LEFT && dir != RIGHT)
			return;

		/* on bound: no object on left or right */
		if(dir == RIGHT && oc.getCurObjPos() == 0 || dir == LEFT && oc.getCurObjPos() == oc.objectsNumber() - 1)
			oc.getCurObject().GetComponent<Animation>().Play("bound_reached" + (dir == LEFT ? "_left" : "_right"));
		else {
			int showPos = oc.getCurObjPos() + dir;
			int remoPos = oc.getCurObjPos();
			oc.setCurObjPos(showPos);

			oc.getObject(showPos).GetComponent<Animation>().Play("show_object"   + (dir == LEFT ? "_left" : "_right"));
			oc.getObject(remoPos).GetComponent<Animation>().Play("remove_object" + (dir == LEFT ? "_left" : "_right"));

			updatePreviewPanel(dir);
		}

		changeDescription();
	}

	/* simple tutorial verification */
	public void checkTutorial(int DESIRED_SCREEN){
		if(getPlace() != DESIRED_SCREEN)
			return;

		/* extra verifications here, like minimun velocity.
		   pass verification parameters in a struct or class */

		this.successTutorial();
	}

	public void rotateAnimation(float radius, int clockwise){
		if(!canAnimate())
			return;
		if(getPlace() != LABORATORY)
			return;

		oc.rotateObject(radius, clockwise);
	}

	public void scaleAnimation(float scaleFactor){
		if(!canAnimate())
			return;
		if(getPlace() != LABORATORY)
			return;

		oc.scaleObject(scaleFactor);
	}

	public bool canSetObject(){
		return oc.getSetObject() && canAnimate();
	}

	public int getVolume(){
		return this.volume;
	}

	public void changeVolume(int power){
		if(!canAnimate())
			return;
		if(getPlace() != OPTIONS_SCREEN)
			return;

		volume += power;
		volume  = Mathf.Max(0,   volume);
		volume  = Mathf.Min(100, volume);

		AudioListener.volume = volume / 100.0f;

		string val;
		if(volume == 0)
			val = "none";
		else if(volume < 45)
			val = "low";
		else if(volume < 90)
			val = "medium";
		else
			val = "high";

		string path = "Sprites/SoundIcon/128/" + val;
		soundIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>(path);
	}

	public void downshowLeapmotion(){
		leapObject.GetComponent<Animation>().Play("downshow");
	}
	
	public void uphideLeapmotion(){
		leapObject.GetComponent<Animation>().Play("uphide");
	}

	public void showLeapmotion(){
		leapObject.GetComponent<Animation>().Play("showleap");
	}

	public void hideLeapmotion(){
		leapObject.GetComponent<Animation>().Play("hideleap");
	}

	public bool rotateDisco(GameObject arco, float angle){
		if(getPlace() != LABORATORY)
			return false;
		if(oc.getCurObjPos() != 1 /* default disco pos */)
			return false;

		float delta = Time.deltaTime * 10;
		arco.transform.Rotate(Vector3.back, angle*delta);

		float inc = Random.Range(-2.65f, 4.0f);
		bloom = Mathf.Min(bloom + inc*delta, 25);
		discoCamera.GetComponent<Bloom>().bloomIntensity = bloom;

		return true;
	}

	public void playBellSound(){
		if(getPlace() != LABORATORY)
			return;
		if(oc.getCurObjPos() != 0 /* default bell pos */)
			return;

		AudioSource audio = bellSound.GetComponent<AudioSource>();
		if(audio.isPlaying)
			return;

		audio.volume = getVolume() / 100.0f;
		audio.Play();
	}

	public void terminate(){
		if(this.canAnimate() && getPlace() == OPTIONS_SCREEN)
			Application.Quit();
	}

	public void update(bool isInteracting){
		if(this.canSetObject()){
			oc.showLabObject();
			changeDescription();
			oc.setSetObject(false);
		}

		if(!isInteracting){
			float delta = Time.deltaTime * 45;
			bloom = Mathf.Max(bloom-delta, 0);
			discoCamera.GetComponent<Bloom>().bloomIntensity = bloom;
		}
	}
}
