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
        float pivotX;
        float pivotY;
        float revolutionSpeed;

        public DebrisSpawnInfo(int debrisType, float size, float xPos, float yPos, float pivotXPos, float pivotYPos, float revSpeed)
        {
            type = debrisType;
            scale = size;
            initialX = xPos;
            initialY = yPos;
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

        Arrays are spaced into groups of 10 for readability
    */

    //asteroid type
    //0 - standard
    //1 - homing
    //2 - exploding
    //3 - duplicating
    //4 - teleporting
    private int[] types = 
    {
        0, 0, 1, -1,                        //two medium standards on the sides, one smaller homing in the middle
        0, 0, -1,                           //2 spinning standards
        0, 0, 0, 0, 0, -1,                  //5 standards in an X shape
        1, 1, -1,                           //one homing above another
        0, 0, 0, -1,                        //two medium standards on the sides, one smaller standard in the middle
        0, -1,                              //standard on the left
        0, -1,                              //standard on the right
        0, -1,                              //standard in middle
        2, 0, 2, -1,                        //exploding on sides, standard in middle
        3, 3, -1,                           //two duplicators side by side

        0, 1, 0, -1,                        //homing sandwiched between standards
        4, 0, -1,                           //teleporter left, standard right
        2, 2, 2, -1,                        //three exploding in a row
        0, 3, 0, -1,                        //duplicator flanked by standards
        1, 4, 1, -1,                        //teleporter in middle, homings on sides
        2, 1, -1,                           //exploding above, homing below
        0, 0, 0, 0, 0, -1,                  //5 fast standards in a V shape
        3, 0, 3, -1,                        //duplicators on sides, standard center
        4, 4, -1,                           //two teleporters
        2, 3, -1,                           //exploding left, duplicating right

        1, 1, 4, -1,                        //two homings with a teleporter
        0, 2, 0, 0, -1,                     //exploder surrounded by standards
        4, -1,                              //single teleporter center
        0, 0, 0, -1,                        //standards with concentric orbits
        1, -1,                              //single homing
        0, 0, 0, 0, 0, -1,                  //random fast standards (variation 1)
        0, 0, 0, 0, 0, -1,                  //random fast standards (variation 2)
        1, 0, 2, 0, 3, 0, 4, 0, -1,         //ring of all types
        2, 0, -1,                           //exploding with standard in the middle
        3, 3, -1,                           //duplicating on left and right with offset heights

        2, 1, -1,                           //exploding followed by homing
        4, 4, 4, 4, -1,                     //4 teleports in a horizontal line
        0, 0, 0, -1,                        //rotating triangle of standards
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1,   //diagonal path of standards
        0, 4, 0, -1,                        //large teleporter with small standards on sides
        1, 1, 1, -1,                        //homing on left, right, middle with offset heights
        2, -1,                              //single exploding center
        0, 0, 0, 0, -1,                     //2 overlapping pairs of standards rotating
        1, 2, 1, -1,                        //homing-exploding-homing flank
        3, 3, 3, -1,                        //diagonal stair of duplicators

        2, -1,                              //single exploding slightly off-center
        0, 0, 0, 0, 0, 0, -1,               //wide wall of six standards
        1, -1,                              //single large slow homing
        4, 4, -1,                           //offset teleporter pair
        2, 2, 2, -1,                        //triple exploding wide spread
        0, 0, 0, 0, -1,                     //four standards orbiting center
        1, 1, 1, 1, -1,                     //slow homing diagonal barrage
        0, 2, 0, 1, -1,                     //standard, exploding, standard, homing
        0, -1,                              //single wide standard slight offset
        3, 0, 3, -1,                        //duplicator pair with standard center

        4, -1,                              //single spinning teleporter
        1, 1, -1,                           //two homings stacked
        3, -1,                              //easy single duplicator
        0, 0, 0, 0, 0, -1,                  //debris cluster
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1,   //field 1
        0, 0, 0, 0, 0, 0, 0, -1,            //field 2
        2, 2, 2, 2, 2, -1,                  //minefield
        0, 0, 0, 1, -1,                     //field with homing
        0, 0, 0, 0, 0, 0, 0, 0, -1,         //field of small standards
        1, 1, 1, -1                         //3 homings
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
        0.5f, 0f,
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
        0.4f, 0f,
        0.3f, 0.2f, 0.1f, 0f,
        0.4f, 0f,
        0.25f, 0.3f, 0.2f, 0.4f, 0.3f, 0f,
        0.3f, 0.3f, 0.3f, 0.3f, 0.3f, 0f,
        0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0f,
        0.5f, 0.25f, 0f,
        0.4f, 0.4f, 0f, 

        0.5f, 0.3f, 0f,
        0.25f, 0.25f, 0.25f, 0.25f, 0f,
        0.25f, 0.25f, 0.25f, 0f,
        0.3f, 0.3f, 0.3f, 0.3f, 0.3f, 0.3f, 0.3f, 0.3f, 0.3f, 0.3f, 0f,
        0.1f, 0.4f, 0.1f, 0f,
        0.4f, 0.4f, 0.4f, 0f,
        0.5f, 0f,
        0.25f, 0.25f, 0.25f, 0.25f, 0f,
        0.25f, 0.3f, 0.25f, 0f,
        0.25f, 0.25f, 0.25f, 0f,

        0.5f, 0f,
        0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0f,
        0.6f, 0f,
        0.3f, 0.3f, 0f,
        0.25f, 0.25f, 0.25f, 0f,
        0.2f, 0.2f, 0.2f, 0.2f, 0f,
        0.2f, 0.2f, 0.2f, 0.2f, 0f,
        0.25f, 0.3f, 0.25f, 0.2f, 0f,
        0.4f, 0f,
        0.3f, 0.2f, 0.3f, 0f,

        0.4f, 0f,
        0.25f, 0.25f, 0f,
        0.3f, 0f,
        0.05f, 0.1f, 0.15f, 0.1f, 0.07f, 0f,
        0.05f, 0.05f, 0.1f, 0.1f, 0.15f, 0.2f, 0.2f, 0.3f, 0.3f, 0.4f, 0f,
        0.2f, 0.11f, 0.4f, 0.267f, 0.44f, 0.267f, 0.133f, 0f,
        0.4f, 0.1f, 0.1f, 0.1f, 0.4f, 0f,
        0.5f, 0.5f, 0.2f, 0.1f, 0f,
        0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0f,
        0.3f, 0.4f, 0.3f, 0f
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
        1f, 0f,
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
        0f, 0f,
        0f, 0.75f, -1.5f, 0f,
        0f, 0f,
        0.5f, 0f, 1.3f, -1.3f, -0.4f, 0f,
        -0.8f, 1.4f, 0f, 0.4f, -1.4f, 0f,
        0f, 1.061f, 1.5f, 1.061f, 0f, -1.061f, -1.5f, -1.061f, 0f,
        0f, 0f, 0f,
        -1.2f, 1.2f, 0f,

        0f, 0f, 0f,
        -1.5f, -0.5f, 0.5f, 1.5f, 0f,
        -0.5f, 0.5f, 0f, 0f,
        -1.5f, 0.5f, -1.25f, 0.75f, -1f, 1f, -0.75f, 1.25f, -0.5f, 1.5f, 0f,
        -1.25f, 0f, 1.25f, 0f,
        -1.25f, 0f, 1.25f, 0f,
        0f, 0f, 
        -1.5f, 0.5f, 1.5f, 3.5f, 0f,
        -1.5f, 0f, 1.5f, 0f,
        -1f, 0f, 1f, 0f,

        -1f, 0f,
        -2f, -1.2f, -0.4f, 0.4f, 1.2f, 2f, 0f,
        0f, 0f,
        -1.5f, 1.5f, 0f,
        -2f, 0f, 2f, 0f,
        -0.5f, 0.5f, 0.5f, -0.5f, 0f,
        -1.5f, -0.5f, 0.5f, 1.5f, 0f,
        0f, -1.5f, 0f, 1.5f, 0f,
        0.5f, 0f,
        -1f, 0f, 1f, 0f,

        0f, 0f,
        0f, 0f, 0f,
        0f, 0f,
        0f, 0.6f, -0.4f, -1.2f, 1f, 0f,
        -2f, 0.5f, -1.5f, 1.5f, 0f, 0.75f, -1f,2f, 1f, -0.5f, 0f,
        -2f, -1.8f, -0.5f, -0.2f, 1.8f, 2f, 1.8f, 0f,
        -2f, -1.5f, 1.5f, 1.8f, 2f, 0f,
        -2f, 0f, 1.5f, 2f, 0f,
        -2f, -1.3f, -1f, 0f, 1f, 1.4f, 1.8f, 2f, 0f,
        -1.5f, 0f, 1.5f, 0f
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
        10f, 0f,
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
        10f, 0f,
        10f, 10f, 10f, 0f,
        10f, 0f,
        10f, 14f, 18f, 22f, 26f, 30f,
        10f, 14f, 18f, 22f, 26f, 30f,
        13.5f, 13.061f, 12f, 11.939f, 10.5f, 11.939f, 12f, 13.061f, 0f,
        10f, 10f, 0f,
        10f, 15f, 0f,

        10f, 20f, 0f,
        10f, 10f, 10f, 10f, 0f,
        10f, 10f, 10.86602f, 0f,
        10f, 10f, 12f, 12f, 14f, 14f, 16f, 16f, 18f, 18f, 0f,
        10f, 10f, 10f, 0f,
        10f, 18f, 14f, 0f,
        10f, 0f,
        10f, 10f, 10f, 10f, 0f,
        10f, 10f, 10f, 0f,
        10f, 14f, 18f, 0f,

        10f, 0f,
        10f, 10f, 10f, 10f, 10f, 10f, 0f,
        10f, 0f,
        10f, 14f, 0f,
        10f, 12f, 10f, 0f,
        10f, 10f, 11f, 11f, 0f,
        10f, 13f, 16f, 19f, 0f,
        8f, 11f, 14f, 11f, 0f,
        10f, 0f,
        10f, 12f, 10f, 0f,

        10f, 0f,
        10f, 14f, 0f,
        10f, 0f,
        10f, 10.1f, 10.2f, 10.1f, 10.3f, 0f,
        10f, 10.5f, 12f, 11f, 14f, 16f, 16f, 18f, 20f, 18f, 0f,
        11.5f, 17.3f, 14.5f, 8.6f, 10f, 6f, 8f, 0f,
        13f, 19f, 11f, 20f, 17f, 0f,
        16f, 15f, 11.5f, 19f, 0f,
        18f, 11.5f, 14.2f, 17f, 11f, 15.5f, 13.2f, 19f, 0f,
        10f, 10f, 10f, 0f
    };

    private float[] pivotXs =
    {
        0f, 0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f, 0f,
        0f, 0f, 
        0f, 0f, 
        0f, 0f,
        0f, 0f, 0f, 0f,
        0f, 0f, 0f,

        0f, 0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f,

        0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f,
        0f, 0f,
        0f, 0f, 0f, 0f,
        0f, 0f, 
        0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f,

        0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f,
        0.5f, -0.5f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f,
        0f, 0f,
        0f, -1.5f, 0f, 1.5f, 0f,
        0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f,

        0f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f,
        0f, 0f,
        0f, 0f, 0f, 0f,

        0f, 0f,
        0f, 0f, 0f,
        0f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f
    };

    private float[] pivotYs =
    {
        0f, 0f, 0f, 0f,
        10f, 10f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f, 0f,
        0f, 0f, 
        0f, 0f, 
        0f, 0f,
        0f, 0f, 0f, 0f,
        0f, 0f, 0f,

        0f, 0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f,
        12f, 12f, 12f, 0f,
        10f, 10f, 0f,

        0f, 0f, 0f,
        0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f,
        0f, 0f,
        10f, 10f, 10f, 0f,
        0f, 0f, 
        0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f,

        0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0.86602f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f,
        0f, 0f,
        0f, 10f, 0f, 10f, 0f,
        0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f,

        0f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f, 0f,
        10.5f, 10.5f, 10.5f, 10.5f, 0f,
        0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f,
        0f, 0f,
        0f, 0f, 0f, 0f,

        10f, 0f,
        0f, 0f, 0f,
        0f, 0f,
        10f, 10f, 10f, 10f, 10f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f
    };

    private float[] revSpeeds =
    {
        0f, 0f, 0f, 0f,
        0.2f, 0.2f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f, 0f,
        0f, 0f, 
        0f, 0f, 
        0f, 0f,
        0f, 0f, 0f, 0f,
        0f, 0f, 0f,

        0f, 0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f,
        0.2f, 0.2f, 0.2f, 0f,
        0.25f, 0.25f, 0f,
        0f, 0f, 0f,

        0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f,
        0f, 0f,
        0f, 0.6f, -1f, 0f,
        0f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f,

        0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f,
        1f, 1f, 1f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f,
        0f, 0f,
        0f, 0.8f, 0f, 0.6f, 0f,
        0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f,

        0f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f,
        0f, 0f, 0f,
        0f, 0f, 0f, 0f,
        0.5f, 0.5f, 0.5f, 0.5f, 0f,
        0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f,
        0f, 0f,
        0f, 0f, 0f, 0f,

        0.5f, 0f,
        0f, 0f, 0f,
        0f, 0f,
        0f, 1f, 0.8f, 0.3f, 1.5f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 
        0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
        0f, 0f, 0f, 0f
    };


    //difficulty rating, 0 is easiest, 2 is hardest, 3 for testing tiles
    private int[] sets = {
        1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 
        1, 1, 2, 1, 2, 1, 0, 1, 2, 2, 
        2, 1, 2, 0, 0, 2, 2, 2, 1, 1, 
        1, 2, 1, 2, 1, 2, 1, 2, 2, 2,
        0, 2, 1, 2, 2, 1, 2, 2, 0, 1,
        2, 1, 0, 0, 2, 2, 2, 2, 2, 1
    }; 


    //chance of getting a tile from each set
    private float[] setSpawnProbabilities = {0.75f, 0.175f, 0.075f};
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        obstacleTiles = new List<List<DebrisSpawnInfo>>[4];
        for(int i = 0; i < 4; i++){
            obstacleTiles[i] = new List<List<DebrisSpawnInfo>>();
        }

        if(types.Length != scales.Length || types.Length != xPositions.Length || types.Length != yPositions.Length
            || types.Length != pivotXs.Length || types.Length != pivotYs.Length || types.Length != revSpeeds.Length)
        {
            Debug.Log(types.Length);
            Debug.Log(scales.Length);
            Debug.Log(xPositions.Length);
            Debug.Log(yPositions.Length);
            Debug.Log(pivotXs.Length);
            Debug.Log(pivotYs.Length);
            Debug.Log(revSpeeds.Length);
            throw new Exception("Obstacle tile attribute array lengths do not match");
        } else {
            int currIndex = 0;
            int setIndex = 0;
            while(currIndex < types.Length)
            {
                List<DebrisSpawnInfo> newList = new List<DebrisSpawnInfo>();
                while(currIndex < types.Length && types[currIndex] != -1)
                {
                    DebrisSpawnInfo newSpawnInfo = new DebrisSpawnInfo(types[currIndex], scales[currIndex], xPositions[currIndex], yPositions[currIndex], pivotXs[currIndex], pivotYs[currIndex], revSpeeds[currIndex]);
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
        bool validTileFound = false;
        List<DebrisSpawnInfo> spawnInfos = new List<DebrisSpawnInfo>();
        while(!validTileFound){
            float seed = Random.Range(0f, 1f);
            int chosenTileSet = setSpawnProbabilities.Length - 1;
            float currTotal = 0f;
            for(int i = 0; i < setSpawnProbabilities.Length; i++)
            {
                currTotal += setSpawnProbabilities[i];
                if(seed < currTotal)
                {
                    chosenTileSet = i;
                    break;
                }
            }

            if(tileTestMode)
            {
                chosenTileSet = 3;
            }

            int chosenTile = Random.Range(0, obstacleTiles[chosenTileSet].Count);
            spawnInfos = obstacleTiles[chosenTileSet][chosenTile];


            //ensure spawned debris are of unlocked types
            validTileFound = true;
            for(int i = 0; i < spawnInfos.Count; i++){
                if(
                    (spawnInfos[i].GetType() == 1 && GameStateManager.Instance.GetGameStageInt() < 3) ||
                    ((spawnInfos[i].GetType() == 2 || spawnInfos[i].GetType() == 3) && GameStateManager.Instance.GetGameStageInt() < 4) ||
                    (spawnInfos[i].GetType() == 4 && GameStateManager.Instance.GetGameStageInt() < 5)
                )
                {
                    validTileFound = false;
                }
            }
            if(tileTestMode) validTileFound = true;
        }
        return spawnInfos;
    }
}