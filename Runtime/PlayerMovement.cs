using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using jmayberry.CustomAttributes;
using jmayberry.GeneralInfrastructure.Manager;

namespace jmayberry.PlayerPhysics2D {
	/*
	 * Controls are using the new Unity Controls package.
	 * See: [How to use Unity's New INPUT System EASILY](https://www.youtube.com/watch?v=HmXU4dZbaMw)
	 */
	public class PlayerMovement : PhysicsObject {
		[InspectorRename("Debug Mode")] public bool debugging = true;

		[Header("References")]
		[Required] [SerializeField] private PlayerInputHandler inputManager;
		[Required] [SerializeField] private PlayerAnimationHandler animationManager;

		[Header("Environment")]
		[InspectorRename("Friction")][Range(0, 0.25f)] public float speed_friction = 0.0f;
		[InspectorRename("Drag")][Range(0, 0.025f)] public float speed_drag = 0f;
		[InspectorRename("Ground Check Size")] public Vector2 ground_checkSize = new Vector2(0.49f, 0.03f);
		[InspectorRename("Ground Check Position")][Required] public Transform ground_checkPosition;
		[InspectorRename("Is On Ground")][Readonly] public bool is_onGround;

		[Header("Gravity")]
		[InspectorRename("Coyote Time")][Range(0, 0.25f)] public float gravity_coyoteTime = 0.1f;
		[InspectorRename("Fall Multiplier")][Range(0, 4)] public float gravity_fallMultiplier = 1.5f;
		[InspectorRename("Jump Cut Multiplier")][Range(0, 4)] public float gravity_jumpCutMultiplier = 2f;
		[InspectorRename("Dive Multiplier")][Range(0, 8)] public float gravity_diveMultiplier = 4f;
		[InspectorRename("Air Time Multiplier")][Range(0, 1)] public float gravity_airTimeMultiplier = 0.5f;
		[InspectorRename("Max Fall Speed")][Range(0, 50)] public float fallSpeed_normal = 25f;
		[InspectorRename("Max Jump Cut Speed")][Range(0, 50)] public float fallSpeed_jumpCut = 30f;
		[InspectorRename("Max Dive Speed")][Range(0, 50)] public float fallSpeed_dive = 40f;
		[InspectorRename("Recover Time Threshold")][Range(0, 50)] public float fallRecover_threshold = 35f;
		[InspectorRename("Recover Time")][Range(0, 50)] public float fallRecover_time = 0.25f;
		[InspectorRename("Gravity Strength")][Readonly] public float gravity_strength;
		[InspectorRename("Normal Gravity")][Readonly] public float gravity_scale;
		[InspectorRename("Current Gravity")][Readonly] public float debug_gravity_scale;
		[InspectorRename("Min Gravity")][Readonly] public float debug_gravity_scaleMin;
		[InspectorRename("Max Gravity")][Readonly] public float debug_gravity_scaleMax;
		[InspectorRename("Is Falling")][Readonly] public bool is_falling;
		[InspectorRename("Is Recover Needed")][Readonly] public bool is_fallRecoverNeeded;
		[InspectorRename("Is Fall Recovering")][Readonly] public bool is_fallRecovering;
		[InspectorRename("Is Diving")][Readonly] public bool is_diveFalling;
		[InspectorRename("Is Falling After a Jump")][Readonly] public bool is_jumpFalling;

		[Header("Movement")]
		[InspectorRename("Max Speed")][Range(0, 30)] public float run_max = 11f;
		[InspectorRename("Acceleration")][Range(0, 30)] public float run_acceleration = 2.5f;
		[InspectorRename("Decceleration")][Range(0, 30)] public float run_decceleration = 5f;
		[InspectorRename("Acceleration Air Multiplier")][Range(0, 2)] public float run_airAccelerationMultiplier = 0.65f;
		[InspectorRename("Decceleration Air Multiplier")][Range(0, 2)] public float run_airDeccelerationMultiplier = 0.65f;
		[InspectorRename("Keep Momentum")] public bool run_keepMomentum = true;
		[InspectorRename("Movement Inputs")][Readonly] public Vector2 input_move;
		[InspectorRename("Current Speed")][Readonly] public Vector2 debug_speed;
		[InspectorRename("Min Speed")][Readonly] public Vector2 debug_speedMin;
		[InspectorRename("Max Speed")][Readonly] public Vector2 debug_speedMax;

		[Header("Sprint")]
		[InspectorRename("Max Speed Multiplier")][Range(0, 30)] public float sprint_maxMultiplier = 11f;
		[InspectorRename("Acceleration Multiplier")][Range(0, 30)] public float sprint_accelerationMultiplier = 2f;
		[InspectorRename("Decceleration Multiplier")][Range(0, 30)] public float sprint_deccelerationMultiplier = 2f;
		[InspectorRename("Sprint Grace")][Range(0, 0.25f)] public float sprint_graceTime = 0.1f; // How long a 'sprint button press counts for'
		[InspectorRename("Is Sprinting")][Readonly] public bool is_sprinting;

