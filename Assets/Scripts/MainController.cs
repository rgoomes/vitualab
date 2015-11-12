using UnityEngine;
using System.Collections;
using Leap;

public class MainController : MonoBehaviour {
	public GameObject controlPanel;
	public GameObject optionsPanel;
	public GameObject successPanel;

	StateController sc; /* for menus */
	Controller ct; /* to control leapmotion */

	const int LFPS = 30; /* leapmotion frame every x game frames */
	Frame lastFrame; /* the frame to detect motion */
	int frameCounter; /* counter from 0 to fps */
	
	void Start(){
		ct = new Controller();
		ct.EnableGesture(Gesture.GestureType.TYPESWIPE);
		ct.EnableGesture(Gesture.GestureType.TYPECIRCLE);
		ct.Config.Save();

		sc = new StateController(controlPanel, optionsPanel, successPanel);

		lastFrame = null;
		frameCounter = 0;
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

				bool isHorizontal = Mathf.Abs(Swipe.Direction.x) > Mathf.Abs(Swipe.Direction.y);
				if (isHorizontal){
					if (Swipe.Direction.x > 0)
						rightSwipe();
					else
						leftSwipe();
				}
				else{
					if (Swipe.Direction.y > 0)
						upSwipe();
					else
						downSwipe();
				}
			} else if(g.Type == Gesture.GestureType.TYPECIRCLE){
				// TODO: Rotate object with circular finger movement
			}
		}
	}

	void detectMotion(){
		if(!isTick())
			return;

		Frame frame = ct.Frame();
		HandList hands = frame.Hands;

		/* scale gesture requires two hands */
		if(hands.Count != 2)
			return;

		float scaleFactor = hands[0].ScaleFactor(lastFrame)+hands[1].ScaleFactor(lastFrame);
		scaleFactor /= 2; /* use average scale factor between both hands */

		const float minScaleFactor = 0.25f;
		if(Mathf.Abs(scaleFactor) > minScaleFactor){
			// TODO: Apply rotation to selected object;
		}

	}

	void kbInputTest(){
		if(Input.GetKeyDown(KeyCode.LeftControl) == true )
			upSwipe();
		if(Input.GetKeyDown(KeyCode.RightControl) == true )
			downSwipe();
		if(Input.GetKeyDown(KeyCode.LeftShift) == true )
			leftSwipe();
		if(Input.GetKeyDown(KeyCode.RightShift) == true )
			rightSwipe();
		if(Input.GetKeyDown(KeyCode.Space) == true )
			sc.successTutorial();
	}

	bool isTick(){
		return frameCounter == 0;
	}

	void updateFrame(){
		// TODO FIX: take into account frame delta
		frameCounter = ++frameCounter % LFPS;

		if(isTick())
			lastFrame = ct.Frame();
	}

	void Update(){
		// Update leapmotion frame
		updateFrame();

		// Listen do leapmotion gestures
		listenGestures();
		// Detect to leapmotion motion
		detectMotion();

		// Debug only
		kbInputTest();
	}
}
