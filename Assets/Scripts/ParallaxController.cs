using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private List<ParallaxTarget> _targets;

    private Vector2 _prevTargetPosition;
    
    // Start is called before the first frame update
    public void Move(Vector2 delta)
    {
        foreach (var target in _targets)
        {
            target.UpdatePosition(delta);
        }
    }

    private void Start()
    {
        _prevTargetPosition = _target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 position = _target.position;
        
        if (_prevTargetPosition == position)
        {
            return;
        }

        Move(position - _prevTargetPosition);
        _prevTargetPosition = position;
    }
}

