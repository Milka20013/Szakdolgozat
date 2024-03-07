using UnityEngine;
using UnityEngine.EventSystems;

namespace WFC
{
    public class ImageButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        private Canvas canvas;
        [HideInInspector] public ImageContext imageContext;
        private void Awake()
        {
            canvas = GetComponent<Canvas>();
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.localScale = new Vector3(2, 2, 2);
            canvas.sortingOrder = 2;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.localScale = Vector3.one;
            canvas.sortingOrder = 1;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            imageContext.Show(this);
        }
    }
}
