using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SCR_IntroManager : MonoBehaviour
{
    public List<GameObject> frameList = new List<GameObject>();
    public int currentFrame = 0;

    private void Awake()
    {
        foreach (Transform child in transform)
        {
            frameList.Add(child.gameObject);
        }

        currentFrame = 0;
        frameList[currentFrame].SetActive(true);
    }

    public void NextFrame()
    {
        ChangeFrame(1);
    }

    public void PreviousFrame()
    {
        ChangeFrame(-1);
    }

    public void ChangeFrame(int frameChange)
    {
        frameList[currentFrame].SetActive(false);
        currentFrame = currentFrame + frameChange;
        if (currentFrame >= frameList.Count)
        {
            LoadNextScene();
            Debug.Log("Triggering LoadNextScene");
            return;
        }

        if (currentFrame < 0)
        {
            currentFrame = 0;
        }
        
        frameList[currentFrame].SetActive(true);
    }

    public void LoadNextScene()
    {
        Debug.Log("Loading next scene");
        SceneManager.LoadScene("MainScene");
    }
    
    public void Skip()
    {
        Debug.Log("Skip");
        LoadNextScene();
    }
}
