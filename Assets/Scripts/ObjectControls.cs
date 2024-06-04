// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectControls : MonoBehaviour
{
    static string RGB_ENABLED_KEYWORD = "RGB_ON";
    static string MOTIONVECTOR_ENABLED_KEYWORD = "MOTIONVECTORS_ON";
    static string ZWRITE_KEYWORD = "_ZWrite";
    static string TEXTURE_KEYWORD = "_BaseMap";

    public OVRInput.RawButton ToggleButton;

    [Header("Translation")]
    public bool AllowTranslationChange;
    public Vector3 TranslationDirection;
    [Tooltip("X-axis is time (seconds). Y-axis is distance along TranslationDirection (meters).")]
    public AnimationCurve TranslationCurve;
    public float[] TranslationMultipliers;

    [Header("Rotation")]
    public bool AllowRotationChange;
    public Vector3 RotationAxis;
    [Tooltip("Measured in degrees per second")]
    public float[] RotationSpeeds;

    [Header("Material Controls")]
    public bool AllowToggleRGB;
    public bool AllowToggleZWrite;
    public bool AllowToggleMotionVec;
    public bool AllowToggleRenderer;
    public bool AllowToggleTexture;
    public Texture2D[] TextureValues;
    public bool AllowToggleMesh;
    public Mesh[] MeshValues;

    private Vector3 m_startingTranslation;
    private float m_currTranslationTime = 0;
    private int m_toggleCount = 0;
    private MeshRenderer m_assocMeshRenderer;
    private MeshFilter m_assocMeshFilter;

    void Awake()
    {
        m_startingTranslation = this.transform.position;

        m_assocMeshRenderer = GetComponent<MeshRenderer>();
        m_assocMeshFilter = GetComponent<MeshFilter>();
        if (AllowToggleTexture)
            m_assocMeshRenderer.material.SetTexture(TEXTURE_KEYWORD, TextureValues[0]);
        if (AllowToggleMesh)
            m_assocMeshFilter.sharedMesh = MeshValues[0];
    }

    void Update()
    {
        if (OVRInput.GetDown(ToggleButton))
        {
            ++m_toggleCount;

            if (AllowToggleRGB)
            {
                if (m_assocMeshRenderer.material.IsKeywordEnabled(RGB_ENABLED_KEYWORD))
                    m_assocMeshRenderer.material.DisableKeyword(RGB_ENABLED_KEYWORD);
                else
                    m_assocMeshRenderer.material.EnableKeyword(RGB_ENABLED_KEYWORD);
            }

            if (AllowToggleZWrite)
                m_assocMeshRenderer.material.SetInt(ZWRITE_KEYWORD, m_assocMeshRenderer.material.GetInt(ZWRITE_KEYWORD) > 0f ? 0 : 1);

            if (AllowToggleMotionVec)
            {
                if (m_assocMeshRenderer.material.IsKeywordEnabled(MOTIONVECTOR_ENABLED_KEYWORD))
                    m_assocMeshRenderer.material.DisableKeyword(MOTIONVECTOR_ENABLED_KEYWORD);
                else
                    m_assocMeshRenderer.material.EnableKeyword(MOTIONVECTOR_ENABLED_KEYWORD);
            }
                
            if (AllowToggleTexture)
                m_assocMeshRenderer.material.SetTexture(TEXTURE_KEYWORD, TextureValues[m_toggleCount % TextureValues.Length]);

            if (AllowToggleMesh)
                m_assocMeshFilter.sharedMesh = MeshValues[m_toggleCount % MeshValues.Length];

            if (AllowToggleRenderer)
                m_assocMeshRenderer.enabled = !m_assocMeshRenderer.enabled;
        }

        if (AllowTranslationChange)
        {
            m_currTranslationTime += Time.deltaTime * TranslationMultipliers[m_toggleCount % TranslationMultipliers.Length];
            this.transform.position = m_startingTranslation + (TranslationDirection * TranslationCurve.Evaluate(m_currTranslationTime));
        }

        if (AllowRotationChange)
        {
            this.transform.rotation = this.transform.rotation * Quaternion.AngleAxis(Time.deltaTime * RotationSpeeds[m_toggleCount % RotationSpeeds.Length], RotationAxis);
        }
    }
}
