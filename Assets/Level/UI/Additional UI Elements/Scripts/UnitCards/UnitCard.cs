using Code.Scripts;
using Code.Scripts.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using MouseButton = UnityEngine.UIElements.MouseButton;

namespace Game.CustomUI
{
    public class UnitCard : VisualElement
    {
        #region Boilerplate Component Code
        [UnityEngine.Scripting.Preserve]
        public new class UxmlFactory : UxmlFactory<UnitCard, UxmlTraits> { }
        #endregion

        public string Name;
        public string Description;
        public int Cost;
        public int SelectedUnitsAmount = 0;

        public static readonly int MAX_UNITS_SELECTABLE = 10;
        public static readonly int UNIT_SELECT_STEPS = 1;
        private readonly string VIEW_ASSET_PATH = "Assets/Level/UI/Additional UI Elements/Scripts/UnitCards/UnitCard.uxml";

        private Label _nameLabel;
        private Label _descriptionLabel;
        private Label _costLabel;
        private VisualElement _image;
        private VisualElement _backgroundDefault;
        private VisualElement _backgroundSelected;
        private VisualElement _selectCounter;
        private Label _selectCounterText;
        private VisualElement _mainContainer;
        public UnitCardPanel ParentUnitCardPanel;

        public GameResource _unitGameResource { get; private set; }
        private Bank _bank;

        public override VisualElement contentContainer => _mainContainer;

        public UnitCard()
        {
            Init();

            Name = _nameLabel.text;
            Description = _descriptionLabel.text;
            bool isParsed = int.TryParse(_costLabel.text, out Cost);
        }

        public UnitCard(string name, string description, int cost, GameResource gameResource)
        {
            Init();

            Name = name;
            Description = description;
            Cost = cost;
            _unitGameResource = gameResource;

            _nameLabel.text = name;
            _descriptionLabel.text = description;
            _costLabel.text = $"{cost}";
        }

        private void Init()
        {
            // load view and set values to view
            VisualTreeAsset viewAsset;
            var __viewAssetResource = new GameResource(VIEW_ASSET_PATH, null, GameResourceType.UI);
            viewAsset = __viewAssetResource.LoadRessource<VisualTreeAsset>();
            viewAsset.CloneTree(this);

            _nameLabel = this.Q<Label>("unit-card__header__title");
            _descriptionLabel = this.Q<Label>("unit-card__body__description__text");
            _costLabel = this.Q<Label>("unit-card__header__cost__text");
            _image = this.Q<VisualElement>("unit-card__body__image").Children().ElementAt(0);
            _backgroundDefault = this.Q<VisualElement>("unit-card-bg--default");
            _backgroundSelected = this.Q<VisualElement>("unit-card-bg--selected");
            _selectCounter = this.Q<VisualElement>("unit-card__select-counter");
            _selectCounterText = this.Q<Label>("unit-card__select-counter__text");
            _mainContainer = this.Q<VisualElement>("unit-card-container");

            _mainContainer.RegisterCallback<MouseDownEvent>(OnMouseClick);

            _bank = GameObject.Find("Player").GetComponent<Bank>();
            _bank.OnBalanceChanged += currentBalance => IsAffordable(currentBalance);
        }

        #region Events

        private void OnMouseClick(MouseDownEvent e)
        {
            if (e.button == (int)MouseButton.LeftMouse)
            {
                if (Select()) Buy();
            }

            if (e.button == (int)MouseButton.RightMouse)
            {
                if (Deselect()) Sell();
            }
        }
        #endregion

        private bool IsAffordable(int globalCurrencyAmount)
        {
            bool isAffordable = Cost <= globalCurrencyAmount;
            _costLabel.style.backgroundColor = new StyleColor(isAffordable ? new Color(0, 0, 0, 0) : Color.red);
            return isAffordable;
        }

        private void SpawnUnit(int amount = 1)
        {
            // spawn unit on level grid
        }

        private bool Select()
        {
            int _MAX_UNITS_SELECTABLE;

            if (ParentUnitCardPanel != null && ParentUnitCardPanel.UseSingleSelectionOnly)
            {
                _MAX_UNITS_SELECTABLE = 1;
            }
            else
            {
                _MAX_UNITS_SELECTABLE = MAX_UNITS_SELECTABLE;
            }

            if (SelectedUnitsAmount >= _MAX_UNITS_SELECTABLE)
            {
                return false;
            }

            if (!IsAffordable(_bank.CurrentBalance))
            {
                return false;
            }

            if (ParentUnitCardPanel != null && ParentUnitCardPanel.UseSingleSelectionOnly)
            {
                ParentUnitCardPanel.DeselectAllUnits(true);
            }

            if (SelectedUnitsAmount == 0)
            {
                // only if state changes from 0 -> 1
                _backgroundDefault.style.display = DisplayStyle.None;
                _backgroundSelected.style.display = DisplayStyle.Flex;
                _selectCounter.style.display = DisplayStyle.Flex;
            }

            SelectedUnitsAmount += UNIT_SELECT_STEPS;
            if (ParentUnitCardPanel != null) ParentUnitCardPanel.SetSelectedUnits(UNIT_SELECT_STEPS);
            _selectCounterText.text = $"x{SelectedUnitsAmount}";
            return true;
        }

        private bool Deselect()
        {
            if (SelectedUnitsAmount <= 0)
            {
                return false;
            }

            if (SelectedUnitsAmount - UNIT_SELECT_STEPS <= 0)
            {
                // only if state changes from 1 -> 0
                _backgroundDefault.style.display = DisplayStyle.Flex;
                _backgroundSelected.style.display = DisplayStyle.None;
                _selectCounter.style.display = DisplayStyle.None;
            }

            SelectedUnitsAmount -= UNIT_SELECT_STEPS;
            if (ParentUnitCardPanel != null) ParentUnitCardPanel.SetSelectedUnits(-UNIT_SELECT_STEPS);
            _selectCounterText.text = $"x{SelectedUnitsAmount}";
            return true;
        }

        public void ResetSelection()
        {
            SelectedUnitsAmount = 0;
            _backgroundDefault.style.display = DisplayStyle.Flex;
            _backgroundSelected.style.display = DisplayStyle.None;
            _selectCounter.style.display = DisplayStyle.None;
        }
        
        public void Buy()
        {
            _bank?.Withdraw(Cost);
        }

        public void Sell(int amount = 1)
        {
            if (amount <= 0) return;
            _bank?.Deposit(Cost * amount);
        }

        public void SetBackgroundImage(string path)
        {
            var ressourceObject = new GameResource(path, $"unitcard_ui_{Name}", GameResourceType.UI);
            Texture2D texture = ressourceObject.LoadRessource<Texture2D>();
            _image.style.backgroundImage = new StyleBackground(texture);
        }
    }
}