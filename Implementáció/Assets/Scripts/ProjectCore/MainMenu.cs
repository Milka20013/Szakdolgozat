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

        public void OnRunnerClicked()
        {
            SceneManager.LoadScene("DinoRunner");

        }
        public void OnExitPressed()
        {
            Application.Quit();
        }

        public void SetSeed(string text)
        {
            if (int.TryParse(text, out int result))
            {
                ProjectManager.SetSeed(result);
                Random.InitState(result);
            }
        }
        public void Back()
        {
            menu.SetActive(false);
        }
    }
}
