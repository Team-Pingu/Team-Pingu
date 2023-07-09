using Code.Scripts.Player;
using Code.Scripts;
using Game.CustomUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using Game.UI;

namespace Game.CustomUI
{
    public class UpgradeElement : VisualElement
    {
        #region Boilerplate Component Code
        [UnityEngine.Scripting.Preserve]
        public new class UxmlFactory : UxmlFactory<UpgradeElement, UxmlTraits> { }

        [UnityEngine.Scripting.Preserve]
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription tier = new UxmlStringAttributeDescription { name = "tier", defaultValue = "" };
            private readonly UxmlBoolAttributeDescription bought = new UxmlBoolAttributeDescription { name = "bought", defaultValue = false };
            private readonly UxmlIntAttributeDescription cost = new UxmlIntAttributeDescription { name = "cost", defaultValue = 499 };
            private readonly UxmlBoolAttributeDescription locked = new UxmlBoolAttributeDescription { name = "locked", defaultValue = false };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var item = ve as UpgradeElement;
                string _tier = tier.GetValueFromBag(bag, cc);
                bool _bought = bought.GetValueFromBag(bag, cc);
                int _cost = cost.GetValueFromBag(bag, cc);
                bool _locked = locked.GetValueFromBag(bag, cc);

                item.SetCost(_cost);
                item.SetTier(_tier);
                if (_bought)
                {
                    item.Buy();
                    item.Select();
                }
                else
                {
                    item.Sell();
                    item.Deselect();
                }

