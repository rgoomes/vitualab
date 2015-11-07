using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateController {
	const int OPTIONS_SCREEN = -1;
	const int MAIN_SCREEN    =  0;
	const int TUT0_SCREEN    =  1; // ZOOM
	const int TUT1_SCREEN    =  2; // ROTATE
	const int TUT2_SCREEN    =  3; // CHANGE
	const int TUT3_SCREEN    =  4; // PICK
	const int TUT4_SCREEN    =  5; // CIRCULAR
	const int LABORATORY     = 10;

	int place, last_place;
	
	GameObject controlPanel;
	GameObject optionsPanel;
	
	List<Animation> animations;
	List<string> animationStrings;
	
	public StateController(GameObject cp, GameObject op){
		this.place = last_place = MAIN_SCREEN;
		this.controlPanel = cp;
		this.optionsPanel = op;
		
		animations = new List<Animation>();
		this.animations.Add(op.GetComponent<Animation>());
		this.animations.Add(cp.GetComponent<Animation>());

		animationStrings = new List<string>(new string[] { 
			"goto_rotate", "goto_switch", "goto_pick", "goto_circular"
		});
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
		this.last_place = this.place;
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
		
		this.setLastPlace(getPlace());
		this.setPlace(OPTIONS_SCREEN);
		optionsPanel.GetComponent<Animation>().Play("show_options");
	}
	
	public void hideOptions(){
		if(!this.canAnimate())
			return;
		if(getPlace() != OPTIONS_SCREEN)
			return;
		
		this.setPlace(this.getLastPlace());
		this.setLastPlace(OPTIONS_SCREEN);
		optionsPanel.GetComponent<Animation>().Play("hide_options");
	}

	public void skipTutorial(){
		if(!this.canAnimate())
			return;
		if(getPlace() != MAIN_SCREEN)
			return;

		this.setPlace(LABORATORY);
		this.setLastPlace(MAIN_SCREEN);
		controlPanel.GetComponent<Animation>().Play("skip_tutorial");
	}

	public void enterTutorial(){
		if(!this.canAnimate())
			return;
		if(getPlace() != MAIN_SCREEN)
			return;
		
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
		controlPanel.GetComponent<Animation>().Play(animationStrings[getPlace() - 2]);
	}
}
