using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Reflection;

public class PauseButtonTestScript
{
    private GameObject pauseMenuManagerObject;
    private PauseMenuManager pauseMenuManager;
    private GameObject pauseButton;
    private GameObject pauseMenuPanel;
    private GameObject resumeButton;
    private GameObject quitButton;

    [SetUp]
    public void Setup()
    {
        // Create test GameObjects
        pauseMenuManagerObject = new GameObject("PauseMenuManager");
        pauseMenuManager = pauseMenuManagerObject.AddComponent<PauseMenuManager>();

        pauseButton = new GameObject("PauseButton");
        pauseMenuPanel = new GameObject("PauseMenuPanel");
        resumeButton = new GameObject("ResumeButton");
        quitButton = new GameObject("QuitButton");

        // Use reflection to set private serialized fields
        var pauseButtonField = typeof(PauseMenuManager).GetField("pauseButton", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        var pauseMenuPanelField = typeof(PauseMenuManager).GetField("pauseMenuPanel", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        var resumeButtonField = typeof(PauseMenuManager).GetField("resumeButton", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        var quitButtonField = typeof(PauseMenuManager).GetField("quitButton", 
            BindingFlags.NonPublic | BindingFlags.Instance);

        pauseButtonField?.SetValue(pauseMenuManager, pauseButton);
        pauseMenuPanelField?.SetValue(pauseMenuManager, pauseMenuPanel);
        resumeButtonField?.SetValue(pauseMenuManager, resumeButton);
        quitButtonField?.SetValue(pauseMenuManager, quitButton);

        // Manually call Start using reflection
        var startMethod = typeof(PauseMenuManager).GetMethod("Start",
            BindingFlags.NonPublic | BindingFlags.Instance);
        startMethod?.Invoke(pauseMenuManager, null);
    }

    [TearDown]
    public void Teardown()
    {
        // Clean up test objects
        Object.DestroyImmediate(pauseMenuManagerObject);
        Object.DestroyImmediate(pauseButton);
        Object.DestroyImmediate(pauseMenuPanel);
        Object.DestroyImmediate(resumeButton);
        Object.DestroyImmediate(quitButton);

        // Reset time scale
        Time.timeScale = 1f;
    }

    [Test]
    public void PauseMenuManager_CanBeCreated()
    {
        // Arrange & Act
        GameObject go = new GameObject();
        PauseMenuManager manager = go.AddComponent<PauseMenuManager>();

        // Assert
        Assert.IsNotNull(manager);

        // Cleanup
        Object.DestroyImmediate(go);
    }

    [Test]
    public void InitialState_PauseButtonVisible_MenuHidden()
    {
        // Assert
        Assert.IsTrue(pauseButton.activeSelf, "Pause button should be visible initially");
        Assert.IsFalse(pauseMenuPanel.activeSelf, "Pause menu panel should be hidden initially");
        Assert.AreEqual(1f, Time.timeScale, "Time scale should be 1 (unpaused) initially");
    }

    [Test]
    public void PauseGame_FreezesTime()
    {
        // Act
        pauseMenuManager.PauseGame();

        // Assert
        Assert.AreEqual(0f, Time.timeScale, "Time scale should be 0 when paused");
    }

    [Test]
    public void PauseGame_ShowsMenuAndHidesButton()
    {
        // Act
        pauseMenuManager.PauseGame();

        // Assert
        Assert.IsFalse(pauseButton.activeSelf, "Pause button should be hidden when paused");
        Assert.IsTrue(pauseMenuPanel.activeSelf, "Pause menu panel should be visible when paused");
    }

    [Test]
    public void ResumeGame_RestoresTimeScale()
    {
        // Arrange
        pauseMenuManager.PauseGame();

        // Act
        pauseMenuManager.ResumeGame();

        // Assert
        Assert.AreEqual(1f, Time.timeScale, "Time scale should be 1 when resumed");
    }

    [Test]
    public void ResumeGame_ShowsButtonAndHidesMenu()
    {
        // Arrange
        pauseMenuManager.PauseGame();

        // Act
        pauseMenuManager.ResumeGame();

        // Assert
        Assert.IsTrue(pauseButton.activeSelf, "Pause button should be visible when resumed");
        Assert.IsFalse(pauseMenuPanel.activeSelf, "Pause menu panel should be hidden when resumed");
    }

    [Test]
    public void PauseAndResume_TogglesBetweenStates()
    {
        // Act & Assert - First pause
        pauseMenuManager.PauseGame();
        Assert.AreEqual(0f, Time.timeScale);
        Assert.IsFalse(pauseButton.activeSelf);
        Assert.IsTrue(pauseMenuPanel.activeSelf);

        // Act & Assert - Resume
        pauseMenuManager.ResumeGame();
        Assert.AreEqual(1f, Time.timeScale);
        Assert.IsTrue(pauseButton.activeSelf);
        Assert.IsFalse(pauseMenuPanel.activeSelf);

        // Act & Assert - Pause again
        pauseMenuManager.PauseGame();
        Assert.AreEqual(0f, Time.timeScale);
        Assert.IsFalse(pauseButton.activeSelf);
        Assert.IsTrue(pauseMenuPanel.activeSelf);
    }

    [Test]
    public void MultiplePauseCalls_MaintainsPausedState()
    {
        // Act
        pauseMenuManager.PauseGame();
        pauseMenuManager.PauseGame();
        pauseMenuManager.PauseGame();

        // Assert
        Assert.AreEqual(0f, Time.timeScale, "Time scale should remain 0 after multiple pause calls");
        Assert.IsFalse(pauseButton.activeSelf);
        Assert.IsTrue(pauseMenuPanel.activeSelf);
    }

    [Test]
    public void MultipleResumeCalls_MaintainsResumedState()
    {
        // Arrange
        pauseMenuManager.PauseGame();

        // Act
        pauseMenuManager.ResumeGame();
        pauseMenuManager.ResumeGame();
        pauseMenuManager.ResumeGame();

        // Assert
        Assert.AreEqual(1f, Time.timeScale, "Time scale should remain 1 after multiple resume calls");
        Assert.IsTrue(pauseButton.activeSelf);
        Assert.IsFalse(pauseMenuPanel.activeSelf);
    }
}
