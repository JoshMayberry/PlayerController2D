using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using jmayberry.CustomAttributes;

namespace jmayberry.PlayerPhysics2D {
	/**
     * See: [How To Ledge Climb in Unity](https://www.youtube.com/watch?v=Kh5n63A-YBw)
     */
	public class LedgeDetect : MonoBehaviour {
		[InspectorRename("Radius")][Range(0, 1)] public float radius = 0.1f;
		[InspectorRename("Can Detect")][Readonly] public bool can_detect;
		[InspectorRename("Is Detected")][Readonly] public bool is_Detected;
		void OnDrawGizmos() {
			 Gizmos.color = Color.red;
			 Gizmos.DrawWireSphere(this.transform.position, this.radius);
		}

		//void Update() {
		//	this.is_Detected = this.can_detect && Physics2D.OverlapCircle(this.transform.position, this.radius);
		//}

		//void OnTriggerEnter2D(Collider2D other) {
		//	if (other.gameObject.layer == GameManager.instance.groundLayer.value) {
		//		this.can_detect = false;
		//	}
		//}

		//void OnTriggerExit2D(Collider2D other) {
		//	if (other.gameObject.layer == GameManager.instance.groundLayer.value) {
		//		this.can_detect = true;
		//	}
		//}
	}
}
