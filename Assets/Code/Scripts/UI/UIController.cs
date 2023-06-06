using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Game.CustomUI;

public class UIController : MonoBehaviour
{
    private VisualElement _root;
    private UnitCardPanel _cardPanel;
    private VisualElement _popupContainer;

    private void OnEnable()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _cardPanel = _root.Q<VisualElement>("UnitCardPanel") as UnitCardPanel;
        _popupContainer = _root.Q<VisualElement>("popup-container");
        //_cardPanel.AddUnitCard(new UnitCard());
    }

    void Update()
    {
        
    }
}
