﻿using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace GameEngine
{
    [Flight]
    [Cleanup(CleanupMode.DestroyEntity)]
    public sealed class ResurrectionComponent : IComponent
    {
    }
}