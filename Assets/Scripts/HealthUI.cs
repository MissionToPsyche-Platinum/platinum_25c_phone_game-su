using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{

    public GameObject healthPoint;
    private GameObject[] hpInstances;
    private int nextHPToDelete;
    public int maxHealth;

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
        hpInstances = new GameObject[maxHealth];
    }

    void Start()
    {
        nextHPToDelete = maxHealth - 1;
        hpWidth = xMax - xMin;
        hpHeight =((yMax - yMin) - spacing * (float)(maxHealth - 1)) / (float)maxHealth;

        //check if spacing and hpHeight values fit in bounding box
        float maxSpacing = ((yMax - yMin) - minHPHeight * (float)maxHealth) / (float)(maxHealth - 1);
        spacing = hpHeight <= 0 ? maxSpacing : spacing;
        hpHeight = hpHeight <= 0 ? minHPHeight : hpHeight;

        float hpPosY = yMin;

        RectTransform rectTransform = healthPoint.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(0.5f, 0.5f);
        for(int i = 0; i < maxHealth; i++)
        {
            Vector3 spawnPos = new Vector3(xMin, hpPosY, 0f);
            hpInstances[i] = Instantiate(healthPoint, spawnPos, Quaternion.identity);
            Debug.Log("Spawning HP at " + xMin + ", " + hpPosY);
            hpPosY += hpHeight + spacing;
        }
    }

    void Update()
    {

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
        Destroy(hpInstances[nextHPToDelete]);
        if(nextHPToDelete > 0)
        {
            nextHPToDelete--;
        }
    }
}