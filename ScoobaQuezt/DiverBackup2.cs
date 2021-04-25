using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Diver : MonoBehaviour
{
    public float _shimmySpeed = 2.0f;
    public float _shimmyTwist = 40.0f;
    public float _swimTurnSpeed = 250.0f;
    public float _reorientSpeed = 4.0f;
    public float _reshimmySpeed = 3.0f;
    public float _diveSpeed = 70.0f;
    public float _diveDepth = 3.0f;

    Animator _animator;
    Quaternion _shimmyLeft;
    Quaternion _shimmyRight;

    int _isSwimmingHash;
    public float _swimTurn = 0; // moving left increases turn (negative around y axis), moving right decreases turn (positive around y axis) 
    bool _reorientingTread = false;
    float _reshimmy = 0.0f;

    public float _diveAngle = 0; // not currently diving (0.0), transitioning from straight to downward (to 90.0) or vice versa (to -90.0)
    float _diveDirection = 0.0f; // not currently diving (0.0), flipping down (1.0), flipping up (-1.0)
    float _diveBoostRemaining = 0.0f;

    // in degrees
    const float FULLY_TURNED = 90.0f;
    const float FULLY_DIVED = 90.0f;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _isSwimmingHash = Animator.StringToHash("isSwimming");

        _shimmyLeft = Quaternion.Euler(0, 0, -_shimmyTwist);
        _shimmyRight = Quaternion.Euler(0, 0, _shimmyTwist);
    }

    // Update is called once per frame
    void Update()
    {
        bool isSwimming = _animator.GetBool(_isSwimmingHash);
        bool downPressed = Input.GetKey("s");
        bool isDiving = _diveDirection != 0.0f;// _diveAngle > 0.0f;

        // from treading to swimming and back

        if (!isSwimming && downPressed)
        {
            _animator.SetBool("isSwimming", true);

        }

        if (isSwimming && !downPressed)
        {
            _animator.SetBool("isSwimming", false);
        }


        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _animator.SetBool("isSwimming", true);

            // start flipping down
            if (_diveAngle <= 0)
                _diveDirection = 1.0f;
            // start flipping up
            else if (_diveAngle >= FULLY_DIVED)
                _diveDirection = -1.0f;
            // toggle between flip
            else
                _diveDirection = -_diveDirection;
        }

        if (_diveDirection != 0.0f)
        {
            float nextDive = _diveAngle + _diveDirection * _diveSpeed * Time.deltaTime;
            _diveAngle = Mathf.Clamp(nextDive, 0, FULLY_DIVED);
            Vector3 eulers = transform.rotation.eulerAngles;
            eulers.x = _diveAngle;
            transform.rotation = Quaternion.Euler(eulers);

            // if angle has restored to zero, no more diving to do
            if (_diveAngle == 0.0f)
                _diveDirection = 0.0f;
        }

        // READJUSTMENTS

        // to treading water idle
        if (_reorientingTread)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, _reorientSpeed * Time.deltaTime);
            if (Quaternion.Dot(transform.rotation, Quaternion.identity) < 0.001f)
                _reorientingTread = false;
        }

        // treading over to the left (when facing diver)
        if (_reshimmy > 0.0f) // shimmy left
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _shimmyLeft, _reshimmySpeed * Time.deltaTime);
        }
        // trading over to the right (when facing diver)
        else if (_reshimmy < 0.0f) // shimmy right
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _shimmyRight, _reshimmySpeed * Time.deltaTime);
        }
           
        // left
        if (Input.GetKey("a"))
        {
            if (!isSwimming)
            {
                // translate to the left
                transform.position = transform.position + new Vector3(+_shimmySpeed * Time.deltaTime, 0, 0);

                // also keep slanted left
                _reshimmy = 1.0f;
            }
            else
            {
                // turn left while swimming 
                float nextTurn = _swimTurn + _swimTurnSpeed * Time.deltaTime;
                _swimTurn = Mathf.Clamp(nextTurn, -FULLY_TURNED, FULLY_TURNED);
                //transform.rotation = Quaternion.Euler(_swimTurn * Vector3.up);
                Vector3 eulers = transform.rotation.eulerAngles;
                eulers.y = _swimTurn;
                transform.rotation = Quaternion.Euler(eulers);
            }
        }
        // right
        else if (Input.GetKey("d"))
        {
            if (!isSwimming)
            {
                // translate to the right
                transform.position = transform.position + new Vector3(-_shimmySpeed * Time.deltaTime, 0, 0);

                // also keep slanted right
                _reshimmy = -1.0f;
            }
            else
            {
                // turn right while swimming
                float nextTurn = _swimTurn - _swimTurnSpeed * Time.deltaTime;
                _swimTurn = Mathf.Clamp(nextTurn, -FULLY_TURNED, FULLY_TURNED);
                //transform.rotation = Quaternion.Euler(_swimTurn * Vector3.up);
                Vector3 eulers = transform.rotation.eulerAngles;
                eulers.y = _swimTurn;
                transform.rotation = Quaternion.Euler(eulers);
            }
        }
        // Reorient the tread and shimmy if needed!
        else
        {
            _reorientingTread = true;
            _reshimmy = 0.0f;
            _swimTurn = 0.0f;
        }

        

        
            
    }
}
