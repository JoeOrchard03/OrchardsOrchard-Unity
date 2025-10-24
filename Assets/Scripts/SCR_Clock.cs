using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SCR_Clock : MonoBehaviour, INT_Interactable
{
    public bool isDay;
    
    public SpriteRenderer BackgroundSpriteRenderer;
    public SpriteRenderer ClockSpriteRenderer;
    public Light2D LightRef;
    public Sprite dayBackgroundSprite;
    public Sprite nightBackgroundSprite;

    public Sprite dayClockSprite;
    public Sprite nightClockSprite;
    
    public Color dayLightColor;
    public Color nightLightColor;
    public float nightLightIntensity;

    private AudioSource audioSource;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    public void Interact(GameObject interactor)
    {
        if (isDay)
        {
            SetNight();
        }
        else
        {
            SetDay();
        }
    }

    public void SetDay(bool playAudio = true)
    {
        if (playAudio)
        {
            audioSource.Play();
        }
        isDay = true;
        LightRef.color = dayLightColor;
        LightRef.intensity = 1.0f;
        BackgroundSpriteRenderer.sprite = dayBackgroundSprite;
        ClockSpriteRenderer.sprite = dayClockSprite;
        SaveTime();
    }

    public void SetNight(bool playAudio = true)
    {
        if (playAudio)
        {
            audioSource.Play();
        }
        isDay = false;
        LightRef.color = nightLightColor;
        LightRef.intensity = nightLightIntensity;
        BackgroundSpriteRenderer.sprite = nightBackgroundSprite;
        ClockSpriteRenderer.sprite = nightClockSprite;
        SaveTime();
    }

    private void SaveTime()
    {
        SCR_SaveData data = SCR_SaveSystem.LoadGame();
        data.isDay = isDay;
        SCR_SaveSystem.SaveGame(data);
    }
}
