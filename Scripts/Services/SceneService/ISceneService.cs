using UnityEngine;

namespace GameEngine
{
    public interface ISceneService
    {
        void LoadScene(int sceneBuildIndex);
        void LoadScene(string sceneName);
        AsyncOperation LoadSceneAsync(int sceneBuildIndex);
        AsyncOperation LoadSceneAsync(string sceneName);
    }
}