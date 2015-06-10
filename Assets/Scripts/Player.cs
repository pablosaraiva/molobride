using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour {

	public float jumpHeight = 4;
	public float timeToJumpApex = .4f;
	public GameObject myCamera;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	float moveSpeed = 6;
	float gravity;
	float jumpVelocity;
	Vector3 velocity;
	float velocityXSmoothing;
	Controller2D controller;

	void Start () {
		controller = GetComponent<Controller2D> ();
		gravity = -(2 * jumpHeight / Mathf.Pow(timeToJumpApex,2));
		jumpVelocity = Mathf.Abs(gravity * timeToJumpApex);
		print ("Gravity: " + gravity + " jumpVelocity: " + jumpVelocity);
	}

	void Update() {
		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;
		}

		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		if (Input.GetKeyDown (KeyCode.Space) && controller.collisions.below) {
			velocity.y = jumpVelocity;
		}


		float targetVelocityX = input.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below?accelerationTimeGrounded:accelerationTimeAirborne));
		velocity.y = velocity.y + gravity * Time.deltaTime;
		controller.Move (velocity * Time.deltaTime);
		Vector3 cameraPosition = new Vector3 (this.transform.position.x, this.transform.position.y, -10);

		myCamera.transform.position = cameraPosition;


	}

}
