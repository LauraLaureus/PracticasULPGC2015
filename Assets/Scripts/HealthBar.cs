using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {

    private float healthBarLength, healthBarHeight;
    private float percentOfHealth;
    public Texture2D loadBar;

    // Use this for initialization
    void Start()
    {
        percentOfHealth = 0.0f;
    }

    void OnGUI()
    {
        healthBarLength = Screen.width / 2;
        healthBarHeight = Screen.height / 12;
        GUI.TextField(new Rect(0, 0, healthBarHeight * 1.5f, healthBarHeight), ((int)(percentOfHealth * 100)).ToString() + "%");
        GUI.BeginGroup(new Rect(healthBarHeight * 1.5f, 0, percentOfHealth * healthBarLength, healthBarHeight));
        GUI.Box(new Rect(0, 0, healthBarLength, healthBarHeight), loadBar);
        GUI.EndGroup();
    }

    public void UpdateHealthBar(float health)
    {
        percentOfHealth = health/100.0f;
    }
}
