using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Drone : MonoBehaviour
{
    [SerializeField] private float droneDriveSpeed = 5f;
    [SerializeField] private float armExtendSpeed = 2f;

    private Vector3 chargerPosition;
    
    private Vector3 originalArmPosition;
    private Vector3 armInventoryPosition;
    
    public GameObject armAnchor;
    public GameObject droneArm;

    public float harvestTime = 0.25f;
    public SpriteRenderer fruitRenderer;

    public List<Transform> targetFruitTransforms =  new List<Transform>();
    public Transform targetFruitTransform;
    private float targetLength;
    
    public List<Transform> fruitTransforms;
    
    private bool movingToFruit = false;
    private bool extending = false;
    private bool retracting = false;
    private bool returningArmPosition = false;
    private bool returningToCharger = false;
    private bool idle = true;

    private void Start()
    {
        chargerPosition = transform.position;
        originalArmPosition = droneArm.transform.localPosition;
        armInventoryPosition = new Vector3(originalArmPosition.x, originalArmPosition.y - 1.0f, originalArmPosition.z);
    }

    public void SetTarget(Transform fruitTransform)
    {
        targetFruitTransforms.Add(fruitTransform);

        if (idle)
        {
            AssignNextTarget();
        }
    }

    private void MoveUnderFruit()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(targetFruitTransform.position.x, transform.position.y), droneDriveSpeed * Time.deltaTime);
        float distanceFromUnderFruit = Mathf.Abs(transform.position.x - targetFruitTransform.position.x);
        if (distanceFromUnderFruit < 0.05f)
        {
            Debug.Log("Drone has reached fruit, extending arm");
            movingToFruit = false;
            extending = true;
        }
    }

    private void AssignNextTarget()
    {
        while (targetFruitTransforms.Count > 0)
        {
            if (targetFruitTransforms[0] == null)
            {
                targetFruitTransforms.RemoveAt(0);
            }
            
            targetFruitTransform = targetFruitTransforms[0];
            
            float armHeight = droneArm.GetComponent<SpriteRenderer>().bounds.size.y;
            Vector3 localTargetPos = armAnchor.transform.InverseTransformPoint(targetFruitTransform.position);
            targetLength = localTargetPos.y - 0.1f - armHeight;
            
            movingToFruit = true;
            return;
        }
        returningToCharger = true;
    }
    
    private void Update()
    {
        if (movingToFruit)
        {
            MoveUnderFruit();
        }
        
        if (extending)
        {
            ExtendArm();
        }
        
        if (retracting)
        {
            RetractArm();
        }

        if (returningArmPosition)
        {
            ReturnArmToRegularPos();
        }

        if (returningToCharger)
        {
            ReturnToCharger();
        }

        if (!movingToFruit && !extending && !retracting && !returningArmPosition)
        {
            idle = true;
            return;
        }
        idle = false;
    }

    private void ExtendArm()
    {
        Vector3 pos = droneArm.transform.localPosition;
        if (pos.y < targetLength)
        {
            pos.y += armExtendSpeed * Time.deltaTime;
            droneArm.transform.localPosition = pos;
        }
        else
        {
            extending = false;
            StartCoroutine(HarvestDelay());
        }
    }

    IEnumerator HarvestDelay()
    {
        if (targetFruitTransform != null)
        {
            fruitRenderer.sprite = targetFruitTransform.GetComponent<SpriteRenderer>().sprite;
            Destroy(targetFruitTransform.gameObject);
        }
        yield return new WaitForSeconds(harvestTime);
        retracting = true;
    }
    
    private void RetractArm()
    {
        Vector3 pos = droneArm.transform.localPosition;
        pos.y -= armExtendSpeed * Time.deltaTime;

        if (pos.y <= armInventoryPosition.y)
        {
            pos.y = armInventoryPosition.y; // clamp
            retracting = false;
            returningArmPosition = true;
        }

        droneArm.transform.localPosition = pos;
    }
    
    // private void RetractArm()
    // {
    //     Vector3 pos = droneArm.transform.localPosition;
    //     if (pos.y > armInventoryPosition.y)
    //     {
    //         pos.y -= armExtendSpeed * Time.deltaTime;
    //         droneArm.transform.localPosition = pos;
    //     }
    //     else
    //     {
    //         retracting = false;
    //         returningArmPosition = true;
    //     }
    // }

    private void ReturnArmToRegularPos()
    {
        fruitRenderer.sprite = null;
        Vector3 pos = droneArm.transform.localPosition;
        if (pos.y < originalArmPosition.y)
        {
            pos.y += armExtendSpeed * Time.deltaTime;
            droneArm.transform.localPosition = pos;
        }
        else
        {
            returningArmPosition = false;
            
            if (targetFruitTransforms.Count > 0)
            {
                targetFruitTransforms.RemoveAt(0);
            }
            
            AssignNextTarget();
        }
    }

    private void ReturnToCharger()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(chargerPosition.x, transform.position.y), droneDriveSpeed * Time.deltaTime);
        float distanceFromCharger = Mathf.Abs(transform.position.x - chargerPosition.x);
        if (distanceFromCharger < 0.03f)
        {
            Debug.Log("Drone has reached charger, resting");
            returningToCharger = false;
        }
    }
}
