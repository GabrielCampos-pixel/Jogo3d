using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

/// Cria todos os elementos de UI por código, em tema escuro de terror.
public static class UIFactory
{
    public static readonly Color Bg = new Color(0.04f, 0.04f, 0.05f, 1f);
    public static readonly Color PanelColor = new Color(0.09f, 0.08f, 0.10f, 1f);
    public static readonly Color Blood = new Color(0.55f, 0.10f, 0.10f, 1f);
    public static readonly Color BloodBright = new Color(0.80f, 0.15f, 0.15f, 1f);
    public static readonly Color Bone = new Color(0.91f, 0.89f, 0.82f, 1f);
    public static readonly Color Dim = new Color(0.60f, 0.58f, 0.55f, 1f);

    static Font font;
    public static Font DefaultFont
    {
        get
        {
            if (font == null) font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return font;
        }
    }

    public static void Stretch(RectTransform r)
    {
        r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
        r.offsetMin = Vector2.zero; r.offsetMax = Vector2.zero;
    }

    static Image NewImage(string name, Transform parent, Color c)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var img = go.GetComponent<Image>();
        img.color = c;
        return img;
    }

    public static Canvas CreateCanvas(string name, int sortingOrder)
    {
        var go = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var c = go.GetComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;   // para VR depois: trocar para World Space
        c.sortingOrder = sortingOrder;
        var s = go.GetComponent<CanvasScaler>();
        s.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        s.referenceResolution = new Vector2(1920, 1080);
        s.matchWidthOrHeight = 0.5f;
        return c;
    }

    /// Garante um EventSystem usando o NOVO Input System (necessário para navegar com o controle).
    public static void EnsureEventSystem()
    {
        var es = Object.FindFirstObjectByType<EventSystem>();
        if (es == null) es = new GameObject("EventSystem", typeof(EventSystem)).GetComponent<EventSystem>();

        var old = es.GetComponent<StandaloneInputModule>();
        if (old != null) Object.DestroyImmediate(old);

        if (es.GetComponent<InputSystemUIInputModule>() == null)
        {
            var module = es.gameObject.AddComponent<InputSystemUIInputModule>();
            module.AssignDefaultActions();
        }
    }

    public static Text CreateText(Transform parent, string content, int size, Color color,
        TextAnchor anchor = TextAnchor.MiddleCenter, FontStyle style = FontStyle.Normal)
    {
        var go = new GameObject("Text", typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);
        var t = go.GetComponent<Text>();
        t.font = DefaultFont;
        t.text = content;
        t.fontSize = size;
        t.color = color;
        t.alignment = anchor;
        t.fontStyle = style;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.raycastTarget = false;
        return t;
    }

    public static void CreateSpacer(Transform parent, float height)
    {
        var go = new GameObject("Spacer", typeof(RectTransform), typeof(LayoutElement));
        go.transform.SetParent(parent, false);
        go.GetComponent<LayoutElement>().preferredHeight = height;
    }

    public static Image CreateBackground(Transform parent, float alpha)
    {
        var img = NewImage("Background", parent, new Color(Bg.r, Bg.g, Bg.b, alpha));
        Stretch(img.rectTransform);
        img.transform.SetAsFirstSibling();
        return img;
    }

    public static Image CreateMushroomIcon(Transform parent)
    {
        var img = NewImage("MushroomIcon", parent, Color.white);
        img.sprite = MushroomSprite();
        img.preserveAspect = true;
        img.raycastTarget = false;
        var r = img.rectTransform;
        r.anchorMin = r.anchorMax = r.pivot = new Vector2(1f, 0f);   // canto inferior direito
        r.anchoredPosition = new Vector2(-48f, 48f);
        r.sizeDelta = new Vector2(128f, 128f);
        return img;
    }

    public static Button CreateButton(Transform parent, string label, UnityAction onClick, float height = 72f)
    {
        var go = new GameObject(label + "Button", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
        go.transform.SetParent(parent, false);
        go.GetComponent<LayoutElement>().preferredHeight = height;

        var img = go.GetComponent<Image>();
        img.color = Color.white;

        var b = go.GetComponent<Button>();
        b.targetGraphic = img;
        var cb = b.colors;
        cb.normalColor = PanelColor;
        cb.highlightedColor = Blood;
        cb.selectedColor = Blood;        // destaque ao navegar com o controle
        cb.pressedColor = BloodBright;
        cb.fadeDuration = 0.08f;
        b.colors = cb;

        var outline = go.AddComponent<Outline>();
        outline.effectColor = Blood;
        outline.effectDistance = new Vector2(2f, -2f);

        var t = CreateText(go.transform, label, 34, Bone);
        Stretch(t.rectTransform);

        if (onClick != null) b.onClick.AddListener(onClick);
        return b;
    }

    /// Botão que alterna LIGADO/DESLIGADO.
    public static Button CreateToggle(Transform parent, string label, bool initial, UnityAction<bool> onChanged)
    {
        bool state = initial;
        Button b = null;
        string Fmt() => label + ": " + (state ? "LIGADO" : "DESLIGADO");
        b = CreateButton(parent, Fmt(), () =>
        {
            state = !state;
            b.GetComponentInChildren<Text>().text = Fmt();
            onChanged?.Invoke(state);
        });
        return b;
    }

    public static Slider CreateSlider(Transform parent, string label, float min, float max, float value, UnityAction<float> onChanged)
    {
        var row = new GameObject(label + "Row", typeof(RectTransform), typeof(LayoutElement));
        row.transform.SetParent(parent, false);
        row.GetComponent<LayoutElement>().preferredHeight = 80f;

        var lbl = CreateText(row.transform, label, 32, Bone, TextAnchor.MiddleLeft);
        var lr = lbl.rectTransform;
        lr.anchorMin = Vector2.zero; lr.anchorMax = new Vector2(0.38f, 1f);
        lr.offsetMin = Vector2.zero; lr.offsetMax = Vector2.zero;

        var sGo = new GameObject("Slider", typeof(RectTransform), typeof(Slider));
        sGo.transform.SetParent(row.transform, false);
        var sr = (RectTransform)sGo.transform;
        sr.anchorMin = new Vector2(0.4f, 0f); sr.anchorMax = new Vector2(1f, 1f);
        sr.offsetMin = Vector2.zero; sr.offsetMax = Vector2.zero;

        // Trilho
        var bg = NewImage("Background", sr, PanelColor);
        var bgr = bg.rectTransform;
        bgr.anchorMin = new Vector2(0f, 0.35f); bgr.anchorMax = new Vector2(1f, 0.65f);
        bgr.offsetMin = Vector2.zero; bgr.offsetMax = Vector2.zero;

        // Preenchimento
        var fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(sr, false);
        var far = (RectTransform)fillArea.transform;
        far.anchorMin = new Vector2(0f, 0.35f); far.anchorMax = new Vector2(1f, 0.65f);
        far.offsetMin = new Vector2(8f, 0f); far.offsetMax = new Vector2(-8f, 0f);
        var fill = NewImage("Fill", far, Blood);
        var fr = fill.rectTransform;
        fr.anchorMin = Vector2.zero; fr.anchorMax = Vector2.one;
        fr.offsetMin = Vector2.zero; fr.offsetMax = Vector2.zero;

        // Cursor
        var handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
        handleArea.transform.SetParent(sr, false);
        var har = (RectTransform)handleArea.transform;
        har.anchorMin = Vector2.zero; har.anchorMax = Vector2.one;
        har.offsetMin = new Vector2(14f, 0f); har.offsetMax = new Vector2(-14f, 0f);
        var handle = NewImage("Handle", har, Bone);
        var hr = handle.rectTransform;
        hr.anchorMin = new Vector2(0f, 0.15f); hr.anchorMax = new Vector2(0f, 0.85f);
        hr.sizeDelta = new Vector2(26f, 0f);
        hr.anchoredPosition = Vector2.zero;

        var s = sGo.GetComponent<Slider>();
        s.fillRect = fr;
        s.handleRect = hr;
        s.targetGraphic = handle;
        s.direction = Slider.Direction.LeftToRight;
        s.minValue = min;
        s.maxValue = max;
        var cb = s.colors;
        cb.normalColor = Bone;
        cb.highlightedColor = BloodBright;
        cb.selectedColor = BloodBright;
        cb.pressedColor = BloodBright;
        s.colors = cb;
        s.SetValueWithoutNotify(value);
        if (onChanged != null) s.onValueChanged.AddListener(onChanged);
        return s;
    }

    // ---------- Ícone de cogumelo desenhado por código  ----------
    static Sprite mushroom;
    public static Sprite MushroomSprite()
    {
        if (mushroom != null) return mushroom;
        const int N = 64;
        var clear = new Color(0, 0, 0, 0);
        var red = new Color(0.78f, 0.12f, 0.12f, 1f);
        var white = new Color(0.95f, 0.93f, 0.88f, 1f);
        var stem = new Color(0.88f, 0.80f, 0.65f, 1f);
        var black = new Color(0.05f, 0.05f, 0.06f, 1f);

        var px = new Color[N * N];
        for (int y = 0; y < N; y++)
        {
            for (int x = 0; x < N; x++)
            {
                float dx = (x - 31.5f) / 29f;
                float dy = y >= 34 ? (y - 34f) / 26f : 0f;
                bool cap = y >= 30 && dx * dx + dy * dy <= 1f;
                bool stemPx = x >= 23 && x <= 40 && y >= 6 && y < 30;

                Color c = clear;
                if (stemPx)
                {
                    c = stem;
                    if ((x == 28 || x == 29 || x == 35 || x == 36) && (y == 18 || y == 19)) c = black; // olhos
                }
                if (cap)
                {
                    c = red;
                    if (InCircle(x, y, 20, 46, 6) || InCircle(x, y, 42, 50, 7) ||
                        InCircle(x, y, 31, 40, 4) || InCircle(x, y, 52, 38, 3)) c = white;
                }
                px[y * N + x] = c;
            }
        }

        // contorno escuro
        var outlined = (Color[])px.Clone();
        for (int y = 0; y < N; y++)
            for (int x = 0; x < N; x++)
            {
                if (px[y * N + x].a > 0f) continue;
                bool near = (x > 0 && px[y * N + x - 1].a > 0f) || (x < N - 1 && px[y * N + x + 1].a > 0f) ||
                            (y > 0 && px[(y - 1) * N + x].a > 0f) || (y < N - 1 && px[(y + 1) * N + x].a > 0f);
                if (near) outlined[y * N + x] = black;
            }

        var tex = new Texture2D(N, N, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        tex.SetPixels(outlined);
        tex.Apply();
        mushroom = Sprite.Create(tex, new Rect(0, 0, N, N), new Vector2(0.5f, 0.5f), 100f);
        return mushroom;
    }

    static bool InCircle(int x, int y, int cx, int cy, int r) =>
        (x - cx) * (x - cx) + (y - cy) * (y - cy) <= r * r;
}