		[Header("Jump")]
		[InspectorRename("Height")][Range(0, 10)] public float jump_height = 3.5f;
		[InspectorRename("Air Time at Apex")][Range(0, 2)] public float airTime_apex = 0.3f;
		[InspectorRename("Air Time Threshold")][Range(0, 2)] public float airTime_threshold = 1f;
		[InspectorRename("Air Time Acceleration Mult")][Range(0, 2)] public float airTime_accelerationMultiplier = 1.1f;
		[InspectorRename("Air Time Decceleration Mult")][Range(0, 2)] public float airTime_deccelerationMultiplier = 1.1f;
		[InspectorRename("Air Time Max Speed Mult")][Range(0, 2)] public float airTime_maxSpeedMultiplier = 1.3f;

		[InspectorRename("Force")][Range(0, 30)] public float jump_force = 13f;
		[InspectorRename("Extra Jumps")][Range(0, 3)] public int jump_amount = 1;
		[InspectorRename("Extra Jumps Left")][Readonly] public int jump_amountLeft;
		[InspectorRename("Extra Jump Refill Time")][Range(0, 1)] public float jump_refillTime = 0.1f;
		[InspectorRename("Jump Grace")][Range(0, 0.25f)] public float jump_graceTime = 0.1f; // How long a 'jump button press counts for'
		[InspectorRename("Is Jump Cut")][Readonly] public bool is_jumpCut;
		[InspectorRename("Is Jumping")][Readonly] public bool is_jumping;
		[InspectorRename("Is Extra Jump Refilling")][Readonly] public bool is_jumpRefilling;
		[InspectorRename("Is at Jump Apex")][Readonly] public bool is_airTime;

		[Header("Ledge Grab")]
		[InspectorRename("Ledge Check Position")][Required] public LedgeDetect ledgeGrab;
		[InspectorRename("Grace Time")][Range(0, 1)] public float ledgeGrab_graceTime = 0.1f; // How long a 'ledge climb button press counts for'
		[InspectorRename("Ledge Grabable Begin")][Readonly] public Vector2 ledgeGrab_beginPosition;
		[InspectorRename("Ledge Grabable End")][Readonly] public Vector2 ledgeGrab_endPosition;
		[InspectorRename("Ledge Grabable Begin Offset")][Readonly] public Vector2 ledgeGrab_beginPositionOffset;
		[InspectorRename("Ledge Grabable End Offset")][Readonly] public Vector2 ledgeGrab_endPositionOffset;
		[InspectorRename("Is Ledge Grabbable")][Readonly] public bool is_ledgeGrabbable;
		[InspectorRename("Is Ledge Grabbing")][Readonly] public bool is_ledgeGrabbing;

		[Header("Wall Jump")]
		[InspectorRename("Force")] public Vector2 wallJump_force = new Vector2(15f, 25f);
		[InspectorRename("Wall Jumps")][Range(0, 10)] public int wallJump_amount = 3;
		[InspectorRename("Wall Jumps Left")][Readonly] public int wallJump_amountLeft;
		[InspectorRename("Wall Jump Refill Time")][Range(0, 1)] public float wallJump_refillTime = 0.1f;
		[InspectorRename("LERP")][Range(0, 1)] public float wallJump_lerp = 0.5f;
		[InspectorRename("Jump Time")][Range(0, 1)] public float wallJump_time = 0.15f;
		[InspectorRename("Turn on Jump")] public bool wallJump_doTurn = false;
		[InspectorRename("Slide Speed")][Range(0, 1)] public float run_wallSlide = 0f;
		[InspectorRename("Slide Acceleration")][Range(0, 1)] public float run_wallSlideAcceleration = 0f;
		[InspectorRename("Wall Check Position")][Required] public Transform wall_checkPosition;
		[InspectorRename("Wall Check Size")] public Vector2 wall_checkSize = new Vector2(0.05f, 0.5f);
		[InspectorRename("Is On Wall")][Readonly] public bool is_onWall;
		[InspectorRename("Is On Left Wall")][Readonly] public bool is_onWallLeft;
		[InspectorRename("Is On Right Wall")][Readonly] public bool is_onWallRight;
		[InspectorRename("Is Exausted")][Readonly] public bool is_wallSliding;
		[InspectorRename("Is Wall Jumping")][Readonly] public bool is_wallJumping;
		[InspectorRename("Last Wall was Left")][Readonly] public bool wallJump_lastWasLeft;
		[InspectorRename("Is Wall Jump Refilling")][Readonly] public bool is_wallJumpRefilling;

