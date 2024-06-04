// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class StatsText : MonoBehaviour
{
    [Tooltip("{0} is ASW status. {1} is FPS. {2} is scene name."), Multiline(8)]
    public string TextFormat;

    TMP_Text m_assocText;
    float m_aveFrameTime = -1;

    void Start()
    {
        m_assocText = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (m_aveFrameTime <= 0)
            m_aveFrameTime = Time.unscaledDeltaTime;
        else
            m_aveFrameTime = 0.9f * m_aveFrameTime + 0.1f * Time.unscaledDeltaTime;

        bool isASWenabled = OVRManager.GetSpaceWarp(); 

        m_assocText.text = string.Format(TextFormat, isASWenabled, (1f / m_aveFrameTime).ToString("F1"), SceneManager.GetActiveScene().name);
    }
}
