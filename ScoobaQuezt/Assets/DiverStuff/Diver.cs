using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Diver : MonoBehaviour
{
    public float _shimmySpeed = 1.2f;
    public float _shimmyTwist = 40.0f;
    public float _swimTurnSpeed = 250.0f;
    public float _swimMoveSpeed = 2.2f;
    public float _reorientSpeed = 4.0f;
    public float _reshimmySpeed = 4.0f;
    public float _diveFlipSpeed = 70.0f;
    public float _diveDownSpeed = 1.4f;
    public float _descentSpeed = 0.8f;
    public float _ascentSpeed = 2.0f;
    public float _ascentFlipSpeed = 80.0f;
    public float _ascentMoveSpeed = 0.5f;
    public Vector3 UpdateVelocity { get; set; } = new Vector3();

    Vector3 _lastPos;
    //public float _diveDepth = 3.0f;

    Animator _animator;
    Quaternion _shimmyLeft;
    Quaternion _shimmyRight;

    int _isSwimmingHash;
    float _swimTurn = 0; // moving left increases turn (negative around y axis), moving right decreases turn (positive around y axis) 
    bool _reorientingTread = false;
    float _reshimmy = 0.0f;

    float _diveAngle = 0; // not currently diving (0.0), transitioning from straight to downward (to 90.0) or vice versa (to -90.0)
    float _diveDirection = 0.0f; // not currently diving (0.0), flipping down (1.0), flipping up (-1.0)
    float _diveBoostRemaining = 0.0f;
    float _ascentAngle = 0;

    // in degrees
    const float FULLY_TURNED = 90.0f;
    const float FULLY_DIVED = 90.0f;
    const float FULLY_ASCENDED = -90.0f; // need to flip guy upwards fully from swim face down position

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
        UpdateVelocity = transform.position - _lastPos;
        _lastPos = transform.position;

        bool isSwimming = _animator.GetBool(_isSwimmingHash);
        bool downPressed = Input.GetKey("s");
        bool upPressed = Input.GetKey("w");
        bool isDiving = _diveDirection != 0.0f;// _diveAngle > 0.0f;
        bool isDescending = _diveAngle == FULLY_DIVED;
        bool isAscending = upPressed && !isDiving;

        // from treading to swimming and back

        if (!isSwimming && (downPressed || upPressed))
        {
            _animator.SetBool("isSwimming", true);

        }

        if (isSwimming && !downPressed && !upPressed && !isDiving)
        {
            _animator.SetBool("isSwimming", false);
        }

        if(Input.GetKeyDown("w"))
        {
           
           
        }

        if(isAscending)
        {
            // since swim animation is face downwards, need to flip dude back upwards when climbing
            float nextAscent = _ascentAngle - _ascentFlipSpeed * Time.deltaTime;
            _ascentAngle = Mathf.Clamp(nextAscent, FULLY_ASCENDED, 0);
            Vector3 eulers = transform.rotation.eulerAngles;
            eulers.x = _ascentAngle;
            transform.rotation = Quaternion.Euler(eulers);

            // also move up in the world
            Vector3 pos = transform.position;
            float ascent = _ascentSpeed * Time.deltaTime;
            transform.position = new Vector3(pos.x, pos.y + ascent, pos.z);

            // also handle keys for translation
            if (Input.GetKey("a"))
            {
                // translate to the left
                transform.position = transform.position + new Vector3(+_ascentMoveSpeed * Time.deltaTime, 0, 0);
            }

            if(Input.GetKey("d"))
            {
                // translate to the left
                transform.position = transform.position + new Vector3(-_ascentMoveSpeed * Time.deltaTime, 0, 0);
            }
        }
        else
        {
            _ascentAngle = 0.0f;

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

            if (isDiving)
            {
                // flip
                float nextDive = _diveAngle + _diveDirection * _diveFlipSpeed * Time.deltaTime;
                _diveAngle = Mathf.Clamp(nextDive, 0, FULLY_DIVED);
                Vector3 eulers = transform.rotation.eulerAngles;
                eulers.x = _diveAngle;
                transform.rotation = Quaternion.Euler(eulers);

                // move
                Vector3 pos = transform.position;
                float descent = _diveDownSpeed * Time.deltaTime;
                transform.position = new Vector3(pos.x, pos.y - descent, pos.z);

                // if angle has restored to zero, no more diving to do
                if (_diveAngle == 0.0f)
                    _diveDirection = 0.0f;
            }

            if (isDescending)
            {
                Vector3 pos = transform.position;
                float descent = _descentSpeed * Time.deltaTime;
                transform.position = new Vector3(pos.x, pos.y - descent, pos.z);
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
            if (_reshimmy > 0.0f && !isDiving) // shimmy left
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, _shimmyLeft, _reshimmySpeed * Time.deltaTime);
            }
            // trading over to the right (when facing diver)
            else if (_reshimmy < 0.0f && !isDiving) // shimmy right
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, _shimmyRight, _reshimmySpeed * Time.deltaTime);
            }

            // left
            if (Input.GetKey("a"))// && !isDiving)
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
                    // translate left
                    transform.position = transform.position + new Vector3(+_swimMoveSpeed * Time.deltaTime, 0, 0);

                    if (!isDiving && !isDescending)
                    {
                        // turn left while swimming 
                        float nextTurn = _swimTurn + _swimTurnSpeed * Time.deltaTime;
                        _swimTurn = Mathf.Clamp(nextTurn, -FULLY_TURNED, FULLY_TURNED);
                        transform.rotation = Quaternion.Euler(_swimTurn * Vector3.up);
                    }
                    /*
                    Vector3 eulers = transform.rotation.eulerAngles;
                    eulers.y = _swimTurn;
                    transform.rotation = Quaternion.Euler(eulers);
                    */
                }
            }
            // right
            else if (Input.GetKey("d"))// && !isDiving)
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
                    // translate right
                    transform.position = transform.position + new Vector3(-_swimMoveSpeed * Time.deltaTime, 0, 0);

                    if (!isDiving && !isDescending)
                    {
                        // turn right while swimming
                        float nextTurn = _swimTurn - _swimTurnSpeed * Time.deltaTime;
                        _swimTurn = Mathf.Clamp(nextTurn, -FULLY_TURNED, FULLY_TURNED);
                        transform.rotation = Quaternion.Euler(_swimTurn * Vector3.up);
                    }
                    /*
                    Vector3 eulers = transform.rotation.eulerAngles;
                    eulers.y = _swimTurn;
                    transform.rotation = Quaternion.Euler(eulers);
                    */
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
}
