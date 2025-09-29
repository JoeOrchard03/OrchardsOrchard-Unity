using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Weather : MonoBehaviour
{
    public GameObject windyLeaves;
    public GameObject rain;

    public float minWeatherStateDuration = 1f;
    public float maxWeatherStateDuration = 3f;
    
    // Start is called before the first frame update
    void Start()
    {
        windyLeaves.SetActive(false);
        rain.SetActive(false);
        StartCoroutine(WeatherChange());
    }

    private IEnumerator WeatherChange()
    {
        yield return new WaitForSeconds(Random.Range(minWeatherStateDuration, maxWeatherStateDuration));
        int random = Random.Range(0, 2);
        if (windyLeaves.activeSelf)
        {
            windyLeaves.SetActive(false);
            if (random == 1)
            {
                rain.SetActive(true);
            }
        }
        else if (rain.activeSelf)
        {
            rain.SetActive(false);
            if (random == 1)
            {
                windyLeaves.SetActive(true);
            }
        }
        else
        {
            if (random == 1)
            {
                rain.SetActive(true);
                windyLeaves.SetActive(false);
            }
            else if (random == 0)
            {
                rain.SetActive(false);
                windyLeaves.SetActive(true);
            }
        }
        StartCoroutine(WeatherChange());
    }
}
