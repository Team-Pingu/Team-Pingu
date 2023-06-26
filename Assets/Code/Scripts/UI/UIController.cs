using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Game.CustomUI;
using Game.CustomUI.Seed;
using Code.Scripts.Player;

enum ModalType
{
    UpgradeModal,
    AttackerInitModal,
    DefenderInitModal
}

public class UIController : MonoBehaviour
{
    [SerializeField]
    private bool UseSeedInitializer = false;
    public bool IsUpgradeMenuOpen;

    private VisualElement _root;
    private UnitCardPanel _cardPanel;
    private VisualElement _popupContainer;
    private VisualElement _upgradeMenu;
    private Button _upgradeMenuOpenButton;
    private Button _upgradeMenuCloseButton;

    private readonly string UPGRADE_MODAL_NAME = "game-upgrade-popup";
    private readonly string ATTACKER_INIT_MODAL_NAME = "game-start-popup-attacker";
    private readonly string DEFENDER_INIT_MODAL_NAME = "game-start-popup-defender";

    private Player _player;

    private void OnEnable()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        _cardPanel = _root.Q<VisualElement>("unit-card-panel") as UnitCardPanel;
        _popupContainer = _root.Q<VisualElement>("popup-container");
        _upgradeMenuOpenButton = _root.Q<Button>("player-controls__upgrade-btn");
        _upgradeMenuCloseButton = _root.Q<Button>("game-upgrade-popup__actions__close");
        _upgradeMenu = _root.Q<VisualElement>(UPGRADE_MODAL_NAME);

        _upgradeMenuOpenButton.RegisterCallback<ClickEvent, VisualElement>(OnUpgradeMenuOpenClick, _upgradeMenu);
        _upgradeMenuCloseButton.RegisterCallback<ClickEvent>(OnUpgradeMenuCloseClick);
        _root.Q<Button>($"{ATTACKER_INIT_MODAL_NAME}__actions__close").RegisterCallback<ClickEvent>(OnModalClose);
        _root.Q<Button>($"{DEFENDER_INIT_MODAL_NAME}__actions__close").RegisterCallback<ClickEvent>(OnModalClose);
        _root.Q<VisualElement>("static")?.SendToBack();

        _player = GameObject.Find("Player").GetComponent<Player>();

        InitSeed();
    }

    private void InitSeed()
    {
        if (!UseSeedInitializer) return;
        if (_player.PlayerController == null) return;

        ISeed Seed;
        if (_player.Role == PlayerRole.Attacker)
        {
            _cardPanel.UseSingleSelectionOnly = false;
            Seed = new AttackerSeed();
        }
        else
        {
            _cardPanel.UseSingleSelectionOnly = true;
            Seed = new DefenderSeed();
        }
        Seed.InflateUI(_root);
    }

    void Update()
    {

    }

    #region Events
    private void OnUpgradeMenuOpenClick(ClickEvent e, VisualElement vs)
    {
        Debug.Log("Open");
        if (IsUpgradeMenuOpen) return;
        IsUpgradeMenuOpen = true;
        OpenModal(ModalType.UpgradeModal);
    }

    private void OnUpgradeMenuCloseClick(ClickEvent e)
    {
        Debug.Log("Close");
        if (!IsUpgradeMenuOpen) return;
        IsUpgradeMenuOpen = false;
        OnModalClose(e);
    }

    private void OnModalClose(ClickEvent e)
    {
        _popupContainer.style.display = DisplayStyle.None;
    }
    #endregion

    #region Methods
    private string GetModalNameFromType(ModalType modalType)
    {
        if (modalType == ModalType.UpgradeModal)
        {
            return UPGRADE_MODAL_NAME;
        }
        else if (modalType == ModalType.AttackerInitModal)
        {
            return ATTACKER_INIT_MODAL_NAME;
        }
        else if (modalType == ModalType.DefenderInitModal)
        {
            return DEFENDER_INIT_MODAL_NAME;
        }
        else
        {
            return "";
        }
    }
    private void OpenModal(ModalType modalType)
    {
        _popupContainer.style.display = DisplayStyle.Flex;
        foreach (VisualElement ve in _popupContainer.Children())
        {
            if (ve.name == GetModalNameFromType(modalType))
            {
                ve.style.display = DisplayStyle.Flex;
            } else
            {
                ve.style.display = DisplayStyle.None;
            }
        }

        // TODO: implement timer close
    }
    #endregion
}
