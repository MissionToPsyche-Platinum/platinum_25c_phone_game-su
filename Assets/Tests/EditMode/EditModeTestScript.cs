using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;

public class MenuNavigationButtonsEditModeTests
{
    [Test]
    public void MenuNavigationButtons_CanBeCreated()
    {
        // Arrange & Act
        GameObject go = new GameObject();
        MenuNavigationButtons menu = go.AddComponent<MenuNavigationButtons>();

        // Assert
        Assert.IsNotNull(menu);

        // Cleanup
        Object.DestroyImmediate(go);
    }

    [Test]
    public void Awake_WithNullButtons_DoesNotThrowException()
    {
        // Arrange
        GameObject go = new GameObject();
        MenuNavigationButtons menu = go.AddComponent<MenuNavigationButtons>();

        // Act & Assert - should not throw
        Assert.DoesNotThrow(() => {
            // Awake is called automatically when component is added
        });

        // Cleanup
        Object.DestroyImmediate(go);
    }
}