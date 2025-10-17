using System.Collections;
using UnityEngine;

public class SCR_SaplingDeleter : MonoBehaviour
{
    public void SaveSapling(GameObject saplingOBJ)
    {
        Debug.Log("Trying to save saplings");
        StartCoroutine(SaveSaplings(saplingOBJ));
    }

    private IEnumerator SaveSaplings(GameObject saplingOBJ)
    {
        Debug.Log("Saving Saplings");
        Destroy(saplingOBJ);
        
        yield return new WaitForEndOfFrame();
        
        SCR_SaveData data = SCR_SaveSystem.LoadGame();
        data.saplings = SCR_SaveSystem.GetSaplingData(gameObject.transform);
        SCR_SaveSystem.SaveGame(data);
    }
}
