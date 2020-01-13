using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SceneRig : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float CameraTrackingSpeed = 1f;
    [SerializeField] private float CameraTrackingDelay = 0.5f;
    
    private Vector3 _lastTargetPosition;

    private Tweener _tweener;
    
    private void Start()
    {
        _lastTargetPosition = _target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (_target.position == _lastTargetPosition)
        {
            return;
        }

        _lastTargetPosition = _target.position;

        var enableInEase = true;
        
        if (_tweener != null && !_tweener.IsComplete())
        {
            _tweener.Kill();
            enableInEase = false;
        }
        
        _tweener = transform.DOMoveX(_target.transform.position.x, CameraTrackingSpeed)
            .SetEase(enableInEase ? Ease.InOutCubic : Ease.OutCubic);

    }
}
