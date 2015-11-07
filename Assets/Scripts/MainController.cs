using UnityEngine;
using System.Collections;
using Leap;

public class MainController : MonoBehaviour {
	public GameObject controlPanel;
	public GameObject optionsPanel;

	Controller ct;
	StateController sc;
	
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

		sc = new StateController(controlPanel, optionsPanel);

		// horzFreq = vertFreq = 0;
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
			sc.showOptions();
		if(Input.GetKeyDown(KeyCode.RightControl) == true )
			sc.hideOptions();

		// Test skip/enter tutorial. TODO: REMOVE THIS LATER
		if(Input.GetKeyDown(KeyCode.LeftShift) == true )
			leftSwipe();
		if(Input.GetKeyDown(KeyCode.RightShift) == true )
			sc.skipTutorial();
	}
}
