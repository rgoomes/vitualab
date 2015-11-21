using UnityEngine;
using System.Collections;
using Leap;

public class MainController : MonoBehaviour {
	public GameObject controlPanel;
	public GameObject optionsPanel;
	public GameObject successPanel;
	public GameObject congratzPanel;
	public GameObject soundIcon;

	// Declare here all objects
	public GameObject balanca;
	public GameObject discodelezenne;

	StateController sc; /* for menus */
	Controller ct; /* to control leapmotion */

	Frame curFrame = null;
	
	void Start(){
		ct = new Controller();
		ct.EnableGesture(Gesture.GestureType.TYPESWIPE);
		ct.EnableGesture(Gesture.GestureType.TYPECIRCLE);
		ct.EnableGesture(Gesture.GestureType.TYPESCREENTAP);
		ct.Config.Save();

		sc = new StateController(controlPanel, optionsPanel, successPanel, congratzPanel, soundIcon);
		sc.addLabObject(balanca);
		sc.addLabObject(discodelezenne);
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
		sc.terminate();
		sc.enterTutorial();
		sc.nextTutorial(); // REMOVE LATER
		sc.changeObject(1 /* left direction */);
	}

	void rightSwipe(){
		sc.checkSwipeTutorial();
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
				float angle = Circle.Radius;
				// TODO: sc.rotateDisco(angle);

			} else if(g.Type == Gesture.GestureType.TYPESCREENTAP){
				// TODO: To animate the bell and play the sound
			}
		}
	}

	void scaleMotion(){
		/* get scale factor from the tenth frame to current frame */
		float scaleFactor = curFrame.ScaleFactor(ct.Frame(10));

		const float lowerFactor = 0.75f;
		const float upperFactor = 1.25f;

		if(scaleFactor < lowerFactor || scaleFactor > upperFactor)
			sc.scaleAnimation(scaleFactor);
	}

	void rotateMotion(){
		/* rotation angle in degrees from the tenth frame to current frame */
		float rotationAngle = curFrame.RotationAngle(ct.Frame(10), Vector.YAxis) * (180 / Mathf.PI);

		const float minAngle = 25.0f;
		if(Mathf.Abs(rotationAngle) < minAngle)
			return;

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
