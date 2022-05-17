using System.Collections.Generic;
using Entitas;

namespace GameEngine
{
    public sealed class ContentObjectConfigSystem : IInitializeSystem, IExecuteSystem/*, ITearDownSystem*/
    {
        private readonly Contexts _contexts;
        private readonly ContentSettingsObject _contentSettings;
        private readonly IGroup<ConfigEntity> _contentObjectGroup;
        private readonly List<ConfigEntity> _contentObjectBuffer;

        private readonly bool _contentAlwaysIncluded;
        private bool _loadSkipped;

        public ContentObjectConfigSystem(Contexts contexts, Services services, Data data)
        {
            _contexts = contexts;
            _contentSettings = data.ContentData;
            _contentObjectGroup = contexts.config.GetGroup(ConfigMatcher.ContentObject);
            _contentObjectBuffer = new List<ConfigEntity>();

            _contentAlwaysIncluded = GameSettings.CONTENT_ALWAYS_INCLUDED;
        }

        public void Initialize()
        {
            foreach (var bonus in _contentSettings.BonusList)
            {
                if (!_contentAlwaysIncluded && !bonus.IsInclude()) continue;
                var e = CreateContentObjectEntity(bonus);
                e.AddBonusBehaviourType(bonus.BehaviourType);
            }

            foreach (var item in _contentSettings.ItemList)
            {
                if (!_contentAlwaysIncluded && !item.IsInclude()) continue;
                var e = CreateContentObjectEntity(item);
                e.AddItemBehaviourType(item.BehaviourType);
                e.AddLevel(item.Level, 0);
            }
        }

        private ConfigEntity CreateContentObjectEntity(StandContentObject contentObject)
        {
            var e = _contexts.config.CreateEntity();
            {
                e.isContentObject = true;
                e.isControlled = true;
                e.AddCountParameter(contentObject.LifetimeParameter);
            }
            return e;
        }

        /*private void _OnChangeLifetime()
        {
            if (!_loadSkipped)
            {
                _contentObjectGroup.GetEntities(_contentObjectBuffer);
                _loadSkipped = _contentObjectBuffer.Count == 0;
            }

            if (_loadSkipped) return;

            for (var i = 0; i < _contentObjectBuffer.Count; i++)
            {
                if (!_contentObjectBuffer[i].isControlled) continue;
                if (_contentObjectBuffer[i].countParameter.Value.Value > 0 &&
                    _contentObjectBuffer[i].countParameter.Value.Value >=
                    _contentSettings.MinLifetimeToEnable) continue;
                if (!_contentAlwaysIncluded)
                {
                    _contentObjectBuffer[i].isCleanup = true;
                    _contentObjectBuffer[i].countParameter.Value.OnChangeValue.RemoveListener(_OnChangeLifetime);
                }
            }
        }*/


        public void Execute()
        {
            if (!_loadSkipped)
            {
                _contentObjectGroup.GetEntities(_contentObjectBuffer);
                _loadSkipped = _contentObjectBuffer.Count == 0;
            }

            if (_loadSkipped) return;

            for (var i = 0; i < _contentObjectBuffer.Count; i++)
            {
                if (!_contentObjectBuffer[i].isControlled) continue;
                if (_contentObjectBuffer[i].countParameter.Value.Value > 0 &&
                    _contentObjectBuffer[i].countParameter.Value.Value > 0) continue;
                if (!_contentAlwaysIncluded)
                    _contentObjectBuffer[i].isCleanup = true;
            }
        }

        /*public void TearDown()
        {
            if (!_loadSkipped)
            {
                _contentObjectGroup.GetEntities(_contentObjectBuffer);
                _loadSkipped = _contentObjectBuffer.Count == 0;
            }

            if (_loadSkipped) return;

            for (var i = 0; i < _contentObjectBuffer.Count; i++)
                _contentObjectBuffer[i].countParameter.Value.OnChangeValue.RemoveListener(_OnChangeLifetime);
        }*/
    }
}