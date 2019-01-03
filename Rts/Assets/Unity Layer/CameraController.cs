using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float PanSpeed = 200f;
    protected void Update()
    {
        Vector2 vel = Vector2.zero;
        if (Input.GetKey(KeyCode.W)) { vel.y += 1; }
        if (Input.GetKey(KeyCode.A)) { vel.x -= 1; }
        if (Input.GetKey(KeyCode.S)) { vel.y -= 1; }
        if (Input.GetKey(KeyCode.D)) { vel.x += 1; }
        
        transform.Translate(vel * PanSpeed * (Input.GetKey(KeyCode.LeftShift) ? 2 : 1) * Time.deltaTime);

        Camera.main.orthographicSize = Camera.main.orthographicSize * (1 + -1 * Input.GetAxis("Mouse ScrollWheel"));
    }
}
