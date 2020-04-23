#define Debug
using UnityEngine;

namespace CharacterController.ThirdPerson
{
    public class ThirdPersonController_4 : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] bool MoveForward;
        [SerializeField] bool Jump;
        [SerializeField] bool Grounded;
        [SerializeField] [Range(0.0f, 10.0f)] float thrust = 1.0f;
        [SerializeField] [Range(0.0f, 10.0f)] float smoothingFactor = 1.0f;
        [SerializeField] [Range(0.0f, 10.0f)] float TurnSpeed = 1.0f;


        [Header("Compontents")]
        [SerializeField] Rigidbody Rigidbody;

        // Start is called before the first frame update
        void Start()
        {
            if (Rigidbody == null)
                Rigidbody = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
#if Debug
            Debug.Log("TH4 Velocity" + Rigidbody.velocity);
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
    }
}