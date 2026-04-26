using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleTileController : MonoBehaviour
{
    public struct DebrisSpawnInfo {

        int type;
        float scale;
        float initialX;
        float initialY;
        float moveSpeed;
        float pivotX;
        float pivotY;
        float revolutionSpeed;

        public DebrisSpawnInfo(int debrisType, float size, float xPos, float yPos, float speed, float pivotXPos, float pivotYPos, float revSpeed)
        {
            type = debrisType;
            scale = size;
            initialX = xPos;
            initialY = yPos;
            moveSpeed = speed;
            pivotX = pivotXPos;
            pivotY = pivotYPos;
            revolutionSpeed = revSpeed;
        }

        public int GetType()
        {
            return type;
        }

        public float GetScale()
        {
            return scale;
        }

        public float GetPosX()
        {
            return initialX;
        }

        public float GetPosY()
        {
            return initialY;
        }

        public float GetSpeed()
        {
            return moveSpeed;
        }

        public float GetPivotX()
        {
            return pivotX;
        }

        public float GetPivotY()
        {
            return pivotY;
        }

        public float GetRevSpeed()
        {
            return revolutionSpeed;
        }
    };

    private List<List<DebrisSpawnInfo>>[] obstacleTiles;
    bool tileTestMode = false;

    /*
        Arrays below are used to initialize the obstacleTiles list

        Each line of each array below must have the same number of elements
        Value of -1 is used to delineate different lines in types array, 0 is used for consistency in other arrays

        Each entry (apart from line delineators) correspond to one obstacle attribute value, a single GameObject
        Each line corresponds to attribute values for one obstacle tile, an assortment of debris GameObjects
        Matching indices correspond to different attributes of the same GameObject
    */

    //asteroid type (see above)
    private int[] types = 
    {
        0, 0, 1, -1,        //two medium standards on the sides, one smaller homing in the middle
        0, 0, -1,           //2 spinning standards
        0, 0, 0, 0, 0, -1,  //5 standards in an X shape
        1, 1, -1,           //one homing above another
        0, 0, 0, -1,        //two medium standards on the sides, one smaller standard in the middle
        0, -1,              //standard on the left
        0, -1,              //standard on the right
        0,                   //standard in middle
        2, 0, 2, -1,        //exploding on sides, standard in middle
        3, 3, -1,           //two duplicators side by side
        0, 1, 0, -1,        //homing sandwiched between standards
        4, 0, -1,           //teleporter left, standard right
        2, 2, 2, -1,        //three exploding in a row
        0, 3, 0, -1,        //duplicator flanked by standards
        1, 4, 1, -1,        //teleporter in middle, homings on sides
        2, 1, -1,           //exploding above, homing below
        0, 0, 0, 0, 0, -1,  //5 fast standards in a V shape
        3, 0, 3, -1,        //duplicators on sides, standard center
        4, 4, -1,           //two teleporters
        2, 3, -1,           //exploding left, duplicating right
        1, 1, 4, -1,        //two homings with a teleporter
        0, 2, 0, 0, -1,     //exploder surrounded by standards
        4                   //single teleporter center
    };

    //asteroid size
    private float[] scales = 
    {
        0.3f, 0.3f, 0.1f, 0f,
        0.2f, 0.2f, 0f,
        0.3f, 0.3f, 0.3f, 0.3f, 0.3f, 0f,
        0.3f, 0.3f, 0f,
        0.3f, 0.3f, 0.1f, 0f,
        0.5f, 0f,
        0.5f, 0f,
        0.5f,
        0.3f, 0.2f, 0.3f, 0f,
        0.3f, 0.3f, 0f,
        0.25f, 0.2f, 0.25f, 0f,
        0.3f, 0.3f, 0f,
        0.25f, 0.25f, 0.25f, 0f,
        0.3f, 0.25f, 0.3f, 0f,
        0.25f, 0.3f, 0.25f, 0f,
        0.3f, 0.2f, 0f,
        0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0f,
        0.3f, 0.2f, 0.3f, 0f,
        0.35f, 0.35f, 0f,
        0.3f, 0.3f, 0f,
        0.2f, 0.2f, 0.3f, 0f,
        0.25f, 0.3f, 0.25f, 0.25f, 0f,
        0.4f,
    };

    //starting x position of asteroids
    private float[] xPositions = 
    {
        -1.5f, 1.5f, 0f, 0f,
        -1f, 1f, 0f,
        -1.5f, 1.5f, 0f, -1.5f, 1.5f, 0f,
        0f, 0f, 0f,
        -1.5f, 1.5f, 0f, 0f,
        -1f, 0f, 
        0f, 0f, 
        1f,
        -1.5f, 0f, 1.5f, 0f,
        -1f, 1f, 0f,
        -1.5f, 0f, 1.5f, 0f,
        -1f, 1f, 0f,
        -1.5f, 0f, 1.5f, 0f,
        -1.5f, 0f, 1.5f, 0f,
        -1.5f, 0f, 1.5f, 0f,
        -1f, 1f, 0f,
        -2f, -1f, 0f, 1f, 2f, 0f,
        -1.5f, 0f, 1.5f, 0f,
        -1f, 1f, 0f,
        -1f, 1f, 0f,
        -1.5f, 0f, 1.5f, 0f,
        -1.5f, 0f, 1.5f, 0f, 0f,
        0f,
    };

    //starting y position of asteroids
    private float[] yPositions = 
    {
        10f, 10f, 15f, 0f,
        10f, 10f, 0f,
        10f, 10f, 13, 16, 16f, 0f,
        10f, 12f, 0f,
        10f, 10f, 15f, 0f,
        10f, 0f,
        10f, 0f,
        10f,
        10f, 10f, 10f, 0f,
        10f, 10f, 0f,
        10f, 13f, 10f, 0f,
        10f, 10f, 0f,
        10f, 10f, 10f, 0f,
        10f, 13f, 10f, 0f,
        10f, 13f, 10f, 0f,
        12f, 10f, 0f,
        10f, 10f, 10f, 12f, 12f, 0f,
        10f, 13f, 10f, 0f,
        10f, 10f, 0f,
        10f, 10f, 0f,
        10f, 13f, 10f, 0f,
        10f, 13f, 10f, 16f, 0f,
        10f,
    };

    //pivotX, pivotY, revolutions per second
    //point for asteroids to revolve around as they fall
    private float[] rotations = 
    {
        0f, 0f, 0f, 0f,
        0f, 10f, 0.2f,
        0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 12f, 0.2f,
        0f, 10f, 0.25f,
        0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f
    };

    //asteroid move speed
    private float[] speeds = 
    {
        3f, 3f, 6f, 0f,
        3f, 3f, 0f,
        3f, 3f, 3f, 3f, 3f, 0f,
        5f, 5f, 0f,
        3f, 3f, 6f, 0f,
        3f, 0f, 
        3f, 0f, 
        3f,
        4f, 3f, 4f, 0f,
        4f, 4f, 0f,
        3f, 5f, 3f, 0f,
        4f, 3f, 0f,
        4f, 4f, 4f, 0f,
        3f, 4f, 3f, 0f,
        3f, 4f, 3f, 0f,
        4f, 5f, 0f,
        3f, 3f, 3f, 3f, 3f, 0f,
        4f, 3f, 4f, 0f,
        4f, 4f, 0f,
        4f, 5f, 0f,
        5f, 5f, 4f, 0f,
        3f, 4f, 3f, 3f, 0f,
        4f
    };

    //difficulty rating, 0 is easiest, 2 is hardest, 3 for testing tiles
    private int[] sets = {1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 2, 1, 2, 1, 0, 1, 2, 2, 2, 1, 2}; 

    //chance of getting a tile from each set
    private float[] setSpawnProbabilities = {0.75f, 0.175f, 0.075f};
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        obstacleTiles = new List<List<DebrisSpawnInfo>>[4];
        for(int i = 0; i < 4; i++){
            obstacleTiles[i] = new List<List<DebrisSpawnInfo>>();
        }

        if(types.Length != scales.Length || types.Length != xPositions.Length || types.Length != yPositions.Length || types.Length != speeds.Length)
        {
            Debug.Log(types.Length);
            Debug.Log(scales.Length);
            Debug.Log(xPositions.Length);
            Debug.Log(yPositions.Length);
            Debug.Log(speeds.Length);
            throw new Exception("Obstacle tile attribute array lengths do not match");
        } else {
            int currIndex = 0;
            int setIndex = 0;
            while(currIndex < types.Length)
            {
                List<DebrisSpawnInfo> newList = new List<DebrisSpawnInfo>();
                while(currIndex < types.Length && types[currIndex] != -1)
                {
                    DebrisSpawnInfo newSpawnInfo = new DebrisSpawnInfo(types[currIndex], scales[currIndex], xPositions[currIndex], yPositions[currIndex], speeds[currIndex], rotations[setIndex * 3], rotations[setIndex * 3 + 1], rotations[setIndex * 3 + 2]);
                    newList.Add(newSpawnInfo);
                    currIndex++;
                }
                currIndex++;
                if(setIndex < sets.Length){
                    obstacleTiles[sets[setIndex]].Add(newList);
                } else {
                    throw new Exception("Number of assigned sets does not match number of sets given");
                }
                setIndex++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool UsingTestMode()
    {
        return tileTestMode;
    }

    public List<DebrisSpawnInfo> GetRandomTile()
    {
        float seed = Random.Range(0f, 1f);
        int chosenTileSet = -1;
        float currTotal = setSpawnProbabilities[0];
        for(int i = 0; i < setSpawnProbabilities.Length - 2; i++)
        {
            if(seed < currTotal)
            {
                chosenTileSet = i;
                break;
            }
            currTotal += setSpawnProbabilities[i + 1];
        }
        if(chosenTileSet == -1)
        {
            chosenTileSet = setSpawnProbabilities.Length - 2;
        }

        if(tileTestMode)
        {
            chosenTileSet = 3;
        }

        int chosenTile = Random.Range(0, obstacleTiles[chosenTileSet].Count - 1);
        List<DebrisSpawnInfo> spawnInfos = obstacleTiles[chosenTileSet][chosenTile];
        return spawnInfos;
    }
}