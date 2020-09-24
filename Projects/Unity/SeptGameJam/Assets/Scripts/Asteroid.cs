using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Asteroid : MonoBehaviour
{
  float rotz;

  // Start is called before the first frame update
  void Start()
  {
    float _camAspect = Camera.main.aspect;
    float _camTop = Camera.main.orthographicSize;
    float _camBottom = -_camTop;
    float _camRight = _camAspect * _camTop;
    float _camLeft = -_camRight;

    float scalex = Random.Range(0.5f, 2.0f);
    float scaley = scalex + Random.Range(-0.3f, 0.3f);
    float posx = Random.Range(_camLeft, _camRight);
    float posy = Random.Range(_camBottom, _camTop);
    rotz = Random.Range(-360.0f, 360.0f);

    transform.position = new Vector3(posx, posy, 0);   
    transform.localScale = new Vector3(scalex, scalex, 1);
    transform.eulerAngles = new Vector3(0, 0, rotz);

    rotz = Random.Range(-50, 50);
  }

  // Update is called once per frame
  void Update()
  {
    var angles = transform.eulerAngles;
    transform.eulerAngles = angles + new Vector3(0, 0, rotz*Time.deltaTime);
  }
}
