using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKeyInteractableObject
{
    public void ShowText();
    public void HideText();
    public void Interact();

    public void InteractEnd();
}
