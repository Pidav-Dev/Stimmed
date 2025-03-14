using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Threading.Tasks;

public class GestureHandler : MonoBehaviour
{
    public UnityEvent correctGesture; // Called when the correct gesture is executed

    [SerializeField] private float gestureDelay = 0.5f; // The delay between the focus mode entering and the gesture manager enabling
    [SerializeField] private float shakeThreshold = 0.1f; // Threshold for shake detection 
    
    private readonly List<InputAction> _trackedActions = new(); // List of all possible gestures
    private int _gestureType; // Int value that describes the kind of gesture required by the stimulus
    private CameraInteractions _tap; // Input map for interactions
    private float _lastShakeTime; 
    
    void Awake()
    {
        _tap = new CameraInteractions(); // Create a new instance of the input map
        // Assign all the Gesture to the list of all tracked ones
        _trackedActions.Add(_tap.Stimuli.Tap);
        _trackedActions.Add(_tap.Stimuli.Swipe);
        _trackedActions.Add(_tap.Stimuli.Longpress);
        _trackedActions.Add(_tap.Stimuli.Shake);
    }
    
    // Called when the desired gesture is triggered, check gesture's parameters and eventually give confirmation
    private void OnGesture(InputAction.CallbackContext context)
    {
        // True if the interaction is corrected
        if (CheckedGesture(context))
        {
            // Unsubscribe from input action and call correct gesture's event
            _trackedActions[_gestureType].performed -= OnGesture;
            _tap.Stimuli.Disable();
            correctGesture.Invoke();
        }
    }

    // Invoked by event when user taps on stimulus, assign desired gesture after the needed delay to let the focus animation end
    public async void TapGesture(int i)
    {
        await Task.Delay((int)(gestureDelay*1000)); 
        _gestureType = Math.Clamp(i, 0, _trackedActions.Count); 
        // Subscribe to input action the gesture manager method
        _tap.Stimuli.Enable();
        _trackedActions[_gestureType].performed += OnGesture; 
    }

    // Checks if the shake is correctly done or if the gesture is directed to the game object
    private bool CheckedGesture(InputAction.CallbackContext context)
    {
        // True if the gesture is not the shake
        if (_gestureType != _trackedActions.IndexOf(_tap.Stimuli.Shake))
        {
            Vector2 touchPosition; // Get touch position to raycast
    
            // Get primary touch position from mobile
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed) 
            {
                touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            } 
            // Get mouse position from PC
            else if (Mouse.current != null) 
            {
                touchPosition = Mouse.current.position.ReadValue();
            }
            // In case the two input methods are not successful
            else
            {
                touchPosition = Vector2.zero;
            }
            
            // Raycast from touch position
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);

            // True if the Raycast hit the caller game object 
            if ((Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == this.gameObject))
            {
                return true; 
            }
            return false;
        }
        // Executed only if the gesture is shake
        InputSystem.EnableDevice(Accelerometer.current); // Enables device's accelerometer
        if (Accelerometer.current == null) return false; // Return false if there is no accelerometer 
        Vector3 acceleration = context.ReadValue<Vector3>(); // Get input values in form of Vector3

        // Compare acceleration's magnitude with threshold
        if (acceleration.sqrMagnitude > shakeThreshold * shakeThreshold)
        {
            _lastShakeTime = Time.time;
            return true;
        }

        return false; // Acceleration not sufficient
    }
}
