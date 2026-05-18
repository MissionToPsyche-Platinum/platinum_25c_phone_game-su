using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class IntroText : MonoBehaviour
{
	private Animator myAnimator;
	[SerializeField] private Animator startScreenAnimator;
	[SerializeField] private GameObject introPanel;

	private void Awake()
	{
		myAnimator = GetComponent<Animator>();
	}

	private void Update()
	{
		bool isHolding =
			Mouse.current != null && Mouse.current.leftButton.isPressed
			||
			Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed;

		myAnimator.speed = isHolding ? 5f : 1.5f;
	}
	
	public void EndScroll()
	{
		myAnimator.speed = 1.5f;
		introPanel.SetActive(false);
		startScreenAnimator.SetTrigger("QueueSlide");
	}
}