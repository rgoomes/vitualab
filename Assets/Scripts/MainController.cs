using UnityEngine;
using System.Collections;
using Leap;

public class MainController : MonoBehaviour {
	public GameObject controlPanel;
	public GameObject optionsPanel;
	public GameObject successPanel;

	StateController sc; /* for menus */
	Controller ct; /* to control leapmotion */
	
	// This can be later used if leapmotion is buggy
	/*
	const int MIN_FREQ = 5;
	int horzFreq;
	int vertFreq;
	*/
	
	void Start(){
		ct = new Controller();
		ct.EnableGesture(Gesture.GestureType.TYPESWIPE);
		ct.Config.SetFloat("Gesture.Swipe.MinLength", 200.0f);
		ct.Config.SetFloat("Gesture.Swipe.MinVelocity", 100.0f);
		ct.Config.Save();

		sc = new StateController(controlPanel, optionsPanel, successPanel);

		// horzFreq = vertFreq = 0;
	}

	public StateController getStateController(){
		return this.sc;
	}

	void upSwipe(){
		sc.showOptions();
	}

	void downSwipe(){
		sc.hideOptions();
	}

	void leftSwipe(){
		sc.enterTutorial();
		sc.nextTutorial();
	}

	void rightSwipe(){
		sc.skipTutorial();
		sc.backToMainMenu();
	}

	void listenGestures(){
		Frame frame = ct.Frame();
		GestureList list = frame.Gestures();
		
		for(int i = 0; i < list.Count; i++){
			Gesture g = list[i];
			
			if(g.Type == Gesture.GestureType.TYPESWIPE){
				SwipeGesture Swipe = new SwipeGesture(g);
				Vector swipeDirection = Swipe.Direction;
				
				if(swipeDirection.x < 0)
					leftSwipe();
				else if(swipeDirection.x > 0)
					rightSwipe();
				else if(swipeDirection.y < 0)
					downSwipe();
				else if(swipeDirection.y > 0)
					upSwipe();
			}
		}
	}
	
	void Update(){
		// Listen do leapmotion gestures
		listenGestures();

		// Test show/hide options. TODO: REMOVE THIS LATER
		if(Input.GetKeyDown(KeyCode.LeftControl) == true )
			upSwipe();
		if(Input.GetKeyDown(KeyCode.RightControl) == true )
			downSwipe();

		// Test skip/enter tutorial. TODO: REMOVE THIS LATER
		if(Input.GetKeyDown(KeyCode.LeftShift) == true )
			leftSwipe();
		if(Input.GetKeyDown(KeyCode.RightShift) == true )
			rightSwipe();

		if(Input.GetKeyDown(KeyCode.Space) == true )
			sc.successTutorial();

		// TODO: REMOVE THIS LATER, ONLY FOR PRESENTATION
		if(sc.getPlace() == 6 && sc.canAnimate()){
			this.transform.position = new Vector3(
				-1700, transform.position.y, transform.position.z
			);
		} else if(sc.getPlace() == 0 && sc.canAnimate()){
			this.transform.position = new Vector3(
				-500, transform.position.y, transform.position.z
			);
		}
	}
}
