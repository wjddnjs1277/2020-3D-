using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusMark : MonoBehaviour
{
    GameObject parentTr;
    // Start is called before the first frame update
    void Start()
    {
        parentTr = this.transform.parent.gameObject;
        parentTr.GetComponent<DropWood>().createBonus = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Axe"))
        {
            parentTr.GetComponent<DropWood>().bonusCount++;
            Debug.Log("BonusHit");
            parentTr.GetComponent<DropWood>().createBonus = true;
            Destroy(this.gameObject);
        }
    }
}
