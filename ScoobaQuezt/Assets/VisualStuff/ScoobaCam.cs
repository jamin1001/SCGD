using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ScoobaCam : MonoBehaviour
{
    public Diver _target;
    public float _twistScale = 0.2f; // how much to twist left or right based on target distance from center
    public float _twistSpeed = 3.0f; // how fast to do the twist adjustment
    public float _pedestalSpeed = 2.0f; // pedastal is a camera movement term, for vertical up and down
    public float _targetVelocityScale = 60.0f;

    Camera _camera;

    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_target == null)
            Debug.Log("WARNING: No target GameObject attached to ScoobaCam!");

        // Shift camera a bit based on location of the target left and right from the center of the world.
        Vector3 eulers = transform.rotation.eulerAngles;
        float targetEulerY = 180.0f + _twistScale * _target.transform.position.x;
        eulers.y = Mathf.Lerp(eulers.y, targetEulerY, _twistSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(eulers);

        // Move camera based on vertical scale extents of target.
        Vector3 camPos = transform.position;
        Vector3 targetPos = _target.transform.position;
        float extentY = _target.transform.localScale.y;
        float boundaryYTop = targetPos.y + extentY;
        float boundaryYBot = targetPos.y - extentY;

        // Velocity offset for scaling according to speed
        float velYScale = _targetVelocityScale * _target.UpdateVelocity.y;

        // Do the vertical movement
        if (camPos.y > boundaryYTop)
        {
            float diff = camPos.y - boundaryYTop;
            float targetCamY = camPos.y - diff + velYScale;
            float newY = Mathf.Lerp(camPos.y, targetCamY, _pedestalSpeed * Time.deltaTime);
            transform.position = new Vector3(camPos.x, newY, camPos.z);
        }
        else if(camPos.y < boundaryYBot)
        {
            float diff = boundaryYBot - camPos.y;
            float targetCamY = camPos.y + diff + velYScale;
            float newY = Mathf.Lerp(camPos.y, targetCamY, _pedestalSpeed * Time.deltaTime);
            transform.position = new Vector3(camPos.x, newY, camPos.z);
        }

    }
}
