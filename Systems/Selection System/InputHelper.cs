/**
 *  Source code from Youtube channel: TheScreamingFedora. 
 *  
 *  Extra features and bug fixes by: Erick Luis de Souza.
 *  Code by : Erick Luis de Souza.
 *  
 *  email me at: erickluiss@gmail.com 
 *  for aditional information.
 * 
 */


#if ENABLE_INPUT_SYSTEM
#define USING_NEW_INPUT_SYSTEM
#endif

#if USING_NEW_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using UnityEngine;

namespace SLE.Systems.Selection.Input
{
    internal static class InputHelper
    {
        internal static bool LeftMouseButtonWasPressed()
        {
#if USING_NEW_INPUT_SYSTEM
            return Mouse.current.leftButton.wasPressedThisFrame;
#else
            return Input.GetMouseButtonDown(0);
#endif
        }

        internal static bool LeftMouseButtonIsDown()
        {
#if USING_NEW_INPUT_SYSTEM
            return Mouse.current.leftButton.isPressed;
#else
            return Input.GetMouseButton(0);
#endif
        }

        internal static bool LeftMouseButtonWasReleased()
        {
#if USING_NEW_INPUT_SYSTEM
            return Mouse.current.leftButton.wasReleasedThisFrame;
#else
            return Input.GetMouseButtonUp(0);
#endif
        }

        internal static bool ShiftKeyIsPressed()
        {
#if USING_NEW_INPUT_SYSTEM
            return Keyboard.current.shiftKey.isPressed;
#else
            return Input.GetKey(KeyCode.LeftShift | KeyCode.RightShift);
#endif
        }

        internal static Vector3 GetCursorPosition()
        {
#if USING_NEW_INPUT_SYSTEM
            return Mouse.current.position.ReadValue();
#else
            return Input.mousePosition;
#endif
        }
    }
}
