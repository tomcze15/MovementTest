using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))   Application.Quit();
        if (Input.GetKey(KeyCode.Q))        Debug.Break();
    }
}
