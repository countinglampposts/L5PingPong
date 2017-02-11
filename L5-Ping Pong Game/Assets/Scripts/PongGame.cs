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
	private Transform ballStartPin;
	[SerializeField]
	private Transform rPaddle;
	[SerializeField]
	private Transform lPaddle;
	[SerializeField]
	private ColliderSurrogate rGoal;
	[SerializeField]
	private ColliderSurrogate lGoal;
	[SerializeField]
	private GameObject goalTextDisplay;

	private void Start(){
		LaunchBall();

		rGoal.Initialize(HandleObjectCollidingWithGoal);
		lGoal.Initialize(HandleObjectCollidingWithGoal);
	}

	private void LaunchBall(){
		// Ball is moved to the launch position first
		ballRigidbody.transform.position = ballStartPin.position;

		// Activate after moving to prevent a collision even being called
		ballRigidbody.gameObject.SetActive(true);

		// Stop the balls movement before applying velocity;
		ballRigidbody.velocity = Vector3.zero;

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

		// Because Vector3 is a struct and not a class its values are passed by value, not reference
		// Because of this, paddleObject.position is not a reference to an object but a copy of the values stored
		Vector3 currentPosition = paddleObject.position;
		currentPosition.y += paddleYDelta;
		paddleObject.position = currentPosition;
	}

	private void HandleObjectCollidingWithGoal(GameObject collidedObject){
		if(collidedObject == ballRigidbody.gameObject){
			StartCoroutine(GoalCoroutine());
		}
	}

	// Coroutines are functions that can be executed over multiple updates
	// "yield return null" pauses the function until next update
	private IEnumerator GoalCoroutine(){
		// Deactivate the ball
		ballRigidbody.gameObject.SetActive(false);
		// Activate the "Goal!" sign
		goalTextDisplay.gameObject.SetActive(true);

		// Calculate the time of the goal celebration
		float endTime = Time.time + 2f;
		// While the system is alive
		while(Time.time < endTime){
			// Keep pausing
			yield return null;
		}

		// Deactivate the "Goal!" sign
		goalTextDisplay.gameObject.SetActive(false);
		LaunchBall();
	}

}
