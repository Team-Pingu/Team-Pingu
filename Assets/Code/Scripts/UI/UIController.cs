using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    private VisualElement _root;
    private UnitCardPanel _cardPanel;

    private void OnEnable()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _cardPanel = _root.Q<UnitCardPanel>("UnitCardPanel");
    }

    void Update()
    {
        
    }
}
