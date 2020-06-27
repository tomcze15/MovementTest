#define UNITY_EDITOR

#if     UNITY_EDITOR
//#define MOVEMENT
//#define RIGIDBODY
#define RAY
#endif

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterController.ThirdPerson
{
    public class ThirdPersonCharacterController : MonoBehaviour
    {
        [Serializable]
        public class MovementSettings
        {
            //public AnimationCurve   RunTransitionCurve;
            //public AnimationCurve   JumpCurve;
            public AnimationCurve   SlopeCurveModifier;         
            public float Speed;
            public float RunMultiplier;
            public float BackMultiplier;
            public float SideMultiplier;

            public float JumpForce;
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
                Speed = 9f;
                JumpForce = 5f;
                RunMultiplier = 1.3f;
                BackMultiplier = 0.6f;
                SideMultiplier = 0.8f;

                TurnSpeed = 1f;
                SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
            }
        }

        [Serializable]
        public class AdvancedSettings
        {
            public float Gravity;
            public float GravityMultiplier;
            public float AngleSlope;
            public float DistanceRayInTheAir;
            public float DistanceRayOnTheGround;
            public float CurrentDistanceRay;


            public AdvancedSettings()
            {
                DistanceRayInTheAir = 0.45f;
                DistanceRayOnTheGround = 0.8f;
                CurrentDistanceRay = DistanceRayOnTheGround;
                Gravity = -9.81f;
                GravityMultiplier = 1f;
            }
        }

        public MovementSettings movementSettings = new MovementSettings();
        public AdvancedSettings advancedSettings = new AdvancedSettings();
        public GameObject ColliderEdgePrefab;

        [SerializeField] GameObject GroundCheck;
        [SerializeField] LayerMask groundMask;

        private Vector3 _bottom;
        private Vector3 _curve;
        private float   _currentGravityForce;

        [SerializeField] Vector3 WalkDirection;
        [SerializeField] Vector3 SlopeDirection;
        [SerializeField] List<GameObject> GroundEdgeList;

        [Header("Compontents")]
        [SerializeField] Rigidbody Rigidbody;
        [SerializeField] CapsuleCollider CapsuleCollider;

        // Start is called before the first frame update
        private void Awake()
        {
            WalkDirection = Vector3.zero;
            SlopeDirection = Vector3.zero;
            if (Rigidbody       == null)    Rigidbody       = GetComponent< Rigidbody        >();
            if (CapsuleCollider == null)    CapsuleCollider = GetComponent< CapsuleCollider  >();
        }

        private void Start()
        {
            _bottom = CapsuleCollider.bounds.center - (Vector3.up * CapsuleCollider.bounds.extents.y);
            _curve = _bottom + (Vector3.up * CapsuleCollider.radius);
            CreateEdgeObjects();
        }

        private void FixedUpdate()
        {
            CheckGroundStatus();

            if (movementSettings.Grounded && Rigidbody.velocity.y < 0)
                Rigidbody.velocity = new Vector3(Rigidbody.velocity.x, 0, Rigidbody.velocity.z);

            if (movementSettings.MoveForward)
                if(movementSettings.MoveLeft == false && movementSettings.MoveRight == false && movementSettings.MoveBack == false)
                    RotateToDirectionCamera();

            UpdateWalkDirection();
            ApplyGravity();

            Move();
        }

        private void Move()
        {
            if (movementSettings.MoveForward || movementSettings.MoveBack || movementSettings.MoveLeft || movementSettings.MoveRight)
            {
                if(movementSettings.Grounded)
                    Rigidbody.velocity = WalkDirection;
                else
                    Rigidbody.velocity = new Vector3(WalkDirection.x, _currentGravityForce * advancedSettings.GravityMultiplier, WalkDirection.z);
            }
            else
            {
                Rigidbody.velocity = new Vector3(WalkDirection.x, _currentGravityForce * advancedSettings.GravityMultiplier, WalkDirection.z);
            }
        }

        private void ApplyGravity()
        {
            if (!movementSettings.Grounded)
                _currentGravityForce += Physics.gravity.y * Time.deltaTime;
            else
                _currentGravityForce = 0;
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
#if UNITY_EDITOR
            DebugInfo();
#endif
        }

        private void JumpUp()
        {
            Rigidbody.AddForce(new Vector3(0, 5, 0) * movementSettings.JumpForce, ForceMode.Impulse);
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

        private void CheckGroundStatus2() => 
            movementSettings.Grounded = Physics.CheckSphere(GroundCheck.transform.position, 0.45f, groundMask);

        /*private void OnDrawGizmosSelected()
        {
#if RAY
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_curve, CapsuleCollider.radius);
#endif
        }*/

        private void CheckGroundStatus()
        {
            Vector3 bottom = CapsuleCollider.bounds.center - (Vector3.up * CapsuleCollider.bounds.extents.y);
            Vector3 curve = bottom + (Vector3.up * CapsuleCollider.radius);
#if RAY
            foreach (GameObject obj in GroundEdgeList)
                Debug.DrawRay(obj.transform.position, -Vector3.up * advancedSettings.CurrentDistanceRay, Color.red);
#endif
            RaycastHit hit;

            foreach (GameObject obj in GroundEdgeList)
            {
                if (Physics.Raycast(curve, -Vector3.up, out hit, advancedSettings.CurrentDistanceRay, groundMask))
                {
                    movementSettings.Grounded = true;
                    advancedSettings.CurrentDistanceRay = advancedSettings.DistanceRayOnTheGround;
                    return;
                }
            }

            advancedSettings.CurrentDistanceRay = advancedSettings.DistanceRayInTheAir;
            movementSettings.Grounded = false;
            return;
        }

        private void UpdateWalkDirection()
        {
            CheckSlopeStatus();

            float currentSpeed = movementSettings.Speed;

            WalkDirection = (this.transform.right * Input.GetAxis("Horizontal") + SlopeDirection * Input.GetAxis("Vertical"));

            // Reset direction
            if (movementSettings.MoveForward && movementSettings.MoveBack || movementSettings.MoveLeft && movementSettings.MoveRight)
            {
                WalkDirection = Vector3.zero;
                return;
            }

            if (movementSettings.MoveForward)
            {
                if (movementSettings.Run && movementSettings.Grounded)
                    currentSpeed *= movementSettings.RunMultiplier;
            }

            if (movementSettings.MoveBack)
            {
                if (movementSettings.Run && movementSettings.Grounded)
                    currentSpeed *= movementSettings.BackMultiplier * movementSettings.RunMultiplier;
                else                        
                    currentSpeed *= movementSettings.BackMultiplier;
            }

            if (movementSettings.MoveLeft || movementSettings.MoveRight)
            {
                if (movementSettings.Run && movementSettings.Grounded)
                    currentSpeed *= movementSettings.SideMultiplier * movementSettings.RunMultiplier;
                else                        
                    currentSpeed *= movementSettings.SideMultiplier;
            }

            WalkDirection *= currentSpeed * SlopeMultiplier();
        }

        private float SlopeMultiplier()
            => movementSettings.SlopeCurveModifier.Evaluate(advancedSettings.AngleSlope);

        private double ConvertDegreesToRadians(double degrees) 
            => ((Math.PI / 180) * degrees);

        private void CreateEdgeObjects()
        {
            GameObject MiddleEdge = CreateEdgeSphere(_curve);
            MiddleEdge.transform.parent = this.transform;
            GroundEdgeList.Add(MiddleEdge);

            float radiusOffset = -0.01f;
            int part = 16, sector;

            sector = 360 / part;

            for (int i = 0; i < 360; i += sector)
            {
                double x = (_curve.x + (CapsuleCollider.radius + radiusOffset) * Math.Cos(ConvertDegreesToRadians((double)i)));
                double z = (_curve.z + (CapsuleCollider.radius + radiusOffset) * Math.Sin(ConvertDegreesToRadians((double)i)));
                var obj = CreateEdgeSphere(new Vector3((float)x, _curve.y, (float)z));
                obj.transform.parent = this.transform;
                GroundEdgeList.Add(obj);
            }
        }

        private GameObject CreateEdgeSphere(Vector3 pos)
            => Instantiate(ColliderEdgePrefab, pos, Quaternion.identity);

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
    }
}