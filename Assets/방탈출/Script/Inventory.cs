using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public List<Item> item_List;
    //public GameObject LargeHint;  //힌트 크게보이게하는 패널
    [SerializeField]
    public List<Item> items;
    public Image image;
    [SerializeField]
    private Transform slotParent;
    [SerializeField]
    private Slot[] slots;

    public GameObject Flashlight;

    private void OnValidate() // 스크립트 로드나 인스펙터 창에서 값이 변경될 때 호출됨
    {
        slots = slotParent.GetComponentsInChildren<Slot>(); // Bag 오브젝트의 자식 오브젝트를 한 번에 넣는 코드
    }
    private static Inventory Instance;

    public static Inventory _Instance
    {
        get { return Instance; }
    }

    void Awake()
    {
        FreshSlot();
        Instance = this;
    }

    public void FreshSlot()
    {
        int i = 0;
        for (; i < items.Count && i < slots.Length; i++)
        {
            slots[i].item = items[i];
        }
        for (; i < slots.Length; i++)
        {
            slots[i].item = null;
        }
    }
    public void AddItem(Item _item)
    {
        print("AddItem 호출됨");
        if (items.Count < slots.Length)
        {
            items.Add(_item);
            FreshSlot();
        }
        else
        {
            print("슬롯이 가득 차 있습니다.");
        }
    }
    public void GetItem(string name)
    {
        for (int i = 0; i < item_List.Count; i++)
        {
            if (item_List[i].itemName == name)
            {
                AddItem(item_List[i]);
                break;
            }
        }
    }

    //public void ClickInfo()
    //{
    //    if()
    //}

    //#region 슬롯클릭이벤트
    //public void SlotClickEvent(GameObject item)
    //{
    //    LargeHint.SetActive(true);
    //    //슬롯을 클릭해서 아이템 사용 구현.... 은 못해서 아이템 클릭시 이미지 크게 보이게 구현함
    //    //for (int i = 0; i < item_List.Count; i++)
    //    //{
    //    //    if (item_List[i].name == item.name)
    //    //    {
    //    //        image.sprite = item_List[i].itemImage;
    //    //    }
    //    //}
    //}
    //#endregion
}