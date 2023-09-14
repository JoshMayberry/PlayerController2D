using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using jmayberry.CustomAttributes;

public class PlayerInputHandler : MonoBehaviour {
	[Readonly] public UnityEvent<Vector2> EventMove;
	[Readonly] public UnityEvent EventJumpPress;
	[Readonly] public UnityEvent EventJumpRelease;
	[Readonly] public UnityEvent EventLedgeGrabPress;
	[Readonly] public UnityEvent EventDashPress;
	[Readonly] public UnityEvent EventSprintPress;
	[Readonly] public UnityEvent EventSprintRelease;

	public void OnMove(InputValue context) {
		this.EventMove.Invoke(context.Get<Vector2>());
	}

	public void OnJumpPress() {
		this.EventJumpPress.Invoke();
	}

	public void OnJumpRelease() {
		this.EventJumpRelease.Invoke();
	}

	public void OnLedgeGrabPress() {
		this.EventLedgeGrabPress.Invoke();
	}

	public void OnDashPress() {
		this.EventDashPress.Invoke();
	}

	public void OnSprintPress() {
		this.EventSprintPress.Invoke();
	}

	public void OnSprintRelease() {
		this.EventSprintRelease.Invoke();
	}
}
