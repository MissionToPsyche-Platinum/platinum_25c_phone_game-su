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
                    Debug.Log("Collision with Hyperspace power up");
                    healthSystem.DisableDamage();
                    scoreSystem.ScaleScoreRate(hyperspaceScoreScale);
                    break;
                case 1: //Shield
                    Debug.Log("Collision with Shield power up");
                    healthSystem.AddShield(shieldHealth);
                    break;
                case 2: //2x
                    Debug.Log("Collision with 2x power up");
                    GameStateManager.Instance.SetCoinMultiplier(coinMultiplier);
                    break;
                case 3: //Star
                    Debug.Log("Collision with Star power up");
                    debrisSpawnSystem.DisableSpawning();
                    break;
                default:
                    Debug.Log("Invalid index given to beginPowerUp()");
                    break;
            }
        } else {
            Debug.Log("Collision with Hex power up");
            ultimateProgress += 1;
            if(ultimateProgress == 5){
                powerUpsActive[index] = true;
                powerUpTimers[index] = powerUpLengths[index];
                debrisSpawnSystem.DisableSpawning();
                coinSpawnSystem.SetSpawnRateMultiplier(ultimateCoinSpawnIncrease);
            }
        }
    }

    void endPowerUp(int index){
        SFXController.instance.PlaySoundFXClip(powerUpEndSoundClip, transform, 1f);
        powerUpsActive[index] = false;
        switch(index) 
        {
            case 0:
                Debug.Log("Ending Hyperspace power up");
                scoreSystem.ScaleScoreRate(1 / hyperspaceScoreScale);
                healthSystem.EnableDamage();
                break;
            case 1:
            Debug.Log("Ending Shield power up");
                healthSystem.RemoveShield();
                break;
            case 2:
                Debug.Log("Ending 2x power up");
                GameStateManager.Instance.SetCoinMultiplier(1);
                break;
            case 3:
                Debug.Log("Ending Star power up");
                debrisSpawnSystem.EnableSpawning();
                break;
            case 4:
                Debug.Log("Ending Hex power up");
                debrisSpawnSystem.EnableSpawning();
                coinSpawnSystem.SetSpawnRateMultiplier(1f);
                ultimateProgress = 0;
                break;
            default:
                Debug.Log("Invalid index given to endPowerUp()");
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