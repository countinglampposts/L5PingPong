using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PongGame : MonoBehaviour {
	[SerializeField]
	private Text rScoreText;
	[SerializeField]
	private Text lScoreText;
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

	private int rScore;
	private int lScore;

	private void Start(){
		LaunchBall();

		// Lambda expressions serve as micro functions within a function. 
		// Their sytax is: "(Parameter p) => {do function actions here;}"
		// This allows you to create functions that would only exist within the scope that is necessary
		rGoal.Initialize((GameObject collidedObject) => {

			// Increase the score for lPlayer for scoring on rGoal
			lScore++;

			// Update the scoreboard
			lScoreText.text = lScore.ToString();

			// Continue with previous actions
			HandleObjectCollidingWithGoal(collidedObject);
		});
		lGoal.Initialize((GameObject collidedObject) => {
			rScore++;
			rScoreText.text = rScore.ToString();
			HandleObjectCollidingWithGoal(collidedObject);
		});
	}

	private void LaunchBall(){
		// Ball is moved to the launch position first
		ballRigidbody.transform.position = ballStartPin.position;

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
			LaunchBall();
		}
	}
}
