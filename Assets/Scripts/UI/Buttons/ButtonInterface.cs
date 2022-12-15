using System.Collections;
using System.Collections.Generic;

using UnityEngine;
public interface ButtonInterface {
    /// <summary>
    /// Try to execute a button press. Don't do anything if the button is not interactable.
    /// </summary>
    public void TryToPress();
}