using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameObject AsteroidPrefab;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(AsteroidPrefab, new Vector3(-2, -2, 0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
