using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongGame : MonoBehaviour {
	[SerializeField]
	private float paddleSpeed;
	[SerializeField]
	private float initialBallVelocity;
	[SerializeField]
	private Rigidbody ballRigidbody;
	[SerializeField]
	private Transform rPaddle;
	[SerializeField]
	private Transform lPaddle;

	private void Start(){
		// Normalize the the force vector so it's magnitude (ie the ball's velocity) will always be 1
		Vector3 appliedForce = Random.insideUnitCircle.normalized;
		// Multiply normalized value by the initialBallVelocity
		appliedForce *= initialBallVelocity;
		// Use ForceMode.VelocityChange to apply the velocity change in a single impulse regardless of weight
		ballRigidbody.AddForce(appliedForce, ForceMode.VelocityChange);
	}

	// Using FixedUpdate because the paddle movement should be done on physics engine time
	private void FixedUpdate(){
		HandlePaddleInput(KeyCode.W,      KeyCode.S,        lPaddle);
		HandlePaddleInput(KeyCode.UpArrow,KeyCode.DownArrow,rPaddle);
	}

	private void HandlePaddleInput(KeyCode upKey, KeyCode downKey, Transform paddleObject){
		// This is the velocity of the paddle along the y-axis
		float paddleYDelta = 0;
		if(Input.GetKey(upKey)){
			//handle up key
			paddleYDelta += paddleSpeed;
		}else if(Input.GetKey(downKey)){
			//handle down key
			paddleYDelta -= paddleSpeed;
		}
		// Multiply by delta time so the velocity of the paddle is independent of framerate
		// This uses fixedDeltaTime because this method is called on FixedUpdate()
		paddleYDelta *= Time.fixedDeltaTime;

		Vector3 currentPosition = paddleObject.position;
		currentPosition.y += paddleYDelta;
		paddleObject.position = currentPosition;
	}
}
