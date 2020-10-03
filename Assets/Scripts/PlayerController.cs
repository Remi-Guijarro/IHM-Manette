using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float dashDuration = 1f;
    [SerializeField] float dashSpeed = 20f;
    [SerializeField] float dashDistance = 0.01f;
    [SerializeField] float dashIncrement = 0.5f;
    
    private float currentDashTime;

    void Update()
    {
        float xAxis = Input.GetAxis("Joystick X");
        transform.position += new Vector3(xAxis, 0f) * Time.deltaTime * speed;
        Dash(xAxis);
    }

    private void MoveTo(Vector3 location, float speed)
    {
        transform.position += location * speed * Time.deltaTime;
    }

    private void Dash(float xAxis)
    {
        if (Input.GetButton("Dash"))
        {
            Vector3 moveDirection;
            if (currentDashTime < dashDuration)
            {
                moveDirection = new Vector3(xAxis * dashDistance, 0.0f);
                currentDashTime += dashIncrement;
            }
            else
            {
                moveDirection = Vector3.zero;
            }
            MoveTo(moveDirection, dashSpeed);
        }
        else
        {
            currentDashTime = 0f;
        }
    }
}
 