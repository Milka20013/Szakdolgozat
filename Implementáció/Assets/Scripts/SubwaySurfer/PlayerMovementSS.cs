using UnityEngine;

namespace SubwaySurfer
{
    public class PlayerMovementSS : MonoBehaviour
    {
        [SerializeField] private float horizontalDistance;
        [SerializeField] private float verticalDistance;



        private void Update()
        {
            if (HorizontalKey(out float dir))
            {
                MoveHorizontally(dir);
            }
            if (VerticalKey(out float dir2))
            {
                MoveVertically(dir2);
            }

        }
        private bool HorizontalKey(out float dir)
        {
            dir = 0f;
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                dir = -1;
                return true;
            }
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                dir = 1;
                return true;
            }
            return false;
        }
        private bool VerticalKey(out float dir)
        {
            dir = 0f;
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                dir = 1;
                return true;
            }
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                dir = -1;
                return true;
            }
            return false;
        }
        private void MoveVertically(float direction)
        {
            if (direction == 0f)
            {
                return;
            }
            if (direction > 0f)
            {
                if (transform.position.y >= 2 * verticalDistance)
                {
                    return;
                }
            }
            if (direction < 0f)
            {
                if (transform.position.y <= 0)
                {
                    return;
                }
            }
            transform.position += new Vector3(0, verticalDistance * Mathf.Sign(direction), 0);
        }
        private void MoveHorizontally(float direction)
        {
            if (direction == 0f)
            {
                return;
            }
            if (direction > 0f)
            {
                if (transform.position.x >= horizontalDistance)
                {
                    return;
                }
            }
            if (direction < 0f)
            {
                if (transform.position.x <= -horizontalDistance)
                {
                    return;
                }
            }
            transform.position += new Vector3(horizontalDistance * Mathf.Sign(direction), 0, 0);
        }
    }
}
