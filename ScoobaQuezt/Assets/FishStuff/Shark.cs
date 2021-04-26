using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark : MonoBehaviour
{
    public Transform _target;
    public GameObject _question;
    public GameObject _exclamation;

    public float _detectDistance = 5.0f; // sqrMag not actual
    public float _detectTime = 3.0f; // ? to !
    public float _detectMoveSpeed = 5.0f; // when detected, how fast to move
    public float _detectOrientSpeed = 10.0f; // when detected, how fast to orient

    float _detectCounter = 0.0f;

    bool IsSetup()
    {
        bool isSetup = _target != null && _question != null && _exclamation != null;
        return isSetup;
    }

    void Start()
    {
        if (!IsSetup())
            Debug.Log("WARNING Shark: target or question or exclamation object not set!");

        _question.SetActive(false);
        _exclamation.SetActive(false);
    }

    void Update()
    {
        if (!IsSetup())
            return;

        if ((_target.position - transform.position).sqrMagnitude < _detectDistance)
        {
            _detectCounter += Time.deltaTime;
            
            // ?
            if (_detectCounter < _detectTime)
            {
                _question.SetActive(true);
            }
            // !
            else
            { 
                _question.SetActive(false);
                _exclamation.SetActive(true);

                // go towards target now 
                Vector3 targetUnitDir = (_target.position - transform.position).normalized;
                transform.position += _detectMoveSpeed * Time.deltaTime * targetUnitDir;

                // orient towards target
                Quaternion targetDirOrient = Quaternion.LookRotation(targetUnitDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetDirOrient, _detectOrientSpeed * Time.deltaTime);

            }
        }
        else
        {
            _detectCounter = 0.0f;
            _question.SetActive(false);
            _exclamation.SetActive(false);
        }

        

    }
}
