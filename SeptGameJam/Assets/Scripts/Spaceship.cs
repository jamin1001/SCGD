
using System;
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

  public float W_R = 0, W_L = 0, W_T = 0, W_B=0;
  public float x_ship = 0, y_ship = 0;
  public float x_right=0, y_right = 0, x_left = 0, y_left = 0;

  public float slope = 0;

  public Vector3 xy_ship=Vector3.zero, xy_exit=Vector3.zero, xy_entrance=Vector3.zero;

  public string keyPress = "";
  public string logEntry = "";

  float _camAspect;
  float _camTop;
  float _camBottom;
  float _camRight;
  float _camLeft;

  public GameObject MissilePreFab;
  GameObject ob;
  Rigidbody2D missile;

  // Start is called before the first frame update
  void Start()
  {
    _camAspect = Camera.main.aspect;
    _camTop = Camera.main.orthographicSize;
    _camBottom = -_camTop;
    _camRight = _camAspect * _camTop;
    _camLeft = -_camRight;

    // for point 10% outside of the view port
    W_R = 1.2F * Camera.main.orthographicSize;
    W_L = -W_R;
    W_T = W_R * Camera.main.aspect;
    W_B = -W_T;
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
    shootMissile();
    resetSpaceShip();

    transform.Translate(shipVelocity * Time.deltaTime, Space.World);
    CheckWarp(true);

    // warping detection
    /*
    x_ship = transform.position.x;
    y_ship = transform.position.y;
    xy_ship = new Vector3(x_ship, y_ship, 0);
    if (x_ship >= W_R || x_ship <= W_L || y_ship >= W_T || y_ship <= W_B)
    {
      warpShip();
      transform.position = xy_entrance;
    }
    */

    PrintDebugLog();
    return;
  }//Update.end

  void warpShip()
  {
    slope = shipVelocity.y / shipVelocity.x;
    Vector3 warpSwapper = new Vector3();

    // point outside window in direction of motion
    y_right = slope * W_R + y_ship - slope * x_ship;
    xy_exit = new Vector3(W_R, y_right, 0);

    // point outside window opposite to direction of motion
    y_left = slope * W_L + y_ship - slope * x_ship;
    xy_entrance = new Vector3(W_L, y_left, 0);

    if (shipVelocity.x <= 0)
    {
      warpSwapper = xy_entrance;
      xy_entrance = xy_exit;
      xy_exit = warpSwapper;
    }

    Debug.DrawLine(xy_ship, xy_exit, Color.blue);// worp exit indicator
    Debug.DrawLine(xy_ship, xy_entrance, Color.red);// worp entrance indicator

  }

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

  void shootMissile()
  {
    keyPress += ", Space: " + Input.GetKey(KeyCode.Space);
    if (Input.GetKeyDown(KeyCode.Space))
    {
      try
      {
        ob = Instantiate(MissilePreFab, transform.position, transform.rotation);
        missile = ob.GetComponent<Rigidbody2D>();
        missile.AddForce(10 * transform.right, ForceMode2D.Impulse);
      } catch (Exception ex)
      {
        ex = ex;
      }
    }
    return;
  }

  void resetSpaceShip()
  {
    // Brake and Reset the ship ........................................
    // keyPress += ", RCtrl: " + Input.GetKey(KeyCode.RightControl);
    if (Input.GetKey(KeyCode.RightControl) && Input.GetKey(KeyCode.Space))
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
    return;
  }

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

  void PrintDebugLog()
  {
    logEntry += keyPress;
    // logEntry += ", " + transform.right;// changing
    // logEntry += ", " + transform.right;// changing
    // logEntry += ", " + transform.forward;// constant

    // logEntry += ", " + shipSpeed*transform.right;
    // logEntry += ", " + Time.realtimeSinceStartup;

    // logEntry += ", Vx = " + shipVelocity.x;
    // logEntry += ", Vy = " + shipVelocity.y;

    // slope = shipVelocity.x/shipVelocity.y;
    // logEntry += ", m = " + slope;
    // logEntry += "(" + dtheta + " --> " + rtheta + " --> " + slope + ")";

    // logEntry += "Screen.width = " + Screen.width;
    // logEntry += ", Screen.height = " + Screen.height;

    // logEntry += ", W_R = " + W_R;
    // logEntry += ", W_L = " + W_L;
    // logEntry += ", W_T = " + W_T;
    // logEntry += ", W_B = " + W_B;

    // logEntry += ", xy_exit = " + xy_exit;
    // logEntry += ", xy_entrance = " + xy_entrance;

    Debug.Log(logEntry);
    return;
  }//PrintDebugLog.end

}

