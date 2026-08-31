using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Search;
using UnityEngine;

public class LevelUp : MonoBehaviour
{
    RectTransform rect;
    Item[] items;
    Item[] consumeItems;
    
    void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true); // true면 비활성화 된 item 컴포넌트도 가져온다.
        consumeItems = items.Where(x => x.isConsumable).ToArray();

    }

    private void Start()
    {
        GameManager.Instance.weaponPool = items.Where(x => x.data.itemType == ItemData.ItemType.Melee || x.data.itemType == ItemData.ItemType.Range).Select(x => x.data.itemId).ToList();

        GameManager.Instance.passivePool = items.Where(x => x.data.itemType == ItemData.ItemType.Passive).Select(x => x.data.itemId).ToList();
    }

    public void Show()
    {
        Next();
        rect.localScale = Vector3.one;
        GameManager.Instance.Stop();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
        AudioManager.instance.EffectBgm(true);
    }

    public void Hide()
    {
        rect.localScale = Vector3.zero;
        GameManager.Instance.Resume();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        AudioManager.instance.EffectBgm(false);
    }

    public void Select(int index)
    {
        items[index].OnClick();
    }

    void Next()
    {
        //1. 모든 아이템 비활성화
        foreach(Item item in items)
        {
            item.gameObject.SetActive(false); //item 컴포넌트의 게임오브젝트로 넘어가 비활성화
        }
        List<int> selectList = new List<int>();
        selectList.AddRange(GameManager.Instance.weaponPool);
        selectList.AddRange(GameManager.Instance.passivePool);

        List<int> randomList = new List<int>(selectList);

        for(int i=0;i<3 ; i++)
        {
            int ranIndex = Random.Range(0, randomList.Count);
            int selectItemId = randomList[ranIndex];

            randomList.RemoveAt(ranIndex);
            Item ranItem = items.First(x => x.data.itemId == selectItemId);
            if (ranItem.level-1 == ranItem.data.damages.Length)
            {
                //consumeItems[SelectRanItem(consumeItems)].gameObject.SetActive(true);
            }
            else
            {
                ranItem.gameObject.SetActive(true);
            }

        }
    }
    int SelectRanItem(Item[] itemArray)
    {
        int index = Random.Range(0, itemArray.Length);
        return index;
    }

}

