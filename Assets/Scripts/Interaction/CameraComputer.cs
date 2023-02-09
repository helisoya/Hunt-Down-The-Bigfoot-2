using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraComputer : InteractableObject
{
    protected override void Interaction()
    {
        base.Interaction();
        PlayerGUI.instance.OpenCameraMenu();
    }
}