		[Header("Dash")]
		[InspectorRename("Amount")][Range(0, 30)] public float dash_amount = 1f;
		[InspectorRename("Amount Left")][Readonly] public float dash_amountLeft;
		[InspectorRename("Speed")][Range(0, 30)] public float dash_speed = 20f;
		[InspectorRename("Sleep Time")][Range(0, 1)] public float dash_sleepTime = 0.05f; // Freezes the game for this amount of time before starting the dash
		[InspectorRename("Attack Time")][Range(0, 1)] public float dash_attackTime = 0.15f; // How long the attack portion of a dash lasts for
		[InspectorRename("End Time")][Range(0, 1)] public float dash_endTime = 0.15f; // How long the rest of the dash lasts for
		[InspectorRename("End Speed")] public Vector2 dash_endSpeed = new Vector2(15f, 15f);
		[InspectorRename("Drag Multiplier")][Range(0, 1)] public float dash_dragMultiplier = 0.5f;
		[InspectorRename("Grace Time")][Range(0, 1)] public float dash_graceTime = 0.1f; // How long a 'dash button press counts for'
		[InspectorRename("End LERP")][Range(0, 1)] public float dash_endLerp = 0.5f;
		[InspectorRename("Refill Time")][Range(0, 1)] public float dash_refillTime = 0.1f;
		[InspectorRename("Is Dashing")][Readonly] public bool is_dashing;
		[InspectorRename("Is Dash Attacking")][Readonly] public bool is_dashAttacking;
		[InspectorRename("Is Dash Refilling")][Readonly] public bool is_dashRefilling;
		[InspectorRename("Last Dash Direction")][Readonly] public Vector2 dash_lastDirection;

		[Header("Stamina")]
		[InspectorRename("Is Enabled")] public bool stamina_enabled;
		[InspectorRename("Stamina Points")][Range(0, 100)] public float stamina_amount = 20f; // How many points of stamina you have until you need to recover again
		[InspectorRename("Stamina Left")][Readonly] public float stamina_amountLeft;
		[InspectorRename("Stamina Regen")][Range(0, 1)] public float stamina_refillTime = 0.01f; // How many seconds it normally takes to recover 1 stamina point
		[InspectorRename("Exaustion Penalty")][Range(0, 5)] public float stamina_exaustionPenalty = 2f; // How much slower it takes for stamina to recover when exausted
		[InspectorRename("Cost: Dash")][Range(0, 50)] public float cost_dash = 20f;
		[InspectorRename("Cost: Sprint")][Range(0, 50)] public float cost_sprint = 0.01f;
		[InspectorRename("Cost: Extra Jump")][Range(0, 50)] public float cost_jump = 10f;
		[InspectorRename("Cost: Wall Jump")][Range(0, 50)] public float cost_wallJump = 3f;
		[InspectorRename("Is Stamina Refilling")][Readonly] public bool is_staminaRefilling;
		[InspectorRename("Is Exausted")][Readonly] public bool is_exausted;

		float wallJump_startTime;
		float ledgeGrab_startTime;
		float wall_lastTimer;
		float wallRight_lastTimer;
		float wallLeft_lastTimer;
		float ground_lastTimer;
		float fallRecover_lastTimer;

		// [Readonly] public float dash_startTime;

		float jump_lastPressed;
		float dash_lastPressed;
		float ledgeGrab_lastPressed;

		float run_accelerationAmount;
		float run_deccelerationAmount;

		void OnDrawGizmos() {
			// Gizmos.color = Color.red;
			// Gizmos.DrawWireCube(this.ground_checkPosition.position, this.ground_checkSize);
			// Gizmos.DrawWireCube(this.wall_checkPosition.position, this.wall_checkSize);
		}

		// Runs when the inspector window is updated
		void OnValidate() {
			this.gravity_strength = -(2 * this.jump_height) / Mathf.Pow(this.airTime_apex, 2);
			this.gravity_scale = this.gravity_strength / Physics2D.gravity.y;
			this.run_accelerationAmount = (50 * this.run_acceleration) / this.run_max;
			this.run_deccelerationAmount = (50 * this.run_decceleration) / this.run_max;
			this.jump_force = Mathf.Abs(this.gravity_strength) * this.airTime_apex;

			this.run_acceleration = Mathf.Clamp(this.run_acceleration, 0.01f, this.run_max);
			this.run_decceleration = Mathf.Clamp(this.run_decceleration, 0.01f, this.run_max);
        }

        private void OnEnable() {
            this.inputManager.EventMove.AddListener(this.OnMove);
            this.inputManager.EventJumpPress.AddListener(this.OnJumpPress);
            this.inputManager.EventJumpRelease.AddListener(this.OnJumpRelease);
            this.inputManager.EventLedgeGrabPress.AddListener(this.OnLedgeGrabPress);
            this.inputManager.EventDashPress.AddListener(this.OnDashPress);
            this.inputManager.EventSprintPress.AddListener(this.OnSprintPress);
            this.inputManager.EventSprintRelease.AddListener(this.OnSprintRelease);
        }

        private void OnDisable() {
            this.inputManager.EventMove.RemoveListener(this.OnMove);
            this.inputManager.EventJumpPress.RemoveListener(this.OnJumpPress);
            this.inputManager.EventJumpRelease.RemoveListener(this.OnJumpRelease);
            this.inputManager.EventLedgeGrabPress.RemoveListener(this.OnLedgeGrabPress);
            this.inputManager.EventDashPress.RemoveListener(this.OnDashPress);
            this.inputManager.EventSprintPress.RemoveListener(this.OnSprintPress);
            this.inputManager.EventSprintRelease.RemoveListener(this.OnSprintRelease);
        }

