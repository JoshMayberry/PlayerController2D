using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace jmayberry.PlayerPhysics2D {
   public interface IInputs {
		UnityEvent<Vector2> EventMove { get; }
		UnityEvent EventJumpPress { get; }
		UnityEvent EventJumpRelease { get; }
		UnityEvent EventLedgeGrabPress { get; }
		UnityEvent EventDashPress { get; }
		UnityEvent EventSprintPress { get; }
		UnityEvent EventSprintRelease { get; }

		void OnMove(InputValue context);

		void OnJumpPress();

		void OnJumpRelease();

		void OnLedgeGrabPress();

		void OnDashPress();

		void OnSprintPress();

		void OnSprintRelease();
	}
}
