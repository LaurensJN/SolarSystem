using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowValueScript : MonoBehaviour
{
    Text displayText;
    string baseText;

    // Start is called before the first frame update
    void Start()
    {
        displayText = GetComponent<Text>();
        baseText = displayText.text;
    }

    // Update is called once per frame
    public void TextUpdate(float value)
    {
        displayText.text = $"{baseText}{value}";
    }

    public void TextIntUpdate(float value)
    {
        displayText.text = $"{baseText}{Mathf.RoundToInt(value)}";

    }
}
