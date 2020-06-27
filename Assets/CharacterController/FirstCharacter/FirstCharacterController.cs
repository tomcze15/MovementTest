#define UNITY_EDITOR
#if     UNITY_EDITOR
#define RAY
#endif

using System;
using UnityEngine;

namespace CharacterController.FirstPerson
{
    [RequireComponent(typeof(UnityEngine.CharacterController))]
    public class FirstCharacterController : MonoBehaviour
    {
        [Serializable]
        public class MovementSettings 
        {
            [Header("Movement")]
            public bool MoveForward;
            public bool MoveBack;
            public bool MoveLeft;
            public bool MoveRight;
            public bool Jump;
            public bool Run;

            [Header("Other")]
            public bool isGrounded;
            public float Horizontal;
            public float Vertical;
            public float Speed;
            public float RunMultiplier;
            public float BackMultiplier;
            public float SideMultiplier;
            public AnimationCurve SlopeCurveModifier;
        }

        [Serializable] 
        public class AdvancedSettings
        {
            public float Gravity;
            public float GravityMultiplier;
            public float AngleSlope;
            public Vector3 Velocity;

            public AdvancedSettings()
            {
                Velocity = Vector3.zero;
                GravityMultiplier = 1f;
                Gravity = -9.18f;
            }
        }

        [SerializeField] UnityEngine.CharacterController CharacterController;
        public MovementSettings movementSettings = new MovementSettings();
        [SerializeField] AdvancedSettings advancedSettings = new AdvancedSettings();
        [SerializeField] Vector3 MoveForce;
        [SerializeField] Vector3 SlopeDirection;
        [SerializeField] float groundDistance;

        [SerializeField] LayerMask GroundMask;
        private Vector3 _curve;

        // Start is called before the first frame update
        void Start()
        {
            if (!CharacterController)
                CharacterController = this.GetComponent<UnityEngine.CharacterController>();
        }

        private void FixedUpdate()
        {
            var bottom = CharacterController.bounds.center - (Vector3.up * CharacterController.bounds.extents.y);
            _curve = bottom + (Vector3.up * CharacterController.radius);

            movementSettings.Horizontal = Input.GetAxis("Horizontal");
            movementSettings.Vertical = Input.GetAxis("Vertical");

            UpdateCheckGround();
            UpdateSlopeStatus();
            UpdateDirectionalMove();
            UpdateGravity();
            Move();
        }

        private void Move()
        {
            CharacterController.Move(MoveForce * Time.deltaTime);
            CharacterController.Move(advancedSettings.Velocity * Time.deltaTime);
        }

        private void UpdateDirectionalMove()
        {
            //MoveForce = this.transform.right * movementSettings.Horizontal + SlopeDirection * movementSettings.Vertical * movementSettings.Speed;
            MoveForce = this.transform.right * movementSettings.Horizontal + this.transform.forward * movementSettings.Vertical * movementSettings.Speed;

            if (movementSettings.Run)
                MoveForce *= movementSettings.RunMultiplier;

            //MoveForce *= SlopeMultiplier();
        }

        private void UpdateGravity() 
            => advancedSettings.Velocity.y += advancedSettings.Gravity * advancedSettings.GravityMultiplier * Time.deltaTime;

        private void UpdateCheckGround()
        {
            movementSettings.isGrounded = Physics.CheckSphere(_curve - new Vector3(0, 0.1f, 0), CharacterController.radius, GroundMask);
            if (movementSettings.isGrounded && advancedSettings.Velocity.y < 0)
            {
                advancedSettings.Velocity.y = 0;
            }
        }

        private void UpdateSlopeStatus()
        {
            RaycastHit hitInfo;

            Vector3 start = this.transform.position;
            start.y += +CharacterController.center.y;

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

        private void OnDrawGizmosSelected()
        {
#if RAY
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_curve - new Vector3(0, 0.1f, 0), CharacterController.radius);
#endif
        }
    }
}