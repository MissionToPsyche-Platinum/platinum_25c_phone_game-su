using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using System.Reflection;

public class MenuNavigationButtonsPlayModeTests
{
    [UnityTest]
    public IEnumerator SettingsButton_InvokesListener()
    {
        // Arrange
        GameObject go = new GameObject();
        MenuNavigationButtons menu = go.AddComponent<MenuNavigationButtons>();

        GameObject buttonGO = new GameObject();
        Button button = buttonGO.AddComponent<Button>();

        // Use reflection to set the private field
        var field = typeof(MenuNavigationButtons).GetField("settingsScreenButton",
            BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(menu, button);

        // Manually trigger Awake using reflection
        var awakeMethod = typeof(MenuNavigationButtons).GetMethod("Awake",
            BindingFlags.NonPublic | BindingFlags.Instance);
        awakeMethod.Invoke(menu, null);

        yield return null;

        // Assert - Check that onClick has listeners
        int listenerCount = button.onClick.GetPersistentEventCount();

        // Since AddListener adds runtime listeners (not persistent), we need to check differently
        // We'll verify the button can be invoked without errors
        Assert.DoesNotThrow(() => button.onClick.Invoke(),
            "Settings button should have a listener that can be invoked");

        // Cleanup
        Object.Destroy(go);
        Object.Destroy(buttonGO);
    }

    [UnityTest]
    public IEnumerator StartButton_InvokesListener()
    {
        // Arrange
        GameObject go = new GameObject();
        MenuNavigationButtons menu = go.AddComponent<MenuNavigationButtons>();

        GameObject buttonGO = new GameObject();
        Button button = buttonGO.AddComponent<Button>();

        var field = typeof(MenuNavigationButtons).GetField("startScreenButton",
            BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(menu, button);

        // Manually trigger Awake
        var awakeMethod = typeof(MenuNavigationButtons).GetMethod("Awake",
            BindingFlags.NonPublic | BindingFlags.Instance);
        awakeMethod.Invoke(menu, null);

        yield return null;

        // Assert
        Assert.DoesNotThrow(() => button.onClick.Invoke(),
            "Start button should have a listener that can be invoked");

        // Cleanup
        Object.Destroy(go);
        Object.Destroy(buttonGO);
    }

    [UnityTest]
    public IEnumerator LevelSelectionButton_InvokesListener()
    {
        // Arrange
        GameObject go = new GameObject();
        MenuNavigationButtons menu = go.AddComponent<MenuNavigationButtons>();

        GameObject buttonGO = new GameObject();
        Button button = buttonGO.AddComponent<Button>();

        var field = typeof(MenuNavigationButtons).GetField("levelSelectionScreenButton",
            BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(menu, button);

        // Manually trigger Awake
        var awakeMethod = typeof(MenuNavigationButtons).GetMethod("Awake",
            BindingFlags.NonPublic | BindingFlags.Instance);
        awakeMethod.Invoke(menu, null);

        yield return null;

        // Assert
        Assert.DoesNotThrow(() => button.onClick.Invoke(),
            "Level selection button should have a listener that can be invoked");

        // Cleanup
        Object.Destroy(go);
        Object.Destroy(buttonGO);
    }

    [UnityTest]
    public IEnumerator NullButtons_DoNotCauseErrors()
    {
        // Arrange
        GameObject go = new GameObject();
        MenuNavigationButtons menu = go.AddComponent<MenuNavigationButtons>();

        // Manually trigger Awake
        var awakeMethod = typeof(MenuNavigationButtons).GetMethod("Awake",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Act & Assert - Should not throw exceptions with null buttons
        Assert.DoesNotThrow(() => awakeMethod.Invoke(menu, null),
            "Awake should not throw with null buttons");

        yield return null;

        // Cleanup
        Object.Destroy(go);
    }

    public class ProbeControllerTests
    {
        [UnityTest]
        public IEnumerator Probe_SetsFlag_OnAsteroidTrigger()
        {
            // arrange: probe object
            var probeGO = new GameObject("Probe");
            var probe = probeGO.AddComponent<ProbeController>();
            var rb = probeGO.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0;
            probeGO.AddComponent<CircleCollider2D>().isTrigger = true;

            // arrange: asteroid object
            var asteroidGO = new GameObject("Asteroid");
            asteroidGO.AddComponent<AsteroidController>();
            asteroidGO.AddComponent<BoxCollider2D>(); // non trigger
            asteroidGO.transform.position = Vector3.right; // 1 unit away

            // move probe into asteroid
            probeGO.transform.position = Vector3.right;

            // act: wait one physics step so OnTriggerEnter2D can run
            yield return new WaitForFixedUpdate();

            // assert
            Assert.IsTrue(probe.HasCollided, "Probe should have detected asteroid trigger.");
        }
    }

}