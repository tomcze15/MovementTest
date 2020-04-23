using UnityEngine;
using CharacterController.ThirdPerson;

namespace MovementTest.ManagerInput
{
    public class ManulInput : MonoBehaviour
    {
        [SerializeField] private ThirdPersonCharacterController CharacterController;

        // Start is called before the first frame update
        void Awake()
        {
            CharacterController = GetComponent<ThirdPersonCharacterController>();
        }

        // Update is called once per frame
        void Update()
        {
            if (VirtualInputManager.Instance.Forward) CharacterController.MoveForward = true;
            else CharacterController.MoveForward = false;

            if (VirtualInputManager.Instance.Back) CharacterController.MoveBack = true;
            else CharacterController.MoveBack = false;

            if (VirtualInputManager.Instance.Right) CharacterController.MoveRight = true;
            else CharacterController.MoveRight = false;

            if (VirtualInputManager.Instance.Left) CharacterController.MoveLeft = true;
            else CharacterController.MoveLeft = false;

            if (VirtualInputManager.Instance.Jump) CharacterController.Jump = true;
            else CharacterController.Jump = false;

            //if (VirtualInputManager.Instance.TurnLeft) CharacterController.TurnLeft = true;
            //else CharacterController.TurnLeft = false;

            //if (VirtualInputManager.Instance.TurnRight) CharacterController.TurnRight = true;
            //else CharacterController.TurnRight = false;
        }
    }
}