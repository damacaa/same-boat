using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RuleIcon : MonoBehaviour
{
    public GameObject A, A2, B, Icon;
    public TextMeshProUGUI Text;

    private void Start()
    {
        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}
