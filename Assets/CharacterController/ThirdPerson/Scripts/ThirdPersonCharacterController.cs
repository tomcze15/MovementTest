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

        [Header("Compontents")]
        [SerializeField] Rigidbody Rigidbody;
        [SerializeField] CapsuleCollider CapsuleCollider;

#if RAY
        [SerializeField] [Range(0.0f, 10.0f)] float DistanceRay = 5.0f;
#endif

        // Start is called before the first frame update
        void Start()
        {
            if (Rigidbody       == null) Rigidbody          = GetComponent<Rigidbody>();
            if (CapsuleCollider == null) CapsuleCollider    = GetComponent<CapsuleCollider>();
        }

        private void FixedUpdate()
        {
            if (MoveLeft)
            {
                Rigidbody.velocity = transform.right * thrust * 0.5f * Input.GetAxis("Horizontal");
            }

            if (MoveRight)
            {
                Rigidbody.velocity = transform.right * thrust * 0.5f * Input.GetAxis("Horizontal");
            }

            if (MoveForward)
            {
                Rigidbody.velocity = transform.forward * thrust * Input.GetAxis("Vertical");
                RotateToDirectionCamera();
            }

            if (MoveBack)
            {
                Rigidbody.velocity = transform.forward * thrust * 0.5f * Input.GetAxis("Vertical");
            }

            //if (Rigidbody.velocity.magnitude < .01)
            //{
            //    Rigidbody.velocity = Vector3.zero;
            //    Rigidbody.angularVelocity = Vector3.zero;
            //}

            if (!Grounded)
                UpdateInTheAir();

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


        void UpdateInTheAir()
        {
            Vector3 extraGravityForce = (Physics.gravity * GravityMultiplier) - Physics.gravity;
            Rigidbody.AddForce(extraGravityForce);
        }

        void CheckGroundStatus()
        {
            //Środek collidera
            Vector3 bottom = CapsuleCollider.bounds.center - (Vector3.up * CapsuleCollider.bounds.extents.y);
            //Promień kuli
            Vector3 curve = bottom + (Vector3.up * CapsuleCollider.radius);
#if RAY
            Debug.DrawRay(transform.position, -Vector3.up * DistanceRay, Color.blue);
#endif
            RaycastHit hit;

            if (Physics.Raycast(transform.position, -Vector3.up, out hit, DistanceRay))
            {
                if (hit.transform.tag == "Player")
                    Grounded = false;
                Grounded = true;
            }
        }

        private void DebugInfo()
        {
#if RIGIDBODY
            Debug.Log("TH1 Velocity: " + Rigidbody.velocity);
#endif
#if MOVEMENT
            if (MoveLeft) Debug.Log("TH1 Move Left");
            if (MoveRight) Debug.Log("TH1 Move Right");
            if (MoveForward) Debug.Log("TH1 Move Forward");
            if (MoveBack) Debug.Log("TH1 Move Back");
#endif
        }
    }
}