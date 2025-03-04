using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GestureHandler : MonoBehaviour
{
    public UnityEvent correctGesture;
    private readonly List<InputAction> _trackedActions = new();

    private int _gestureType; 
    private CameraInteractions _tap;
    
    void Awake()
    {
        _tap = new CameraInteractions();
        _trackedActions.Add(_tap.Stimuli.Tap);
    }
    
    // Focus on the stimulus or interacts and restore part of endurance with it if already focused
    private void OnGesture(InputAction.CallbackContext context)
    {
        _trackedActions[_gestureType].performed -= OnGesture; 
        _tap.Stimuli.Disable();
        correctGesture.Invoke();
    }

    public void TapGesture(int i)
    {
        _gestureType = Math.Clamp(i, 0, _trackedActions.Count); 
        _tap.Stimuli.Enable();
        _trackedActions[_gestureType].performed += OnGesture; 
    }
}
