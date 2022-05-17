using System.Collections.Generic;
using Entitas;

namespace GameEngine
{
    public abstract class EntitasManager<T> : CustomMonoBehaviour where T: EntitasManager<T>
    {
        private Contexts _contexts;
        private Services _services;
        private Data _data;
        private Feature _systems;

        protected Contexts Contexts => _contexts;
        protected Services Services => _services;
        protected Data Data => _data;

        // -------------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            base.Awake();

#if !GE_DEBUG_DISABLED
            if (FindObjectsOfType<T>().Length > 1) throw new CustomException();
#endif

            _contexts = Contexts.sharedInstance;
            _services = Services.sharedInstance;
            _data = Data.sharedInstance;

#if !GE_DEBUG_DISABLED
            if (Contexts == null) throw new CustomException();
            if (Services == null) throw new CustomException();
            if (Data == null) throw new CustomException();
#endif

            _systems = new Feature("GameSystems");

            Initialize();
        }

        // -------------------------------------------------------------------------------------------------------------

        private void Update()
        {
            Execute();
            Cleanup();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            TearDown();
        }

        protected virtual void Initialize()
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

        // -------------------------------------------------------------------------------------------------------------

        private readonly List<string> _addGroupNameList = new List<string>();
        private readonly List<List<ISystem>> _addGroupSystemList = new List<List<ISystem>>();

        protected void Add(ISystem system)
        {
#if !GE_DEBUG_DISABLED
            if (system == null) throw new CustomArgumentException();
#endif
            if (_addGroupNameList.Count != 0)
            {
                _addGroupSystemList[_addGroupSystemList.Count - 1].Add(system);
                return;
            }

            _systems.Add(system);
        }

        protected void Add(string groupName, params ISystem[] systems)
        {
#if !GE_DEBUG_DISABLED
            if (string.IsNullOrEmpty(groupName) || systems == null || systems.Length == 0)
                throw new CustomArgumentException();
#endif
            var feature = new Feature(groupName);
            {
                for (var i = 0; i < systems.Length; i++)
                    feature.Add(systems[i]);
            }
            Add(feature);
        }

        protected void StartSystemGroup(string groupName)
        {
#if !GE_DEBUG_DISABLED
            if (string.IsNullOrEmpty(groupName)) throw new CustomArgumentException();
#endif
            _addGroupNameList.Add(groupName);
            _addGroupSystemList.Add(new List<ISystem>());
        }

        protected void EndSystemGroup()
        {
#if !GE_DEBUG_DISABLED
            if (_addGroupNameList.Count == 0) throw new CustomArgumentException();
#endif

            var index = _addGroupNameList.Count - 1;
            if (_addGroupNameList.Count == 1)
            {
                if (_addGroupSystemList[index].Count != 0)
                {
                    var feature = new Feature(_addGroupNameList[index]);
                    for (var i = 0; i < _addGroupSystemList[index].Count; i++)
                        feature.Add(_addGroupSystemList[index][i]);
                    _systems.Add(feature);
                }

                _addGroupNameList.Clear();
                _addGroupSystemList.Clear();
                return;
            }

            index = _addGroupNameList.Count - 1;
            if (_addGroupSystemList[index].Count != 0)
            {
                var feature = new Feature(_addGroupNameList[index]);
                for (var i = 0; i < _addGroupSystemList[index].Count; i++)
                    feature.Add(_addGroupSystemList[index][i]);
                _addGroupSystemList[index - 1].Add(feature);
            }

            _addGroupNameList.RemoveAt(index);
            _addGroupSystemList.RemoveAt(index);
        }
    }
}