        void Start() {
			this.dash_amountLeft = this.dash_amount;
			this.jump_amountLeft = this.jump_amount;
			this.stamina_amountLeft = this.stamina_amount;

			this.rb.gravityScale = this.gravity_scale;
			this.debug_gravity_scaleMin = this.gravity_scale;
			this.debug_gravity_scaleMax = this.gravity_scale;
		}

		void Update() {
			this.UpdateTimers();
			this.UpdateIsStates();
			this.UpdateActions();
			this.UpdateRefills();
			this.UpdateGravity();

			if (this.debugging) {
				this.debug_gravity_scaleMin = Mathf.Min(this.debug_gravity_scaleMin, this.rb.gravityScale);
				this.debug_gravity_scaleMax = Mathf.Max(this.debug_gravity_scaleMax, this.rb.gravityScale);
				this.debug_gravity_scale = this.gravity_scale;
			}
		}

		void FixedUpdate() {
			this.UpdateMovement();
			this.UpdateFriction();

			if (this.is_wallSliding) {
				this.DoWallSlide();
			}

			if (this.debugging) {
				this.debug_speedMin = new Vector2(Mathf.Min(this.debug_speedMin.x, this.rb.velocity.x), Mathf.Min(this.debug_speedMin.y, this.rb.velocity.y));
				this.debug_speedMax = new Vector2(Mathf.Max(this.debug_speedMax.x, this.rb.velocity.x), Mathf.Max(this.debug_speedMax.y, this.rb.velocity.y));
				this.debug_speed = this.rb.velocity;
			}
		}

		void OnMove(Vector2 input_move) {
			this.input_move = input_move;
		}

		void OnJumpPress() {
			this.jump_lastPressed = this.jump_graceTime;
		}

		void OnJumpRelease() {
			if (this.CanJumpCut()) {
				this.is_jumpCut = true;
			}
		}

		void OnLedgeGrabPress() {
			this.ledgeGrab_lastPressed = this.ledgeGrab_graceTime;
		}

		void OnDashPress() {
			this.dash_lastPressed = this.dash_graceTime;
		}

		void OnSprintPress() {
			if (!this.stamina_enabled || (!this.is_exausted && (this.stamina_amountLeft >= this.cost_sprint))) {
				this.is_sprinting = true;
			}
		}

		void OnSprintRelease() {
			this.is_sprinting = false;
		}

		/**
		 * We want the player to be able to jump for a short amount of time after they last touched the ground.
		 * This is done to make controls "feel" more responsive.
		 * See: [Improve Your Platformer with Forces](https://youtu.be/KbtcEVCM7bw?t=249)
		 * 
		 * We want the player to be able to grab a ledge they barely missed.
		 * See: [How To Ledge Climb in Unity](https://youtu.be/Kh5n63A-YBw?t=328)
		 */
		void UpdateTimers() {
			if (!this.is_dashing) {
				if (Physics2D.OverlapBox(this.ground_checkPosition.position, this.ground_checkSize, 0, GameManager.instance.groundLayer)) {
					if (this.ground_lastTimer < -0.1f) {
						this.animationManager.TriggerLanded();
					}

					this.ground_lastTimer = this.gravity_coyoteTime;
				}

				if (!this.is_jumping) {
					if (Physics2D.OverlapBox(this.wall_checkPosition.position, this.wall_checkSize, 0, GameManager.instance.groundLayer)) {
						if (this.animationManager.is_facingRight) {
							this.wallRight_lastTimer = this.gravity_coyoteTime;
						}
						else {
							this.wallLeft_lastTimer = this.gravity_coyoteTime;
						}
					}

					this.wall_lastTimer = Mathf.Max(this.wallLeft_lastTimer, this.wallRight_lastTimer);
				}
			}

			this.wall_lastTimer -= Time.deltaTime;
			this.ground_lastTimer -= Time.deltaTime;
			this.wallJump_startTime -= Time.deltaTime;
			this.wallLeft_lastTimer -= Time.deltaTime;
			this.wallRight_lastTimer -= Time.deltaTime;
			this.fallRecover_lastTimer -= Time.deltaTime;

			this.dash_lastPressed -= Time.deltaTime;
			this.jump_lastPressed -= Time.deltaTime;
			this.ledgeGrab_lastPressed -= Time.deltaTime;

		}

