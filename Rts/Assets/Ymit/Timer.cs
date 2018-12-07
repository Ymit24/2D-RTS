using System;
using UnityEngine;
namespace Ymit
{
    public class Timer : MonoBehaviour
    {
        private float Rate;
        private float CurrentTick;
        private event Action OnCycle;
        private bool Running;

        public void SetRate(float Rate) { this.Rate = Rate; }
        public void SetCycleCallback(Action OnCycle) { this.OnCycle = OnCycle; }

        public void Play() { Running = true; }
        public void Pause() { Running = false; }
        public void Clear() { CurrentTick = 0; }

        private void Update()
        {
            if (!Running) return;
            CurrentTick -= Time.deltaTime;
            if (CurrentTick <= 0)
            {
                CurrentTick = Rate;
                if (OnCycle != null)
                {
                    OnCycle();
                }
            }
        }
    }
}
