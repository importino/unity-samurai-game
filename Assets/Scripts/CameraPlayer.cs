using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlayer : MonoBehaviour
{
    public Transform Samurai;
    Vector3 Target;

    public float CameraSpeed = 1.5f;

    // Update is called once per frame
    void Update()
    {
        if (Samurai)
        {
            Vector3 currentPosition = Vector3.Lerp(transform.position, Target, CameraSpeed * Time.deltaTime);
            transform.position = currentPosition;

            Target = new Vector3(Samurai.transform.position.x + 1, Samurai.transform.position.y + 1, -10f);
        }
    }
}
