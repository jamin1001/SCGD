using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Prize : MonoBehaviour
{
    public float _scaleSpeed = 10.0f;

    AudioSource _collectSound;
    float _scale = 0.0f;
    
    public void CollectPrize()
    {
        _collectSound = GetComponent<AudioSource>();
        _collectSound.PlayScheduled(0);
        _scale = 1.0f;

        Invoke("KillMe", 3.0f);
    }

    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(0.2f);

        // Code to execute after the delay
    }

    void KillMe()
    {
        Destroy(this.gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_scale > 0)
        {
            transform.localScale = new Vector3(_scale, _scale, _scale);
            _scale = Mathf.Lerp(_scale, 0, _scaleSpeed * Time.deltaTime);
        }
    }
}
