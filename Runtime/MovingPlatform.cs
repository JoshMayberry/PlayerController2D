using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using jmayberry.CustomAttributes;

namespace jmayberry.PlayerPhysics2D {
	/**
     * See: [Unity 2D Advanced MOVING PLATFORM](https://www.youtube.com/watch?v=Ra26YGd5o5g)
     */
	public class MovingPlatform : PhysicsObject {
		[Range(0, 30)] public float speed = 3f;
		[Required] public Transform[] destination;
		[Readonly] public int destination_index = 0;

		Vector2 currentDirection;
		Vector2 currentDestination;

		void OnDrawGizmos() {
			if (destination == null || destination.Length < 2)
				return;

			Gizmos.color = Color.blue;

			for (int i = 0; i < destination.Length - 1; i++) {
				Gizmos.DrawLine(destination[i].position, destination[i + 1].position);
			}

			Gizmos.DrawLine(destination[destination.Length - 1].position, destination[0].position);
		}

		void Start() {
			this.UpdateDirection();
		}

		void FixedUpdate() {
			this.rb.velocity = this.currentDirection * this.speed;

			if (Vector2.Distance(transform.position, this.currentDestination) < 0.05f) {
				this.DoNextDestination();
				this.UpdateDirection();
			}
		}

		void DoNextDestination() {
			this.rb.velocity = Vector2.zero; // Stop the platform

			if (this.destination_index == (this.destination.Length - 1)) {
				this.destination_index = 0;
				return;
			}

			this.destination_index++;
		}

		void UpdateDirection() {
			this.currentDestination = this.destination[this.destination_index].position;
			Vector2 newDirection = (this.currentDestination - (Vector2)this.transform.position).normalized;
			float directionChange = Vector2.Dot(this.currentDirection, newDirection);

			if (directionChange < 0.95f) { // Threshold to detect a significant change
				foreach (var rider in this.riders) {
					// Apply force or impulse here to adjust the rider's velocity
					Vector2 compensatingForce = (newDirection - this.currentDirection) * this.speed;
					rider.rb.AddForce(compensatingForce, ForceMode2D.Impulse);
				}
			}

			this.currentDirection = newDirection;
		}

		void OnTriggerEnter2D(Collider2D other) {
			//Debug.Log(other.tag + "; " + LayerMask.LayerToName(other.gameObject.layer));
			//if (other.tag == "Player_SubElement") {
			//	PhysicsObject rider = other.transform.parent.gameObject.GetComponent<PhysicsObject>();
			//	rider.is_riding = true;
			//	rider.riding = this;
			//	this.riders.Add(rider);

			//}
			//else if (GameManager.instance.CheckIsGround(other)) {
			//	this.DoNextDestination(); // Destination is unreachable, so just go to the next one
			//	this.UpdateDirection();
			//}
		}

		void OnTriggerExit2D(Collider2D other) {
			if (other.tag == "Player_SubElement") {
				PhysicsObject rider = other.transform.parent.gameObject.GetComponent<PhysicsObject>();
				rider.is_riding = false;
				rider.riding = null;
				this.riders.Remove(rider);
			}
		}
	}
}