                if (_locked)
                {
                    item.Lock();
                }
                else
                {
                    item.Unlock();
                }
            }
        }
        #endregion

        public string Name;// { get; private set; }
        public string Description;
        public int Cost;
        public string Tier { get; private set; } = "";
        public bool IsSelected { get; private set; } = false;
        public bool IsBought { get; private set; } = false;
        public bool IsLocked { get; private set; } = false;
        public bool IsSpecializationUpgrade { get; private set; } = false;

        public UpgradeElement PreviousChainElement { get; private set; } = null;
        public UpgradeElement NextChainElement { get; private set; } = null;

        private readonly string VIEW_ASSET_PATH = "Assets/Level/UI/Additional UI Elements/Scripts/UpgradeElement/UpgradeElement.uxml";
        private readonly float SELL_PENALTY = 0.6f;

        private Label _costLabel;
        private VisualElement _image;
        private VisualElement _backgroundDefault;
        private VisualElement _backgroundSelected;
        private VisualElement _lockedOverlay;
        private VisualElement _mainContainer;
        private VisualElement _tierContainer;
        private Label _tierLabel;
        public UnitCardPanel ParentUnitCardPanel;
        private PopupPanelCustom _popupPanel;

        private UpgradeManager _upgradeManager;
        public Action<UpgradeManager> BuyAction;
        public Action<UpgradeManager> SellAction;
        private Bank _bank;

        public override VisualElement contentContainer => _mainContainer;

        public UpgradeElement()
        {
            Init();

            //Name = _nameLabel.text;
            //Description = _descriptionLabel.text;
            int _cost;
            bool isParsed = int.TryParse(_costLabel.text, out _cost);
            Cost = _cost;
            Tier = _tierLabel.text;
            IsLocked = _lockedOverlay.style.display == DisplayStyle.Flex;
            IsSelected = _backgroundSelected.style.display == DisplayStyle.Flex;
            IsBought = IsSelected;
            IsSpecializationUpgrade = string.IsNullOrEmpty(Tier);
        }

        public UpgradeElement(string name, string description, int cost, Action<UpgradeManager> buyAction = null, Action<UpgradeManager> sellAction = null, string tier = null)
        {
            Init();

            Name = name;
            Description = description;
            SetCost(cost);
            SetTier(tier);
            BuyAction = buyAction;
            SellAction = sellAction;
            IsSpecializationUpgrade = string.IsNullOrEmpty(tier);

            var popupExtraInfo = IsSpecializationUpgrade ? "This is a Specialization, it cannot be sold!" : $"Upgrades can be sold for {SELL_PENALTY * 100}% of their original value.";
            _popupPanel = new PopupPanelCustom(name, description, this, popupExtraInfo);
        }

        public void SetChainedElements(UpgradeElement previous, UpgradeElement next)
        {
            if (previous == null) Unlock();
            else Lock();
            PreviousChainElement = previous;
            NextChainElement = next;
        }

        private void Init()
        {
            // load view and set values to view
            VisualTreeAsset viewAsset;
            var __viewAssetResource = new GameResource(VIEW_ASSET_PATH, null, GameResourceType.UI);
            viewAsset = __viewAssetResource.LoadRessource<VisualTreeAsset>();
            viewAsset.CloneTree(this);

            _upgradeManager = GameObject.Find("UpgradeManager")?.GetComponent<UpgradeManager>();

            _costLabel = this.Q<Label>("upgrade-element__cost__text");
            _image = this.Q<VisualElement>("upgrade-element__content");
            _backgroundDefault = this.Q<VisualElement>("upgrade-element__bg-default");
            _backgroundSelected = this.Q<VisualElement>("upgrade-element__bg-selected");
            _mainContainer = this.Q<VisualElement>("upgrade-element-container");
            _tierContainer = this.Q<VisualElement>("upgrade-element__tier");
            _tierLabel = this.Q<Label>("upgrade-element__tier__text");
            _lockedOverlay = this.Q<VisualElement>("upgrade-element__locked-overlay");

            _mainContainer.RegisterCallback<MouseDownEvent>(OnMouseClick);
            _mainContainer.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            _mainContainer.RegisterCallback<MouseLeaveEvent>(OnMouseExit);

            _bank = GameObject.Find("Player").GetComponent<Bank>();
            _bank.OnBalanceChanged += currentBalance => IsAffordable(currentBalance);
            this.CaptureMouse();
        }

        #region Events

        private void OnMouseClick(MouseDownEvent e)
        {
            if (e.button == (int)UnityEngine.UIElements.MouseButton.LeftMouse)
            {
                Buy();
            }

            if (e.button == (int)UnityEngine.UIElements.MouseButton.RightMouse)
            {
                Sell();
            }
        }

        private void OnMouseEnter(MouseEnterEvent e)
        {
            Debug.Log("ENTER");
            _popupPanel.SetScreenPos(this);
            _popupPanel.Show();
        }

        private void OnMouseExit(MouseLeaveEvent e)
        {
            Debug.Log("EXIT");
            _popupPanel.Hide();
        }
        #endregion

        #region Methods
        private bool IsAffordable(int globalCurrencyAmount)
        {
            bool isAffordable = Cost <= globalCurrencyAmount;
            _costLabel.style.backgroundColor = new StyleColor(isAffordable ? new Color(0, 0, 0, 0) : Color.red);
            return isAffordable;
        }

        private void Select()
        {
            if (IsLocked) return;
            if (IsSelected) return;
            if (!IsAffordable(_bank.CurrentBalance)) return;
            IsSelected = true;
            _backgroundDefault.style.display = DisplayStyle.None;
            _backgroundSelected.style.display = DisplayStyle.Flex;
        }

        private void Deselect()
        {
            if (IsLocked) return;
            if (!IsSelected) return;
            IsSelected = false;
            _backgroundDefault.style.display = DisplayStyle.Flex;
            _backgroundSelected.style.display = DisplayStyle.None;
        }

        private bool Buy()
        {
            if (IsLocked) return false;
            if (IsBought) return false;
            if (!IsAffordable(_bank.CurrentBalance)) return false;
            Select();
            IsBought = true;
            BuyAction?.Invoke(_upgradeManager);
            _bank.Withdraw(Cost);
            NextChainElement?.Unlock();
            return true;
        }

        private bool Sell()
        {
            if (IsLocked) return false;
            if (!IsBought) return false;
            if (IsSpecializationUpgrade) return false;
            if (NextChainElement != null && NextChainElement.IsBought) return false;
            Deselect();
            IsBought = false;
            SellAction?.Invoke(_upgradeManager);
            _bank.Deposit(Mathf.RoundToInt(Cost * SELL_PENALTY));
            NextChainElement?.Lock();
            return true;
        }

        public void SetTier(string tier)
        {
            if (string.IsNullOrEmpty(tier))
            {
                _tierContainer.visible = false;
            }
            else
            {
                _tierContainer.visible = true;
                _tierLabel.text = tier;
            }
            Tier = tier;
        }

        public void SetCost(int cost)
        {
            _costLabel.text = $"{cost}";
            Cost = cost;
        }

        public void Lock()
        {
            if (IsLocked) return;
            IsLocked = true;
            _lockedOverlay.style.display = DisplayStyle.Flex;
        }

        public void Unlock()
        {
            if (!IsLocked) return;
            IsLocked = false;
            _lockedOverlay.style.display = DisplayStyle.None;
        }
        #endregion
    }
}
