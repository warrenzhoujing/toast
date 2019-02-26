using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

	public float maxJumpHeight = 4.5f;
	public float minJumpHeight = 1f;
	public float TimeToJumpApex;
	public float MoveSpeed = 6;

	public Vector2 wallJumpClimb;
	public Vector2 wallJumpLeap;

	public float accelerationTimeAirborne = 0.2f;
	public float accelerationTimeGrounded = 0.15f;
	public float wallSlideSpeedMax = 3f;
	public float wallStickTime;
	float timeToWallUnstick;
	float delay = 1f;
	float velocityXSmoothing;
	float gravity = -20;
	float maxJumpForce = 10;
	float minJumpForce = 10;

	[HideInInspector]
	public Vector2 public_input;

	Vector3 velocity;
	Controller2D controller;

	void Start() {
		controller = GetComponent<Controller2D> ();

		gravity = -(2 * maxJumpHeight) / Mathf.Pow(TimeToJumpApex, 2);
		maxJumpForce = Mathf.Abs(gravity * TimeToJumpApex);
		minJumpForce = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
	}

	void Update () {

		bool onWall = false;

		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		public_input = input;

		int wallDirX = (controller.collisions.left)? -1 : 1;

		float targetVelocityX = input.x * MoveSpeed;
		velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
		
		if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below) {
			onWall = true;
			if (velocity.y < 0) {
				if (velocity.y < -wallSlideSpeedMax) {
					velocity.y = -wallSlideSpeedMax;
				}

				if (timeToWallUnstick > 0) {
					velocityXSmoothing = 0;
					velocity.x = 0;

					if (input.x != wallDirX && input.x != 0) {
						timeToWallUnstick -= Time.deltaTime;
					} else {
						timeToWallUnstick = wallStickTime;
					}
				} else {
					timeToWallUnstick = wallStickTime;
				}
			}
			
		}

		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;
		}

		
		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			delay = 1f;
			if (onWall) {
				if (wallDirX == input.x) {
					velocity.x = -wallDirX * wallJumpClimb.x;
					velocity.y = wallJumpClimb.y;
				} else if (wallDirX == -input.x) {
					velocity.x = -wallDirX * wallJumpLeap.x;
					velocity.y = wallJumpLeap.y;
				}
			}
			
			if (controller.collisions.below) {
				velocity.y = maxJumpForce;
			}
		} else {
			delay -= 0.3f;
			if (delay < -1) {
				delay = -1;
			}
		}

		if (Input.GetKeyUp(KeyCode.UpArrow)){
			if (velocity.y > minJumpForce) {
				velocity.y = minJumpForce;
			}
			
		}

		if (delay > 0 && wallDirX == input.x && onWall) {
			velocity.x = -wallDirX * wallJumpClimb.x;
			velocity.y = wallJumpClimb.y;
		} else if (delay > 0 && wallDirX == -input.x && onWall) {
			velocity.x = -wallDirX * wallJumpLeap.x;
			velocity.y = wallJumpLeap.y;
		}

		
		
		velocity.y += gravity * Time.deltaTime;
		controller.Move(velocity * Time.deltaTime);
	}
}
