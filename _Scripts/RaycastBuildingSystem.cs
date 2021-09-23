using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class BuildCraft
{
    public GameObject go_prefab;
    public GameObject go_PreviewPrefab;
}
public class RaycastBuildingSystem : MonoBehaviour
{
    public GameObject ObjToMove;
    public GameObject ObjToPlace;

    public BuildCraft[] GO;

    public QuantumInventory QI;
    public BuildingBase[] BB;

    public bool isPrefab = false;
    bool isActive = false; 

    public LayerMask mask;
    float lastPosX, lastPosY, lastPosZ;
    Vector3 mousePos;

    Camera myCam;
    public GameObject player;
    PhotonView pv;
    QuantumInventory inven;
    bool onBuild = false;

    GameObject canvas;
    GameObject buildScreenBase;
    public GameObject buildScreen;

    public  GameObject turnback;

    private void Start()
    {
        Invoke("BD_Find", 2);
    }

    private int FindIndex(string name)
    {
        int _index = 0;
        for (int i = 0; i < BB.Length; i++)
        {
            if (BB[i].item == name)  _index = i;
        }

        return _index;
    }
    private void BD_Find()
    {
        QI = FindObjectOfType<QuantumInventory>();

        for (int i = 0; i < GO.Length; i++)
        {
            BB[i] = GO[i].go_prefab.GetComponent<BuildingBase>();
        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        pv = GetComponent<PhotonView>();
        myCam = GetComponentInChildren<Camera>();
        inven = GetComponent<QuantumInventory>();
        canvas = GameObject.Find("Canvas");

        if (pv.IsMine)
        {
            player = this.gameObject;



            buildScreenBase = Resources.Load<GameObject>("Core/QIS/_build");
            buildScreen = Instantiate<GameObject>(buildScreenBase, canvas.transform);
            buildScreen.SetActive(false);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (pv.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.G) && !isPrefab)
            {
                BuildScreen();
            }

            if (isPrefab == true)
            {
                mousePos = Input.mousePosition;
                Ray ray = myCam.ScreenPointToRay(mousePos);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 20f, mask))
                {
                    float PosX = hit.point.x;
                    float PosY = hit.point.y;
                    float PosZ = hit.point.z;

                    if (PosX != lastPosX || PosZ != lastPosZ || PosY != lastPosY)
                    {
                        lastPosX = PosX;
                        lastPosY = PosY;
                        lastPosZ = PosZ;
                        ObjToMove.transform.position = new Vector3(PosX, PosY + .5f, PosZ);
                    }
                    if(Input.GetMouseButtonDown(0))
                    {
                        Build();
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cancel();
            }
        }
    }

    private void Cancel()
    {
        QI.backupItem.quantity = QI.useItem.quantity;
        if (isPrefab)
        {
            if (QI.FindSlot(QI.useItem.item) == null)
            {
                QI.Gather(QI.backupItem);
            }
            else
            {
                if (QI.FindItem(QI.useItem.item))
                {
                    QI.FindSlot(QI.useItem.item).quantity += QI.useItem.quantity;
                }
            }
            Destroy(ObjToMove);
        }
        isActive = false;
        isPrefab = false;
        ObjToMove = null;
        ObjToPlace = null;
        buildScreen.SetActive(false);
        QI.useItem = null;
    }
    void Build()
    {
        if (isPrefab && ObjToMove.name != null)
        {
            PhotonNetwork.Instantiate(ObjToPlace.name, ObjToMove.transform.position, Quaternion.identity);
            Destroy(ObjToMove);
            isActive = false;
            isPrefab = false;
            ObjToMove = null;
            ObjToPlace = null;
           

        }
    }

    void BuildScreen()
    {
        if (!isActive)
        {
            OpenScreen();
        }
        else
            CloseScreen();
    }
    private void OpenScreen()
    {
        isActive = true;
        buildScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void CloseScreen()
    {
        isActive = false;
        buildScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
