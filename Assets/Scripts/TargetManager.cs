using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public class TargetManager : MonoBehaviour
    {
        public event Action<Target> OnPlayerAttacked;
        
        [SerializeField] private TimeManager _timeManager;
        
        private List<Target> _leftEnemyRow = new List<Target>();
        private List<Target> _rightEnemyRow = new List<Target>();
        
        [SerializeField]
        private PlayerControllerNew _playerController;
        private Vector2 PlayerPosition => _playerController.transform.position;

        public bool IsEnabled { get; set; }
        
        private List<Target> GetEnemyRow(AttackDirection direction)
        {
            switch (direction)
            {
                case AttackDirection.Left:
                    return _leftEnemyRow;
                case AttackDirection.Right:
                    return _rightEnemyRow;
            }
            
            throw new ArgumentException($"No Enemy row for direction {direction}");
        }

        private void FixedUpdate()
        {
            if (!IsEnabled)
            {
                return;
            }
            
            foreach (var target in _leftEnemyRow)
            {
                target.UpdatePosition(_timeManager.GetFixedDeltaScaled);
                target.UpdateAnimatorSpeed(_timeManager.TimeScale);
                
                
                if (Vector2.Distance(PlayerPosition, target.transform.position) < target.AttackRange)
                {
                    OnPlayerAttacked?.Invoke(target);
                    return;
                }
            }
            
            foreach (var target in _rightEnemyRow)
            {   
                target.UpdatePosition(_timeManager.GetFixedDeltaScaled);
                target.UpdateAnimatorSpeed(_timeManager.TimeScale);
                
                if (Vector2.Distance(PlayerPosition, target.transform.position) < target.AttackRange)
                {
                    OnPlayerAttacked?.Invoke(target);
                    return;
                }
            }
        }

        public void AddTarget(Target target)
        {
            GetEnemyRow(target.AttackDirection).Add(target);
        }

        public void DestroyTarget(Target target)
        {
            GetEnemyRow(target.AttackDirection).Remove(target);
            target.DestroyTarget(target.gameObject);
        }

        public bool AnyTargetInRange(Vector2 position, float attackRange)
        {
            return _leftEnemyRow.Any(t => Vector2.Distance(t.transform.position, position) < attackRange) ||
                   _rightEnemyRow.Any(t => Vector2.Distance(t.transform.position, position) < attackRange);
        }
        
        public Target GetTargetInRange(AttackDirection direction, Vector2 senderPosition, float attackRange)
        {
            var orderedByDistance = GetEnemyRow(direction).OrderBy(t => Vector2.Distance(t.transform.position, senderPosition)).ToList();

            if (orderedByDistance.Count <= 0)
            {
                return null;
            }

            var closest = orderedByDistance.First();

            if(Vector2.Distance(closest.transform.position, senderPosition) < attackRange)
                return closest;

            return null;
        }
        
        
        public void ClearSpawned()
        {
            foreach (var e in _leftEnemyRow)
            {
                if(e != null)
                    Destroy(e.gameObject);
            }

            foreach (var e in _rightEnemyRow)
            {
                if(e != null)
                    Destroy(e.gameObject);
            }
            
            _leftEnemyRow.Clear();
            _rightEnemyRow.Clear();
        }
    }
}