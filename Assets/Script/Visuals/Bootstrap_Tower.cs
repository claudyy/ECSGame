using System.Collections;
using System.Collections.Generic;
using Unity.Rendering;
using UnityEngine;

public class Bootstrap_Tower : MonoBehaviour
{
    public static MeshInstanceRenderer unitRenderer;
    [SerializeField] private MeshInstanceRenderer _unitRenderer;

    private void Awake()
    {
        unitRenderer = _unitRenderer;

    }
}
