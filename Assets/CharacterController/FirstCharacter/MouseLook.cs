using UnityEngine;

namespace CharacterController.FirstPerson
{
    public class MouseLook : MonoBehaviour
    {
        [SerializeField] float mouseSensitivity;
        [SerializeField] Transform PlayerBody;

        private float _mouseX;
        private float _mouseY;
        private float _rotationX;

        // Start is called before the first frame update
        void Start()
        {
            if (!PlayerBody)
                PlayerBody = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

            _rotationX = 0;

            Cursor.lockState = CursorLockMode.Locked;
        }

        // Update is called once per frame
        void Update()
        {
            _mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            _mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            _rotationX -= _mouseY;
            _rotationX = Mathf.Clamp(_rotationX, -90f, 90f);


            this.transform.localRotation = Quaternion.Euler(_rotationX, 0f, 0f);
            PlayerBody.Rotate(Vector3.up * _mouseX);
        }
    }
}
