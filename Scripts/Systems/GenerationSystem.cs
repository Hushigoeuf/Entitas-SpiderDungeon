using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    public sealed class GenerationSystem : IExecuteSystem
    {
        private readonly IGroup<EnvironmentEntity> _group;
        private readonly List<EnvironmentEntity> _buffer;

        public GenerationSystem(Contexts contexts)
        {
            _group = contexts.environment.GetGroup(EnvironmentMatcher.AllOf(
                EnvironmentMatcher.Generation,
                EnvironmentMatcher.Transform,
                EnvironmentMatcher.Offset,
                EnvironmentMatcher.Position,
                EnvironmentMatcher.Length,
                EnvironmentMatcher.Pool,
                EnvironmentMatcher.Index));
            _buffer = new List<EnvironmentEntity>();
        }

        public void Execute()
        {
            foreach (var e in _group.GetEntities(_buffer))
            {
                var lastPosition = e.position.Value.y + e.length.Value * e.index.Value;
                var currentPosition = e.transform.Value.position.y + e.offset.Value;
                if (currentPosition <= lastPosition) continue;

                e.ReplaceIndex(e.index.Value + 1);

                var eTask = Contexts.sharedInstance.environment.CreateEntity();
                {
                    eTask.isGenerationTask = true;
                    eTask.AddPosition(new Vector3(0, lastPosition + e.length.Value, 0));
                    eTask.AddPool(e.pool.Value);
                    eTask.AddLength(e.length.Value);
                    eTask.isCleanup = true;
                }
            }
        }
    }
}