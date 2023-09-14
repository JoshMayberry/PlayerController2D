using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using jmayberry.CustomAttributes;

namespace jmayberry.PlayerPhysics2D {
	public class PhysicsObject : MonoBehaviour {
		internal Rigidbody2D rb;
		[Readonly] public bool is_riding;
		[Readonly] public PhysicsObject riding;
		[Readonly] public List<PhysicsObject> riders = new List<PhysicsObject>();

		public virtual void Awake() {
			this.rb = this.GetComponent<Rigidbody2D>();
		}
	}
}