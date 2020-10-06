using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using DG.Tweening;
using DigitalRubyShared;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerControllerNew : MonoBehaviour
{
    public event Action OnEnemyCollisionEnter;
    public event Action<Target> OnEnemyDestroyed;


    [SerializeField]
    private Vector2 MissAttackTranslation = new Vector2(1f, 0f);
    [SerializeField]
    private float StaggeredInterval = 0.25f;
    [SerializeField]
    private float AttackRange = 3f;
    [SerializeField]
    private float AttackSpeed = 5f;
    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    [SerializeField] 
    private Animator _animator;
    
    [SerializeField]
    private TargetManager _targetManager;
    
    private SwipeGestureRecognizer swipeGesture;
    private TapGestureRecognizer tapGesture;

    private float _staggerEndTimestamp;
    private bool _isAlive;
    private bool _isStaggered;
    private bool _isSuccessfulAttackAerial;
    private Vector2 _desiredPosition;

    private List<string> AttackAnimList = new List<string> {"Attack1", "Attack2"};
    
    public bool EnableControl
    {
        set => _isAlive = value;
    }

    public Vector2 DesiredPosition => _desiredPosition;

    // Start is called before the first frame update
    void Start()
    {
        CreateTapGesture();
    }

    private void Update()
    {
        ProcessKeyboardInput();
    }

    private void FixedUpdate()
    {
        CheckStaggerState();
    }

    private void SetStaggeredState()
    {
         _staggerEndTimestamp = Time.realtimeSinceStartup + StaggeredInterval;
         _isStaggered = true;
    }

    public void AttemptAttack(float direction)
    {
        var attackDirection = direction > 0 ? AttackDirection.Right : AttackDirection.Left;
        PlayMove(attackDirection);
        
        var target = GetTargetInAttackRange(attackDirection);
        if (target == null)
        {
            MoveMissAttack(direction);
            SetStaggeredState();
            return;
        }

        MoveToTarget(target);

        OnEnemyDestroyed?.Invoke(target);
    }

    private void PlayMove(AttackDirection attackDirection)
    {
        _spriteRenderer.flipX = attackDirection == AttackDirection.Left;
        _animator.Play(AttackAnimList[Random.Range(0, AttackAnimList.Count)], -1, 0f);
    }

    private void MoveMissAttack(float direction)
    {
        _desiredPosition = transform.position + Vector3.Scale(MissAttackTranslation, new Vector2(direction, 1f));
        var duration = Vector2.Distance(_desiredPosition, transform.position) / AttackSpeed;
        transform.DOLocalMove(_desiredPosition, duration).SetEase(Ease.OutCubic);

    }
    
    private void MoveToTarget(Target target)
    {
        var attackPosition =  transform.position - target.transform.position;
        attackPosition = Vector3.ClampMagnitude(attackPosition, 0.5f);
        _desiredPosition = target.transform.position + attackPosition;
        _desiredPosition.y = Mathf.Max(-0.5f, _desiredPosition.y);
        
        var duration = Vector2.Distance(_desiredPosition, transform.position) / AttackSpeed;
        transform.DOLocalMove(_desiredPosition, duration).SetEase(Ease.OutCubic);
    }

    public bool AnyTargetInAttackRange()
    {
        return _targetManager.AnyTargetInRange(_desiredPosition, AttackRange);
    }
    
    private Target GetTargetInAttackRange(AttackDirection direction)
    {
        return _targetManager.GetTargetInRange(direction, DesiredPosition, AttackRange);
    }

    public void CheckStaggerState()
    {
        if (!_isStaggered)
        {
            return;
        }
        
        if (Time.realtimeSinceStartup > _staggerEndTimestamp)
        {
            _isStaggered = false;
        }
    }
    
    #region Input

    private void TapGestureCallback(GestureRecognizer gesture)
    {
        if (!_isAlive ||
            _isStaggered) return;

        if (gesture.State == GestureRecognizerState.Began)
        {
            ProcessTapInput();
            Debug.LogFormat("Tapped at {0}, {1}", gesture.FocusX, gesture.FocusY);
        }
    }

    private void ProcessTapInput()
    {
        var directionX =
            Mathf.Sign(tapGesture.FocusX - Camera.main.WorldToScreenPoint(transform.position).x);

        AttemptAttack(directionX);
    }

    private void CreateTapGesture()
    {
        tapGesture = new TapGestureRecognizer();
        tapGesture.SendBeginState = true;
        tapGesture.StateUpdated += TapGestureCallback;
        FingersScript.Instance.AddGesture(tapGesture);
    }

    private void ProcessKeyboardInput()
    {
        if (!_isAlive ||
            _isStaggered) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            AttemptAttack(-1);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            AttemptAttack(1);
        }
    }
    
 #endregion
}
