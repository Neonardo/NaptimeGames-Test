using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button Button50;
    [SerializeField] private Button Button100;
    [SerializeField] private Button Button250;
    [SerializeField] private Button Button500;
    

    [Header("Start Button")]
    [SerializeField] private Button StartButton;
    [SerializeField] private Image StartButtonPadPrompt;
    [SerializeField] private Sprite StartDisabledBackground;
    [SerializeField] private Sprite StartEnabledBackground;


    private EventSystem _eventSystem;

    private void Start()
    {
        _eventSystem = EventSystem.current;
    }

    private void Update()
    {
        // if user clicked on the empty space, reset all buttons' status
        if(Input.GetMouseButtonDown(0) && !IsPointerOverButton())
        {
            ResetScale();
            ToggleStartButton(false);
        }
    }

    public void On50Click()
    {
        ResetScale();
        Button50.GetComponent<RectTransform>().localScale = new Vector3(1.1f, 1.1f, 1.1f);
        Persistent.Instance.SetInfo(50);
        ToggleStartButton(true);
    }

    public void On100Click()
    {
        ResetScale();
        Button100.GetComponent<RectTransform>().localScale  = new Vector3(1.1f, 1.1f, 1.1f);
        Persistent.Instance.SetInfo(100);
        ToggleStartButton(true);
    }

    public void On250Click()
    {
        ResetScale();
        Button250.GetComponent<RectTransform>().localScale  = new Vector3(1.1f, 1.1f, 1.1f);
        Persistent.Instance.SetInfo(250);
        ToggleStartButton(true);
    }

    public void On500Click()
    {
        ResetScale();
        Button500.GetComponent<RectTransform>().localScale  = new Vector3(1.1f, 1.1f, 1.1f);
        Persistent.Instance.SetInfo(500);
        ToggleStartButton(true);
    }

    public void OnStartClick()
    {
        SceneManager.LoadScene("Main");
    }

    private void ToggleStartButton(bool enabled)
    {
        StartButtonPadPrompt.enabled = enabled;
        StartButton.interactable = enabled;

        if(enabled)
        {
            StartButton.image.sprite = StartEnabledBackground;
        }
        else{
            StartButton.image.sprite = StartDisabledBackground;
        }
    }

    private void ResetScale()
    {
        Button50.GetComponent<RectTransform>().localScale = Vector3.one;
        Button100.GetComponent<RectTransform>().localScale = Vector3.one;
        Button250.GetComponent<RectTransform>().localScale = Vector3.one;
        Button500.GetComponent<RectTransform>().localScale = Vector3.one;
    }

    private bool IsPointerOverButton()
    {
        PointerEventData eventData = new PointerEventData(_eventSystem);
        eventData.position = Input.mousePosition;
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject.GetComponent<Button>() != null)
                return true;
        }

        return false;
    }
}
