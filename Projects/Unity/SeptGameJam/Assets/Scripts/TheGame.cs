using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;

public class TheGame : MonoBehaviour
{
 
  public GameObject AsteroidPreFab;

  // Start is called before the first frame update
  void Start()
  {
    for (int i=0; i <= 5; i++)
    {
      Instantiate(AsteroidPreFab);
    }
  }

  // Update is called once per frame
  void Update()
  {

  }
}
