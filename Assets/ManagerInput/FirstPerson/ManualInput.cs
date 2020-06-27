using ManagerInput;
using UnityEngine;

namespace CharacterController.FirstPerson.ManagerInput
{
    public class ManualInput : MonoBehaviour
    {
        [SerializeField] private FirstCharacterController CharacterController;

        // Start is called before the first frame update
        void Awake()
        {
            CharacterController = GetComponent<FirstCharacterController>();
        }

        // Update is called once per frame
        void Update()
        {
            if (VirtualInputManager.Instance.Forward) CharacterController.movementSettings.MoveForward = true;
            else CharacterController.movementSettings.MoveForward = false;

            if (VirtualInputManager.Instance.Back) CharacterController.movementSettings.MoveBack = true;
            else CharacterController.movementSettings.MoveBack = false;

            if (VirtualInputManager.Instance.Right) CharacterController.movementSettings.MoveRight = true;
            else CharacterController.movementSettings.MoveRight = false;

            if (VirtualInputManager.Instance.Left) CharacterController.movementSettings.MoveLeft = true;
            else CharacterController.movementSettings.MoveLeft = false;

            if (VirtualInputManager.Instance.Jump) CharacterController.movementSettings.Jump = true;
            else CharacterController.movementSettings.Jump = false;

            if (VirtualInputManager.Instance.Run) CharacterController.movementSettings.Run = true;
            else CharacterController.movementSettings.Run = false;
        }
    }
}