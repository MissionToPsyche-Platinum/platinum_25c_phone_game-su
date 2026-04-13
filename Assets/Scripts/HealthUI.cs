using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{

    public GameObject healthPoint;
    [SerializeField] RectTransform container;
    private GameObject[] hpInstances;
    private int currentHP;

    public float spacing;

    //bounding box edges for hp display area
    public float xMin;
    public float xMax;
    public float yMin;
    public float yMax;

    private float hpWidth;
    private float hpHeight;

    private float minHPHeight = 0.1f;

    void Awake()
    {
        GameStateManager.Instance.OnStartPlaying += OnStartPlaying;
        GameStateManager.Instance.OnStopPlaying += OnStopPlaying;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnStartPlaying -= OnStartPlaying;
        GameStateManager.Instance.OnStopPlaying -= OnStopPlaying;
        LeftHandedManager.OnLeftHandedChanged -= ApplyMirror;
    }

    private void OnStartPlaying(object sender, GameStateManager.GameStateChangeEventArgs e)
    {
        currentHP = GameStateManager.Instance.startingHealth;
        if (currentHP <= 0) return;

        hpInstances = new GameObject[currentHP];

        for (int i = 0; i < currentHP; i++)
            hpInstances[i] = Instantiate(healthPoint, container);

        LeftHandedManager.OnLeftHandedChanged += ApplyMirror;
        ApplyMirror(LeftHandedManager.IsLeftHanded);
    }
    private void ApplyMirror(bool isLeftHanded)
    {
        Vector2 pos = container.anchoredPosition;
        pos.x = isLeftHanded ? Mathf.Abs(pos.x) : -Mathf.Abs(pos.x);
        container.anchoredPosition = pos;
    }

    private void OnStopPlaying(object sender, GameStateManager.GameStateChangeEventArgs e)
    {
        if (hpInstances == null) return;
        foreach (var hp in hpInstances)
        {
            if (hp != null) Destroy(hp);
        }
        hpInstances = null;
        currentHP = 0;
        LeftHandedManager.OnLeftHandedChanged -= ApplyMirror;
    }

    public void TakeDamage(int damage)
    {
        for(int i = 0; i < damage; i++)
        {
            RemoveHP();
        }
    }

    void RemoveHP()
    {
        if (currentHP <= 0) return;
        currentHP--;
        Destroy(hpInstances[currentHP]);
        hpInstances[currentHP] = null;
    }

    public void AddHP(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (currentHP >= hpInstances.Length) break;
            hpInstances[currentHP] = Instantiate(healthPoint, container);
            currentHP++;
        }
    }
}