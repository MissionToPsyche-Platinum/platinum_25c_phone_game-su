using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DebrisTests
{
    [UnityTest]
    public IEnumerator DebrisMoves_Downward()
    {
        // Create debris
        GameObject debris = new GameObject("TestDebris");
        debris.AddComponent<DebrisMoveScript>();
        Vector3 startPos = debris.transform.position;

        yield return null;
        yield return null;

        // Check it moved down
        Assert.Less(debris.transform.position.y, startPos.y);

        Object.Destroy(debris);
    }

    [UnityTest]
    public IEnumerator DebrisSpawner_SpawnsDebris()
    {
        // Create spawner
        GameObject spawner = new GameObject("Spawner");
        DebrisSpawnScript script = spawner.AddComponent<DebrisSpawnScript>();

        // Create a simple debris prefab
        GameObject debrisPrefab = new GameObject("DebrisPrefab");
        debrisPrefab.AddComponent<DebrisMoveScript>();
        script.debris = debrisPrefab;

        yield return null;

        // Wait for a spawn
        yield return new WaitForSeconds(1.5f);

        // Check if debris was spawned
        GameObject spawnedDebris = GameObject.Find("DebrisPrefab(Clone)");
        Assert.IsNotNull(spawnedDebris);

        // Cleanup
        Object.Destroy(spawner);
        Object.Destroy(debrisPrefab);
        if (spawnedDebris != null) Object.Destroy(spawnedDebris);
    }
}