// Copyright (c) Facebook, Inc. and its affiliates.
// Use of the material below is subject to the terms of the MIT License
// https://github.com/oculus-samples/Unity-AppSpaceWarp/tree/main/Assets/LICENSE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public RaycastSelector AssocRaycastSelector;

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.X, OVRInput.Controller.LTouch))
            OVRManager.SetSpaceWarp(!OVRManager.GetSpaceWarp());

        
        var selectedObject = AssocRaycastSelector.SelectedObject;
        var selectedObjectControls = AssocRaycastSelector?.SelectedObject?.GetComponent<ObjectControls>();

        if (selectedObjectControls.AllowTranslationChange && OVRInput.GetDown(OVRInput.RawButton.B, OVRInput.Controller.RTouch))
            selectedObjectControls.ToggleTranslationMultiplier();

        if (selectedObjectControls.AllowToggleMotionVec && OVRInput.GetDown(OVRInput.RawButton.A, OVRInput.Controller.RTouch))
            selectedObjectControls.ToggleMotionVectors();

        if (selectedObjectControls.AllowToggleZWrite && OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger, OVRInput.Controller.RTouch))
            selectedObjectControls.ToggleZWrite();

        if (selectedObjectControls.AllowToggleRGB && OVRInput.GetDown(OVRInput.RawButton.RHandTrigger, OVRInput.Controller.RTouch))
            selectedObjectControls.ToggleRGB();

        if (selectedObjectControls.AllowToggleTexture && OVRInput.GetDown(OVRInput.RawButton.RThumbstickUp, OVRInput.Controller.RTouch))
            selectedObjectControls.ToggleTexture();

        if (selectedObjectControls.AllowToggleObjectEnabled && OVRInput.GetDown(OVRInput.RawButton.RThumbstickDown, OVRInput.Controller.RTouch))
            selectedObjectControls.ToggleObjectEnabled();
    }
}
