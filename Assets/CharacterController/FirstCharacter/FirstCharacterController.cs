#define UNITY_EDITOR
#if     UNITY_EDITOR
#define RAY
#endif

using System;
using UnityEditor.UIElements;
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
            public float JumpHeight;
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
        
        [SerializeField] String TurnOnSlopeByTag;
        [SerializeField] GameObject currentHitObj;
        [SerializeField] float curretnHitDistance;
        [SerializeField] float MaxGroundDetectionDistance;

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
            UpdateData();
            UpdateCheckGround();
            UpdateSlopeStatus();
            UpdateDirectionalMove();

            if (movementSettings.Jump && movementSettings.isGrounded)
                Jump();

            UpdateGravity();
            Move();
        }

        private void Move()
        {
            CharacterController.Move(MoveForce * Time.deltaTime);
            CharacterController.Move(advancedSettings.Velocity * Time.deltaTime);
        }

        private void Jump() 
            => advancedSettings.Velocity.y += Mathf.Sqrt(movementSettings.JumpHeight * -2f * advancedSettings.Gravity);

        private void UpdateDirectionalMove()
        {
            MoveForce = this.transform.right * movementSettings.Horizontal + this.transform.forward * movementSettings.Vertical * movementSettings.Speed;

            if (movementSettings.Run)
                MoveForce *= movementSettings.RunMultiplier;

            if (currentHitObj)
            { 
                if (currentHitObj.tag.Equals(TurnOnSlopeByTag)) 
                    MoveForce *= SlopeMultiplier();
            } 
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
            if(Physics.SphereCast(_curve, CharacterController.radius, -Vector3.up, out hitInfo, MaxGroundDetectionDistance, GroundMask, QueryTriggerInteraction.UseGlobal))
            {
                currentHitObj = hitInfo.transform.gameObject;
                curretnHitDistance = hitInfo.distance;
                SlopeDirection = Vector3.Cross(this.transform.right, hitInfo.normal);
                // Road inclination angle
                advancedSettings.AngleSlope = Vector3.Angle(-Vector3.up, SlopeDirection);
            }
            else 
            {
                curretnHitDistance = MaxGroundDetectionDistance;
                currentHitObj = null;
            }
#if RAY
            Vector3 origin = this.transform.position;
            origin.y += +CharacterController.center.y;
            Debug.DrawLine(origin, origin + SlopeDirection * 5, Color.blue);
            Debug.DrawLine(_curve, _curve - Vector3.up * curretnHitDistance, Color.gray);
#endif
        }

        private void UpdateData()
        {
            var bottom = CharacterController.bounds.center - (Vector3.up * CharacterController.bounds.extents.y);
            _curve = bottom + (Vector3.up * CharacterController.radius);

            movementSettings.Horizontal = Input.GetAxis("Horizontal");
            movementSettings.Vertical = Input.GetAxis("Vertical");
        }

        private float SlopeMultiplier()
            => movementSettings.SlopeCurveModifier.Evaluate(advancedSettings.AngleSlope);

        private void OnDrawGizmosSelected()
        {
#if RAY
            Gizmos.color = Color.blue;
            //Gizmos.DrawWireSphere(_curve - new Vector3(0, 0.1f, 0), CharacterController.radius);
            Gizmos.DrawWireSphere(_curve - Vector3.up * curretnHitDistance, CharacterController.radius);
#endif
        }
    }
}