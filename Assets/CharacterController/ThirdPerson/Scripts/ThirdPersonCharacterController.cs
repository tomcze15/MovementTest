#define UNITY_EDITOR

#if     UNITY_EDITOR
#define MOVEMENT
#define RIGIDBODY
#define RAY
#endif

using UnityEngine;

namespace CharacterController.ThirdPerson
{
    public class ThirdPersonCharacterController : MonoBehaviour
    {
        [Header("Movement")]
        public bool MoveForward;
        public bool MoveBack;
        public bool MoveLeft;
        public bool MoveRight;
        public bool Jump;
        [SerializeField] bool Grounded;

        [Header("MovementValue")]
        [SerializeField] [Range(0.0f, 10.0f)] float thrust              = 1.0f;
        [SerializeField] [Range(0.0f, 10.0f)] float TurnSpeed           = 1.0f;
        [SerializeField] [Range(0.0f, 10.0f)] float SlowDownSpeed       = 1.0f;
        [SerializeField] [Range(0.0f, 10.0f)] float GravityMultiplier   = 2.0f;
        [SerializeField] [Range(0.0f, 10.0f)] float JumpForce           = 2.0f;
        [SerializeField] Vector3 WalkDirection = Vector3.zero;

        [Header("Compontents")]
        [SerializeField] Rigidbody Rigidbody;
        [SerializeField] CapsuleCollider CapsuleCollider;
        [SerializeField] [Range(0.0f, 3.0f)] float DistanceRay = 1.0f;

        // Start is called before the first frame update
        void Start()
        {
            if (Rigidbody       == null) Rigidbody          = GetComponent<Rigidbody>();
            if (CapsuleCollider == null) CapsuleCollider    = GetComponent<CapsuleCollider>();
        }

        private void FixedUpdate()
        {
            WalkDirection = Vector3.zero;
            bool ControlForward = true;

            if (MoveForward && MoveBack)
            {
                Rigidbody.velocity = Vector3.zero;
                return;
            }

            if (MoveLeft && MoveRight)
            {
                Rigidbody.velocity = Vector3.zero;
                return;
            }

            if (MoveLeft)
            {
                WalkDirection += transform.right * thrust * 0.5f * Input.GetAxis("Horizontal");
                ControlForward = false;
            }

            if (MoveRight)
            {
                WalkDirection += transform.right * thrust * 0.5f * Input.GetAxis("Horizontal");
                ControlForward = false;
            }

            if (MoveForward)
            {
                WalkDirection += transform.forward * thrust * Input.GetAxis("Vertical");
            }

            if (MoveBack)
            {
                WalkDirection += transform.forward * thrust * 0.5f * Input.GetAxis("Vertical");
                ControlForward = false;
            }

            if(ControlForward)
                RotateToDirectionCamera();

            CheckSlopeStatus();
            CheckGroundStatus();

            if (Grounded && Jump)
                JumpUp();

            //if (!Grounded)
                //UpdateInTheAir();

            if (Input.anyKey)
            {
                Rigidbody.velocity = WalkDirection;
            }

        }

        // Update is called once per frame
        void Update()
        {
            CheckGroundStatus();
#if UNITY_EDITOR
            DebugInfo();
#endif
        }

        public void RotateToDirectionCamera()
        {
            Vector3 m_Move = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

            if (m_Move.magnitude > 1f) m_Move.Normalize();

            m_Move = transform.InverseTransformDirection(m_Move);
            float m_TurnAmount = Mathf.Atan2(m_Move.x, m_Move.z);
            float m_ForwardAmount = m_Move.z;
            float turnSpeed = Mathf.Lerp(180, 360, m_ForwardAmount);
            transform.Rotate(0, m_TurnAmount * (turnSpeed * TurnSpeed) * Time.deltaTime, 0);
        }

        void JumpUp()
        {
            Rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        }

        void UpdateInTheAir()
        {
            Vector3 extraGravityForce = (Physics.gravity * GravityMultiplier) - Physics.gravity;
            Rigidbody.AddForce(extraGravityForce);
        }

        void CheckSlopeStatus()
        {
            RaycastHit hitInfo;
            Vector3 forward;
            Physics.Raycast(this.transform.position, -Vector3.up, out hitInfo, 5);
            //WalkDirection += Vector3.Cross(this.transform.right, hitInfo.normal);
            forward = Vector3.Cross(this.transform.right, hitInfo.normal);
            Debug.DrawLine(this.transform.position, this.transform.position + forward * 5, Color.red);
        }

        void CheckGroundStatus()
        {
            Vector3 bottom = CapsuleCollider.bounds.center - (Vector3.up * CapsuleCollider.bounds.extents.y);
            Vector3 curve = bottom + (Vector3.up * CapsuleCollider.radius);
#if RAY
            Debug.DrawRay(curve, -Vector3.up * DistanceRay, Color.blue);
#endif
            RaycastHit hit;

            if (Physics.Raycast(curve, -Vector3.up, out hit, DistanceRay))
            {
                Grounded = true;
                return;
            }
            Grounded = false;
            return;
        }

        private void DebugInfo()
        {
#if RIGIDBODY
            Debug.Log("Velocity: " + Rigidbody.velocity);
#endif
#if MOVEMENT
            if (MoveLeft)       Debug.Log("Move Left"   );
            if (MoveRight)      Debug.Log("Move Right"  );
            if (MoveForward)    Debug.Log("Move Forward");
            if (MoveBack)       Debug.Log("Move Back"   );
#endif
        }
    }
}