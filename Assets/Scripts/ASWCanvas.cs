// Copyright (c) Facebook, Inc. and its affiliates.
// Use of the material below is subject to the terms of the MIT License
// https://github.com/oculus-samples/Unity-AppSpaceWarp/tree/main/Assets/LICENSE

using UnityEngine;
using UnityEngine.UI;

public class ASWCanvas : MonoBehaviour
{
    public bool ASWEnabledAtStart;

    [Tooltip("{0} is ASW status. {1} is FPS."), Multiline]
    public string TextFormat;
    public Text AssocText;

    float m_aveFrameTime = -1;

    void Start()
    {
        OVRManager.SetSpaceWarp(ASWEnabledAtStart);
    }

    void Update()
    {
        if (m_aveFrameTime <= 0)
            m_aveFrameTime = Time.unscaledDeltaTime;
        else
            m_aveFrameTime = 0.9f * m_aveFrameTime + 0.1f * Time.unscaledDeltaTime;

        bool isASWenabled = OVRManager.GetSpaceWarp(); 

        AssocText.text = string.Format(TextFormat, isASWenabled, 1f / m_aveFrameTime);
    }
}
