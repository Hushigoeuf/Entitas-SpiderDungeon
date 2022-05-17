using System;

namespace GameEngine
{
    public abstract class EntitasBehaviour : CustomMonoBehaviour
    {
        private Contexts _contexts;
        private Services _services;
        private Data _data;

        protected Contexts Contexts => _contexts;
        protected Services Services => _services;
        protected Data Data => _data;

        protected override void Awake()
        {
            base.Awake();

            _contexts = Contexts.sharedInstance;
            _services = Services.sharedInstance;
            _data = Data.sharedInstance;

#if !GE_DEBUG_DISABLED
            if (Contexts == null) throw new CustomException();
            if (Services == null) throw new CustomException();
            if (Data == null) throw new CustomException();
#endif
        }
    }
}