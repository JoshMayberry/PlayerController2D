using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using jmayberry.CustomAttributes;
using jmayberry.PlayerPhysics2D;

public class PlayerAnimationHandler : MonoBehaviour {
	public PlayerInputHandler inputManager;
	[Readonly] public bool is_facingRight = true;

	private Animator anim;

	void Awake() {
		this.anim = GetComponent<Animator>();
	}

	void OnEnable() {
		inputManager.EventMove.AddListener(this.UpdateTurn);
	}
	void OnDisable() {
		inputManager.EventMove.RemoveListener(this.UpdateTurn);
	}

	void UpdateTurn(Vector2 input_move) {
		if (this.is_facingRight) {
			if (input_move.x > 0.01f) {
				return;
			}
		}
		else if (input_move.x < -0.01f) {
			return;
		}

		this.transform.rotation = Quaternion.Euler(this.transform.rotation.x, (this.is_facingRight ? 180f : 0f), this.transform.rotation.z);
		this.is_facingRight = !this.is_facingRight;
	}

	public void TriggerLanded() {
	}

	public void TriggerJump() {
	}

	public void OnAttack() {
		// Todo: Trigger the attack animation; The animation frame will call the combat system functions
	}
}
