using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class SceneController : MonoBehaviour
    {
        private readonly Vector3 PlayerStartPosition = new Vector3(0f, 0.2f);
        
        [SerializeField] private List<EnemySpawner> _enemySpawners;
        [SerializeField] private TimeManager _timeManager;
        [SerializeField] private TargetManager _targetManager;
        [SerializeField] private PlayerControllerNew _player;
        [SerializeField] private SceneRig _sceneRig;
        
        [SerializeField] private GameObject _menuPanel;
        [SerializeField] private TMP_Text _menuMessage;
        [SerializeField] private Button _startGameButton;
        
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private TMP_Text _comboText;

        [SerializeField] private float _comboExpireInterval = 2f;
        
        [SerializeField] private float _slowmoScale = 0.3f;
        [SerializeField] private float _slowmoDurationTime = 0.5f;
        
        private int _gameScore;
        private int _comboCounter;
        private float _lastEnemyKilledTime;
        private float _comboExpireTime;
        
        private void Start()
        {
            Init();
        }

        private void Init()
        {
            _startGameButton.onClick.AddListener(StartRound);

            _player.OnEnemyDestroyed += _targetManager.DestroyTarget;
            _player.OnEnemyDestroyed += EnemyDestroyed;

            _targetManager.OnPlayerAttacked += EndRound;
            
            foreach (var spawner in _enemySpawners)
            {
                spawner.OnTargetSpawned += _targetManager.AddTarget;
            }
            
            _menuPanel.SetActive(true);
        }

        private void EnemyDestroyed(Target target)
        {
            if (_player.AnyTargetInAttackRange())
            {
                _timeManager.SetScale(_slowmoScale, _slowmoDurationTime);
            }

            foreach (var spawner in _enemySpawners)
            {
                spawner.OnEnemyDestroyed(target);
            }
            
            _gameScore++;
            _comboCounter++;
            _comboExpireTime = Time.realtimeSinceStartup + _comboExpireInterval;
            _scoreText.text = $"Score { _gameScore.ToString()}";
            ComboUpdated();
        }

        private void ComboUpdated()
        {
            _comboText.text = _comboCounter > 0 ? $"Combo x{_comboCounter}" : string.Empty;
        }

        private void FixedUpdate()
        {
            if (_comboCounter > 0 && Time.realtimeSinceStartup > _comboExpireTime)
            {
                _comboCounter = 0;
                ComboUpdated();
            }
        }

        private void StartRound()
        {
            ResetGameState();

            _enemySpawners.ForEach(s =>
            {
                s.EnableSpawn = true;
            });
            
            _targetManager.ClearSpawned();
            
            _player.EnableControl = true;
            _targetManager.IsEnabled = true;
            _menuPanel.gameObject.SetActive(false);
        }

        private void EndRound(Target target)
        {
            _enemySpawners.ForEach(s =>
            {
                s.EnableSpawn = false;
            });

            _targetManager.IsEnabled = false;
            
            _player.EnableControl = false;
            
            _menuPanel.gameObject.SetActive(true);
            _menuMessage.text = $"Your score: {_gameScore.ToString()}";
        }

        private void ResetGameState()
        {
            _gameScore = 0;
            _comboCounter = 0;
            _scoreText.text = "Score: 0";
            _comboText.text = string.Empty;
            
            _enemySpawners.ForEach(s =>
            {
                s.Clear();
            });

        }

    }
}