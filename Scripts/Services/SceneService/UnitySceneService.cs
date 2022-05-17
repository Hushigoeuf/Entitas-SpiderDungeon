using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameEngine
{
    public sealed class UnitySceneService : ISceneService
    {
        public void LoadScene(int sceneBuildIndex)
        {
            SceneManager.LoadScene(sceneBuildIndex);
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public AsyncOperation LoadSceneAsync(int sceneBuildIndex)
        {
            return SceneManager.LoadSceneAsync(sceneBuildIndex);
        }

        public AsyncOperation LoadSceneAsync(string sceneName)
        {
            return SceneManager.LoadSceneAsync(sceneName);
        }
    }
}