using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraBehaviour : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    private void Update()
    {
        transform.LookAt(target);
    }
}
