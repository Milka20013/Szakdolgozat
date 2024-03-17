using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float speed;
    private void Update()
    {
        transform.position -= new Vector3(speed * Time.deltaTime, 0);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Player player))
        {
            Destroy(collision.gameObject);
        }
    }
}
