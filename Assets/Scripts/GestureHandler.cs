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
    [SerializeField] private float shakeThreshold = 0.1f; // Imposta la soglia
    
    private float lastShakeTime = 0f;
    
    private readonly List<InputAction> _trackedActions = new();
    private int _gestureType; 
    private CameraInteractions _tap;
    
    void Awake()
    {
        _tap = new CameraInteractions();
        _trackedActions.Add(_tap.Stimuli.Tap);
        _trackedActions.Add(_tap.Stimuli.Swipe);
        _trackedActions.Add(_tap.Stimuli.Longpress);
        _trackedActions.Add(_tap.Stimuli.Shake); // ALWAYS THE LAST PLS PLS PLS PLS PLS PLS PLS
    }
    
    // Focus on the stimulus or interacts and restore part of endurance with it if already focused
    private void OnGesture(InputAction.CallbackContext context)
    {
        Debug.Log("OnGesture" + _trackedActions[_gestureType]);

        // True if the ray intersects the game object
        if (CheckedGesture(context))
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

    private bool CheckedGesture(InputAction.CallbackContext context)
    {
        if (_gestureType != _trackedActions.IndexOf(_tap.Stimuli.Shake))
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

            if ((Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == this.gameObject))
            {
                return true; 
            }
            return false;
        }
        // Code for shaking
        InputSystem.EnableDevice(Accelerometer.current);

        if (Accelerometer.current == null) return false; 

        Vector3 acceleration = context.ReadValue<Vector3>();

        // Confronta la grandezza dell'accelerazione con la soglia
        if (acceleration.sqrMagnitude > shakeThreshold * shakeThreshold)
        {
            lastShakeTime = Time.time;
            return true;
        }

        return false; 
    }
}
