using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class FirstPersonController : MonoBehaviour
    {
        public enum PlayerState { Standing, Crouching, Proning }
        public bool CameraDisable = false;
        public bool MovementDisable = false;

        [Header("Player")]
        public float MoveSpeed = 4.0f;
        public float SprintSpeed = 6.0f;
        public float RotationSpeed = 1.0f;
        public float SpeedChangeRate = 10.0f;

        [Space(10)]
        public float JumpHeight = 1.2f;
        public float Gravity = -15.0f;

        [Space(10)]
        public float JumpTimeout = 0.1f;
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        public bool Grounded = true;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.5f;
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        public GameObject CinemachineCameraTarget;
        public float TopClamp = 90.0f;
        public float BottomClamp = -90.0f;

        [Header("Stamina System")]
        public float Stamina = 100f;
        public float MaxStamina = 100f;
        public float StaminaDrainRate = 20f;
        public float StaminaRegenRate = 15f;
        public float MinSprintStamina = 10f;

        [Header("Crouch Settings")]
        public float StandingHeight = 2.0f;
        public float CrouchHeight = 1.0f;
        public float CrouchCameraYOffset = -0.5f;

        [Header("Prone Settings")]
        public float ProneHeight = 0.5f;
        public float ProneCameraYOffset = -1.0f;

        [Header("Camera Settings")]
        public float MouseSensitivity = 1.0f;

        [Header("Dolphin Dive Settings")]
        [Tooltip("Initial forward speed applied during dolphin dive (m/s)")]
        public float DolphinDiveInitialSpeed = 14f;
        [Tooltip("Initial upward impulse applied when dive starts (m/s) - increases 'jump' feeling")]
        public float DolphinDiveUpImpulse = 6f;
        [Tooltip("How long the smoothing/ease for forward speed lasts (seconds)")]
        public float DolphinDiveDuration = 0.55f;
        [Tooltip("Time after a dive before you can dive again")]
        public float DolphinDiveCooldown = 1.4f;
        [Tooltip("Stamina cost to perform a dive")]
        public float DolphinDiveStaminaCost = 30f;
        [Tooltip("Allow a small steering influence while diving")]
        public float DolphinDiveSteer = 2.5f;
        [Tooltip("Scale of gravity during initial dive (0..1). Lower = floatier arc")]
        [Range(0f, 1f)]
        public float DolphinDiveGravityScale = 0.6f;

        [Header("Collision")]
        public float CapsuleCastSkin = 0.05f; // small gap to prevent immediate collision

        [Header("Movement Penalties")]
        public float CrouchSpeedMultiplier = 0.5f;
        public float ProneSpeedMultiplier = 0.2f;

        // internal state
        public PlayerState _playerState = PlayerState.Standing;
        private float _lastCrouchPressTime = -10f;
        private float _crouchDoublePressThreshold = 0.3f;

        private Vector3 _originalCameraLocalPos;
        private float _originalControllerHeight;
        private Vector3 _originalControllerCenter;

        // camera & movement
        private float _cinemachineTargetPitch;
        private float _speed;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timers
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private CharacterController _controller;
        private GameObject _mainCamera;
        public ConsoleManager consoleManager;

        private const float _threshold = 0.01f;

        // Dive internals
        public bool _isDiving = false;
        private float _diveElapsed = 0f;          // used to ease forward speed while diving
        private float _diveCooldownTimer = 0f;
        private float _currentDiveSpeed = 0f;
        private Vector3 _diveDirection = Vector3.zero;
        public bool _leftGroundDuringDive = false; // ensures we only end dive after leaving and returning to ground
        private float _cameraTiltZ = 0f;

        private float _heightLerpSpeed = 10f;
        private float _targetControllerHeight;
        private Vector3 _targetControllerCenter;
        private Vector3 _cameraLocalPosTarget;
        private Vector3 _originalControllerCenterCached;

        // For dynamic ground check adjustment
        private float _originalGroundedRadius;
        private float _originalGroundedOffset;
        public float DiveGroundedRadius = 0.7f; // larger radius for dive
        public float DiveGroundedOffset = -0.22f; // closer to ground for dive

        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#endif
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            _originalCameraLocalPos = CinemachineCameraTarget != null ? CinemachineCameraTarget.transform.localPosition : Vector3.zero;
            _originalControllerHeight = _controller.height;
            _originalControllerCenter = _controller.center;
            _originalControllerCenterCached = _originalControllerCenter;

            _targetControllerHeight = _controller.height;
            _targetControllerCenter = _controller.center;
            _cameraLocalPosTarget = _originalCameraLocalPos;

            // Store original ground check values
            _originalGroundedRadius = GroundedRadius;
            _originalGroundedOffset = GroundedOffset;

            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            // Adjust ground check for diving
            if (_isDiving)
            {
                GroundedRadius = DiveGroundedRadius;
                GroundedOffset = DiveGroundedOffset;
            }
            else
            {
                GroundedRadius = _originalGroundedRadius;
                GroundedOffset = _originalGroundedOffset;
            }
            HandleTimers();
            HandleInputsAndStates();
            HandleStanceSmooth();
            HandleStamina();
            JumpAndGravity();
            GroundedCheck();
            Move();
            DeveloperConsoleBind();
        }

        void DeveloperConsoleBind()
        {
            var consoleEnabled = consoleManager.consoleActive;
            if (_playerInput.actions["Console"].WasPressedThisFrame())
            {
                consoleManager.ToggleConsole();
                if (consoleManager.consoleActive)
                {
                    Cursor.lockState = CursorLockMode.None;
                    ToggleDisableCamera(true);
                    ToggleDisableMovement(true);
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    ToggleDisableCamera(false);
                    ToggleDisableMovement(false);
                }
            }
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void HandleTimers()
        {
            if (_diveCooldownTimer > 0f) _diveCooldownTimer -= Time.deltaTime;
            if (_jumpTimeoutDelta > 0f) _jumpTimeoutDelta -= Time.deltaTime;
            if (_fallTimeoutDelta > 0f) _fallTimeoutDelta -= Time.deltaTime;
        }

        private void HandleInputsAndStates()
        {
            if (MovementDisable) return;
#if ENABLE_INPUT_SYSTEM
            bool crouchPressed = _playerInput.actions["Crouch"].WasPressedThisFrame();
            bool pronePressed = _playerInput.actions["Prone"] != null && _playerInput.actions["Prone"].WasPressedThisFrame();
            bool sprintHeld = _playerInput.actions["Sprint"].IsPressed();

            // diveTrigger: single press detection (start dive)
            bool diveTrigger = false;
            var diveAction = _playerInput.actions.FindAction("Dive");
            if (diveAction != null)
            {
                diveTrigger = diveAction.WasPressedThisFrame();
            }
            else
            {
                // fallback: sprint + crouch pressed this frame
                diveTrigger = (sprintHeld && crouchPressed);
            }
#else
			bool crouchPressed = Input.GetKeyDown(KeyCode.C);
			bool pronePressed = Input.GetKeyDown(KeyCode.Z);
			bool sprintHeld = Input.GetKey(KeyCode.LeftShift);
			bool diveTrigger = (sprintHeld && crouchPressed);
#endif
            // stance handling
#if ENABLE_INPUT_SYSTEM
            bool pronePressedLocal = _playerInput.actions["Prone"] != null && _playerInput.actions["Prone"].WasPressedThisFrame();
            if (pronePressedLocal) SetPlayerState(PlayerState.Proning);
            else if (crouchPressed)
#else
			if (pronePressed) SetPlayerState(PlayerState.Proning);
			else if (crouchPressed)
#endif
            {
                if (Time.time - _lastCrouchPressTime < _crouchDoublePressThreshold)
                {
                    SetPlayerState(PlayerState.Proning);
                }
                else if (_playerState == PlayerState.Crouching)
                {
                    SetPlayerState(PlayerState.Standing);
                }
                else
                {
                    SetPlayerState(PlayerState.Crouching);
                }
                _lastCrouchPressTime = Time.time;
            }

            // Try to start dive
#if ENABLE_INPUT_SYSTEM
            bool sprintHeldForDive = _playerInput.actions["Sprint"].IsPressed();
#else
			bool sprintHeldForDive = Input.GetKey(KeyCode.LeftShift);
#endif
            TryStartDive(diveTrigger, sprintHeldForDive);
        }

        private void TryStartDive(bool diveTrigger, bool sprintHeld)
        {
            if (_isDiving) return;
            if (!diveTrigger) return;
            if (!Grounded) return;
            if (_playerState == PlayerState.Proning) return;
            if (Stamina < DolphinDiveStaminaCost) return;
            if (_diveCooldownTimer > 0f) return;

            // consume & set cooldown
            Stamina = Mathf.Max(0f, Stamina - DolphinDiveStaminaCost);
            _diveCooldownTimer = DolphinDiveCooldown;

            // dive setup
            _isDiving = true;
            _diveElapsed = 0f;
            _currentDiveSpeed = DolphinDiveInitialSpeed;
            _diveDirection = transform.forward.normalized;
            _leftGroundDuringDive = false;

            // camera tilt
            _cameraTiltZ = -8f;

            // make prone target (smooth)
            SetPlayerState(PlayerState.Proning);

            // upward impulse: give clear upward pop to feel like a jump-dive
            _verticalVelocity = Mathf.Max(_verticalVelocity, DolphinDiveUpImpulse);
        }

        private void HandleStanceSmooth()
        {
            switch (_playerState)
            {
                case PlayerState.Standing:
                    _targetControllerHeight = StandingHeight;
                    _cameraLocalPosTarget = _originalCameraLocalPos;
                    _targetControllerCenter = _originalControllerCenterCached;
                    break;
                case PlayerState.Crouching:
                    _targetControllerHeight = CrouchHeight;
                    _cameraLocalPosTarget = _originalCameraLocalPos + new Vector3(0, CrouchCameraYOffset, 0);
                    _targetControllerCenter = new Vector3(_originalControllerCenterCached.x, CrouchHeight / 2f, _originalControllerCenterCached.z);
                    break;
                case PlayerState.Proning:
                    _targetControllerHeight = ProneHeight;
                    _cameraLocalPosTarget = _originalCameraLocalPos + new Vector3(0, ProneCameraYOffset, 0);
                    _targetControllerCenter = new Vector3(_originalControllerCenterCached.x, ProneHeight / 2f, _originalControllerCenterCached.z);
                    break;
            }

            _controller.height = Mathf.Lerp(_controller.height, _targetControllerHeight, Time.deltaTime * _heightLerpSpeed);
            _controller.center = Vector3.Lerp(_controller.center, _targetControllerCenter, Time.deltaTime * _heightLerpSpeed);

            if (CinemachineCameraTarget != null)
            {
                CinemachineCameraTarget.transform.localPosition = Vector3.Lerp(CinemachineCameraTarget.transform.localPosition, _cameraLocalPosTarget, Time.deltaTime * _heightLerpSpeed);
            }
        }

        private void HandleStamina()
        {
#if ENABLE_INPUT_SYSTEM
            bool sprint = _playerInput.actions["Sprint"].IsPressed();
            Vector2 move = _playerInput.actions["Move"].ReadValue<Vector2>();
#else
			bool sprint = Input.GetKey(KeyCode.LeftShift);
			Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
#endif
            if (sprint && move != Vector2.zero && Stamina > 0f)
            {
                Stamina -= StaminaDrainRate * Time.deltaTime;
                if (Stamina < 0f) Stamina = 0f;
            }
            else
            {
                Stamina += StaminaRegenRate * Time.deltaTime;
                if (Stamina > MaxStamina) Stamina = MaxStamina;
            }
        }

        private void CameraRotation()
        {
            if (CameraDisable) return;
#if ENABLE_INPUT_SYSTEM
            Vector2 look = _playerInput.actions["Look"].ReadValue<Vector2>() * MouseSensitivity;
#else
			Vector2 look = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * MouseSensitivity;
#endif
            if (look.sqrMagnitude >= _threshold)
            {
                float deltaTimeMultiplier = (IsCurrentDeviceMouse()) ? 1.0f : Time.deltaTime;
                _cinemachineTargetPitch -= look.y * RotationSpeed * deltaTimeMultiplier;
                _rotationVelocity = look.x * RotationSpeed * deltaTimeMultiplier;
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

                if (CinemachineCameraTarget != null)
                    CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, _cameraTiltZ);

                transform.Rotate(Vector3.up * _rotationVelocity);
            }
            else
            {
                if (CinemachineCameraTarget != null)
                {
                    Quaternion target = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, _isDiving ? _cameraTiltZ : 0f);
                    CinemachineCameraTarget.transform.localRotation = Quaternion.Slerp(CinemachineCameraTarget.transform.localRotation, target, Time.deltaTime * 8f);
                }
            }
        }

        private void Move()
        {
            if (MovementDisable) return;
#if ENABLE_INPUT_SYSTEM
            bool sprint = _playerInput.actions["Sprint"].IsPressed();
            Vector2 move = _playerInput.actions["Move"].ReadValue<Vector2>();
#else
			bool sprint = Input.GetKey(KeyCode.LeftShift);
			Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
#endif

            // Diving movement override
            if (_isDiving)
            {
                // small steering allowed
                Vector3 steer = (move.sqrMagnitude > 0.001f) ? (transform.right * move.x + transform.forward * move.y).normalized : _diveDirection;
                _diveDirection = Vector3.Slerp(_diveDirection, steer, Time.deltaTime * DolphinDiveSteer);

                // ease forward speed from initial to MoveSpeed over DolphinDiveDuration (if duration == 0 skip)
                _diveElapsed += Time.deltaTime;
                float t = DolphinDiveDuration > 0f ? Mathf.Clamp01(_diveElapsed / DolphinDiveDuration) : 1f;
                _currentDiveSpeed = Mathf.Lerp(DolphinDiveInitialSpeed, MoveSpeed, t);

                Vector3 horizontalMove = _diveDirection * _currentDiveSpeed;
                Vector3 displacement = horizontalMove * Time.deltaTime + Vector3.up * _verticalVelocity * Time.deltaTime;

                Vector3 safeDisplacement = ComputeSafeDisplacement(displacement);
                _controller.Move(safeDisplacement);

                // If player left the ground during this dive and has now landed, end the dive
                if (_leftGroundDuringDive && Grounded)
                {
                    EndDive();
                }
                return;
            }

            // Normal movement
            float stateSpeedMultiplier = 1.0f;
            switch (_playerState)
            {
                case PlayerState.Crouching:
                    stateSpeedMultiplier = CrouchSpeedMultiplier;
                    break;
                case PlayerState.Proning:
                    stateSpeedMultiplier = ProneSpeedMultiplier;
                    break;
            }

            float targetSpeed = (sprint && Stamina > MinSprintStamina) ? SprintSpeed : MoveSpeed;
            targetSpeed *= stateSpeedMultiplier;
            if (move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            float speedOffset = 0.1f;
            float inputMagnitude = (move != Vector2.zero) ? move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            Vector3 inputDirection = Vector3.zero;
            if (move != Vector2.zero)
            {
                inputDirection = transform.right * move.x + transform.forward * move.y;
                inputDirection = inputDirection.normalized;
            }

            Vector3 desiredMove = inputDirection * _speed;
            Vector3 displacementNoY = desiredMove * Time.deltaTime;
            Vector3 totalDisplacement = displacementNoY + Vector3.up * _verticalVelocity * Time.deltaTime;

            Vector3 safe = ComputeSafeDisplacement(totalDisplacement);
            _controller.Move(safe);
        }

        private Vector3 ComputeSafeDisplacement(Vector3 desiredDisplacement)
        {
            float radius = Mathf.Max(0.01f, _controller.radius);
            float halfHeight = Mathf.Max(radius, (_controller.height * 0.5f) - radius);
            Vector3 center = transform.position + _controller.center;

            Vector3 p1 = center + Vector3.up * halfHeight;
            Vector3 p2 = center - Vector3.up * halfHeight;

            Vector3 horizDisplacement = new Vector3(desiredDisplacement.x, 0f, desiredDisplacement.z);
            float distance = horizDisplacement.magnitude;

            if (distance > 0.001f)
            {
                Vector3 dir = horizDisplacement.normalized;
                if (Physics.CapsuleCast(p1, p2, radius, dir, out RaycastHit hit, distance + CapsuleCastSkin, ~0, QueryTriggerInteraction.Ignore))
                {
                    float moveDist = Mathf.Max(0f, hit.distance - CapsuleCastSkin);
                    Vector3 movePart = dir * moveDist;
                    Vector3 remaining = horizDisplacement - movePart;
                    Vector3 slide = Vector3.ProjectOnPlane(remaining, hit.normal);
                    Vector3 result = movePart + slide + Vector3.up * desiredDisplacement.y;
                    return result;
                }
            }

            return desiredDisplacement;
        }

        private void EndDive()
        {
            _isDiving = false;
            _diveElapsed = 0f;
            _diveCooldownTimer = DolphinDiveCooldown;
            _cameraTiltZ = 0f;
            // remain prone after dive
            SetPlayerState(PlayerState.Proning);
        }

        private void JumpAndGravity()
        {
            if (MovementDisable) return;
#if ENABLE_INPUT_SYSTEM
            bool jump = _playerInput.actions["Jump"].WasPressedThisFrame();
#else
			bool jump = Input.GetButtonDown("Jump");
#endif
            // detect if we leave the ground during a dive (so we can end on next landing)
            if (_isDiving && !_leftGroundDuringDive && !Grounded)
            {
                _leftGroundDuringDive = true;
            }

            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                if (jump && _jumpTimeoutDelta <= 0.0f && !_isDiving)
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                }
            }
            else
            {
                _jumpTimeoutDelta = JumpTimeout;
            }

            // apply gravity; when diving use reduced gravity for floaty arc
            if (_verticalVelocity < _terminalVelocity)
            {
                float scale = _isDiving ? DolphinDiveGravityScale : 1f;
                _verticalVelocity += Gravity * scale * Time.deltaTime;
            }
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void SetPlayerState(PlayerState newState)
        {
            if (_playerState == newState) return;
            _playerState = newState;
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private bool IsCurrentDeviceMouse()
        {
#if ENABLE_INPUT_SYSTEM
            if (_playerInput == null) return true;
            return _playerInput.currentControlScheme == "KeyboardMouse";
#else
			return true;
#endif
        }

        public void ToggleDisableCamera(bool set)
        {
            CameraDisable = set;
        }

        public void ToggleDisableMovement(bool set)
        {
            MovementDisable = set;
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // Draw the ground check sphere (wireframe for clarity)
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Gizmos.DrawWireSphere(spherePosition, GroundedRadius);
        }
    }
}