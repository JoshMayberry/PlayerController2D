using UnityEngine;

namespace jmayberry.PlayerPhysics2D {
	public interface IAnimations {
        Animator anim { get; }

        IInputs inputManager { get; }

        bool is_facingRight { get; }

        void UpdateTurn(Vector2 input_move);

        void TriggerLanded();

        void TriggerJump();

        void OnAttack();
    }
}
