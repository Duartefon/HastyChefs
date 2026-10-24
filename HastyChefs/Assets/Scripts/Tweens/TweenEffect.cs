using PrimeTween;
using UnityEngine;

namespace Tweens
{
    public class TweenEffect : MonoBehaviour
    {
        [SerializeField] private TweenEffectPreset effectPreset;

        private Sequence activeSequence;

        private void OnEnable()
        {
            PlaySequence();
        }

        private void OnDisable()
        {
            // Stop on disable to prevent PrimeTween from trying to animate an inactive object
            activeSequence.Stop();
        }



        public void PlaySequence()
        {
            if (effectPreset == null)
            {
                Debug.LogWarning($"[TweenEffect] No preset assigned to {gameObject.name}", this);
                return;
            }

            // Cancel previous animation if object was rapidly toggled
            activeSequence.Stop();

            // Run the preset logic
            // activeSequence = (gameObject, true);
        }
    }
}