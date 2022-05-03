using UnityEngine;
using UnityEngine.SceneManagement;

namespace PuzzleCat
{
    public class MenuManager : MonoBehaviour
    {
        public static void LoadLevel(int level)
        {
            SceneManager.LoadScene(level);
        }
    }
}
