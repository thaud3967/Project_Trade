using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class Item
{
    public int id;
    public string name;
    public int price;
    public string desc;
}

// JSON의 껍데기(Wrapper)와 짝을 맞출 클래스
[System.Serializable]
public class ItemDataWrapper
{
    public List<Item> items;
}

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    public List<Item> allItems;

    void Awake()
    {
        Instance = this;
        LoadItems();
    }

    void LoadItems()
    {
        // 경로: Assets/Resources/items
        TextAsset jsonFile = Resources.Load<TextAsset>("items");

        if (jsonFile != null)
        {
            ItemDataWrapper wrapper = JsonUtility.FromJson<ItemDataWrapper>(jsonFile.text);
            allItems = wrapper.items;
            Debug.Log("Resources 폴더에서 로드 성공!");
        }
        else
        {
            Debug.LogError("Resources 폴더에 items 파일이 없습니다!");
        }
    }
}