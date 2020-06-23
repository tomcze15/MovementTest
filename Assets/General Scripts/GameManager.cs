using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float V;
    public float H;

    private void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))   Application.Quit();
        if (Input.GetKey(KeyCode.Q))        Debug.Break();

        V = Input.GetAxis("Vertical");
        H = Input.GetAxis("Horizontal");
    }
}
