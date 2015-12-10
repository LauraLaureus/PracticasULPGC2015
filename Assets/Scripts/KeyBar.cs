using UnityEngine;
using System.Collections;

public class KeyBar : MonoBehaviour {
    public Texture2D keyImage;
    private bool haveKey;

    void OnGUI()
    {
        if (haveKey)
        {
            GUI.Box(new Rect(Screen.width - Screen.width / 8, 0, Screen.width / 8, Screen.height / 12), keyImage);
        }
    }

    public void UpdateKeyBar(bool haveKey)
    {
        this.haveKey = haveKey;
    }
}
