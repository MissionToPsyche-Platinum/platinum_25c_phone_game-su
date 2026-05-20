using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class IntroText : MonoBehaviour
{
	private Animator myAnimator;
	[SerializeField] private Animator startScreenAnimator;
	[SerializeField] private GameObject introPanel;

	private bool skippedClicked = false;

	private void Awake()
	{
		myAnimator = GetComponent<Animator>();
		startScreenAnimator.SetTrigger("HidePanel");
	}

	private void Update()
	{
		if (skippedClicked)
		{
			return;
		}
		
		bool isHolding =
			Mouse.current != null && Mouse.current.leftButton.isPressed
			||
			Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed;

		myAnimator.speed = isHolding ? 5f : 1.5f;
	}
	
	public void EndScroll()
	{
		Debug.Log("End Scroll called");
		myAnimator.speed = 1.5f;
		introPanel.SetActive(false);
		startScreenAnimator.SetTrigger("QueueSlide");
		StartScreenAnimationHandler ssAH =  startScreenAnimator.GetComponent<StartScreenAnimationHandler>();
		ssAH.shouldSkipAnimation = true;
	}

	public void SkipAnimation()
	{
		Debug.Log("button pressed");
		skippedClicked = true;
		myAnimator.speed = 100f;
	}
}