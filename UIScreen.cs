using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// Uma "tela" do menu (principal, configurações, pausa...). Layout feito por código.
public class UIScreen : MonoBehaviour
{
    public RectTransform Content { get; private set; }
    public Text Title { get; private set; }
    public GameObject FirstSelected { get; set; }
    public event Action Hidden;

    GameObject lastSelected;

    public static UIScreen Create(Transform parent, string name, string title, int titleSize = 96, string subtitle = null)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        UIFactory.Stretch(go.GetComponent<RectTransform>());
        var screen = go.AddComponent<UIScreen>();

        var cgo = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        var rt = cgo.GetComponent<RectTransform>();
        rt.SetParent(go.transform, false);
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(780f, 0f);

        var v = cgo.GetComponent<VerticalLayoutGroup>();
        v.spacing = 16f;
        v.childAlignment = TextAnchor.UpperCenter;
        v.childControlWidth = true;
        v.childControlHeight = true;
        v.childForceExpandWidth = true;
        v.childForceExpandHeight = false;
        cgo.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        screen.Content = rt;
        screen.Title = UIFactory.CreateText(rt, title, titleSize, UIFactory.BloodBright, TextAnchor.MiddleCenter, FontStyle.Bold);
        if (!string.IsNullOrEmpty(subtitle))
            UIFactory.CreateText(rt, subtitle, 30, UIFactory.Dim, TextAnchor.MiddleCenter, FontStyle.Italic);

        go.SetActive(false);
        return screen;
    }

    public Button AddButton(string label, UnityAction onClick)
    {
        var b = UIFactory.CreateButton(Content, label, onClick);
        if (FirstSelected == null) FirstSelected = b.gameObject;
        return b;
    }

    public Button AddToggle(string label, bool initial, UnityAction<bool> onChanged)
    {
        var b = UIFactory.CreateToggle(Content, label, initial, onChanged);
        if (FirstSelected == null) FirstSelected = b.gameObject;
        return b;
    }

    public Slider AddSlider(string label, float min, float max, float value, UnityAction<float> onChanged)
    {
        var s = UIFactory.CreateSlider(Content, label, min, max, value, onChanged);
        if (FirstSelected == null) FirstSelected = s.gameObject;
        return s;
    }

    public void AddSpacer(float height) => UIFactory.CreateSpacer(Content, height);

    public void Show()
    {
        gameObject.SetActive(true);
        var es = EventSystem.current;
        var target = lastSelected != null ? lastSelected : FirstSelected;
        if (es != null && target != null) es.SetSelectedGameObject(target);
    }

    public void Hide()
    {
        var es = EventSystem.current;
        if (es != null)
        {
            var sel = es.currentSelectedGameObject;
            if (sel != null && sel.transform.IsChildOf(transform)) lastSelected = sel;
        }
        gameObject.SetActive(false);
        Hidden?.Invoke();
    }
}
