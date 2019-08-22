using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseCamera : MonoBehaviour
{
    [SerializeField]
    Camera _cam;

    [SerializeField]
    Transform camTarget;
    [SerializeField]
    float chaseSpeed;

    // Start is called before the first frame update
    void Start()
    {
        transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, camTarget.position, chaseSpeed);
    }

    public void UpdateCamTarget(int direction)
    {
        camTarget.transform.localPosition = direction > 0 ?
            new Vector3(Mathf.Abs(camTarget.transform.localPosition.x), camTarget.transform.localPosition.y, camTarget.transform.localPosition.z) : //move target to right of character
            new Vector3(Mathf.Abs(camTarget.transform.localPosition.x) * -1, camTarget.transform.localPosition.y, camTarget.transform.localPosition.z); //move target to left of character 
    }
}
