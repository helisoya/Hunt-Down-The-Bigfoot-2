using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class PlayerNameText : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;

    void Awake()
    {
        text = GetComponent<TextMeshPro>();
    }
    void Update()
    {
        if (Player.local == null) return;
        transform.LookAt(Player.local.transform);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);
    }

    public void SetText(string value)
    {
        text.text = value;
    }

}
