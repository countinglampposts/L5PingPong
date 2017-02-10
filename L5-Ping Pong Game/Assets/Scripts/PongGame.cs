using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongGame : MonoBehaviour {
	[SerializeField]
	private GameObject ballCollisionEffect;
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
	private ColliderSurrogate rPaddleCollider;
	[SerializeField]
	private ColliderSurrogate lPaddleCollider;
	[SerializeField]
	private ColliderSurrogate rGoal;
	[SerializeField]
	private ColliderSurrogate lGoal;

	private void Start(){
		LaunchBall();

		rGoal.Initialize(HandleObjectCollidingWithGoal);
		lGoal.Initialize(HandleObjectCollidingWithGoal);

		rPaddleCollider.Initialize(HandleObjectCollidingWithPaddle);
		lPaddleCollider.Initialize(HandleObjectCollidingWithPaddle);
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

	private void HandleObjectCollidingWithGoal(Collision collision){
		if(collision.gameObject == ballRigidbody.gameObject){
			LaunchBall();
		}
	}

	private void HandleObjectCollidingWithPaddle(Collision collision){
		if(collision.gameObject == ballRigidbody.gameObject){

			// Get the point of contact
			Vector3 collisionPoint = collision.contacts[0].point;

			// Create the collision effect at the postion of the ball
			GameObject newEffect = Instantiate(ballCollisionEffect,collisionPoint,Quaternion.identity);

			//Get the effect particle effect
			ParticleSystem effectParticleSystem = newEffect.GetComponent<ParticleSystem>();

			//Start the coroutine
			StartCoroutine(DestroyDeadEffect(effectParticleSystem));
		}
	}

	// Coroutines are functions that can be executed over multiple updates
	// The function will pause until the next update on "yield return null"
	IEnumerator DestroyDeadEffect(ParticleSystem targetParticleSystem){
		
		// While the system is alive
		while(targetParticleSystem.IsAlive()){
			
			// Keep pausing
			yield return null;
		}

		// After escaping the loop, destroy the effect
		Destroy(targetParticleSystem.gameObject);
	}
}
