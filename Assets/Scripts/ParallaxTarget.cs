using UnityEngine;

namespace DefaultNamespace
{
    public class ParallaxTarget : MonoBehaviour
    {
        [SerializeField]
        private float _distanceToViewpoint;
        [SerializeField]
        private Material _material;

        public void UpdatePosition(Vector2 offset)
        {
            _material.mainTextureOffset += offset / _distanceToViewpoint;
        }
    }
}