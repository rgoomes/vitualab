using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

public class MainController : MonoBehaviour {
	public GameObject controlPanel;
	public GameObject optionsPanel;
	public GameObject successPanel;
	public GameObject congratzPanel;
	public GameObject soundIcon;
	public GameObject successSound;
	public GameObject descriptionText;

	// Declare here all objects
	public GameObject sino;
	public GameObject discodelezenne;

	StateController sc; /* for menus */
	Controller ct; /* to control leapmotion */

	Frame curFrame = null;

	List<string> descriptions;
	
	void Start(){
		ct = new Controller();
		ct.EnableGesture(Gesture.GestureType.TYPESWIPE);
		ct.EnableGesture(Gesture.GestureType.TYPECIRCLE);
		ct.EnableGesture(Gesture.GestureType.TYPESCREENTAP);
		ct.Config.Save();

		descriptions = new List<string>(new string[] {
			"O sino etc. asdas dasd asd asdas asd as dasd 1231 23ads asd asd as as d",
			"O disco de delezenne asd123asdasd asdasd123as dasdas 123 123 asdas asd asd asd"
		});

		sc = new StateController(controlPanel, optionsPanel, successPanel, congratzPanel, soundIcon,
		                         successSound, descriptionText);

		sc.addLabObject(sino, descriptions[0]);
		sc.addLabObject(discodelezenne, descriptions[1]);
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
		/* verify tutorial */
		sc.checkTutorial(3 /* TUT2_SCREEN */);

		sc.terminate();
		sc.enterTutorial();
		sc.changeObject(1 /* left direction */);
	}

	void rightSwipe(){
		/* verify tutorial */
		sc.checkTutorial(3 /* TUT2_SCREEN */);

		sc.skipTutorial();
		sc.backToMainMenu();
		sc.changeObject(-1 /* right direction */);
	}

	void listenGestures(){
		/* cancel gestures with two hands to not interfere with
		   the other gestures (motion detection gestures) */
		if(curFrame.Hands.Count == 2)
			return;

		GestureList list = curFrame.Gestures();
		
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
				CircleGesture Circle = new CircleGesture(g);

				int clockwise = Circle.Pointable.Direction.AngleTo(Circle.Normal) <= Mathf.PI / 2 ? -1 : 1;

				/* sound power */
				int power = Mathf.RoundToInt(Circle.Radius / 10.0f /* mm to cm */ ) * clockwise;
				sc.changeVolume(power);

				/* disco angle */
				// float angle = Circle.Radius;
				// TODO: sc.rotateDisco(angle);

				/* verify tutorial */
				sc.checkTutorial(5 /* TUT4_SCREEN */);


			} else if(g.Type == Gesture.GestureType.TYPESCREENTAP){
				// TODO: To animate the bell and play the sound

				/* verify tutorial */
				sc.checkTutorial(4 /* TUT3_SCREEN */);
			}
		}
	}

	void scaleMotion(){
		/* get scale factor from the tenth frame to current frame */
		float scaleFactor = curFrame.ScaleFactor(ct.Frame(10));

		const float lowerFactor = 0.75f;
		const float upperFactor = 1.25f;

		if(scaleFactor >= lowerFactor && scaleFactor <= upperFactor)
			return;

		/* verify tutorial */
		sc.checkTutorial(1 /* TUT0_SCREEN */);

		sc.scaleAnimation(scaleFactor);
	}

	void rotateMotion(){
		/* rotation angle in degrees from the tenth frame to current frame */
		float rotationAngle = curFrame.RotationAngle(ct.Frame(10), Vector.YAxis) * (180 / Mathf.PI);

		const float minAngle = 25.0f;
		if(Mathf.Abs(rotationAngle) < minAngle)
			return;

		/* verify tutorial */
		sc.checkTutorial(2 /* TUT1_SCREEN */);

		int isClockwise = (rotationAngle < 0) ? -1 : 1;
		sc.rotateAnimation(rotationAngle, isClockwise);
	}

	void detectMotion(){
		/* motion gestures requires both hands */
		if(curFrame.Hands.Count != 2)
			return;

		scaleMotion();
		rotateMotion();
	}

	void kbInputTest(){
		/* test swipes */
		if(Input.GetKeyDown(KeyCode.UpArrow) == true )
			upSwipe();
		if(Input.GetKeyDown(KeyCode.DownArrow) == true )
			downSwipe();
		if(Input.GetKeyDown(KeyCode.LeftArrow) == true )
			leftSwipe();
		if(Input.GetKeyDown(KeyCode.RightArrow) == true )
			rightSwipe();

		/* test completeness of tutorials */
		if(Input.GetKey(KeyCode.Space) == true ){
			sc.successTutorial();
			sc.endedTutorial();
		}

		/* test motion */
		if(Input.GetKey(KeyCode.R) == true )
			sc.rotateAnimation(2.0f, 1);
		if(Input.GetKey(KeyCode.A) == true )
			sc.scaleAnimation(1.5f);
		if(Input.GetKey(KeyCode.D) == true )
			sc.scaleAnimation(0.5f);

		/* test sound */
		if(Input.GetKey(KeyCode.H) == true )
			sc.changeVolume(1);
		if(Input.GetKey(KeyCode.L) == true )
			sc.changeVolume(-1);

	}

	void Update(){
		/* update current frame */
		curFrame = ct.Frame();

		// Listen to leapmotion default gestures
		listenGestures();
		// Detect leapmotion motion
		detectMotion();

		// Debug only
		kbInputTest();

		// Update state controller
		sc.update();
	}
}
