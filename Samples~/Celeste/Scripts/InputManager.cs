using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using jmayberry.CustomAttributes;
using jmayberry.PlayerPhysics2D;

public class InputManager : MonoBehaviour, IInputs {
    UnityEvent<Vector2> IInputs.EventMove => this.EventMove;
    [Readonly] public UnityEvent<Vector2> EventMove;

    UnityEvent IInputs.EventJumpPress => this.EventJumpPress;
    [Readonly] public UnityEvent EventJumpPress;

    UnityEvent IInputs.EventJumpRelease => this.EventJumpRelease;
    [Readonly] public UnityEvent EventJumpRelease;

    UnityEvent IInputs.EventLedgeGrabPress => this.EventLedgeGrabPress;
    [Readonly] public UnityEvent EventLedgeGrabPress;

    UnityEvent IInputs.EventDashPress => this.EventDashPress;
    [Readonly] public UnityEvent EventDashPress;

    UnityEvent IInputs.EventSprintPress => this.EventSprintPress;
    [Readonly] public UnityEvent EventSprintPress;

    UnityEvent IInputs.EventSprintRelease => this.EventSprintRelease;
    [Readonly] public UnityEvent EventSprintRelease;

    public static InputManager instance { get; private set; }
    private void Awake() {
        if (instance != null) {
            Debug.LogError("Found more than one Input Manager in the scene.");
        }

        instance = this;
    }

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
