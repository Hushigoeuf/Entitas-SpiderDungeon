using Sirenix.OdinInspector;

namespace GameEngine
{
    /// <summary>
    /// Базовый класс для предмета с временем жизни.
    /// </summary>
    public abstract class LifetimeItem : Item
    {
        [Required] public CountParameter LifetimeParameter;

        /// Находится ли предмет в активном состоянии
        public virtual bool IsWorking => Lifetime > 0;

        /// Возвращает время жизни в формате времени
        public virtual string LifetimeString => FormatUtility.ToMinuteString(Lifetime);

        public override string GetPopupDropString(int value) => "+" + FormatUtility.ToMinuteString(value);

        /// Время жизни
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