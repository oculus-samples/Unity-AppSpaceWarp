// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace AppSpaceWarp.Interactions
{
    public class ObjectControls : MonoBehaviour
    {
        enum MATERIAL_TYPE
        {
            MT_TRANSPARENT,
            MT_TRANSPARENT_WITH_MOTION_VECTOR,
            MT_OPAQUE,

            MT_COUNT
        }

        private static string s_rgbEnableKeyword = "RGB_ON";
        private static string s_motionVectorsEnabledKeyword = "MOTIONVECTORS_ON";
        private static string s_zWriteKeyword = "_ZWrite";
        private static string s_textureKeyword = "_BaseMap";

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
        public bool AllowToggleMaterial;
        public Texture2D[] TextureValues;
        public bool AllowToggleMesh;
        public Mesh[] MeshValues;
        public Material TransparentMaterial;
        public Material OpaqueMaterial;
        public Material MotionVectorMaterial;

        private Vector3 m_startingTranslation;
        private float m_currTranslationTime = 0;
        private int m_toggleCount = 0;
        private MeshRenderer m_assocMeshRenderer;
        private MeshFilter m_assocMeshFilter;
        private GameObject m_motionVectorMesh = null;
        private MATERIAL_TYPE m_currentMaterialType = MATERIAL_TYPE.MT_TRANSPARENT;

        private void Awake()
        {
            m_startingTranslation = transform.position;

            m_assocMeshRenderer = GetComponent<MeshRenderer>();
            m_assocMeshFilter = GetComponent<MeshFilter>();
            if (AllowToggleTexture)
                m_assocMeshRenderer.material.SetTexture(s_textureKeyword, TextureValues[0]);
            if (AllowToggleMesh)
                m_assocMeshFilter.sharedMesh = MeshValues[0];
        }

        private void Update()
        {
            if (OVRInput.GetDown(ToggleButton))
            {
                ++m_toggleCount;

                if (AllowToggleRGB)
                {
                    if (m_assocMeshRenderer.material.IsKeywordEnabled(s_rgbEnableKeyword))
                        m_assocMeshRenderer.material.DisableKeyword(s_rgbEnableKeyword);
                    else
                        m_assocMeshRenderer.material.EnableKeyword(s_rgbEnableKeyword);
                }

                if (AllowToggleZWrite)
                    m_assocMeshRenderer.material.SetInt(s_zWriteKeyword, m_assocMeshRenderer.material.GetInt(s_zWriteKeyword) > 0f ? 0 : 1);

                if (AllowToggleMotionVec)
                {
                    if (m_assocMeshRenderer.material.IsKeywordEnabled(s_motionVectorsEnabledKeyword))
                        m_assocMeshRenderer.material.DisableKeyword(s_motionVectorsEnabledKeyword);
                    else
                        m_assocMeshRenderer.material.EnableKeyword(s_motionVectorsEnabledKeyword);
                }

                if (AllowToggleTexture)
                    m_assocMeshRenderer.material.SetTexture(s_textureKeyword, TextureValues[m_toggleCount % TextureValues.Length]);

                if (AllowToggleMesh)
                    m_assocMeshFilter.sharedMesh = MeshValues[m_toggleCount % MeshValues.Length];

                if (AllowToggleRenderer)
                    m_assocMeshRenderer.enabled = !m_assocMeshRenderer.enabled;

                if (AllowToggleMaterial)
                {
                    m_currentMaterialType = (MATERIAL_TYPE)(((int)m_currentMaterialType + 1) % (int)MATERIAL_TYPE.MT_COUNT);
                    if (m_currentMaterialType == MATERIAL_TYPE.MT_TRANSPARENT)
                    {
                        m_assocMeshRenderer.material = TransparentMaterial;
                    }
                    else if (m_currentMaterialType == MATERIAL_TYPE.MT_TRANSPARENT_WITH_MOTION_VECTOR)
                    {
                        m_motionVectorMesh = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                        m_motionVectorMesh.transform.parent = transform;
                        m_motionVectorMesh.transform.localPosition = Vector3.zero;
                        m_motionVectorMesh.transform.localScale = Vector3.one;
                        m_motionVectorMesh.GetComponent<Renderer>().material = MotionVectorMaterial;
                    }
                    else
                    {
                        Destroy(m_motionVectorMesh);
                        m_assocMeshRenderer.material = OpaqueMaterial;
                    }
                }
            }

            if (AllowTranslationChange)
            {
                m_currTranslationTime += Time.deltaTime * TranslationMultipliers[m_toggleCount % TranslationMultipliers.Length];
                transform.position = m_startingTranslation + TranslationDirection * TranslationCurve.Evaluate(m_currTranslationTime);
            }

            if (AllowRotationChange)
            {
                transform.rotation = transform.rotation * Quaternion.AngleAxis(Time.deltaTime * RotationSpeeds[m_toggleCount % RotationSpeeds.Length], RotationAxis);
            }
        }
    }
}
