using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorSymbolsUI : MonoBehaviour
{
    [SerializeField] List<Sprite> _symbolSlots = new List<Sprite>();
    [SerializeField] List<Button> _buttons = new List<Button>();
    int index = 0;

    private void Awake()
    {
        SetOnclickEventsForAllButtons();
    }

    private void SetOnclickEventsForAllButtons()
    {
        foreach (Button button in _buttons)
        {
            button.onClick.AddListener(() => SetSymbol(index, button.transform.GetChild(0).GetComponent<Image>().sprite));
        }
    }

    public void SetSymbol(int index, Sprite symbol)
    {
        _symbolSlots[index] = symbol;

        if (index < _buttons.Count - 1)
        {
            index++;
        }
        else
        {
            CheckCombination();
        }
    }

    private void CheckCombination()
    {
        throw new NotImplementedException();
    }
}
