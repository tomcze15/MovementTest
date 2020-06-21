#define UNITY_EDITOR

#if     UNITY_EDITOR
//#define MOVEMENT
//#define RIGIDBODY
#define RAY
#endif

using System;
using UnityEngine;

namespace CharacterController.ThirdPerson
{
    public class ThirdPersonCharacterController : MonoBehaviour
    {
        [Serializable]
        public class MovementSettings
        {
            public KeyCode          RunKey;
            public AnimationCurve   RunTransitionCurve;
            public AnimationCurve   JumpCurve;
            public AnimationCurve   SlopeCurveModifier;

            public float Speed;
            public float RunMultiplier;
            [Range(0.0f, 10.0f)] public float TurnSpeed;

            [Header("Movement")]
            public bool MoveForward;
            public bool MoveBack;
            public bool MoveLeft;
            public bool MoveRight;
            public bool Jump;
            public bool Run;
            public bool Grounded;

            public MovementSettings()
            {
                Speed = 4f;
                RunMultiplier = 2f;
                TurnSpeed = 1f;
                RunKey = KeyCode.LeftShift;
                SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
            }
        }

        [Serializable]
        public class AdvancedSettings
        {
            public float Gravity;
            public float AngleSlope;
            public float DistanceRayInTheAir;
            public float DistanceRayOnTheGround;
            public float CurrentDistanceRay;

            public AdvancedSettings()
            {
                DistanceRayInTheAir = 0.41f;
                DistanceRayOnTheGround = 0.8f;
                CurrentDistanceRay = DistanceRayOnTheGround;
                Gravity = 4f;
            }
        }

        public MovementSettings movementSettings = new MovementSettings();
        public AdvancedSettings advancedSettings = new AdvancedSettings();
        
        [SerializeField] Vector3 WalkDirection = Vector3.zero;
        [SerializeField] Vector3 SlopeDirection = Vector3.zero;

        [Header("Compontents")]
        [SerializeField] Rigidbody Rigidbody;
        [SerializeField] CapsuleCollider CapsuleCollider;

        // Start is called before the first frame update
        private void Start()
        {
            if (Rigidbody == null) Rigidbody = GetComponent<Rigidbody>();
            if (CapsuleCollider == null) CapsuleCollider = GetComponent<CapsuleCollider>();
        }

        private void FixedUpdate()
        { 
            CheckGroundStatus();
            CheckSlopeStatus();
            UpdateWalkDirection();

            Move();

            if (movementSettings.MoveForward)
                RotateToDirectionCamera();

            if (!movementSettings.Grounded /*&& (movementSettings.MoveForward || movementSettings.MoveBack || movementSettings.MoveLeft || movementSettings.MoveRight)*/)
            {
                UpdateInTheAir();
            }
            else // ?? If you land, reset the force of gravity to count again when it loses ground ??
                advancedSettings.Gravity = 0f;
        }


        private void Move()
        {
            if (movementSettings.MoveForward)
            {
                Rigidbody.velocity = WalkDirection * SlopeMultiplier();
            }
            else 
            { 
                if (movementSettings.Grounded) 
                    Rigidbody.velocity = Vector3.zero;
            }
        }



        private void RotateToDirectionCamera()
        {
            Vector3 move = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

            if (move.magnitude > 1f) move.Normalize();

            move = transform.InverseTransformDirection(move);
            float turnAmount = Mathf.Atan2(move.x, move.z);
            float forwardAmount = move.z;
            float turnSpeed = Mathf.Lerp(180, 360, forwardAmount);
            transform.Rotate(0, turnAmount * (turnSpeed * movementSettings.TurnSpeed) * Time.deltaTime, 0);
        }

        // Update is called once per frame
        private void Update()
        {
            CheckGroundStatus();
#if UNITY_EDITOR
            DebugInfo();
#endif
        }

        //void JumpUp()
        //{
        //    Rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        //}

        private void UpdateInTheAir()
        {
            advancedSettings.Gravity += Physics.gravity.y * Time.deltaTime; //Gravity -= 9.8f * Time.deltaTime;
            Rigidbody.velocity = new Vector3(Rigidbody.velocity.x, Rigidbody.velocity.y + advancedSettings.Gravity, Rigidbody.velocity.z);
        }

        /// <summary>
        /// I check the slope of the road and measure the angle of inclination.
        /// </summary>
        private void CheckSlopeStatus()
        {
            RaycastHit hitInfo;

            Vector3 start = this.transform.position;
            start.y += +CapsuleCollider.center.y;

            Physics.Raycast(start, -Vector3.up, out hitInfo, 15);
            SlopeDirection = Vector3.Cross(this.transform.right, hitInfo.normal);

            // Road inclination angle
            advancedSettings.AngleSlope = Vector3.Angle(-Vector3.up, SlopeDirection);
#if RAY
            Debug.DrawLine(start, start + SlopeDirection * 5, Color.blue);
#endif
        }

        private float SlopeMultiplier()
            => movementSettings.SlopeCurveModifier.Evaluate(advancedSettings.AngleSlope);

        /// <summary>
        /// I check if the character touches the ground
        /// </summary>
        private void CheckGroundStatus()
        {
            Vector3 bottom = CapsuleCollider.bounds.center - (Vector3.up * CapsuleCollider.bounds.extents.y);
            Vector3 curve = bottom + (Vector3.up * CapsuleCollider.radius);
#if RAY
            Debug.DrawRay(curve, -Vector3.up * advancedSettings.CurrentDistanceRay, Color.red);
#endif
            RaycastHit hit;

            if (Physics.Raycast(curve, -Vector3.up, out hit, advancedSettings.CurrentDistanceRay))
            {
                movementSettings.Grounded = true;
                advancedSettings.CurrentDistanceRay = advancedSettings.DistanceRayOnTheGround;
                return;
            }
            advancedSettings.CurrentDistanceRay = advancedSettings.DistanceRayInTheAir;
            movementSettings.Grounded = false;
            return;
        }

        private void DebugInfo()
        {
#if RIGIDBODY
            Debug.Log("Velocity: " + Rigidbody.velocity);
#endif
#if MOVEMENT
            if (MoveLeft) Debug.Log("Move Left");
            if (MoveRight) Debug.Log("Move Right");
            if (MoveForward) Debug.Log("Move Forward");
            if (MoveBack) Debug.Log("Move Back");
#endif
        }

        private void UpdateWalkDirection()
        {
            // Set current direction
            WalkDirection = SlopeDirection;

            // Reset direction
            if (movementSettings.MoveForward && movementSettings.MoveBack || movementSettings.MoveLeft && movementSettings.MoveRight)
            {
                WalkDirection = Vector3.zero;
                return;
            }

            // Rotation with direction of camera only just when player's moving forward. 
            // Move forward have to be before other moves
            bool ControlForward = false;

            if (movementSettings.MoveForward)
            {
                WalkDirection *= movementSettings.Speed * Input.GetAxis("Vertical");

                return;
            }
        }
    }
}