using TMPro;
using UnityEngine;

public class TextFader : MonoBehaviour
{

    float fadeSpeed = 0.1f;
    private TMP_Text _text;

    void Start()
    {
        _text = gameObject.GetComponent<TMP_Text>();
    }

    void Update()
    {
        float a = _text.alpha;
        a -= fadeSpeed * Time.deltaTime;
        _text.alpha = a;
    }
}
