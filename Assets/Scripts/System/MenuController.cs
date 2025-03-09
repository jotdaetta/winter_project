using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using System.Collections;

[Serializable]
public class Btn
{
    public Image img;
    public Btn up;
    public Btn down;
    public Btn enter;
    public Btn back;
    public EventTrigger.Entry enterTrigger;
    public EventTrigger.Entry backTrigger;
    public float delay;

    public void EnterTriggerActivate()
    {
        enterTrigger.callback.Invoke(new BaseEventData(EventSystem.current));
    }
    public void BackTriggerActivate()
    {
        backTrigger.callback.Invoke(new BaseEventData(EventSystem.current));
    }
}
[Serializable]
public class IntListWrapper  // ✅ 새로운 클래스 추가
{
    public int up;
    public int down;
    public int enter;
    public int back;
}

public class MenuController : MonoBehaviour
{
    public bool isWorking = true;
    public Btn[] buttons;
    public List<IntListWrapper> buttonTo;
    Btn curButton;
    bool KeyControlActive;
    readonly Color activateColor = new Color(1, 1, 1, 0.2f);
    readonly Color deactivateColor = new Color(0, 0, 0, 0.8f);
    void Start()
    {
        if (buttons.Length != 0)
        {
            curButton = buttons[0];
            SetButtonTo();
            KeyActvate();
        }
    }
    void SetButtonTo()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i != buttonTo[i].up)
                buttons[i].up = buttons[buttonTo[i].up];
            if (i != buttonTo[i].down)
                buttons[i].down = buttons[buttonTo[i].down];
            if (i != buttonTo[i].enter)
                buttons[i].enter = buttons[buttonTo[i].enter];
            if (i != buttonTo[i].back)
                buttons[i].back = buttons[buttonTo[i].back];
        }
    }
    void Update()
    {
        if (!isWorking) return;
        Input_VerticalAxis();
        Input_A_Button();
        Input_B_Button();
    }
    bool verticalInputBlocked = false;
    [SerializeField] Text verticalAxisTest;
    [SerializeField] float verticalAxisDead = 0.5f;
    void Input_VerticalAxis()
    {
        float vertical = Input.GetAxisRaw("Vertical");
        verticalAxisTest.text = $"Vertical Axis : {vertical}";
        if (Mathf.Abs(vertical) < verticalAxisDead)
        {
            verticalInputBlocked = false;
            return;
        }
        if (verticalInputBlocked) return;
        verticalInputBlocked = true;
        // KeyActvate();
        verticalAxisTest.text = $"Vertical Axis : {vertical}";
        print("Axis");
        if (vertical > 0 && curButton.up != null)
            ButtonAndImageColorChange(curButton.up);
        else if (vertical < 0 && curButton.down != null)
            ButtonAndImageColorChange(curButton.down);
    }
    bool onDelay;
    void Input_A_Button()
    {
        if (!(Input.GetButtonDown("A") || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.A)) || onDelay) return;
        print("A");
        // KeyActvate();
        if (curButton.enterTrigger != null)
        {
            StartCoroutine(ButtonDelay(curButton.delay));
            curButton.EnterTriggerActivate();
        }
        if (curButton.enter != null)
            ButtonAndImageColorChange(curButton.enter);
    }
    void Input_B_Button()
    {
        if (!(Input.GetButtonDown("B") || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.B)) || onDelay) return;
        print("B");
        // KeyActvate();
        if (curButton.backTrigger != null)
        {
            StartCoroutine(ButtonDelay(curButton.delay));
            curButton.BackTriggerActivate();
        }
        if (curButton.back != null)
            ButtonAndImageColorChange(curButton.back);
    }
    IEnumerator ButtonDelay(float delay)
    {
        onDelay = true;
        yield return new WaitForSeconds(delay);
        onDelay = false;
    }
    void ButtonAndImageColorChange(Btn toButton)
    {
        curButtonImageControl(false);
        curButton = toButton;
        curButtonImageControl(true);
    }
    void curButtonImageControl(bool isActive)
    {
        curButton.img.color = isActive ? activateColor : deactivateColor;
    }
    void KeyActvate()
    {
        if (KeyControlActive) return;
        KeyControlActive = true;
        curButtonImageControl(true);
    }
}
