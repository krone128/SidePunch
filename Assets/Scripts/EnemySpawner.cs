
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


namespace DefaultNamespace
{
    public class EnemySpawner : MonoBehaviour
    {
        public event Action<Target> OnTargetSpawned;

        [SerializeField]
        private float MinGroupSpawnTimeInterval = 3f;
        [SerializeField]
        private float MaxGroupSpawnTimeInterval = 4f;
        
        [SerializeField]
        private float MinGroupMobDistance = 1f;
        [SerializeField]
        private float MaxGroupMobDistance = 1.2f;
        
        [SerializeField]
        private float MinSpawnTimeInterval = 0.5f;
        [SerializeField] 
        private float groupDistance = 1f; 

        [SerializeField]
        private int MaxGroupSize = 3;


        [SerializeField]
        private Vector2 _movementVelocity = new Vector2(1f, 0);
        [SerializeField] 
        private float _attackRange = 0.5f; 
      
        [SerializeField] 
        private AttackDirection _attackDirection;
        
        [SerializeField]
        private GameObject enemyPrefab;

        private List<Target> _spawned = new List<Target>();
        
        private float _nextSpawnTime;
        public bool EnableSpawn { get; set; }

        private float DirectionMultiplier => _attackDirection == AttackDirection.Left ? -1f : 1f;
        
        private void Start()
        {
            GenerateNextSpawnTime();
        }

        private void Update()
        {
            if (EnableSpawn)// && Time.realtimeSinceStartup > _nextSpawnTime)
            {
                SpawnGroup();
                GenerateNextSpawnTime();
                EnableSpawn = false;
            }
        }

        private void GenerateNextSpawnTime()
        {
            _nextSpawnTime = Time.realtimeSinceStartup + Random.Range(MinGroupSpawnTimeInterval, MaxGroupSpawnTimeInterval);
        }

        private void SpawnEnemy(Vector3 spawnPosition)
        {
            var enemyComp = GameObject.Instantiate(enemyPrefab, spawnPosition, Quaternion.identity).GetComponent<Target>();
            enemyComp.Init(_movementVelocity,_attackRange, _attackDirection);
            _spawned.Add(enemyComp);
            OnTargetSpawned?.Invoke(enemyComp);
        }

        public void SpawnGroup()
        {
            var groupSize = Random.Range(1, MaxGroupSize);
            
            for (var i = 0; i < groupSize; i++)
            {
                SpawnEnemy(transform.position + new Vector3(Random.Range(MinGroupMobDistance, MaxGroupMobDistance) * DirectionMultiplier * i, 0f));
            }
        }

        public void OnEnemyDestroyed(Target destroyed)
        {
            if (_spawned.Contains(destroyed))
            {
                _spawned.Remove(destroyed);
                
                if (_spawned.Count == 0)
                {
                    SpawnGroup();
                }
            }
        }

        public void Clear()
        {
            _spawned.Clear();
        }
    }
}