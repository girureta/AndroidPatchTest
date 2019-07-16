using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptToModify : MonoBehaviour
{
    public Transform cube;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
#if ROTATE_CUBE
        cube.transform.rotation *= Quaternion.Euler(0.0f, 180.0f * Time.deltaTime, 0.0f);
#endif
    }
}
