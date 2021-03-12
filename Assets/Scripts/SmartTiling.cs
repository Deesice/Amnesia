#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartTiling : MonoBehaviour
{
    public float tileAmount = 0.4f;
    public Vector3 tileOffset;
    [SerializeField] Vector2[] uv;
    private void Awake()
    {
        ApplyUV();
    }
    public void Compute()
    {
        var m = GetComponent<MeshFilter>();

        uv = m.sharedMesh.uv;
        for (int y = 0; y < 11; y++)
            for (int x = 0; x < 11; x++)
            {
                uv[y * 11 + x].x = (x * transform.localScale.x) * tileAmount + tileOffset.x;
                uv[y * 11 + x].y = (y * transform.localScale.z) * tileAmount + tileOffset.z;
            }
    }
    public void ApplyUV()
    {
        GetComponent<MeshFilter>().mesh.uv = uv;
    }
}
#endif
