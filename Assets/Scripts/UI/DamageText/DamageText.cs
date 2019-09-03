using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    public void SetValue(float value)
    {
        GetComponentInChildren<Text>().text = string.Format("{0:0}", value);
    }

    public void DestroyText()
    {
        Destroy(gameObject);
    }
}
