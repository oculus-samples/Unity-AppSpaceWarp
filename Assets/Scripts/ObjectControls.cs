// Copyright (c) Facebook, Inc. and its affiliates.
// Use of the material below is subject to the terms of the MIT License
// https://github.com/oculus-samples/Unity-AppSpaceWarp/tree/main/Assets/LICENSE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectControls : MonoBehaviour
{
    static string RGB_ENABLED_KEYWORD = "RGB_ON";
    static string MOTIONVECTOR_ENABLED_KEYWORD = "MOTIONVECTORS_ON";
    static string ZWRITE_KEYWORD = "_ZWrite";
    static string TEXTURE_KEYWORD = "_BaseMap";

    [Header("Translation")]
    public bool AllowTranslationChange;
    public Vector3 TranslationDirection;
    [Tooltip("X-axis is time (seconds). Y-axis is distance along TranslationDirection (meters).")]
    public AnimationCurve TranslationCurve;
    public float[] TranslationMultipliers;

    [Header("Material Controls")]
    public bool AllowToggleRGB;
    public bool AllowToggleZWrite;
    public bool AllowToggleMotionVec;
    public bool AllowToggleTexture;
    public Texture2D[] TextureValues;

    [Header("Object Enabled")]
    public bool AllowToggleObjectEnabled;
    public GameObject ObjectToEnable;

    private Vector3 m_startingTranslation;
    private int m_currTranslationMultIndex = 0;
    private float m_currTranslationTime = 0;

    private int m_currTextureIndex = 0;
    private MeshRenderer m_assocMeshRenderer;

    public float CurrentTranslationMultiplier { get { return TranslationMultipliers[m_currTranslationMultIndex]; } }
    public Texture2D CurrentTexture { get { return TextureValues[m_currTextureIndex]; } }
    public bool RGBEnabled { get { return m_assocMeshRenderer.material.IsKeywordEnabled(RGB_ENABLED_KEYWORD); } }
    public bool MotionVecEnabled { get { return m_assocMeshRenderer.material.IsKeywordEnabled(MOTIONVECTOR_ENABLED_KEYWORD); } }
    public bool ZWriteEnabled { get { return m_assocMeshRenderer.material.GetInt(ZWRITE_KEYWORD) > 0f; } }

    void Awake()
    {
        m_startingTranslation = this.transform.position;
        m_assocMeshRenderer = GetComponent<MeshRenderer>();
        if (AllowToggleTexture)
            m_assocMeshRenderer.material.SetTexture(TEXTURE_KEYWORD, CurrentTexture);
    }

    void Update()
    {
        if (AllowTranslationChange)
        {
            m_currTranslationTime += Time.deltaTime * CurrentTranslationMultiplier;
            this.transform.position = m_startingTranslation + (TranslationDirection * TranslationCurve.Evaluate(m_currTranslationTime));
        }
    }

    public void ToggleTranslationMultiplier()
    {
        if (AllowTranslationChange)
            m_currTranslationMultIndex = (m_currTranslationMultIndex + 1) % TranslationMultipliers.Length;
    }

    public void ToggleRGB()
    {
        if (AllowToggleRGB)
        {
            if (RGBEnabled)
                m_assocMeshRenderer.material.DisableKeyword(RGB_ENABLED_KEYWORD);
            else
                m_assocMeshRenderer.material.EnableKeyword(RGB_ENABLED_KEYWORD);
        }
    }

    public void ToggleMotionVectors()
    {
        if (AllowToggleMotionVec)
        {
            if (MotionVecEnabled)
                m_assocMeshRenderer.material.DisableKeyword(MOTIONVECTOR_ENABLED_KEYWORD);
            else
                m_assocMeshRenderer.material.EnableKeyword(MOTIONVECTOR_ENABLED_KEYWORD);
        }
    }

    public void ToggleZWrite()
    {
        if (AllowToggleZWrite)
        {
            m_assocMeshRenderer.material.SetInt(ZWRITE_KEYWORD, ZWriteEnabled ? 0 : 1);
        }
    }

    public void ToggleTexture()
    {
        if (AllowToggleTexture)
        {
            m_currTextureIndex = (m_currTextureIndex + 1) % TextureValues.Length;
            m_assocMeshRenderer.material.SetTexture(TEXTURE_KEYWORD, CurrentTexture);
        }
        
    }
    
    public void ToggleObjectEnabled()
    {
        if (AllowToggleObjectEnabled)
        {
            ObjectToEnable.SetActive(!ObjectToEnable.activeSelf);
        }
    }
}
