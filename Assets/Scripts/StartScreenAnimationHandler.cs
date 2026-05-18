using System;
using UnityEngine;

public class StartScreenAnimationHandler : MonoBehaviour
{
	[SerializeField] private AudioClip whooshSound;
	public bool shouldSlide = false;

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
		Debug.Log("OnEnable heard");
		if (shouldSlide)
		{
			Debug.Log("OnEnable heard and shouldSlide = true");
			Debug.Log(myAnimator.speed);
			myAnimator.SetTrigger("QueueSlide");
		}
	}
}