using UnityEngine;
using System.Collections;

namespace util
{
    public class SceneUtils : MonoBehaviour
    {

        public void OpenScene(string name)
        {
            Application.LoadLevel(name);
        }

        public void OpenScene(int level)
        {
            Application.LoadLevel(level);
        }

        public void CloseGame()
        {
            Application.Quit();
        }

        public void ReloadScene()
        {
            Application.LoadLevel(Application.loadedLevelName);
        }

        public void ReloadLastScene()
        {
            Application.LoadLevel(GameManager.instance.LastScene);
        }
    }
}