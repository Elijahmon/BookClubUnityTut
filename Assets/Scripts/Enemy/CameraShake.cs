using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    [SerializeField]
    Camera _cam;
    [SerializeField]
    float shakeLerpSpeed;

    float shakeXStrength;
    float shakeYStrength;
    float shakeTimer;
    bool anyYDirection;


    public void ActivateScreenShake(float xStrength, float yStrength, float duration, bool anyDirection=false)
    {
        shakeXStrength = xStrength;
        shakeYStrength = yStrength;
        shakeTimer = duration;
        anyYDirection = anyDirection;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(shakeTimer > 0)
        {
            Vector2 thisShakeDirection = Vector2.zero;
            if (anyYDirection)
            {
                thisShakeDirection = new Vector2(_cam.transform.localPosition.x + Random.Range(-shakeXStrength, 0), _cam.transform.localPosition.y + shakeYStrength);
            }
            else
            {
                thisShakeDirection = new Vector2(_cam.transform.localPosition.x + Random.Range(-shakeXStrength, 0), _cam.transform.localPosition.y + shakeYStrength);
            }
            _cam.transform.localPosition = Vector2.Lerp(_cam.transform.localPosition, thisShakeDirection, shakeLerpSpeed);
        }
        shakeTimer--;
    }
}
