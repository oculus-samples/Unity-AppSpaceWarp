// Copyright (c) Facebook, Inc. and its affiliates.
// Use of the material below is subject to the terms of the MIT License
// https://github.com/oculus-samples/Unity-AppSpaceWarp/tree/main/Assets/LICENSE

using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class SelectedObjectCanvas : MonoBehaviour
{
    public Text AssocText;
    public RaycastSelector AssocRaycastSelector;

    [Tooltip("{0} is selected object."), Multiline]
    public string TextFormat;
    [Tooltip("{0} is current translation multiplier."), Multiline]
    public string TextFormat_TranslationControl;
    [Tooltip("{0} is current MotionVector-enabled status."), Multiline]
    public string TextFormat_MotionVecControl;
    [Tooltip("{0} is current ZWrite-enabled status."), Multiline]
    public string TextFormat_ZWriteControl;
    [Tooltip("{0} is current RGB-enabled status."), Multiline]
    public string TextFormat_RGBControl;
    [Tooltip("{0} is name of current texture."), Multiline]
    public string TextFormat_Texture;
    [Tooltip("{0} is name of object to enable, {1} is enabled status."), Multiline]
    public string TextFormat_EnableObjectControl;

    StringBuilder m_stringBuilder = new StringBuilder();

    void Update()
    {
        var selectedObject = AssocRaycastSelector.SelectedObject;
        var objectControls = selectedObject?.GetComponent<ObjectControls>();

        m_stringBuilder.Clear();
        m_stringBuilder.AppendFormat(TextFormat, selectedObject?.name ?? "[none]");
        
        if (objectControls && objectControls.AllowTranslationChange)
        {
            m_stringBuilder.Append('\n');
            m_stringBuilder.AppendFormat(TextFormat_TranslationControl, objectControls.CurrentTranslationMultiplier);
        }
        
        if (objectControls && objectControls.AllowToggleMotionVec)
        {
            m_stringBuilder.Append('\n');
            m_stringBuilder.AppendFormat(TextFormat_MotionVecControl, objectControls.MotionVecEnabled);
        }
        
        if (objectControls && objectControls.AllowToggleZWrite)
        {
            m_stringBuilder.Append('\n');
            m_stringBuilder.AppendFormat(TextFormat_ZWriteControl, objectControls.ZWriteEnabled);
        }
        
        if (objectControls && objectControls.AllowToggleRGB)
        {
            m_stringBuilder.Append('\n');
            m_stringBuilder.AppendFormat(TextFormat_RGBControl, objectControls.RGBEnabled);
        }
        
        if (objectControls && objectControls.AllowToggleTexture)
        {
            m_stringBuilder.Append('\n');
            m_stringBuilder.AppendFormat(TextFormat_Texture, objectControls.CurrentTexture.name);
        }
        
        if (objectControls && objectControls.AllowToggleObjectEnabled)
        {
            m_stringBuilder.Append('\n');
            m_stringBuilder.AppendFormat(TextFormat_EnableObjectControl, objectControls.ObjectToEnable.name, objectControls.ObjectToEnable.activeSelf);
        }

        AssocText.text = m_stringBuilder.ToString();
    }
}
