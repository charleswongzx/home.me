using UnityEngine;
using System.Collections;

public class ActivateOnEnable : MonoBehaviour {

	public EasyTween EasyTweenStart;

	private IEnumerator Start () 
	{
		yield return new WaitForEndOfFrame();
		EasyTweenStart.OpenCloseObjectAnimation();
	}
}
