using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Game.CustomUI;
using UnityEditor.VersionControl;

public class UIController : MonoBehaviour
{
    public bool IsUpgradeMenuOpen;

    private VisualElement _root;
    private UnitCardPanel _cardPanel;
    private VisualElement _popupContainer;
    private VisualElement _upgradeMenu;
    private Button _upgradeButton;

    private void OnEnable()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _cardPanel = _root.Q<VisualElement>("UnitCardPanel") as UnitCardPanel;
        _popupContainer = _root.Q<VisualElement>("popup-container");
        //_cardPanel.AddUnitCard(new UnitCard());

        _upgradeButton = _root.Q<Button>("upgrade-btn");
        _upgradeMenu = _root.Q<VisualElement>("game-upgrade-popup");
        _upgradeButton.RegisterCallback<ClickEvent, VisualElement>(OnClick, _upgradeMenu);
    }

    void Update()
    {
        
    }

    #region Events
    private void OnClick(ClickEvent e, VisualElement vs)
    {
        Debug.Log("Click");
        if (IsUpgradeMenuOpen) return;
        IsUpgradeMenuOpen = true;
        vs.style.display = DisplayStyle.Flex;
    }
    #endregion
}
