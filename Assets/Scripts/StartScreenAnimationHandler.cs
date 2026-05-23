using System;
using System.Collections;
using UnityEngine;

public class StartScreenAnimationHandler : MonoBehaviour
{
	[SerializeField] private AudioClip whooshSound;
	public bool shouldSkipAnimation = false;

	private Animator myAnimator;

	private void Awake()
	{
		myAnimator = GetComponent<Animator>();
	}

	public void PlaySound()
	{
		SFXController.instance.PlaySoundFXClip(whooshSound, this.transform, .2f);
	}

	private void OnEnable()
	{
		if (shouldSkipAnimation)
		{
			StartCoroutine(SkipToIdle());
		}
	}

	private IEnumerator SkipToIdle()
	{
		yield return null;
		myAnimator.Play("StartPanelIdle", 0, 0f);
	}
}