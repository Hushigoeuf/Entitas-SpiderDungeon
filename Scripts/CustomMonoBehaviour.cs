using UnityEngine;

namespace GameEngine
{
    public abstract class CustomMonoBehaviour : MonoBehaviour
    {
        private GameObject _gameObject;
        private Transform _transform;

        public GameObject GameObject => _gameObject;
        public Transform Transform => _transform;

        // -------------------------------------------------------------------------------------------------------------

        protected virtual void Awake()
        {
            _gameObject = gameObject;
            _transform = transform;
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }

        protected virtual void OnDestroy()
        {
        }
    }
}