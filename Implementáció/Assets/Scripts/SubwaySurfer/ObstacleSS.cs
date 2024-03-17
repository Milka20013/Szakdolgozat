using UnityEngine;

namespace SubwaySurfer
{
    public class ObstacleSS : MonoBehaviour
    {
        [SerializeField] private float speed = 1f;
        private bool dead;
        private void Update()
        {
            if (dead)
            {
                return;
            }
            transform.Translate(new Vector3(0, 0, -1 * speed * Time.deltaTime));
            if (transform.position.z <= -4.6f)
            {
                Destroy(gameObject);
                dead = true;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out PlayerSS _))
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
