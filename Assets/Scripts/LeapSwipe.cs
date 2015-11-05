using UnityEngine;
using System.Collections;
using Leap;

public class LeapSwipe : MonoBehaviour {
	Controller controller;
	
	void Start(){
		controller = new Controller();
		controller.EnableGesture(Gesture.GestureType.TYPESWIPE);
		controller.Config.SetFloat("Gesture.Swipe.MinLength", 100.0f);
		controller.Config.SetFloat("Gesture.Swipe.MinVelocity", 100.0f);
		controller.Config.Save();
	}

	void listenGestures(){
		Frame frame = controller.Frame();
		GestureList list = frame.Gestures();
		
		for(int i = 0; i < list.Count; i++){
			Gesture g = list[i];
			
			if(g.Type == Gesture.GestureType.TYPESWIPE){
				SwipeGesture Swipe = new SwipeGesture(g);
				Vector swipeDirection = Swipe.Direction;
				
				if(swipeDirection.x < 0)
					Debug.Log("Left Swipe");
				else if(swipeDirection.x > 0)
					Debug.Log("Right Swipe");
				/*
				if(swipeDirection.y < 0)
					Debug.Log("Down Swipe");
				else if(swipeDirection.y > 0)
					Debug.Log("Up Swipe");
				*/
			}
		}
	}
	
	void Update(){
		listenGestures();
	}
}
