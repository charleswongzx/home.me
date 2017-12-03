using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CreateAnimImage : MonoBehaviour {

	public CreateAnimImage[] createImageOtherReference;

	public GameObject CreateInstance;

	public int HowManyButtons;

	public Vector3 StartAnim;
	public Vector3 EndAnim;

	public float Offset;

	public AnimationCurve EnterAnim;
	public AnimationCurve ExitAnim;

	public RectTransform RootRect;
	public RectTransform RootCanvas;

	private List<EasyTween> Created = new List<EasyTween>();

	private Vector2 InitialCanvasScrollSize;
	private float totalWidth = 0f;

	void Start()
	{
		InitialCanvasScrollSize = new Vector2(RootRect.rect.height, RootRect.rect.width);
	}

	public void CallBack()
	{
		if (Created.Count == 0)
		{
			for (int i = 0; i < createImageOtherReference.Length; i++)
			{
				createImageOtherReference[i].DestroyButtons();
			}

			CreateButtons();
		}
	}

	public void DestroyButtons()
	{
		for (int i = 0; i < Created.Count; i++)
		{
			Created[i].OpenCloseObjectAnimation();
		}

		Created.Clear();
	}

	public void CreateButtons()
	{
		CreatePanels();
		AdaptCanvas();
	}

	private void CreatePanels()
	{
		Vector3 InstancePosition = EndAnim;

		totalWidth = 0f;

		for (int i = 0; i < HowManyButtons; i++)
		{
			// Creates Instance
			GameObject createInstance = Instantiate(CreateInstance) as GameObject;
			// Changes the Parent, Assing to scroll List
			createInstance.transform.SetParent(RootRect, false);
			EasyTween easy = createInstance.GetComponent<EasyTween>();
			// Add Tween To List
			Created.Add(easy);
			// Final Position
			StartAnim.y = InstancePosition.y;
			// Pass the positions to the Tween system
			easy.SetAnimationPosition(StartAnim, InstancePosition , EnterAnim, ExitAnim);
			// Intro fade
			easy.SetFade();
			// Execute Animation
			easy.OpenCloseObjectAnimation();
			// Increases the Y offset
			InstancePosition.y += Offset;

			totalWidth += Offset;
		}
	}

	private void AdaptCanvas()
	{
		// Vertical Dynamic Adapter
		if (InitialCanvasScrollSize.x < Mathf.Abs(totalWidth) )
		{
			RootRect.offsetMin = new Vector2(RootRect.offsetMin.x, totalWidth + InitialCanvasScrollSize.x + RootRect.offsetMax.y);
		}
	}
}