		void UpdateIsStates() {
			this.is_onGround = (this.ground_lastTimer > 0.01f);
			this.is_wallJumping = (this.wallJump_startTime > 0.01f);
			this.is_onWall = !is_onGround && (this.wall_lastTimer > 0.01f);
			this.is_onWallRight = !is_onGround && (this.wallRight_lastTimer > 0.01f);
			this.is_onWallLeft = !is_onGround && (this.wallLeft_lastTimer > 0.01f);
			this.is_falling = (this.rb.velocity.y < -0.01f);
			this.is_wallJumping = (this.wallJump_startTime > 0.01f);
			this.is_fallRecovering = (this.fallRecover_lastTimer > 0.01f);
			this.is_diveFalling = (this.is_falling && (this.input_move.y < -0.01f));

			if (this.is_falling) {
				if (this.is_jumping) {
					this.is_jumping = false; // We have stopped moving upwards, so we are not jumping anymore
					this.is_jumpFalling = true;
				}

				if (!this.is_fallRecoverNeeded && (this.rb.velocity.y < -this.fallRecover_threshold)) {
					this.is_fallRecoverNeeded = true;
				}
			}

			if (this.is_wallJumping && ((Time.time - this.wallJump_startTime > this.wallJump_time))) {
				this.is_wallJumping = false;
			}

			if (this.is_onGround) {
				this.is_jumpCut = false;
				this.is_jumpFalling = false;
				this.is_jumping = false;
				this.is_wallJumping = false;

				if (this.is_fallRecoverNeeded) {
					this.is_fallRecoverNeeded = false;
					this.fallRecover_lastTimer = this.fallRecover_time;
				}
			}

			if (this.is_sprinting) {
				this.stamina_amountLeft -= this.cost_sprint;
			}

			if (this.stamina_enabled) {
				if (this.stamina_amountLeft <= 0) {
					this.stamina_amountLeft = 0;
					this.is_exausted = true;
				}
				else if (this.stamina_amountLeft >= this.stamina_amount) {
					this.is_exausted = false;
				}
			}
			else {
				this.stamina_amountLeft = this.stamina_amount;
				this.is_exausted = false;
			}

			this.is_airTime = ((this.is_jumping || this.is_wallJumping || this.is_jumpFalling) && (Mathf.Abs(this.rb.velocity.y) < this.airTime_threshold));

			this.is_ledgeGrabbable = this.ledgeGrab.is_Detected;
		}

		void UpdateActions() {
			if (this.CanJump()) {
				this.DoJump();
				this.animationManager.TriggerJump();
			}
			else if (this.CanWallJump()) {
				this.DoWallJump();
			}
			else if (this.CanDash()) {
				this.DoDash();
			}

			if (CanLedgeGrab()) {
				this.DoLedgeGrab();
			}

			this.is_wallSliding = this.CanWallSlide();
		}

		/**
		 * Having extra gravity while falling helps things feel better.
		 * See: [Improve Your Platformer with Forces](https://youtu.be/KbtcEVCM7bw?t=303)
		 * 
		 * We can lower gravity for a short time when at the apex of our jump.
		 * See: [Improve your Platformer's Jump](https://youtu.be/2S3g8CgBG1g?t=92)
		 * 
		 * Your speed should speed up as you fall- but cap off at a max fall speed.
		 * See: [Improve your Platformer's Jump](https://youtu.be/2S3g8CgBG1g?t=63)
		 */
		void UpdateGravity() {
			if (this.is_dashAttacking || this.is_wallSliding) {
				this.rb.gravityScale = 0;
			}
			else if (this.is_diveFalling) {
				this.rb.gravityScale = this.gravity_scale * this.gravity_diveMultiplier;
				this.rb.velocity = new Vector2(this.rb.velocity.x, Mathf.Max(this.rb.velocity.y, -this.fallSpeed_dive));
			}
			else if (this.is_jumpCut) {
				this.rb.gravityScale = this.gravity_scale * this.gravity_jumpCutMultiplier;
				this.rb.velocity = new Vector2(this.rb.velocity.x, Mathf.Max(this.rb.velocity.y, -this.fallSpeed_jumpCut));
			}
			else if (this.is_airTime) {
				this.rb.gravityScale = this.gravity_scale * this.gravity_airTimeMultiplier;
			}
			else if (this.is_falling) {
				this.rb.gravityScale = this.gravity_scale * this.gravity_fallMultiplier;
				this.rb.velocity = new Vector2(this.rb.velocity.x, Mathf.Max(this.rb.velocity.y, -this.fallSpeed_normal));
			}
			else {
				this.rb.gravityScale = this.gravity_scale;
			}
		}

