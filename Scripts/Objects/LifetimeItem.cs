using Sirenix.OdinInspector;

namespace GameEngine
{
    public abstract class LifetimeItem : Item
    {
        [Required] public CountParameter LifetimeParameter;

        public virtual bool IsWorking => Lifetime > 0;

        public virtual string LifetimeString => FormatUtility.ToMinuteString(Lifetime);

        public override string GetPopupDropString(int value) => "+" + FormatUtility.ToMinuteString(value);

        public int Lifetime
        {
            get => LifetimeParameter.Value;
            set => LifetimeParameter.Value = value;
        }

        public override void SaveDropValue(int value)
        {
            base.SaveDropValue(value);

            Lifetime += value;
        }
    }
}