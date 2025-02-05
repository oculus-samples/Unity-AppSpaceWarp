// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Linq;
using UnityEngine;
namespace AppSpaceWarp.UI
{
    public class PlaneToRectTransform : MonoBehaviour
    {
        public RectTransform RectToMatch;
        private Mesh m_mesh;
        private Vector3[] m_corners;
        private MeshFilter m_meshFilter;

        private void OnEnable()
        {
            m_mesh = new Mesh();
            m_corners = new Vector3[4];
            m_mesh.vertices = m_corners;
            m_mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
            m_meshFilter = GetComponent<MeshFilter>();
        }

        private void Update()
        {
            if (RectToMatch == null) return;

            RectToMatch.GetWorldCorners(m_corners);
            for (var i = 0; i < m_corners.Length; ++i)
                m_corners[i] = transform.InverseTransformPoint(m_corners[i]);

            m_mesh.vertices = m_corners;
            if (m_meshFilter.sharedMesh == null
                || !Enumerable.SequenceEqual(m_meshFilter.sharedMesh.vertices, m_mesh.vertices))
            {
                m_meshFilter.sharedMesh = m_mesh;
            }
        }
    }
}