		/**
		 * We will use forces to handle moving the character so there can be an acceleration and decceleration stage.
		 * This will also make other mechanics like springs much easier to implement.
		 * See: [Improve Your Platformer with Forces](https://youtu.be/KbtcEVCM7bw?t=112)
		 * See: [Improve your Platformer's Jump](https://youtu.be/2S3g8CgBG1g?t=347)
		 * 
		 * We can increase our speed when at the apex of our jump.
		 * See: [Improve your Platformer's Jump](https://youtu.be/2S3g8CgBG1g?t=148)
		 */
		void UpdateMovement() {
			if (this.is_dashing && !this.is_dashAttacking) {
				return;
			}
			if (this.is_fallRecovering) {
				return;
			}

			// Max Speed
			float maxSpeed = this.run_max;
			if (this.is_sprinting) {
				maxSpeed *= this.sprint_maxMultiplier;
			}
			if (this.is_airTime) {
				maxSpeed *= this.airTime_maxSpeedMultiplier;
			}

			// Target Speed
			float speed_target = this.input_move.x * maxSpeed;
			bool is_accelerating = (Mathf.Abs(speed_target) > 0.01f);

			if (this.is_riding) {
				speed_target += this.riding.rb.velocity.x;
			}

			float lerp_amount;
			if (this.is_dashing) {
				lerp_amount = this.dash_endLerp;
			}
			else if (this.is_wallJumping) {
				lerp_amount = this.wallJump_lerp;
			}
			else {
				lerp_amount = 1;
			}

			if (lerp_amount != 1) {
				speed_target = Mathf.Lerp(this.rb.velocity.x, speed_target, lerp_amount);
			}

			// Acceleration
			float acceleration_rate;
			if (this.run_keepMomentum && this.is_onGround && is_accelerating &&
				(Mathf.Abs(this.rb.velocity.x) > Mathf.Abs(speed_target)) &&
				(Mathf.Sign(this.rb.velocity.x) == Mathf.Sign(speed_target))
			) {
				// Keeps us from slowing down when we are going faster than the max speed
				return;
			}
			else if (is_accelerating) {
				acceleration_rate = this.run_acceleration;
				if (!this.is_onGround) {
					if (this.is_airTime) {
						acceleration_rate *= this.airTime_accelerationMultiplier;
					}
					acceleration_rate *= this.run_airAccelerationMultiplier;
				}
				else if (this.is_sprinting) {
					acceleration_rate *= this.sprint_accelerationMultiplier;
				}
			}
			else {
				acceleration_rate = this.run_decceleration;
				if (!this.is_onGround) {
					if (this.is_airTime) {
						acceleration_rate *= this.airTime_deccelerationMultiplier;
					}
					acceleration_rate *= this.run_airDeccelerationMultiplier;
				}
				else if (this.is_sprinting) {
					acceleration_rate *= this.sprint_deccelerationMultiplier;
				}
			}

			// Calculations
			float speed_delta = speed_target - this.rb.velocity.x;
			float velocity_force = speed_delta * acceleration_rate;

			this.rb.AddForce(velocity_force * Vector2.right, ForceMode2D.Force);
		}

		/**
		 * Because we have Unity friction turned off- it is very slippery. So, we will need to simulate our own friction.
		 * To do this, we apply a force against the direction of movement.
		 * See: [Improve Your Platformer with Forces](https://youtu.be/KbtcEVCM7bw?t=199)
		 * See: [Dawnosaur Drag Function](https://github.com/JoshMayberry/DawnosaurDev-platformer-movement/blob/da8a87a88a10cf49b9e135b8e377fa2b55dbda74/Platformer%20Tutorial/Assets/Scripts/Player/Advanced/PlayerAdvanced.cs#L254)
		 */
		void UpdateFriction() {
			float amount;

			if (!this.is_onGround) {
				amount = this.speed_drag;

				if (this.is_dashing) {
					amount *= this.dash_dragMultiplier;
				}
			}
			else if (this.animationManager.is_notMoving) {
				amount = this.speed_friction;
			}
			else {
				return;
			}

			if (amount == 0) {
				return;
			}

			Vector2 force = amount * this.rb.velocity.normalized;

			Vector2 force_onlySlowDown = new Vector2(
				Mathf.Min(Mathf.Abs(this.rb.velocity.x), Mathf.Abs(force.x)),
				Mathf.Min(Mathf.Abs(this.rb.velocity.y), Mathf.Abs(force.y))
			);

			Vector2 force_directional = new Vector2(
				force_onlySlowDown.x * Mathf.Sign(this.rb.velocity.x),
				force_onlySlowDown.y * Mathf.Sign(this.rb.velocity.y)
			);

			this.rb.AddForce(-force_directional, ForceMode2D.Impulse);
		}

		bool CanWallSlide() {
			if (this.is_onGround || !this.is_onWall || this.is_jumping || this.is_wallJumping || this.is_dashing) {
				return false;
			}

			if ((this.wallLeft_lastTimer > 0) && this.input_move.x < 0) {
				return true;
			}

			if ((this.wallRight_lastTimer > 0) && this.input_move.x > 0) {
				return true;
			}

			return false;
		}

		void DoWallSlide() {
			// Prevent Sliding Upwards
			if (this.rb.velocity.y > 0) {
				this.rb.AddForce(-this.rb.velocity.y * Vector2.up, ForceMode2D.Impulse);
			}

			float speed_target = this.run_wallSlide;
			float speed_delta = speed_target - this.rb.velocity.y;
			float velocity_force = speed_delta * this.run_wallSlideAcceleration;

			float clamp_size = Mathf.Abs(speed_delta) * (1 / Time.fixedDeltaTime);
			float velocity_forceClamped = Mathf.Clamp(velocity_force, -clamp_size, clamp_size); // Clamp to prevents over corrections

			this.rb.AddForce(velocity_forceClamped * Vector2.up, ForceMode2D.Force);
		}

