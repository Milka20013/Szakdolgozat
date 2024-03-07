using UnityEngine;
using UnityEngine.SceneManagement;
namespace CoreGame
{
    public class Menu : MonoBehaviour
    {
        [SerializeField] private GameObject[] objectsOnBaseMenu;

        [SerializeField] private GameObject[] objectsOn2DOption;

        [SerializeField] private string WFCSelectorScene;

        public void On2DPressed()
        {
            foreach (var obj in objectsOnBaseMenu)
            {
                obj.SetActive(false);
            }

            foreach (var obj in objectsOn2DOption)
            {
                obj.SetActive(true);
            }
        }

        public void OnBackPressed()
        {
            foreach (var obj in objectsOnBaseMenu)
            {
                obj.SetActive(true);
            }

            foreach (var obj in objectsOn2DOption)
            {
                obj.SetActive(false);
            }
        }

        public void OnExitPressed()
        {
            Application.Quit();
        }

        public void OnWFCPressed()
        {
            SceneManager.LoadScene(WFCSelectorScene);
        }

        public void OnSimplePressed()
        {

        }
    }
}
