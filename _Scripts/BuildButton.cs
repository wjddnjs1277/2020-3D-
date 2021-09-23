using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildButton : MonoBehaviour
{
    RaycastBuildingSystem rb;
    QuantumInventory used_s;
    GameObject turn_back;
    private void Awake()
    {
        Invoke("find", 3f);
    }
    void find()
    {
        rb = GameObject.FindWithTag("Player").GetComponent<RaycastBuildingSystem>();
    }
    public void OnClickButton(int i)
    {
        
        if (rb.QI.FindSlot(rb.BB[i].item) != null && rb.QI.FindSlot(rb.BB[i].item).quantity >= rb.BB[i].quantity)
        {
            rb.ObjToMove = Instantiate(rb.GO[i].go_PreviewPrefab, rb.player.transform.position + rb.player.transform.forward, Quaternion.identity);
            rb.ObjToPlace = rb.GO[i].go_prefab;
            Cursor.visible = false;
            rb.isPrefab = true;
            rb.buildScreen.SetActive(false);
            rb.QI.useItem = rb.QI.FindSlot(rb.BB[i].item);
            //rb.QI.useItem.quantity = rb.BB[i].quantity;
            rb.QI.FindItemAndRemove(rb.QI.FindSlot(rb.BB[i].item).item, rb.BB[i].quantity);
        }
        else if (rb.QI.FindSlot(rb.BB[i].item) != null)
        {

            Debug.Log(rb.BB[i].item + "재료가 " + (rb.BB[i].quantity - rb.QI.FindSlot(rb.BB[i].item).quantity) + "개 부족합니다.");

        }
        else
        {
            Debug.Log("재료가 없습니다.");
        }
    }
    
}