		/**
		 * To allow for a 'grace time' with the jump, we will use a timer to allow jumps to happen a little 'too early'.
		 * See: [Improve your Platformer's Jump](https://youtu.be/2S3g8CgBG1g?t=246)
		 * 
		 * Allow the player to jump in the air if they have extra jumps left.
		 * See: [Coding a Platformer Controller](https://gamedevrocket.podia.com/view/courses/game-dev-rocket/2019264-make-a-2d-platformer/6342251-coding-a-platformer-controller)
		 */
		bool CanJump() {
			if (this.is_jumping) {
				return false; // We are currently in a jump cycle
			}

			if (this.jump_lastPressed < 0.01f) {
				return false; // The jump button was not pressed recently
			}

			if (this.is_onGround) {
				return true;
			}

			if (this.is_onWall) {
				return false; // Let them do a wall jump instead
			}

			if (this.stamina_enabled && (this.is_exausted || (this.cost_jump > this.stamina_amountLeft))) {
				return false;
			}

			if (this.jump_amountLeft <= 0) {
				return false;
			}

			return true;
		}

		/**
		 * We will add force upwards to make the player jump.
		 * See: [Improve Your Platformer with Forces](https://youtu.be/KbtcEVCM7bw?t=228)
		 * See: [Improve your Platformer's Jump](https://youtu.be/2S3g8CgBG1g?t=189)
		 */
		void DoJump() {
			this.is_jumping = true;
			this.is_wallJumping = false;
			this.is_jumpCut = false;
			this.is_jumpFalling = false;

			this.jump_lastPressed = 0;
			this.ground_lastTimer = 0;

			if (!this.is_onGround) {
				this.jump_amountLeft--;
				this.stamina_amountLeft -= this.cost_jump;
			}

			float force = this.jump_force;
			if (this.is_falling) {
				force -= this.rb.velocity.y;
			}

			this.rb.AddForce(force * Vector2.up, ForceMode2D.Impulse);
		}

		/**
		 * When the player releases the jump key, if they have not yet reached the full jump height- apply a downwards force to cut it off.
		 * This allows for high jumps and short jumps.
		 * See: [Improve Your Platformer with Forces](https://youtu.be/KbtcEVCM7bw?t=276)
		 */
		bool CanJumpCut() {
			if (!this.is_jumping && !this.is_wallJumping) {
				return false;
			}

			if (this.is_falling) {
				return false; // Already falling
			}

			return true;
		}

		bool CanLedgeGrab() {
			if (this.ledgeGrab_lastPressed < 0.01f) {
				return false; // The grab button was not pressed recently
			}

			if (!is_ledgeGrabbable) {
				return false;
			}

			if (!is_ledgeGrabbing) {
				return false;
			}

			return true;
		}

		/**
		 * See: [How To Ledge Climb in Unity](https://youtu.be/Kh5n63A-YBw?t=887)
		 */
		void DoLedgeGrab() {
			this.is_ledgeGrabbing = true;
			this.ledgeGrab_lastPressed = 0;

			Vector2 ledgePosition = this.ledgeGrab.transform.position;
			this.ledgeGrab_beginPosition = ledgePosition + this.ledgeGrab_beginPositionOffset;
			this.ledgeGrab_endPosition = ledgePosition + this.ledgeGrab_endPositionOffset;

			// TODO: Need animations first before we can continue
		}

		bool CanWallJump() {
			if (this.jump_lastPressed < 0.01f) {
				return false; // The jump button was not pressed recently
			}

			if (!this.is_onWall || this.is_onGround) {
				return false;
			}

			if (!this.is_wallJumping) {
				return true;
			}

			if (this.stamina_enabled && (this.is_exausted || (this.cost_wallJump > this.stamina_amountLeft))) {
				return false;
			}

			// Allow for jumping between 2 parallel walls
			if (this.wallJump_lastWasLeft) {
				return (this.wallRight_lastTimer > 0);
			}

			return (this.wallLeft_lastTimer > 0);
		}

		/**
		 * See: [Improve your Platformer's Jump](https://youtu.be/2S3g8CgBG1g?t=270)
		 */
		void DoWallJump() {
			this.is_jumping = false;
			this.is_wallJumping = true;
			this.is_jumpCut = false;
			this.is_jumpFalling = false;

			this.wallJump_startTime = Time.time;
			this.wallJump_lastWasLeft = (this.wallLeft_lastTimer > 0);

			this.jump_lastPressed = 0;
			this.ground_lastTimer = 0;
			this.wallLeft_lastTimer = 0;
			this.wallRight_lastTimer = 0;

			this.wallJump_amountLeft--;
			this.stamina_amountLeft -= this.cost_wallJump;

			Vector2 force = new Vector2(this.wallJump_force.x, this.wallJump_force.y);

			if (!this.wallJump_lastWasLeft) {
				force.x *= -1;
			}

			if (Mathf.Sign(this.rb.velocity.x) != Mathf.Sign(force.x)) {
				force.x -= this.rb.velocity.x;
			}

			if (this.is_falling) {
				force.y -= this.rb.velocity.y;
			}

			this.rb.AddForce(force, ForceMode2D.Impulse);
		}

