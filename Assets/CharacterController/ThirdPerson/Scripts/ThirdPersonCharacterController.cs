#define UNITY_EDITOR

#if     UNITY_EDITOR
#define MOVEMENT
//#define RIGIDBODY
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
        [SerializeField] [Range(0.0f, 10.0f)] float Speed = 1.0f;
        [SerializeField] [Range(0.0f, 10.0f)] float TurnSpeed = 1.0f;
        //[SerializeField] [Range(0.0f, 10.0f)] float SlowDownSpeed       = 1.0f;
        //[SerializeField] [Range(0.1f, 2.0f)]  float GravityMultiplier   = 1.0f;
        //[SerializeField] [Range(0.0f, 10.0f)] float JumpForce           = 2.0f;
        [SerializeField] Vector3 WalkDirection = Vector3.zero;
        [SerializeField] Vector3 SlopeDirection = Vector3.zero;
        [SerializeField] float Gravity;
        [SerializeField] float AngleSlope;
        [SerializeField] [Range(0.0f, 3.0f)] float DistanceRay = 1.0f;

        [Header("Compontents")]
        [SerializeField] Rigidbody Rigidbody;
        [SerializeField] CapsuleCollider CapsuleCollider;


        // Start is called before the first frame update
        void Start()
        {
            if (Rigidbody == null) Rigidbody = GetComponent<Rigidbody>();
            if (CapsuleCollider == null) CapsuleCollider = GetComponent<CapsuleCollider>();
        }

        private void FixedUpdate()
        {
            WalkDirection = Vector3.zero;

            // Rotation with direction of camera only just when player's moving forward. 
            // Move forward have to be before other moves
            bool ControlForward = false;

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

            if (MoveForward)
            {
                WalkDirection += transform.forward * Speed * Input.GetAxis("Vertical");
                ControlForward = true;
            }

            if (MoveLeft)
            {
                WalkDirection += transform.right * Speed * 0.5f * Input.GetAxis("Horizontal");
                ControlForward = false;
            }

            if (MoveRight)
            {
                WalkDirection += transform.right * Speed * 0.5f * Input.GetAxis("Horizontal");
                ControlForward = false;
            }

            if (MoveBack)
            {
                WalkDirection += transform.forward * Speed * 0.5f * Input.GetAxis("Vertical");
                ControlForward = false;
            }

            if (ControlForward) RotateToDirectionCamera();

            CheckSlopeStatus();
            //CheckGroundStatus();



            // Cała do przebudowy, gdyż ten kod jest tylko do testów ruchu na rampie/pochylni.
            //
            // Main move
            if (MoveForward || MoveBack || MoveLeft || MoveRight)
            {
                if (SlopeDirection.y != 0)
                {
                    Rigidbody.velocity = SlopeDirection * Speed;
                }
                else
                {
                    Rigidbody.velocity = WalkDirection;
                }
            }
            else // Lekko się porusza postać w dół, ale to chyba wynika z samej grawitacji w grze i masie którą posiada.
                 // Ma mała masę, więc siła przyciągania jest mniejsza.
            {
                if(Grounded) Rigidbody.velocity = Vector3.zero;
            }

            //Duży problem z długością wykrywania uziemienia :/
            if (!Grounded && (MoveForward || MoveBack || MoveLeft || MoveRight))
            {
                UpdateInTheAir();
            }
            else // Jeżeli wylądujesz zresetuj siłę grawitacji, aby liczyć do nowa, gdy straci podłoże
                Gravity = 0f;
        }

        // Update is called once per frame
        void Update()
        {
            CheckGroundStatus(); // To bym dał do fixedUpdate!!!
#if UNITY_EDITOR
            DebugInfo();
#endif
        }

        public void RotateToDirectionCamera()
        {
            Vector3 move = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

            if (move.magnitude > 1f) move.Normalize();

            move = transform.InverseTransformDirection(move);
            float turnAmount = Mathf.Atan2(move.x, move.z);
            float forwardAmount = move.z;
            float turnSpeed = Mathf.Lerp(180, 360, forwardAmount);
            transform.Rotate(0, turnAmount * (turnSpeed * TurnSpeed) * Time.deltaTime, 0);
        }

        //void JumpUp()
        //{
        //    Rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        //}

        void UpdateInTheAir()
        {
            Gravity += Physics.gravity.y * Time.deltaTime; //Gravity -= 9.8f * Time.deltaTime;
            Rigidbody.velocity = new Vector3(Rigidbody.velocity.x, Rigidbody.velocity.y + Gravity, Rigidbody.velocity.z);
        }

        void CheckSlopeStatus()
        {
            RaycastHit hitInfo;
            Physics.Raycast(this.transform.position, -Vector3.up, out hitInfo, 5);
            SlopeDirection = Vector3.Cross(this.transform.right, hitInfo.normal);

            // Road inclination angle
            AngleSlope = Vector3.Angle(-Vector3.up, SlopeDirection);
#if RAY
            Debug.DrawLine(this.transform.position, this.transform.position + SlopeDirection * 5, Color.blue);
#endif
        }

        void CheckGroundStatus()
        {
            Vector3 bottom = CapsuleCollider.bounds.center - (Vector3.up * CapsuleCollider.bounds.extents.y);
            Vector3 curve = bottom + (Vector3.up * CapsuleCollider.radius);
#if RAY
            Debug.DrawRay(curve, -Vector3.up * DistanceRay, Color.red);
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
            if (MoveLeft) Debug.Log("Move Left");
            if (MoveRight) Debug.Log("Move Right");
            if (MoveForward) Debug.Log("Move Forward");
            if (MoveBack) Debug.Log("Move Back");
#endif
        }
    }
}