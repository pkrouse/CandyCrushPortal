using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BlockValues : MonoBehaviour
{
    private int value;
    public Text text;
    public Text coordinateText;
    public void Init(int v)
    {
        value = v;
        text.text = v.ToString();
    }

    public int GetValue()
    {
        return value;
    }

    // Only called when zapping blocks.
    public void ClearValue()
    {
        value = 0;
    }

    public void Explode()
    {
        StartCoroutine(BlowUp());
    }
    private IEnumerator BlowUp()
    {
        yield return new WaitForSeconds(0.01f);
        Destroy(gameObject);
    }
}
