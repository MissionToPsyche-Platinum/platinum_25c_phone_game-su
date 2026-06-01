using UnityEngine;
using UnityEngine.UI;

public class PowerUpHUD : MonoBehaviour
{
    private PowerUpBehavior _behavior;
    private GameObject[]    _slots  = new GameObject[5];
    private Text[]          _timers = new Text[5];

    void Start()
    {
        _behavior = FindObjectOfType<PowerUpBehavior>();

        Sprite[] icons = new Sprite[5];
        PowerUpSpawnScript spawner = FindObjectOfType<PowerUpSpawnScript>(true);
        if (spawner != null)
        {
            foreach (GameObject prefab in spawner.powerUps)
            {
                int idx = TagToIndex(prefab.tag);
                if (idx < 0) continue;
                SpriteRenderer sr = prefab.GetComponentInChildren<SpriteRenderer>();
                if (sr != null) icons[idx] = sr.sprite;
            }
        }

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        GameObject container = new GameObject("PowerUpHUD");
        container.transform.SetParent(canvas.transform, false);
        container.transform.SetAsLastSibling();

        RectTransform ct = container.AddComponent<RectTransform>();
        ct.anchorMin        = new Vector2(0.5f, 1f);
        ct.anchorMax        = new Vector2(0.5f, 1f);
        ct.pivot            = new Vector2(0.5f, 1f);
        ct.anchoredPosition = new Vector2(0f, -14f);

        HorizontalLayoutGroup hl = container.AddComponent<HorizontalLayoutGroup>();
        hl.spacing               = 6f;
        hl.childAlignment        = TextAnchor.UpperCenter;
        hl.childForceExpandWidth  = false;
        hl.childForceExpandHeight = false;

        ContentSizeFitter csf = container.AddComponent<ContentSizeFitter>();
        csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        csf.verticalFit   = ContentSizeFitter.FitMode.PreferredSize;

        for (int i = 0; i < 5; i++)
            BuildSlot(container.transform, i, icons[i]);
    }

    void BuildSlot(Transform parent, int i, Sprite icon)
    {
        GameObject slot = new GameObject("PUSlot_" + i);
        slot.transform.SetParent(parent, false);

        LayoutElement le   = slot.AddComponent<LayoutElement>();
        le.preferredWidth  = 48f;
        le.preferredHeight = 64f;

        Image bg  = slot.AddComponent<Image>();
        bg.color  = new Color(0f, 0f, 0f, 0.5f);

        VerticalLayoutGroup vl      = slot.AddComponent<VerticalLayoutGroup>();
        vl.childAlignment           = TextAnchor.MiddleCenter;
        vl.childForceExpandWidth    = true;
        vl.childForceExpandHeight   = false;
        vl.padding                  = new RectOffset(4, 4, 6, 4);
        vl.spacing                  = 2f;

        GameObject iconGO  = new GameObject("Icon");
        iconGO.transform.SetParent(slot.transform, false);
        LayoutElement ile  = iconGO.AddComponent<LayoutElement>();
        ile.preferredWidth  = 36f;
        ile.preferredHeight = 36f;
        Image img           = iconGO.AddComponent<Image>();
        img.sprite          = icon;
        img.preserveAspect  = true;

        GameObject timerGO = new GameObject("Timer");
        timerGO.transform.SetParent(slot.transform, false);
        LayoutElement tle  = timerGO.AddComponent<LayoutElement>();
        tle.preferredWidth  = 48f;
        tle.preferredHeight = 18f;
        Text t              = timerGO.AddComponent<Text>();
        t.font              = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize          = 13;
        t.fontStyle         = FontStyle.Bold;
        t.alignment         = TextAnchor.MiddleCenter;
        t.color             = Color.white;
        _timers[i]          = t;

        _slots[i] = slot;
        slot.SetActive(false);
    }

    int TagToIndex(string tag)
    {
        switch (tag)
        {
            case "PowerUpHyperspace": return 0;
            case "PowerUpShield":     return 1;
            case "PowerUp2x":         return 2;
            case "PowerUpStar":       return 3;
            case "PowerUpHex":        return 4;
            default:                  return -1;
        }
    }

    void Update()
    {
        if (_behavior == null) return;
        bool[]  active = _behavior.GetPowerUpsActive();
        float[] timers = _behavior.GetPowerUpTimers();

        for (int i = 0; i < 5; i++)
        {
            _slots[i].SetActive(active[i]);
            if (active[i])
                _timers[i].text = Mathf.CeilToInt(timers[i]) + "s";
        }
    }
}
