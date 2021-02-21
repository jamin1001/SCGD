using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipExaust : MonoBehaviour
{
    int nframe = 0;
    int duration = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      nframe++;
      if (nframe % duration == 0)
      {
        this.GetComponent<Renderer>().enabled = false;
      }
      else if (nframe % duration == 3)
      {
        this.GetComponent<Renderer>().enabled = true;
      }

      return;
    }
}
