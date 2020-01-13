using System;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

namespace DefaultNamespace
{
    public class Target : MonoBehaviour
    {
        private SpriteResolver _spriteResolver;
        public event Action<Target> OnReachedKillZone;

        public AttackDirection AttackDirection { get; private set; }
        public float AttackRange { get; private set; }

        private const float KillZoneX = 15;
        
        [SerializeField]
        private Vector2 _movementVelocity;
        
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        [SerializeField]
        private Animator _animator;
        
        public void Init(Vector2 velocity, float attackRange,  AttackDirection direction)
        {
            _movementVelocity = velocity;
            AttackDirection = direction;
            AttackRange = attackRange;
            ApplyDirectionToVelocity();
            _spriteRenderer.flipX = AttackDirection == AttackDirection.Right;
        }

        public void UpdatePosition(float fixedDelta)
        {
            var newPosition = transform.localPosition + (Vector3)_movementVelocity * fixedDelta;

            transform.localPosition = newPosition;
        }

        public void UpdateAnimatorSpeed(float speed)
        {
            _animator.speed = speed;
        }

        private void ApplyDirectionToVelocity()
        {
            if (AttackDirection == AttackDirection.Right)
            {
                _movementVelocity *= new Vector2(-1f, 1f);
            }
        }

        public void DestroyTarget(GameObject targetGameObject)
        {
            Destroy(gameObject);
        }
    }
}