using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow_Transform : MonoBehaviour
{
    public Transform trans;
    public Vector3 offset;

    void Update()
    {
        if (trans != null) transform.position = trans.position + offset;
    }
}
