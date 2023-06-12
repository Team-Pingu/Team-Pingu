using Game.CustomUI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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

        private readonly string VIEW_ASSET_PATH = "Assets\\Level\\UI\\Additional UI Elements\\Scripts\\UpgradeElement\\UpgradeElement.uxml";

        private Label _costLabel;
        private VisualElement _image;
        private VisualElement _backgroundDefault;
        private VisualElement _backgroundSelected;
        private VisualElement _lockedOverlay;
        private VisualElement _mainContainer;
        private VisualElement _tierContainer;
        private Label _tierLabel;
        public UnitCardPanel ParentUnitCardPanel;

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
        }

        public UpgradeElement(string name, string description, int cost, string tier)
        {
            Init();

            Name = name;
            Description = description;
            SetCost(cost);
            SetTier(tier);
        }

        private void Init()
        {
            // load view and set values to view
            VisualTreeAsset viewAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(VIEW_ASSET_PATH);

            viewAsset.CloneTree(this);

            _costLabel = this.Q<Label>("upgrade-element__cost__text");
            _image = this.Q<VisualElement>("upgrade-element__content");
            _backgroundDefault = this.Q<VisualElement>("upgrade-element__bg-default");
            _backgroundSelected = this.Q<VisualElement>("upgrade-element__bg-selected");
            _mainContainer = this.Q<VisualElement>("upgrade-element-container");
            _tierContainer = this.Q<VisualElement>("upgrade-element__tier");
            _tierLabel = this.Q<Label>("upgrade-element__tier__text");
            _lockedOverlay = this.Q<VisualElement>("upgrade-element__locked-overlay");

            _mainContainer.RegisterCallback<MouseDownEvent>(OnMouseClick);
        }

        #region Events

        private void OnMouseClick(MouseDownEvent e)
        {
            if (e.button == (int)MouseButton.LeftMouse)
            {
                this.Select();
                this.Buy();
            }

            if (e.button == (int)MouseButton.RightMouse)
            {
                this.Deselect();
                this.Sell();
            }
        }
        #endregion

        #region Methods
        private bool IsAffordable(int globalCurrencyAmount)
        {
            if (this.Cost <= globalCurrencyAmount) return true;
            return false;
        }

        private void Select()
        {
            if (IsLocked) return;
            if (IsSelected) return;
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

        private void Buy()
        {
            if (IsLocked) return;
            if (IsBought) return;
            if (!IsAffordable(999)) return;
            IsBought = true;
            // TODO: subtract money!
        }

        private void Sell()
        {
            if (IsLocked) return;
            if (!IsBought) return;
            IsBought = false;
            // TODO: add money to game state!
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
