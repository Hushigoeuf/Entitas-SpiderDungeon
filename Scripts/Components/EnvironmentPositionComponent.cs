using System;
using Entitas;

namespace GameEngine
{
    [Flight]
    [Environment]
    [Config]
    public sealed class EnvironmentPositionComponent : IComponent
    {
        private const int SIZE = 4;

        public int X0;
        public int X1;
        public int X2;
        public int X3;

        public void Set(int index, int value)
        {
            switch (index)
            {
                case 0:
                    X0 = value;
                    break;
                case 1:
                    X1 = value;
                    break;
                case 2:
                    X2 = value;
                    break;
                case 3:
                    X3 = value;
                    break;
                default:
#if !GE_DEBUG_DISABLED
                    throw new CustomArgumentException();
#endif
                    break;
            }
        }

        public void Set(int value)
        {
            for (var i = 0; i < SIZE; i++) Set(i, value);
        }

        public void Set(int[] values)
        {
#if !GE_DEBUG_DISABLED
            if (values == null || values.Length > SIZE) throw new CustomArgumentException();
#endif
            for (var i = 0; i < values.Length; i++) Set(i, values[i]);
        }

        public int Get(int index)
        {
            switch (index)
            {
                case 0:
                    return X0;
                case 1:
                    return X1;
                case 2:
                    return X2;
                case 3:
                    return X3;
                default:
#if !GE_DEBUG_DISABLED
                    throw new CustomArgumentException();
#endif
                    return 0;
            }
        }

        public int[] Get()
        {
            return new[] {Get(0), Get(1), Get(2), Get(3)};
        }

        public void Fill(ref int[] values)
        {
#if !GE_DEBUG_DISABLED
            if (values == null || values.Length > SIZE) throw new CustomArgumentException();
#endif
            for (var i = 0; i < values.Length; i++) values[i] = Get(i);
        }

        public int GetSize()
        {
            return SIZE;
        }
    }
}