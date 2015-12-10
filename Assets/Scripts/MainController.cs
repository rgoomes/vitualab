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
	public GameObject previewImages;
	public GameObject leapMotionObject;
	public GameObject arco;
	public GameObject discoCamera;
	public GameObject bellSound;
	public GameObject backgroundImg;
	public GameObject gameCamera;

	// Declare here all objects
	public GameObject sino;
	public GameObject discodelezenne;

	StateController sc; /* for menus */
	Controller ct; /* to control leapmotion */

	Frame curFrame = null;

	bool interacting = false;

	List<string> descriptions;
	string sinoDesc  = "A Sineta consta de um sino, fixo numa trave horizonal de madeira, " +
		"sustentada por duas colunas verticais erguidas numa prancha rectangular que faz " +
		"de base. Esta sineta destinava-se a provar que os corpos se deformam quando vibram. ";

	string discoDesc = "O Disco de Delezenne é um dispositivo constituido por um anel circular " +
		"em torno do qual está enrolado um fio de cobre que pode efectuar um movimento de rotação" +
		" em torno de um eixo orientado segundo uma linha diametral do anel.";
	
	void Start(){
		ct = new Controller();
		ct.EnableGesture(Gesture.GestureType.TYPESWIPE);
		ct.EnableGesture(Gesture.GestureType.TYPECIRCLE);
		ct.EnableGesture(Gesture.GestureType.TYPESCREENTAP);
		ct.Config.Save();

		descriptions = new List<string>(new string[] { sinoDesc, discoDesc });

		sc = new StateController(controlPanel, optionsPanel, successPanel, congratzPanel, soundIcon,
		                         successSound, descriptionText, previewImages, leapMotionObject,
		                         discoCamera, bellSound, backgroundImg);

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

				/* disco rotation */
				float angle = Circle.Radius;
				interacting = sc.rotateDisco(arco, angle*clockwise);

				/* verify tutorial */
				sc.checkTutorial(5 /* TUT4_SCREEN */);
			} else if(g.Type == Gesture.GestureType.TYPESCREENTAP){
				// TODO: Animate the bell
				sc.playBellSound();

				/* verify tutorial */
				sc.checkTutorial(4 /* TUT3_SCREEN */);
			}
		}
	}

	void scaleMotion(){
		/* get scale factor from the tenth frame to current frame */
		float scaleFactor = curFrame.ScaleFactor(ct.Frame(5));

		const float lowerFactor = 0.90f;
		const float upperFactor = 1.15f;

		if(scaleFactor >= lowerFactor && scaleFactor <= upperFactor)
			return;

		/* verify tutorial */
		sc.checkTutorial(1 /* TUT0_SCREEN */);

		sc.scaleAnimation(scaleFactor);
	}

	void rotateMotion(){
		/* rotation angle in degrees from the tenth frame to current frame */
		float rotationAngle = curFrame.RotationAngle(ct.Frame(10), Vector.YAxis) * (180 / Mathf.PI);

		const float minAngle = 10.0f;

		if(Mathf.Abs(rotationAngle) > minAngle)
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
			sc.rotateAnimation(10.0f, 1);
		if(Input.GetKey(KeyCode.A) == true )
			sc.scaleAnimation(1.50f);
		if(Input.GetKey(KeyCode.D) == true )
			sc.scaleAnimation(0.75f);

		/* test sound */
		if(Input.GetKey(KeyCode.H) == true )
			sc.changeVolume(1);
		if(Input.GetKey(KeyCode.L) == true )
			sc.changeVolume(-1);

		/* test interactions */
		if(Input.GetKey(KeyCode.I) == true ){
			interacting = sc.rotateDisco(arco, 20.0f);
			sc.playBellSound();
		}

	}

	bool isInteracting(){
		return this.interacting;
	}

	void updateBackground(){
		Camera cam = gameCamera.GetComponent<Camera>();

		float a = cam.farClipPlane * 2.0f;
		float A = cam.fieldOfView * 0.5f * Mathf.Deg2Rad;
		float h = (Mathf.Tan(A) * a);
		float w = (h / cam.pixelHeight) * cam.pixelWidth;

		float xscale = (w*0.1f);
		float zscale = (h*0.1f);

		backgroundImg.transform.localScale = new Vector3(xscale, 100.0f, zscale);
	}

	void Update(){
		/* update current frame */
		curFrame = ct.Frame();

		/* update background image size */
		updateBackground();

		/* default value: not interacting */
		this.interacting = false;

		// Listen to leapmotion default gestures
		listenGestures();
		// Detect leapmotion motion
		detectMotion();

		// Debug only
		kbInputTest();

		// Update state controller
		sc.update(isInteracting());
	}
}
