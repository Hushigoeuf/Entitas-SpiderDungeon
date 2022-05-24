using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// Обрабатывает столкновения персонажа с счетчиками.
    /// </summary>
    [AddComponentMenu(nameof(GameEngine) + "/" + nameof(ScoreActivated))]
    public class ScoreActivated : EntitasBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (!collider.CompareTag(GameSettings.TAG_SCORE)) return;
            if (!_contexts.config.isStatConfig) return;

            _contexts.config.statConfigEntity.ReplaceScore(
                _contexts.config.statConfigEntity.score.Value + _settings.ScoreSettings.Rate);
            _contexts.config.statConfigEntity.scoreNoDeath.CurrentValue += _settings.ScoreSettings.Rate;
        }
    }
}