using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace WFC
{
    /// <summary>
    /// UI component in WFC generation
    /// Makes it possible to select an image as the reference
    /// </summary>
    public class ImageContext : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private string nextScene;
        private ImageButton imageButton;
        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void Show(ImageButton imageButton)
        {
            gameObject.SetActive(true);
            image.sprite = imageButton.GetComponent<Image>().sprite;
            this.imageButton = imageButton;
        }

        public void LoadNextScene()
        {
            DataContainer.ChosenImageSprite = imageButton.GetComponent<Image>().sprite;
            SceneManager.LoadScene(nextScene);
        }
    }
}
