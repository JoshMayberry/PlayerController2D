using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using jmayberry.CustomAttributes;
using jmayberry.PlayerPhysics2D;

public class PlayerAnimationHandler : MonoBehaviour {
	public PlayerInputHandler inputManager;
	[Readonly] public bool is_movingRight = true;
	[Readonly] public bool is_facingRight = true;
	[Readonly] public bool is_notMoving = true;

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
		this.is_notMoving = (Mathf.Abs(input_move.x) < 0.01f);

        if (input_move.x > 0.01f) {
            this.is_movingRight = true;
            if (!this.is_facingRight) {
                this.transform.rotation = Quaternion.Euler(this.transform.rotation.x, 0f, this.transform.rotation.z);
                this.is_facingRight = true;
            }
        }
        else if (input_move.x < -0.01f) {
            this.is_movingRight = false;
            if (this.is_facingRight) {
                this.transform.rotation = Quaternion.Euler(this.transform.rotation.x, 180f, this.transform.rotation.z);
                this.is_facingRight = false;
            }
        }
    }

	public void TriggerLanded() {
	}

	public void TriggerJump() {
	}

	public void OnAttack() {
		// Todo: Trigger the attack animation; The animation frame will call the combat system functions
	}
}
