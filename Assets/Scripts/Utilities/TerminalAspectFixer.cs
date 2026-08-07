using UnityEngine;

public class terminalAspectFixer : MonoBehaviour
{
	private float targetAspect = 16f/ 9f;
	
	void Awake()
	{
		Camera cam = GetComponent<Camera>();
		float currentAspect = (float)Screen.width / Screen.height;
		float scale = currentAspect / targetAspect;
		if(scale < 1.0f)
		{
			Rect rect = cam.rect;
			rect.width = 1.0f;
			rect.height = scale;
			rect.x = 0;
			rect.y = (1.0f - scale) / 2.0f;
			cam.rect = rect;
		}
	}
}
