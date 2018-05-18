using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Events;
public class UIBehaviour : MonoBehaviour {

    private void Awake()
    {
        UIManager.Instance.RegisterGameObject(name, gameObject);
    }

    public void AddButtonLister(UnityAction action)
    {
        if(null != action)
        {
            Button btn = transform.GetComponent<Button>();
            btn.onClick.AddListener(action);
        }
    }

    public void RemoveButtonLister(UnityAction action)
    {
        if (null != action)
        {
            Button btn = transform.GetComponent<Button>();
            btn.onClick.RemoveListener(action);
        }
    }

    public void AddToogleLister(UnityAction<bool> action)
    {
        if (null != action)
        {
            Toggle toogle = transform.GetComponent<Toggle>();
            toogle.onValueChanged.AddListener(action);
        }
    }

    public void RemoveToogleLister(UnityAction<bool> action)
    {
        if (null != action)
        {
            Toggle toogle = transform.GetComponent<Toggle>();
            toogle.onValueChanged.RemoveListener(action);
        }
    }

    public void AddSlideLister(UnityAction<float> action)
    {
        if (null != action)
        {
            Slider slide = transform.GetComponent<Slider>();
            slide.onValueChanged.AddListener(action);
        }
    }

    public void RemoveSlideLister(UnityAction<float> action)
    {
        if (null != action)
        {
            Slider slide = transform.GetComponent<Slider>();
            slide.onValueChanged.RemoveListener(action);
        }
    }


    public void AddInputLister(UnityAction<string> action)
    {
        if (null != action)
        {
            InputField input = transform.GetComponent<InputField>();
            input.onValueChanged.AddListener(action);
        }
    }

    public void RemoveInputLister(UnityAction<string> action)
    {
        if (null != action)
        {
            InputField input = transform.GetComponent<InputField>();
            input.onValueChanged.RemoveListener(action);
        }
    }
}
