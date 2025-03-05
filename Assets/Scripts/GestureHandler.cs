using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Threading.Tasks;

public class GestureHandler : MonoBehaviour
{
    public UnityEvent correctGesture;

    [SerializeField] private float gestureDelay = 0.5f; 
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
        Vector2 touchPosition;
    
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed) 
        {
            // Mobile: Usa il tocco primario
            touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        } 
        else if (Mouse.current != null) 
        {
            // PC: Usa il mouse
            touchPosition = Mouse.current.position.ReadValue();
        }
        else
        {
            touchPosition = Vector2.zero;
        }
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);

        // True if the ray intersects the game object
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == this.gameObject)
        {
            _trackedActions[_gestureType].performed -= OnGesture;
            _tap.Stimuli.Disable();
            correctGesture.Invoke();
        }
    }

    public async void TapGesture(int i)
    {
        await Task.Delay((int)(gestureDelay*1000)); 
        _gestureType = Math.Clamp(i, 0, _trackedActions.Count); 
        _tap.Stimuli.Enable();
        _trackedActions[_gestureType].performed += OnGesture; 
    }
}
