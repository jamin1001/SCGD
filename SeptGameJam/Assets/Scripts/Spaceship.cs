
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
  public float shipAcc = 2.0F;
  public float shipSpeed = 0;
  public float shipRotation = 100;
  public Vector3 shipVelocity = Vector3.zero;
  public float dtheta = 0;
  // public float rtheta = 0;
  public float slope = 0;
  public float x1=0, y1=0, x2=0, y2=0, x3=0, y3=0;
  public Vector3 P1=Vector3.zero, P2=Vector3.zero, P3=Vector3.zero;

  public string keyPress = "";
  public string logEntry = "";

  float _camAspect;
  float _camTop;
  float _camBottom;
  float _camRight;
  float _camLeft;

  // Start is called before the first frame update
  void Start()
  {
    _camAspect = Camera.main.aspect;
    _camTop = Camera.main.orthographicSize;
    _camBottom = -_camTop;
    _camRight = _camAspect * _camTop;
    _camLeft = -_camRight;
  }

  // Update is called once per frame
  void Update()
  {
    logEntry = "";
    keyPress = "";

    //########################################
    // {WASD} & {Arrows keys}
    CheckWASD();  
    CheckArrowsKey();

    // Brake and Reset the ship ........................................
    if (Input.GetKey(KeyCode.Space))
    {
      shipSpeed = 0;
      shipVelocity = Vector3.zero;
      transform.position = Vector3.zero;
      transform.rotation = Quaternion.identity;
    }
    else if (Input.GetKey(KeyCode.Escape))
    {
      Application.Quit();
    }

    // transform.Translate(Vector3.zero);// works!
    transform.Translate(shipVelocity * Time.deltaTime, Space.World);
    // transform.Translate(Vector3.up * Time.deltaTime, Space.World);

    CheckWarp(true);

    // check on it!!!!!!
    // Debug.DrawLine(new Vector3(-3, -4, 0), new Vector3(3, 4, 0), Color.red);

    slope = shipVelocity.y/shipVelocity.x;
    x1 = transform.position.x;
    y1 = transform.position.y;
      P1 = new Vector3(x1, y1, 0);
    x2 = 10;// get programatically
    y2 = slope*x2 + y1 - slope*x1;
      P2 = new Vector3(x2, y2, 0);
    x3 = -10;// get programatically
    y3 = slope*x3 + y1 - slope*x1;
      P3 = new Vector3(x3, y3, 0);
    Debug.DrawLine(P1, P2, Color.blue);
    Debug.DrawLine(P1, P3, Color.red);



    // if (Mathf.Approximately(dtheta, 90)) 
    // if (Mathf.Abs(dtheta - 90) < 0.00001) 
    // {
    //   slope = 999999;
    // }

    PrintDebugLog();

    return;
  }//Update.end

  void CheckWASD(){
    // Increment speed .................................................
    if (Input.GetKey(KeyCode.W))
    {
      keyPress = "{W}:";
      shipSpeed = shipAcc * Time.deltaTime;
      shipVelocity += shipSpeed * transform.right;
    }
    // Decrease Speed ..................................................
    else if (Input.GetKey(KeyCode.S))
    {
      keyPress = "{S}:";
      shipSpeed = -shipAcc * Time.deltaTime;
      shipVelocity += shipSpeed * transform.right;
    }
    // Rotate CW .......................................................
    else if (Input.GetKey(KeyCode.D))
    {
      keyPress = "{D}:";
      transform.Rotate(new Vector3(0, 0, -shipRotation * Time.deltaTime));
    }
    // Rotate CCW ......................................................
    else if (Input.GetKey(KeyCode.A))
    {
      keyPress = "{A}:";
      transform.Rotate(new Vector3(0, 0, shipRotation * Time.deltaTime));
    }

    return;
  }//CheckWASD.end

  void CheckArrowsKey()
  {
    if (Input.GetKey(KeyCode.UpArrow))
    {
      keyPress = "{Up-Arrow}:";
      // shipSpeed += shipAcc*Time.deltaTime;
      // shipSpeed = 0.01F*Time.deltaTime;
      shipSpeed = shipAcc * Time.deltaTime;
      shipVelocity += shipSpeed * transform.right;
    }
    else if (Input.GetKey(KeyCode.DownArrow))
    {
      keyPress = "{Down-Arrow}:";
      // shipSpeed -= shipAcc*Time.deltaTime;
      // shipSpeed = -0.01F*Time.deltaTime;
      shipSpeed = -shipAcc * Time.deltaTime;
      shipVelocity += shipSpeed * transform.right;
    }
    if (Input.GetKey(KeyCode.RightArrow))
    {
      keyPress = "{Right-Arrow}:";
      transform.Rotate(new Vector3(0, 0, -shipRotation * Time.deltaTime));
    }
    else if (Input.GetKey(KeyCode.LeftArrow))
    {
      keyPress = "{Left-Arrow}:";
      transform.Rotate(new Vector3(0, 0, shipRotation * Time.deltaTime));
    }

    return;    
  }//CheckArrowsKey().end

  void PrintDebugLog()
  {
    logEntry += keyPress;
    // logEntry += ", " + transform.right;// changing
    // logEntry += ", " + transform.right;// changing
    // logEntry += ", " + transform.forward;// constant
    // logEntry += ", " + shipSpeed*transform.right;
    // logEntry += ", " + Time.realtimeSinceStartup;
    logEntry += ", Vx = " + shipVelocity.x;
    logEntry += ", Vy = " + shipVelocity.y;
    slope = shipVelocity.x/shipVelocity.y;
    logEntry += ", m = " + slope;
    // logEntry += "(" + dtheta + " --> " + rtheta + " --> " + slope + ")";
    Debug.Log(logEntry);
    return;
  }//PrintDebugLog.end

  bool IsWithinCam(Vector3 point, float epsilon)
  {
    if (
        point.x < _camRight + epsilon &&
        point.x > _camLeft - epsilon &&
        point.y < _camTop + epsilon &&
        point.y > _camBottom - epsilon)
      return true;

      return false;
  }

  bool WarpShip(float Px, float Py, float Vx, float Vy, float t)
  {
    float epsilon = 0.01f;

    Vector3 intersectPoint = new Vector3(Px + Vx * t, Py + Vy * t, 0);
    if (IsWithinCam(intersectPoint, epsilon))
    {
      // Warp to other side along line.
      transform.position = intersectPoint;
      return true;
    }
    return false;
  }

  void CheckWarp(bool alongLine)
  {
    int side = -1;

    if (transform.position.x > _camRight) side = 0; // hit right
    else if (transform.position.x < _camLeft) side = 1; // hit left
    else if (transform.position.y > _camTop) side = 2; // hit top
    else if (transform.position.y < _camBottom) side = 3; // hit bottom

    // There was a collision, so check.
    if (side != -1)
    {
      Vector2 V = new Vector2(shipVelocity.x, shipVelocity.y).normalized; // physics
      float X = _camRight;
      float Y = _camTop;
      float Vx = V.x; // normal x component
      float Vy = V.y; // normal y component
      float Px = transform.position.x;
      float Py = transform.position.y;
      float t;

      // Infinity edge case, treat separately.
      if (!alongLine || Mathf.Approximately(Vx, 0) || Mathf.Approximately(Vy, 0))
      {
        if (side == 0) transform.Translate(new Vector3(-2 * X, 0, 0), Space.World); // move left
        else if (side == 1) transform.Translate(new Vector3(2 * X, 0, 0), Space.World); // move right
        else if (side == 2) transform.Translate(new Vector3(0, -2 * Y, 0), Space.World); // move down
        else if (side == 3) transform.Translate(new Vector3(0, 2 * Y, 0), Space.World); // move up
        return;
      }

      if (side != 0)
      {
        t = (X - Px) / Vx;
        if (WarpShip(Px, Py, Vx, Vy, t))
          return;
      }
      if (side != 1)
      {
        t = (-X - Px) / Vx;
        if (WarpShip(Px, Py, Vx, Vy, t))
          return;
      }
      if (side != 2)
      {
        t = (Y - Py) / Vy;
        if (WarpShip(Px, Py, Vx, Vy, t))
          return;
      }
      if (side != 3)
      {
        t = (-Y - Py) / Vy;
        if (WarpShip(Px, Py, Vx, Vy, t))
          return;
      }
    }
  }

  private void OnDrawGizmos()
  {

    //Gizmos.color = Color.red;
    //Gizmos.DrawLine(new Vector3(-3, -4, 0), new Vector3(3, 4, 0));
  }

}

