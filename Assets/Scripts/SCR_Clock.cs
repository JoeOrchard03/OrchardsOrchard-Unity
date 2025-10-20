using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SCR_Clock : MonoBehaviour, INT_Interactable
{
    public bool isDay;
    
    public SpriteRenderer BackgroundSpriteRenderer;
    public Light2D LightRef;
    public Sprite dayBackgroundSprite;
    public Sprite nightBackgroundSprite;
    
    public Color dayLightColor;
    public Color nightLightColor;

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

    public void SetDay()
    {
        isDay = true;
        LightRef.color = dayLightColor;
        BackgroundSpriteRenderer.sprite = dayBackgroundSprite;
    }

    public void SetNight()
    {
        isDay = false;
        LightRef.color = nightLightColor;
        BackgroundSpriteRenderer.sprite = nightBackgroundSprite;
    }
}