		bool CanDash() {
			if (this.is_dashing) {
				return false; // We are currently in a dash cycle
			}

			if (this.dash_lastPressed < 0.01f) {
				return false; // The dash button was not pressed recently
			}

			if (this.stamina_enabled && (this.is_exausted || (this.cost_dash > this.stamina_amountLeft))) {
				return false;
			}

			return (this.dash_amountLeft > 0);
		}
		void DoDash() {
			this.is_jumping = false;
			this.is_wallJumping = false;
			this.is_dashing = true;
			this.is_jumpCut = false;
			this.is_jumpFalling = false;

			this.ground_lastTimer = 0;
			this.dash_lastPressed = 0;

			this.dash_amountLeft--;
			this.stamina_amountLeft -= this.cost_dash;

			if (this.dash_sleepTime != 0) {
				StartCoroutine(GameManager.instance.FreezeTime(this.dash_sleepTime));
			}

			if (this.input_move != Vector2.zero) {
				this.dash_lastDirection = new Vector2(this.input_move.x, this.input_move.y);
			}
			else if (this.animationManager.is_facingRight) {
				this.dash_lastDirection = Vector2.right;
			}
			else {
				this.dash_lastDirection = Vector2.left;
			}

			StartCoroutine(StartDash(this.dash_lastDirection));
		}

		IEnumerator StartDash(Vector2 direction) {
			float startTime = Time.time;

			this.is_dashAttacking = true;
			this.rb.gravityScale = 0;

			// Create our own temporary Update Loop
			while (Time.time - startTime <= this.dash_attackTime) {
				// Takes control away from the player
				this.rb.velocity = this.dash_speed * direction.normalized;
				yield return null;
			}

			startTime = Time.time;
			this.is_dashAttacking = false;
			this.rb.gravityScale = this.gravity_scale;

			this.rb.velocity = this.dash_speed * direction.normalized;
			while (Time.time - startTime <= this.dash_endTime) {
				// Return control to the player
				yield return null;
			}

			this.is_dashing = false;
		}

		/**
		 * Over time, while on the ground, various things will fill back up
		 */
		void UpdateRefills() {
			if (!this.is_onGround) {
				return;
			}

			// TODO: Could we generalize this further with a list of structs?

			if (!this.is_dashRefilling && !this.is_dashing && (this.dash_amountLeft < this.dash_amount)) {
				this.is_dashRefilling = true;
				StartCoroutine(this.StartRefillDash());
			}

			if (!this.is_jumpRefilling && !this.is_jumping && (this.jump_amountLeft < this.jump_amount)) {
				this.is_jumpRefilling = true;
				StartCoroutine(this.StartRefillJump());
			}

			if (!this.is_wallJumpRefilling && !this.is_wallJumping && (this.wallJump_amountLeft < this.wallJump_amount)) {
				this.is_wallJumpRefilling = true;
				StartCoroutine(this.StartRefillWallJump());
			}

			if (!this.is_staminaRefilling && (this.stamina_amountLeft < this.stamina_amount)) {
				this.is_staminaRefilling = true;
				StartCoroutine(this.StartRefillStamina());
			}
		}

		IEnumerator StartRefillDash() {
			if (this.dash_refillTime == 0) {
				this.dash_amountLeft = this.dash_amount;
			}
			else {
				yield return new WaitForSeconds(this.dash_refillTime);
				this.dash_amountLeft = Mathf.Min(this.dash_amount, this.dash_amountLeft + 1);
			}
			this.is_dashRefilling = false;
		}

		IEnumerator StartRefillJump() {
			if (this.jump_refillTime == 0) {
				this.jump_amountLeft = this.jump_amount;
			}
			else {
				yield return new WaitForSeconds(this.jump_refillTime);
				this.jump_amountLeft = Mathf.Min(this.jump_amount, this.jump_amountLeft + 1);
			}
			this.is_jumpRefilling = false;
		}

		IEnumerator StartRefillWallJump() {
			if (this.wallJump_refillTime == 0) {
				this.wallJump_amountLeft = this.wallJump_amount;
			}
			else {
				yield return new WaitForSeconds(this.wallJump_refillTime);
				this.wallJump_amountLeft = Mathf.Min(this.wallJump_amount, this.wallJump_amountLeft + 1);
			}
			this.is_wallJumpRefilling = false;
		}

		IEnumerator StartRefillStamina() {
			if (this.stamina_refillTime == 0) {
				this.stamina_amountLeft = this.stamina_amount;
			}
			else {
				yield return new WaitForSeconds(this.stamina_refillTime);
				this.stamina_amountLeft = Mathf.Min(this.stamina_amount, this.stamina_amountLeft + 1);
			}
			this.is_staminaRefilling = false;
		}
	}
}