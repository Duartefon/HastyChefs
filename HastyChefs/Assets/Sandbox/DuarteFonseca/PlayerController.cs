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
        
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float sprintSpeed = 8f;
        [SerializeField] private float jumpForce = 1f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float groundCheckDistance = 0.2f;
        private float verticalRotation = 0f;
        
        [SerializeField] private float acceleration = 50f;
        
        [Header("Look Settings")]
        [SerializeField] private float lookSensitivity = 1f;
        [SerializeField] private float maxLookAngle = 80f;

        
        [Header("References")]
        [SerializeField] private Camera playerCamera;
        
        private PlayerActions _playerInput;
        private Rigidbody _rigidbody;

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

        private bool isGrounded = false;

        private Collider[] groundColliders;
        
        
        //TODO:
        // Possibly turn this into a machine state
        // player context: having status like isOnGround and stuff like that into a different class so that differrnet stateso f hte state machien can acess it 
        // Fix jump, add a ground check
        // Make gravity follow a fall curve that's smoother (possible exponential on the fall)
        // add crouch, and the remaining states
        // camera rotation is very werid on low values (moving the mosue very slowly)
        // if touching something, werid rotation
        // add camera shakes
        
        void Start()
        {
            PlayerInput playerInput = GetComponent<PlayerInput>();
            _rigidbody =  GetComponent<Rigidbody>();
      
            
            Cursor.lockState = CursorLockMode.Locked;
            _playerInput =  new PlayerActions(playerInput);

            groundColliders = new Collider[16];
        }

        // Update is called once per frame
        void Update()
        {
            groundHeight = Vector3.down * heightOffset;

          
            
            HandleRotation();
        }


        private void FixedUpdate()
        {
          //  HandleLevitation();
          //  UpdateUprightForce();
            HandleMove();
            HandleJump();
            HandleGroundCheck();

        }



        public void HandleGroundCheck()
        {
            Vector3 startPosition = transform.position + groundHeight; 
            var size = Physics.OverlapSphereNonAlloc(startPosition, radius: radius, groundColliders);

            isGrounded = (size >= 2);

            // Only iterate up to 'size' to avoid reading stale data or null slots
            for (int i = 0; i < size; i++)
            {
                Debug.Log($"NOME: {groundColliders[i].name} | Colliders detected: {size}");
            }
            
            
            Debug.Log("Tamanho do buffer: " + groundColliders.Length);
            Debug.Log("isGrounded: " + isGrounded);
            // if(!isGrounded)
            //      _rigidbody.AddForce(Vector3.down * gravity);

        }


        private void OnDrawGizmos()
        {
            Vector3 startPosition = transform.position + groundHeight;
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawSphere(startPosition, radius: radius);
            
            
        }

        public void HandleJump()
        {
            
            Debug.Log("JumpValue: " + _playerInput.JumpVector);
            if(_playerInput.JumpVector > 0  ) //verify if it's on the ground
                _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        private void HandleMove()
        {
            Vector2 input = _playerInput.MoveVector;

            Vector3 camForward = Vector3.ProjectOnPlane(playerCamera.transform.forward, Vector3.up).normalized;
            Vector3 camRight = Vector3.ProjectOnPlane(playerCamera.transform.right, Vector3.up).normalized;
            Vector3 moveDir = Vector3.ClampMagnitude((camForward * input.y) + (camRight * input.x), 1f);

            // Smoothly accelerate toward target horizontal velocity without overwriting gravity
            Vector3 currentHorizontal = new Vector3(_rigidbody.linearVelocity.x, 0f, _rigidbody.linearVelocity.z);
            Vector3 targetHorizontal = moveDir * moveSpeed;
            Vector3 velocityChange = Vector3.MoveTowards(currentHorizontal, targetHorizontal, acceleration * Time.fixedDeltaTime) - currentHorizontal;

            _rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
        
        }
        
        

        private void HandleRotation()
        {
            Vector2 lookInput = _playerInput.LookVector * lookSensitivity;

            verticalRotation -= lookInput.y;
            verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
            playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

            transform.Rotate(Vector3.up * lookInput.x);
        }

        private void HandleLevitation()
        {
            Vector3 startPosition = transform.position + Vector3.down * 0.5f;
            
            bool rayDidHit = Physics.Raycast(startPosition, Vector3.down, out RaycastHit hit, 6f);
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
                
                print("OtherBody: " + hit.rigidbody.name  + " RBSpringForce: " + rayDirection * springForce + " RayDirection: " + rayDirection + " RelativeVelocty: " + relativeVelocity + " SpringScaleForce: " + springScaleForce);

                



            }
            
        }


       public void UpdateUprightForce()
        {
            Quaternion characterCurrentRotation = transform.rotation;
            Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
            
            //I don't understand quaternions the ideia is to get the quaternion that represents the upright rotation 
            Quaternion uprightTarget = flatForward.sqrMagnitude > 0.001f
                ? Quaternion.LookRotation(flatForward, Vector3.up)
                : Quaternion.identity;
            Quaternion goalRotation = MathUtilities.ShortestRotation(uprightTarget, characterCurrentRotation);
           
            Vector3 rotationAxis;
            float rotationDegrees;
            
            goalRotation.ToAngleAxis(out rotationDegrees, out rotationAxis);
            
            rotationAxis.Normalize();
            
            float rotationRadians = rotationDegrees * Mathf.Deg2Rad;
            
            _rigidbody.AddTorque(rotationAxis * (rotationRadians * _uprightJointSpringStrength) - (_rigidbody.angularVelocity * _uprightJointSpringDamper));

        }


    }
}
