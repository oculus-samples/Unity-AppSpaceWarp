// Copyright (c) Facebook, Inc. and its affiliates.
// Use of the material below is subject to the terms of the MIT License
// https://github.com/oculus-samples/Unity-AppSpaceWarp/tree/main/Assets/LICENSE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RaycastSelector : MonoBehaviour
{
    LineRenderer m_assocLineRenderer;
    public LayerMask RaycastLayers;
    public float MaxRaycastDistance;
    public GameObject SelectedObject { get; private set; }
    
    RaycastHit m_raycastHit;

    void Awake()
    {
        m_assocLineRenderer = GetComponent<LineRenderer>();
    }

    void FixedUpdate()
    {
        if (Physics.Raycast(this.transform.position, this.transform.forward, out m_raycastHit, MaxRaycastDistance, RaycastLayers))
        {
            SelectedObject = m_raycastHit.collider.gameObject;
            m_assocLineRenderer.SetPosition(1, new Vector3(0, 0, m_raycastHit.distance));
        } else
        {
            SelectedObject = null;
            m_assocLineRenderer.SetPosition(1, new Vector3(0, 0, MaxRaycastDistance));
        }
    }
}
