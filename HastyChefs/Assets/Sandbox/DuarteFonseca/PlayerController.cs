    using System;
    using System.Numerics;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using UnityEngine.UIElements;
    using Cursor = UnityEngine.Cursor;
    using Quaternion = UnityEngine.Quaternion;
    using Vector2 = UnityEngine.Vector2;
    using Vector3 = UnityEngine.Vector3;

    namespace Sandbox.DuarteFonseca
    {
        using UnityEngine.InputSystem;
        [RequireComponent(typeof(PlayerInput))]  
        public class PlayerController : MonoBehaviour
        {
            
            [Header("Movement Settings")] //to move to a scriptable obj
            [SerializeField] private float moveSpeed = 5f;
            [SerializeField] private float sprintSpeed = 8f;
            [SerializeField] private float jumpForce = 1f;
            [SerializeField] private float gravity = -9.81f;
            [SerializeField] private float groundCheckDistance = 0.2f;
            
            
            [SerializeField] private float acceleration = 50f;

            [SerializeField] private float tumbleVelocityThreshold = 4f;
            [SerializeField] private float tumbleSpeed = 20f;
            [Header("Height Settings")]
            [SerializeField] private float standingEyeHeight = 1.6f;
            [SerializeField] private float crouchingEyeHeight = 0.9f;
            [SerializeField] private float crouchTransitionSpeed = 10f;
    

            
            [Header("References")]
            [SerializeField] private Camera playerCamera;
            [SerializeField] private LayerMask groundLayer;
            [SerializeField] private Transform cameraHolder;
            [SerializeField] private Transform crouchPivot;

            
            [Header("Settings")] //to move to a script obj
            
            [SerializeField] private float heightOffset = 1.0f;
            private Vector3 groundHeight;

            //todo: player stats to a different script 
            [SerializeField] private  float RideHeight = 2f;
            [SerializeField] private  float RideSpringDamper = 4f;
            [SerializeField] private  float RideSpringStrength = 5f;
            [SerializeField]  private float _uprightJointSpringStrength = 4f; //how strong the string is to pull the character upright 
            [SerializeField]   private float _uprightJointSpringDamper = 4f; //how much impact the string absorbs, aka more bouncy or less bouncy 
            
            // Start is called once before the first execution of Update after the MonoBehaviour is created
            [SerializeField] private float radius = 1.0f;
            [SerializeField] private AnimationCurve gravityCurve;

            private bool isGrounded = false;
            private bool _isTumbling;

            private Collider[] groundColliders;
            
            
            private float verticalRotation = 0f;
            private float horizontalRotation = 0f;
            private PlayerActions _playerInput;
            private Rigidbody _rigidbody;
            private CapsuleCollider _capsuleCollider;

            private ClockDelay _airBornTimer;
            
            public bool IsCrouching { get; set; }
            //TODO:
            // Possibly turn this into a machine state
            // player context: having status like isOnGround and stuff like that into a different class so that differrnet stateso f hte state machien can acess it 
            // Make crouch have step-in and step-out, and sprint
            // Fix bugs and test in multiplayer 
            
            [Header("Air Movement (CS-Style)")]
            [SerializeField] private float airAcceleration = 80f;
            [Tooltip("Low speed cap along the wish direction (classic Quake/CS uses ~1.0 to 1.5)")]
            [SerializeField] private float airMaxWishSpeed = 1.2f;
            
            void Start()
            {
                PlayerInput playerInput = GetComponent<PlayerInput>();
                _rigidbody =  GetComponent<Rigidbody>();
                _capsuleCollider = GetComponent<CapsuleCollider>();
                
                Cursor.lockState = CursorLockMode.Locked;
                _playerInput =  new PlayerActions(playerInput);

                groundColliders = new Collider[16];

                _airBornTimer = new ClockDelay(Time.time / 1000);
                _airBornTimer.WaitTime = 5;
            }

            // Update is called once per frame
            void Update()
            {
                groundHeight = Vector3.down * heightOffset; //debug only 
            }

            private void LateUpdate()
            {
                
                    if (_isTumbling)
                    {
                        // Keep CameraHolder's horizon upright in world space so Cinemachine's
                        // pitch clamp (-80° to +80°) retains its full upward and downward range.
                        // We preserve the capsule's world yaw (or keep it free) while leveling pitch/roll.
                        Vector3 flatForward = Vector3.ProjectOnPlane(transform.up, Vector3.up);
                        if (flatForward.sqrMagnitude < 0.001f)
                            flatForward = Vector3.ProjectOnPlane(playerCamera.transform.forward, Vector3.up);

                        cameraHolder.rotation = Quaternion.LookRotation(flatForward.normalized, Vector3.up);
                    }
                    else
                    {
                        cameraHolder.localRotation = Quaternion.identity;
                }
            }


            private void FixedUpdate()
            {
                if(isGrounded)
                    HandleLevitation();
                HandleMove();
                HandleJump();
                HandleGroundCheck();
                HandleCrouch();
                
                HandleTumble();
                


            }

            private float gravAccel;

            public void HandleGroundCheck()
            {
                Vector3 startPosition = transform.position + groundHeight; 
                var size = Physics.OverlapSphereNonAlloc(startPosition, radius: radius, groundColliders, groundLayer);

                isGrounded = size >= 1;

                
                if (!isGrounded)
                {
                    _airBornTimer.UpdateTick(Time.deltaTime);
                     gravAccel = gravityCurve.Evaluate(_airBornTimer.getStopWatchTime());
                    

                    
                    

                }

                else
                {
                    _airBornTimer.UpdateTimeStamp();
                    gravAccel = 0;


                }
                
                _rigidbody.AddForce(Vector3.down * gravAccel, ForceMode.Acceleration);
              
                
            }



            public void HandleTumble()
            {
                if (_isTumbling) return;

                if (_playerInput.SprintVector > 0 && _playerInput.Jump.WasPressedThisFrame() &&
                    _rigidbody.linearVelocity.magnitude > tumbleVelocityThreshold)
                {
                    _isTumbling = true;

                    // 1. Get the flat forward direction where Cinemachine is actually looking
                    Vector3 flatForward = Vector3.ProjectOnPlane(playerCamera.transform.forward, Vector3.up).normalized;
                    if (flatForward.sqrMagnitude < 0.001f)
                        flatForward = transform.forward;

                    // 2. Unfreeze Rigidbody rotation so physics can roll/tumble the capsule
                   // _rigidbody.constraints = RigidbodyConstraints.None;

                    // 3. Face the look direction and pitch 90° forward onto the belly
                    Quaternion lookYaw = Quaternion.LookRotation(flatForward, Vector3.up);
                    Quaternion diveRotation = lookYaw * Quaternion.Euler(90f, 0f, 0f);
                    _rigidbody.MoveRotation(diveRotation);

                    // 4. Launch forward along that look direction
                    _rigidbody.AddForce((flatForward + Vector3.up * 0.25f) * tumbleSpeed, ForceMode.Impulse);
                }
            }

            public void HandleCrouch()
            {
                var cameraPosition = cameraHolder.localPosition;
                IsCrouching = _playerInput.CrouchVector > 0;
                if (IsCrouching)
                {
                   
                    _capsuleCollider.height = 1.45f;
                    _capsuleCollider.center = new Vector3(0, -0.5f, 0);
         
                }
                else
                {
                    _capsuleCollider.height = 2;
                    _capsuleCollider.center = new Vector3();
                }
                
                float targetY = IsCrouching ? crouchingEyeHeight : standingEyeHeight;
                Vector3 currentPos = crouchPivot.localPosition;
                currentPos.y = Mathf.Lerp(currentPos.y, targetY, crouchTransitionSpeed * Time.deltaTime);
                crouchPivot.localPosition = currentPos;
                
            }


            public void HandleJump()
            {
                
            if (_playerInput.JumpVector >0 && isGrounded)
            {
                Vector3 vel = _rigidbody.linearVelocity;
                _rigidbody.linearVelocity = new Vector3(vel.x, 0f, vel.z);
                _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
            }

            //trying air accell to see if it's fun
            private void HandleMove()
            {
                Vector2 input = _playerInput.MoveVector;

                Vector3 camForward = Vector3.ProjectOnPlane(playerCamera.transform.forward, Vector3.up).normalized;
                Vector3 camRight = Vector3.ProjectOnPlane(playerCamera.transform.right, Vector3.up).normalized;
                Vector3 wishDir = Vector3.ClampMagnitude((camForward * input.y) + (camRight * input.x), 1f);

                if (isGrounded)
                {
                    GroundMove(wishDir);
                }
                else
                {
                    AirAccelerate(wishDir);
                }
            }

            private void GroundMove(Vector3 wishDir)
            {
                
                var speed = _playerInput.SprintVector > 0 ? sprintSpeed : moveSpeed;
                Vector3 currentHorizontal = new Vector3(_rigidbody.linearVelocity.x, 0f, _rigidbody.linearVelocity.z);
                Vector3 targetHorizontal = wishDir * speed;
                Vector3 velocityChange = Vector3.MoveTowards(currentHorizontal, targetHorizontal, acceleration * Time.fixedDeltaTime) - currentHorizontal;
    
                
                _rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
            }

            private void AirAccelerate(Vector3 wishDir)
            {
                if (wishDir.sqrMagnitude < 0.001f) return;

                Vector3 currentHorizontal = new Vector3(_rigidbody.linearVelocity.x, 0f, _rigidbody.linearVelocity.z);

                // 1. Project current velocity onto the desired movement direction
                float currentSpeedInWishDir = Vector3.Dot(currentHorizontal, wishDir);

                // 2. Only accelerate up to the low wish-speed cap (the secret to CS air-strafing)
                float addSpeed = airMaxWishSpeed - currentSpeedInWishDir;
                if (addSpeed <= 0f) return;

                // 3. Calculate acceleration step and clamp it so we don't overshoot the cap
                float accelSpeed = airAcceleration * moveSpeed * Time.fixedDeltaTime;
                if (accelSpeed > addSpeed)
                    accelSpeed = addSpeed;

                _rigidbody.AddForce(wishDir * accelSpeed, ForceMode.VelocityChange);
            }

            
      
            private void HandleLevitation()
            {
                Vector3 startPosition = transform.position + Vector3.down * 0.5f;
                
                bool rayDidHit = Physics.Raycast(startPosition, Vector3.down, out RaycastHit hit, 6f, groundLayer);
                Debug.DrawRay(startPosition, Vector3.down, Color.red);

                if (rayDidHit)
                {
                    Vector3 myVelocity = _rigidbody.linearVelocity;
                    Vector3 rayDirection = transform.TransformDirection(Vector3.down);

                    Vector3 otherVelocity = Vector3.zero;
                    Rigidbody otherRigidbody = hit.rigidbody;

                    if (otherRigidbody)
                    {
                        otherVelocity = otherRigidbody.linearVelocity;
                    }

                    //fazer o produto escalar entre a direção do raio (que sai dos pes do jogador) com o vetor de velocidad, esta a calcular o que?
                    
                    float myRayDirectionVelocty = Vector3.Dot(rayDirection, myVelocity);
                    
                    float otherDirectionVelocity = Vector3.Dot(rayDirection, otherVelocity);
                    
                    float relativeVelocity = myRayDirectionVelocty - otherDirectionVelocity;
                    
                    float springScaleForce = hit.distance - RideHeight;

                    float springForce = (springScaleForce * RideSpringStrength) - (relativeVelocity * RideSpringDamper);
                    
                    _rigidbody.AddForce(rayDirection * springForce);
                    

                    



                }
                
            }
            
            
            private void OnDrawGizmos()
            {
                Vector3 startPosition = transform.position + groundHeight;
                Gizmos.color = isGrounded ? Color.green : Color.red;
                Gizmos.DrawSphere(startPosition, radius: radius);
                
                
            }


            public bool IsGrounded => isGrounded;

        }
        
    }
