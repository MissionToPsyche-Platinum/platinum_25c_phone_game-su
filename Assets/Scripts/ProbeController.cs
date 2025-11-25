using UnityEngine;
using UnityEngine.InputSystem;

public class ProbeController : MonoBehaviour
{
    private InputSystem_Actions myInputActions;
    private bool shouldMoveToFinger;

    private void Awake() {
        shouldMoveToFinger = false;
        myInputActions = new InputSystem_Actions();

        myInputActions.Player.Enable();

        myInputActions.Player.FingerDown.performed += FingerDown_performed;
        myInputActions.Player.FingerDown.canceled += FingerDown_canceled;
    }

    //when the finger is pressed, the probe should move to it
    private void FingerDown_performed(InputAction.CallbackContext obj) {
        shouldMoveToFinger = true;
    }
    //when the finger is lifted, the prove should stop moving to it
    private void FingerDown_canceled(InputAction.CallbackContext obj) {
        shouldMoveToFinger = false;
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.GetComponent<AsteroidController>() != null)
        {
            //if the collided object is asteroid
            Debug.Log("Collision with asteroid!");
        }

    }

    private void Update() {
        if (shouldMoveToFinger) {
            //set the position of this game object to that of the finger position
            Vector2 screenPosition = myInputActions.Player.PointPosition.ReadValue<Vector2>();
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            transform.position = new Vector3(worldPosition.x, worldPosition.y, 0);
        }
    }
}
