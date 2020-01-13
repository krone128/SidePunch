
using UnityEngine;

namespace DefaultNamespace
{
    public class TimeManager : MonoBehaviour
    {
        private float _timeScale = 1f;
        private float _scaleModeExpiredTime;

        public float TimeScale => _timeScale;

        public float GetDeltaScaled => Time.deltaTime * _timeScale;
        public float GetFixedDeltaScaled => Time.fixedDeltaTime * _timeScale;

        public void SetScale(float scale, float duration)
        {
            _timeScale = scale;
            
            CancelInvoke(nameof(ExpireScale));
            Invoke(nameof(ExpireScale),duration);
        }

        private void ExpireScale()
        {
            _timeScale = 1f;
        }
    }
}