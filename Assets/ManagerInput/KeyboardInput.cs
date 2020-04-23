//#define Debug
using UnityEngine;

namespace MovementTest.ManagerInput
{
    public class KeyboardInput : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                VirtualInputManager.Instance.Forward = true;
                VirtualInputManager.Instance.Any = true;
#if Debug
                Debug.Log("Forward.");
#endif
            }
            else
            {
                VirtualInputManager.Instance.Forward = false;
            }

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                VirtualInputManager.Instance.Back = true;
                VirtualInputManager.Instance.Any = true;
#if Debug
                Debug.Log("Back.");
#endif
            }
            else
            {
                VirtualInputManager.Instance.Back = false;
            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                VirtualInputManager.Instance.Left = true;
                VirtualInputManager.Instance.Any = true;
#if Debug
                Debug.Log("Left.");
#endif
            }
            else
            {
                VirtualInputManager.Instance.Left = false;
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                VirtualInputManager.Instance.Right = true;
                VirtualInputManager.Instance.Any = true;
#if Debug
                Debug.Log("Right.");
#endif
            }
            else
            {
                VirtualInputManager.Instance.Right = false;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                VirtualInputManager.Instance.Jump = true;
                VirtualInputManager.Instance.Any = true;
#if Debug
                Debug.Log("Space.");
#endif
            }
            else
            {
                VirtualInputManager.Instance.Jump = false;
            }

            if (Input.GetKey(KeyCode.Q))
            {
                VirtualInputManager.Instance.TurnLeft = true;
                VirtualInputManager.Instance.Any = true;
#if Debug
                Debug.Log("Q.");
#endif
            }
            else
            {
                VirtualInputManager.Instance.TurnLeft = false;
            }

            if (Input.GetKey(KeyCode.E))
            {
                VirtualInputManager.Instance.TurnRight = true;
                VirtualInputManager.Instance.Any = true;
#if Debug
                Debug.Log("E.");
#endif
            }
            else
            {
                VirtualInputManager.Instance.TurnRight = false;
            }

            //if (Input.anyKey) ;
        }
    }
}
