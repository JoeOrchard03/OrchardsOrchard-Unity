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

            WeatherType newWeather = currentWeather;

            // Choose valid next state
            if (currentWeather == WeatherType.None)
            {
                // From None: any weather is allowed
                newWeather = (WeatherType)Random.Range(0, 3);
            }
            else
            {
                // From Rain or Wind: MUST go to None
                newWeather = WeatherType.None;
            }

            // If the roll is same as previous, skip
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
}
