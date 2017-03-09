//Written by Kevin Scheitler - Crowbox
//Have any questions? Mail me: contact@crowbox.de

using UnityEngine;
using System;

[ExecuteInEditMode]
public class MeshMorphing : MonoBehaviour
{
    [Serializable]
    struct MorphTarget
    {
        public Mesh MorphMesh;
        [NonSerialized]
        public Vector3[] DiffVertices;

        [Range (-1, 1f)]
        public float Weight;

        internal void Initialize (Vector3[] m_VertsBase)
        {
            //We calculate and store only the difference for each vertex from the base mesh.
            DiffVertices = new Vector3[m_VertsBase.Length];
            var meshVerts = MorphMesh.vertices;
            for (int i = 0; i < m_VertsBase.Length; i++) {
                DiffVertices [i] = meshVerts [i] - m_VertsBase [i];
            }
        }
    }

    float m_LastMorph;

    //Make sure both meshes use the same vertex count and indices! Modify a mesh in a 3D editing tool and manipulate only the position of the vertecies is the best way to do that.
    //Keep in mind that changing values like normals or uvs can affect the vertex count of a mesh and your morph target might not match anymore! This is why this example only reads vertex position.
    public Mesh m_MeshBase;
    [SerializeField]
    MorphTarget[] m_MorphTargets;
    Mesh m_MorphedMesh;
    float[] m_LastWeights;
    public bool m_GenerateNormals;

    //You could add more than just one morp target! Just add the meshes and additional parameters and notice the example below.

    Material m_Mat;

    Vector3[] m_VertsBase;
    Vector3[] m_VertsCurrent;

    void Awake ()
    {
        m_MorphedMesh = new Mesh ();
        //This should improve performance a bit.
        m_MorphedMesh.MarkDynamic ();

        m_MorphedMesh.name = "Morphed Mesh";
        gameObject.GetComponent<MeshFilter> ().mesh = m_MorphedMesh;
        m_Mat = gameObject.GetComponent<MeshRenderer> ().sharedMaterial;
        ResetMesh ();
        UpdateMorphIfChange ();
    }

    public void ChangeWeight (int i, float weight)
    {
        m_MorphTargets [i].Weight = weight;
    }

    public float GetWeight (int i)
    {
        return m_MorphTargets [i].Weight;
    }

    void ResetMesh ()
    {
        m_LastWeights = new float[m_MorphTargets.Length];

        m_MorphedMesh.vertices = m_MeshBase.vertices;
        m_MorphedMesh.triangles = m_MeshBase.triangles;
        m_MorphedMesh.uv = m_MeshBase.uv;
        m_MorphedMesh.uv2 = m_MeshBase.uv2;
        m_MorphedMesh.normals = m_MeshBase.normals;
        m_MorphedMesh.tangents = m_MeshBase.tangents;
        m_MorphedMesh.bounds = m_MeshBase.bounds;
        //Add more properties if you need them

        m_VertsBase = m_MeshBase.vertices;
        for (int i = 0; i < m_MorphTargets.Length; i++) {
            m_MorphTargets [i].Initialize (m_VertsBase);
        }

        m_VertsCurrent = new Vector3[m_VertsBase.Length];
    }

    void Update ()
    {
        #if UNITY_EDITOR
        UpdateMorphIfChange ();
        #endif
    }

    void UpdateMorphIfChange ()
    {
        bool update = false;
        for (int i = 0; i < m_MorphTargets.Length; i++) {
            if (Math.Abs (m_LastWeights [i] - m_MorphTargets [i].Weight) > Mathf.Epsilon) {
                update = true;
            } else {
                continue;
            }
            m_LastWeights [i] = m_MorphTargets [i].Weight;
        }

        if (update)
            UpdateMorph ();

        //Use values in your shader for more advanced effects.
        //m_Mat.SetFloat ("_Fade", m_Morph);
    }

    void UpdateMorph ()
    {
//         Assume that recalculate normal does not change vertex count
//        if (m_GenerateNormals) {
//            ResetMesh ();
//        }


        for (int i = 0; i < m_VertsCurrent.Length; i++) {
            m_VertsCurrent [i] = m_VertsBase [i];
        }
        for (int d = 0; d < m_MorphTargets.Length; d++) {
            var target = m_MorphTargets [d];

            //The vertex cound should be the same, otherwise the meshes don't match.
            if (target.DiffVertices.Length != m_VertsBase.Length)
                continue;
            for (int i = 0; i < m_VertsCurrent.Length; i++) {
                m_VertsCurrent [i] += target.DiffVertices [i] * target.Weight;
            }
        }
        m_MorphedMesh.vertices = m_VertsCurrent;

        //You might want to update bounds or change other values as well such as vertex colors.
        //You can recalculate normals for instance, but you should keep in mind that this can change the vertex count! So in this case you have to reset the mesh before you call UpdateMorph() a second time.
        m_MorphedMesh.RecalculateBounds ();

        if (m_GenerateNormals) {
            m_MorphedMesh.RecalculateNormals ();
        }
            
    }
}

//Have any questions? Mail me: contact@crowbox.de
