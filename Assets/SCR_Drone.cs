using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Drone : MonoBehaviour
{
    [SerializeField] private float droneDriveSpeed = 5f;
    [SerializeField] private float armExtendSpeed = 2f;
    public float harvestTime = 0.25f;

    private Vector3 chargerPosition;
    private Vector3 originalArmPosition;
    private Vector3 armInventoryPosition;
    private Coroutine returnCoroutine;
    private SCR_Interact playerInventory;
    
    public GameObject armAnchor;
    public GameObject droneArm;
    public SpriteRenderer fruitRenderer;
    
    //Que of fruit transforms that have been clicked on for harvest
    private Queue<SCR_FruitBloom> fruitQueue = new Queue<SCR_FruitBloom>();
    
    //Picked up fruits
    private Dictionary<FruitType, int> droneInventory = new Dictionary<FruitType, int>();
    
    private SCR_FruitBloom currentFruit;
    private bool busy = false;

    private void Start()
    {
        playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<SCR_Interact>();
        //Set spawn pos as the charger's pos
        chargerPosition = transform.position;
        //Store the original arm location
        originalArmPosition = droneArm.transform.localPosition;
        //Sets the location the arm should go to when pulling into drone inventory
        armInventoryPosition = new Vector3(originalArmPosition.x, originalArmPosition.y - 1.0f, originalArmPosition.z);
    }

    public void SetTarget(SCR_FruitBloom fruitTransform)
    {
        //Sanity check
        if (fruitTransform == null) return;

        //Add the target fruit to the que
        fruitQueue.Enqueue(fruitTransform);

        //Stops the return coroutine if a new fruit is pressed for harvest
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }
        
        //Makes sure the harvest cycle is not ran multiple times at once, instead a que handles this
        if (!busy)
        {
            StartCoroutine(HarvestCycle());
        }
    }

    private IEnumerator HarvestCycle()
    {
        //Mark coroutine as running and drone as busy
        busy = true;

        // Process fruits until the queue is empty
        while (fruitQueue.Count > 0)
        {
            //Remove the current fruit from the que so it is not handled twice
            currentFruit = fruitQueue.Dequeue();
            //Sanity check
            if (currentFruit == null) continue;

            Transform targetTransform = currentFruit.transform;
            
            //Move drone under the target fruit
            yield return MoveUnderFruit(targetTransform);
            //Extend arm to the fruit
            yield return ExtendArm(targetTransform);
            //Grab fruit
            yield return GrabFruit(currentFruit.gameObject.GetComponent<SpriteRenderer>());
            //Returns the fruit to the inventory and makes the sprite disappear
            yield return RetractToInventory();
            //Returns the arm to its retracted location
            yield return ReturnArmToStart();
        }

        //Return to the charger once the que is empty and the harvest cycle is done
        returnCoroutine = StartCoroutine(ReturnToCharger());
        //Marks drone as idle
        busy = false;
    }

    private IEnumerator MoveUnderFruit(Transform target)
    {
        //While the drone is not almost under the fruit
        while (Mathf.Abs(transform.position.x - target.position.x) > 0.05f)
        {
            //Move towards the fruit
            transform.position = Vector2.MoveTowards(
                transform.position,
                new Vector2(target.position.x, transform.position.y),
                droneDriveSpeed * Time.deltaTime
            );
            yield return null;
        }
    }

    private IEnumerator ExtendArm(Transform target)
    {
        // Calculate target arm extension length
        float armHeight = droneArm.GetComponent<SpriteRenderer>().bounds.size.y;
        Vector3 localTargetPos = armAnchor.transform.InverseTransformPoint(target.position);
        //Sets the target lenght to the pos of the fruit - a small offset - the arm height as the pivot of the arm is at the bottom of it
        float targetLength = localTargetPos.y - 0.1f - armHeight;

        // While the drone arm is not high enough
        while (droneArm.transform.localPosition.y < targetLength)
        {
            //Raise arm
            Vector3 pos = droneArm.transform.localPosition;
            pos.y += armExtendSpeed * Time.deltaTime;
            if (pos.y > targetLength) pos.y = targetLength;
            droneArm.transform.localPosition = pos;
            yield return null;
        }
    }

    private IEnumerator GrabFruit(SpriteRenderer fruitSprite)
    {
        //Sanity check
        if (currentFruit != null)
        {
            //If the drone does not have atleast one of the harvested fruit already
            if(!droneInventory.ContainsKey(currentFruit.fruitType))
            {
                //Add it to the dictionary
                droneInventory[currentFruit.fruitType] = 0;
            }
            
            //Increment the fruit type
            droneInventory[currentFruit.fruitType]++;
            
            //Sets the sprite renderer for the held fruit to be the sprite of the fruit grabbed
            fruitRenderer.sprite = fruitSprite.sprite;
            //Destroys the fruit that was just grabbed
            Destroy(currentFruit.gameObject);
        }
        //Small delay for harvest time
        yield return new WaitForSeconds(harvestTime);
    }

    private IEnumerator RetractToInventory()
    {
        // While drone is above the armInventoryPosition
        while (droneArm.transform.localPosition.y > armInventoryPosition.y)
        {
            //Lower arm towards inventory
            Vector3 pos = droneArm.transform.localPosition;
            pos.y -= armExtendSpeed * Time.deltaTime;
            if (pos.y < armInventoryPosition.y) pos.y = armInventoryPosition.y;
            droneArm.transform.localPosition = pos;
            yield return null;
        }
        
        // Clear sprite from arm (putting in inventory)
        fruitRenderer.sprite = null;
        Debug.Log("Harvested: " + currentFruit.fruitType);
    }

    private IEnumerator ReturnArmToStart()
    {
        // While arm is lower then target pos
        while (droneArm.transform.localPosition.y < originalArmPosition.y)
        {
            //Raise arm
            Vector3 pos = droneArm.transform.localPosition;
            pos.y += armExtendSpeed * Time.deltaTime;
            if (pos.y > originalArmPosition.y) pos.y = originalArmPosition.y;
            droneArm.transform.localPosition = pos;
            yield return null;
        }
    }

    //Return drone to charger after harvest que is cleared
    private IEnumerator ReturnToCharger()
    {
        // While drone is not close to charger
        while (Mathf.Abs(transform.position.x - chargerPosition.x) > 0.03f)
        {
            //Move towards charger
            transform.position = Vector2.MoveTowards(
                transform.position,
                new Vector2(chargerPosition.x, transform.position.y),
                droneDriveSpeed * Time.deltaTime
            );
            yield return null;
        }

        playerInventory.AddFruits(droneInventory);

        droneInventory.Clear();
    }
}
