using UnityEngine;

namespace GC
{
    public class CameraMovement : MonoBehaviour
    {
        private Camera cam;
        public float speed;

        private void Awake()
        {
            cam = GetComponent<Camera>();
        }

        private void Update()
        {
            cam.orthographicSize -= Input.mouseScrollDelta.y;
            transform.position += transform.up * Input.GetAxis("Vertical") * speed * Time.deltaTime;
            transform.position += transform.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        }
    }
}
