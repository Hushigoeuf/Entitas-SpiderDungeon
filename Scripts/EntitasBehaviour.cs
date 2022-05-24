using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// Базовый класс типа MonoBehaviour для работы с ECS (Entitas).
    /// </summary>
    public abstract class EntitasBehaviour : MonoBehaviour
    {
        protected Contexts _contexts;
        protected Services _services;
        protected Settings _settings;

        protected virtual void Awake()
        {
            _contexts = Contexts.sharedInstance;
            _services = Services.sharedInstance;
            _settings = Settings.sharedInstance;
        }
    }
}