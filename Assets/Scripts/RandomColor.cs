using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColor : MonoBehaviour
{
    [SerializeField] private Color[] _colors;

    private void Start()
    {
        int randomIndex = Random.Range(0, _colors.Length);
        GetComponent<Renderer>().material.color = _colors[randomIndex];
    }
}
