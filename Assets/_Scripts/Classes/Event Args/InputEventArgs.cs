using System;
using UnityEngine.InputSystem;

public class InputEventArgs : EventArgs{
        public InputActionPhase inputActionPhase;
        public InputAction.CallbackContext callbackContext;

        public InputEventArgs(InputActionPhase _inputActionPhase, InputAction.CallbackContext _callbackContext){
            inputActionPhase = _inputActionPhase;
            callbackContext = _callbackContext;
        }
    }