﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour {

	public LayerMask collisionMask;

	const float skinWidth = .001f;
	public int horizontalRayCount = 2;
	public int verticalRayCount = 2;
	public CollisionInfo collisions;

	float horizontalRaySpacing;
	float verticalRaySpacing;


	BoxCollider2D boxCollider;
	RaycastOrigins raycastOrigins;

	void Start() {
		boxCollider = GetComponent<BoxCollider2D> ();
		calculateRaySpacing ();
	}

	public void Move (Vector3 velocity)	{
		UpdateRaycastOrigins ();
		collisions.Reset ();

		if (velocity.x != 0) {
			HorizontalCollisions (ref velocity);
		}

		if (velocity.y != 0) {
			VerticalCollisions (ref velocity);
		}

		transform.Translate (velocity);
	}

	void HorizontalCollisions(ref Vector3 velocity) {
		float directionX = Mathf.Sign (velocity.x);
		float rayLength = Mathf.Abs (velocity.x) + skinWidth;
		
		for (int i = 0; i < horizontalRayCount; i++) {
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;

			rayOrigin = rayOrigin + Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

			if (hit) {
				velocity.x = (hit.distance - skinWidth) * directionX;
				rayLength = hit.distance;

				collisions.left = directionX == -1;
				collisions.right = directionX == 1;
			}
		}
	}
	
	void VerticalCollisions(ref Vector3 velocity) {
		float directionY = Mathf.Sign (velocity.y);
		float rayLength = Mathf.Abs (velocity.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i++) {
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin = rayOrigin + Vector2.right * (verticalRaySpacing * i + velocity.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

			if (hit) {
				velocity.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				collisions.below = directionY == -1;
				collisions.above = directionY == 1;

			}
		}
	}


	void UpdateRaycastOrigins() {
		Bounds bounds = boxCollider.bounds;

		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);

	}

	void calculateRaySpacing() {
		Bounds bounds = boxCollider.bounds;

		horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}


	struct RaycastOrigins {
		public Vector2 topLeft;
		public Vector2 topRight;
		public Vector2 bottomLeft;
		public Vector2 bottomRight;
	}

	public struct CollisionInfo {
		public bool above;
		public bool below;
		public bool left;
		public bool right;

		public void Reset() {
			above = below = left = right = false;
		}
	}


}
