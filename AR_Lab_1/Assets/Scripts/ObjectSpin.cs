using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpin : MonoBehaviour
{
    private Vector3 rotate = new Vector3(1, 1, 0);

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles += rotate;
    }
}
