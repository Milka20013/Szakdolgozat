using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectCore
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject menu;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                menu.SetActive(!menu.activeSelf);
            }
        }

        public void OnMenuClicked()
        {
            SceneManager.LoadScene("MenuScene");
        }

        public void OnExitPressed()
        {
            Application.Quit();
        }

        public void Back()
        {
            menu.SetActive(false);
        }
    }
}
