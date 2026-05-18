using UnityEngine;

public class PlayWhooshSound : MonoBehaviour
{
	[SerializeField] private AudioClip whooshSound;

	public void PlaySound()
	{
		SFXController.instance.PlaySoundFXClip(whooshSound, this.transform, .2f);
	}
	
}