using UnityEngine;
using System.Collections;

public class BowBar : MonoBehaviour {

    private float loadBarLength, loadBarHeight;
    private float percentOfLoad;
    public Texture2D loadBar;

	// Use this for initialization
	void Start () {
        percentOfLoad = 0.0f;
    }

	void OnGUI()
    {
        loadBarLength = Screen.width / 2;
        loadBarHeight = Screen.height / 12;
        GUI.TextField(new Rect(0, loadBarHeight, loadBarHeight * 1.5f, loadBarHeight), ((int)(percentOfLoad * 100)).ToString() +"%");
        if (percentOfLoad > 0.1f)
        {
            GUI.BeginGroup(new Rect(loadBarHeight * 1.5f, loadBarHeight, percentOfLoad * loadBarLength, loadBarHeight));
            GUI.Box(new Rect(0, 0, loadBarLength, loadBarHeight), loadBar);
            GUI.EndGroup();
        }
    }

	public void UpdateLoadBar (float loadBow, float maxLoad) {
        percentOfLoad = loadBow/maxLoad;
	}
}
