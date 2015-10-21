using UnityEngine;
using System.Collections;

public class ArrowBar : MonoBehaviour {

    private float loadBarLength;
    private float percentOfLoad;
    public Texture2D loadBar;

    void OnEnable()
    {
        ArrowController.OnBarUpdate += UpdateBar;
    }
	// Use this for initialization
	void Start () {
        loadBarLength = Screen.width / 2;
        percentOfLoad = 0.0f;
    }

	void OnGUI()
    {
        if(percentOfLoad > 0.1f)
        {
            loadBarLength = Screen.width / 2;
            GUI.TextField(new Rect(0, 0, 32, 32), (percentOfLoad*100).ToString());

            GUI.BeginGroup(new Rect(32, 0, percentOfLoad * loadBarLength, 32));
            GUI.Box(new Rect(0, 0, loadBarLength, 32), loadBar);
            GUI.EndGroup();
        }
    }

	void UpdateBar (float loadBow, float maxLoad) {
        percentOfLoad = loadBow/maxLoad;
	}

    void OnDisable()
    {
        ArrowController.OnBarUpdate -= UpdateBar;
    }
}
