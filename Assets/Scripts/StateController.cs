﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateController {
	const int SUCCESS_SCREEN = -2;
	const int OPTIONS_SCREEN = -1;
	const int MAIN_SCREEN    =  0;
	const int TUT0_SCREEN    =  1; // ZOOM
	const int TUT1_SCREEN    =  2; // ROTATE
	const int TUT2_SCREEN    =  3; // CHANGE
	const int TUT3_SCREEN    =  4; // PICK
	const int TUT4_SCREEN    =  5; // CIRCULAR
	const int LABORATORY     =  6;

	int place, last_place;
	
	GameObject controlPanel;
	GameObject optionsPanel;
	GameObject successPanel;

	List<Animation> animations;
	List<string> tutorialAnimations, toMainAnimations;
	
	public StateController(GameObject cp, GameObject op, GameObject sp){
		this.place = last_place = MAIN_SCREEN;
		this.controlPanel = cp;
		this.optionsPanel = op;
		this.successPanel = sp;

		animations = new List<Animation>();
		this.animations.Add(op.GetComponent<Animation>());
		this.animations.Add(cp.GetComponent<Animation>());
		this.animations.Add(sp.GetComponent<Animation>());

		tutorialAnimations = new List<string>(new string[] {
			"goto_rotate", "goto_switch", "goto_pick", "goto_circular"
		});

		toMainAnimations = new List<string>(new string[] {
			"",
			"main_from_zoom", "main_from_rotate",   "main_from_switch",
			"main_from_pick", "main_from_circular", "main_from_lab"
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
		controlPanel.GetComponent<Animation>().Play(tutorialAnimations[getPlace() - 2]);
	}

	public void successTutorial(){
		if(!this.canAnimate())
			return;
		if(getPlace() < TUT0_SCREEN || getPlace() > TUT3_SCREEN)
			return;

		successPanel.GetComponent<Animation>().Play("success");
	}

	public void endedTutorial(){
		if(!this.canAnimate())
			return;
		if(getPlace() != TUT4_SCREEN)
			return;

		this.setLastPlace(TUT4_SCREEN);
		this.setPlace(LABORATORY);
		// ANIMATION: CONGRATULATIONS YOU'VE MASTERED THE LEAPMOTION..
	}

	public void backToMainMenu(){
		if(!this.canAnimate())
			return;
		if(getPlace() != OPTIONS_SCREEN)
			return;
		if(getLastPlace() < MAIN_SCREEN || getLastPlace() > LABORATORY)
			return;

		int old_place = getPlace();
		int old_last = getLastPlace();
		this.setLastPlace(old_place);
		this.setPlace(MAIN_SCREEN);

		optionsPanel.GetComponent<Animation>().Play("hide_options");

		if(old_last == MAIN_SCREEN)
			return;

		controlPanel.GetComponent<Animation>().Play(toMainAnimations[old_last]);
	}
}