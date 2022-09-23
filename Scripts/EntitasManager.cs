using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    public abstract class EntitasManager<T> : MonoBehaviour where T : EntitasManager<T>
    {
        protected Contexts _contexts;
        protected Services _services;
        protected Settings _settings;
        protected Feature _systems;

        protected readonly List<string> _groupNames = new List<string>();
        protected readonly List<List<ISystem>> _groupSystems = new List<List<ISystem>>();

        protected virtual void Awake()
        {
            _contexts = Contexts.sharedInstance;
            _services = Services.sharedInstance;
            _settings = Settings.sharedInstance;

            _systems = new Feature(nameof(T));

            Initialization();
        }

        protected virtual void Update()
        {
            Execute();
            Cleanup();
        }

        protected virtual void OnDestroy()
        {
            TearDown();
        }

        protected virtual void Initialization()
        {
            _systems.Initialize();
        }

        protected virtual void Execute()
        {
            _systems.Execute();
        }

        protected virtual void Cleanup()
        {
            _systems.Cleanup();
        }

        protected virtual void TearDown()
        {
            _systems.DeactivateReactiveSystems();
            _systems.ClearReactiveSystems();
            _systems.TearDown();
        }

        protected virtual void OpenSystemGroup(string groupName)
        {
            if (string.IsNullOrEmpty(groupName)) return;

            _groupNames.Add(groupName);
            _groupSystems.Add(new List<ISystem>());
        }

        protected virtual void Add(ISystem system)
        {
            if (system == null) return;

            if (_groupNames.Count == 0)
                _systems.Add(system);
            else
                _groupSystems[_groupSystems.Count - 1].Add(system);
        }

        protected virtual void CloseSystemGroup()
        {
            var index = _groupNames.Count - 1;
            if (_groupNames.Count == 1)
            {
                if (_groupSystems[index].Count != 0)
                {
                    var feature = new Feature(_groupNames[index]);
                    for (var i = 0; i < _groupSystems[index].Count; i++)
                        feature.Add(_groupSystems[index][i]);
                    _systems.Add(feature);
                }

                _groupNames.Clear();
                _groupSystems.Clear();
                return;
            }

            index = _groupNames.Count - 1;
            if (_groupSystems[index].Count != 0)
            {
                var feature = new Feature(_groupNames[index]);
                for (var i = 0; i < _groupSystems[index].Count; i++)
                    feature.Add(_groupSystems[index][i]);
                _groupSystems[index - 1].Add(feature);
            }

            _groupNames.RemoveAt(index);
            _groupSystems.RemoveAt(index);
        }
    }
}