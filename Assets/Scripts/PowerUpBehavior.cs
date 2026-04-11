using UnityEngine;

public class PowerUpBehavior : MonoBehaviour
{
    [SerializeField] private ScoreIncrement scoreIncrement;
    [SerializeField] private GameObject debrisSpawner;
    [SerializeField] private GameObject coinSpawner;
    private bool[] powerUpsActive;
    private float[] powerUpTimers;
    public float[] powerUpLengths;

    [SerializeField] private AudioClip powerUpStartSoundClip;
    [SerializeField] private AudioClip powerUpEndSoundClip;

    [SerializeField] private GameObject hyperspacePopupPrefab;
    [SerializeField] private GameObject shieldPopupPrefab;
    [SerializeField] private GameObject twoXCoinsPopupPrefab;
    [SerializeField] private GameObject starPopupPrefab;
    [SerializeField] private GameObject hexPopupPrefab;

    private ProbeHealth healthSystem;
    private ScoreIncrement scoreSystem;
    private DebrisSpawnScript debrisSpawnSystem;
    private CoinSpawner coinSpawnSystem;
    
    private int shieldHealth = 1;
    private float hyperspaceScoreScale = 2;
    private int coinMultiplier = 2;
    private int ultimateProgress = 0;
    private float ultimateCoinSpawnIncrease = 20f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        powerUpsActive = new bool[5];
        powerUpTimers = new float[5];
        for(int i = 0; i < 5; i++){
            powerUpsActive[i] = false;
        }

        healthSystem = GetComponent<ProbeHealth>();
        scoreSystem = scoreIncrement.GetComponent<ScoreIncrement>();
        debrisSpawnSystem = debrisSpawner.GetComponent<DebrisSpawnScript>();
        coinSpawnSystem = coinSpawner.GetComponent<CoinSpawner>();
    }

    public void beginPowerUp(int index){
        SFXController.instance.PlaySoundFXClip(powerUpStartSoundClip, transform, 1f);

        if(index != 4){
            powerUpsActive[index] = true;
            powerUpTimers[index] = powerUpLengths[index];
            switch(index)
            {
                case 0: //Hyperspace
                    healthSystem.DisableDamage();
                    scoreSystem.ScaleScoreRate(hyperspaceScoreScale);
                    if (hyperspacePopupPrefab != null) PopupManager.Instance.DisplayPopup(hyperspacePopupPrefab, 3f);
                    break;
                case 1: //Shield
                    healthSystem.AddShield(shieldHealth);
                    if (shieldPopupPrefab != null) PopupManager.Instance.DisplayPopup(shieldPopupPrefab, 3f);
                    break;
                case 2: //2x
                    GameStateManager.Instance.SetCoinMultiplier(coinMultiplier);
                    if (twoXCoinsPopupPrefab != null) PopupManager.Instance.DisplayPopup(twoXCoinsPopupPrefab, 3f);
                    break;
                case 3: //Star
                    debrisSpawnSystem.DisableSpawning();
                    if (starPopupPrefab != null) PopupManager.Instance.DisplayPopup(starPopupPrefab, 3f);
                    break;
                default:
                    break;
            }
        } else {
            ultimateProgress += 1;
            if(ultimateProgress == 5){
                powerUpsActive[index] = true;
                powerUpTimers[index] = powerUpLengths[index];
                debrisSpawnSystem.DisableSpawning();
                coinSpawnSystem.SetSpawnRateMultiplier(ultimateCoinSpawnIncrease);
                if (hexPopupPrefab != null) PopupManager.Instance.DisplayPopup(hexPopupPrefab, 3f);
            }
        }
    }

    void endPowerUp(int index){
        SFXController.instance.PlaySoundFXClip(powerUpEndSoundClip, transform, 1f);
        powerUpsActive[index] = false;
        switch(index) 
        {
            case 0:
                scoreSystem.ScaleScoreRate(1 / hyperspaceScoreScale);
                healthSystem.EnableDamage();
                break;
            case 1:
                healthSystem.RemoveShield();
                break;
            case 2:
                GameStateManager.Instance.SetCoinMultiplier(1);
                break;
            case 3:
                debrisSpawnSystem.EnableSpawning();
                break;
            case 4:
                debrisSpawnSystem.EnableSpawning();
                coinSpawnSystem.SetSpawnRateMultiplier(1f);
                ultimateProgress = 0;
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < 5; i++){
            if(powerUpsActive[i]){
                if(powerUpTimers[i] > 0){
                    powerUpTimers[i] -= Time.deltaTime;
                } else {
                    endPowerUp(i);
                }
            }
        }
    }

    float GetShieldHealth(){
        return shieldHealth;
    }
}