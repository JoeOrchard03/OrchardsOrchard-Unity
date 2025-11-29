using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Weather : MonoBehaviour
{
    public GameObject windyLeaves;
    public GameObject rain;

    public float minWeatherStateDuration = 1f;
    public float maxWeatherStateDuration = 3f;

    public AudioSource rainAudioSource;
    public AudioSource windAudioSource;
    
    public enum WeatherType{None, Rain, Wind}
    private WeatherType currentWeather  = WeatherType.None;
    
    // Start is called before the first frame update
    void Start()
    {
        windyLeaves.SetActive(false);
        rain.SetActive(false);
        StartCoroutine(WeatherChange());
    }

    IEnumerator WeatherChange()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWeatherStateDuration, maxWeatherStateDuration));

            WeatherType newWeather = (WeatherType)Random.Range(0, 3);

            if (newWeather == currentWeather) 
                continue;
            
            ApplyWeather(newWeather);
        }
    }

    void ApplyWeather(WeatherType type)
    {
        currentWeather = type;
        
        bool rainState = type == WeatherType.Rain;
        bool windState = type == WeatherType.Wind;
        
        rain.SetActive(rainState);
        windyLeaves.SetActive(windState);
        
        if (rainState) rainAudioSource.Play(); else rainAudioSource.Stop();
        if (windState) windAudioSource.Play(); else windAudioSource.Stop();
    }
    
    // private IEnumerator WeatherChange()
    // {
    //     yield return new WaitForSeconds(Random.Range(minWeatherStateDuration, maxWeatherStateDuration));
    //     int random = Random.Range(0, 2);
    //     if (windyLeaves.activeSelf)
    //     {
    //         windyLeaves.SetActive(false);
    //         windAudioSource.Stop();
    //         if (random == 1)
    //         {
    //             rainAudioSource.Play();
    //             rain.SetActive(true);
    //         }
    //     }
    //     else if (rain.activeSelf)
    //     {
    //         rainAudioSource.Stop();
    //         rain.SetActive(false);
    //         if (random == 1)
    //         {
    //             windAudioSource.Play();
    //             windyLeaves.SetActive(true);
    //         }
    //     }
    //     else
    //     {
    //         if (random == 1)
    //         {
    //             rainAudioSource.Play();
    //             rain.SetActive(true);
    //             windyLeaves.SetActive(false);
    //         }
    //         else if (random == 0)
    //         {
    //             windAudioSource.Play();
    //             rain.SetActive(false);
    //             windyLeaves.SetActive(true);
    //         }
    //     }
    //     StartCoroutine(WeatherChange());
    // }
}
