using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float jumpForce;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsGrounded())
            {
                Jump();
            }
        }
    }

    private void Jump()
    {
        rb.AddForce(new(0, jumpForce * 10f));
    }

    private bool IsGrounded()
    {
        if (rb.velocity.y == 0f)
        {
            return true;
        }
        return false;
    }
}
