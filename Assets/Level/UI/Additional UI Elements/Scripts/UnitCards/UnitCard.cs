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

        private GameResource _unitGameResource;

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
#if UNITY_EDITOR
            viewAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(VIEW_ASSET_PATH);
#else
            var __viewAssetResource = new GameResource(VIEW_ASSET_PATH, null, GameResourceType.UI);
            viewAsset = __viewAssetResource.LoadRessource<VisualTreeAsset>();
#endif
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
        }

        #region Events

        private void OnMouseClick(MouseDownEvent e)
        {
            if (e.button == (int)MouseButton.LeftMouse)
            {
                this.SelectUnit();
            }

            if (e.button == (int)MouseButton.RightMouse)
            {
                this.DeselectUnit();
            }
        }
        #endregion

        private bool IsUnitCardAffordable(int globalCurrencyAmount)
        {
            if (this.Cost <= globalCurrencyAmount) return true;
            return false;
        }

        private void SpawnUnit(int amount = 1)
        {
            // spawn unit on level grid
        }

        private void SelectUnit()
        {
            if (SelectedUnitsAmount >= MAX_UNITS_SELECTABLE)
            {
                return;
            }

            if (SelectedUnitsAmount == 0)
            {
                // only if state changes from 0 -> 1
                _backgroundDefault.style.display = DisplayStyle.None;
                _backgroundSelected.style.display = DisplayStyle.Flex;
                _selectCounter.style.display = DisplayStyle.Flex;
            }

            SelectedUnitsAmount += UNIT_SELECT_STEPS;
            _selectCounterText.text = $"x{SelectedUnitsAmount}";
        }

        private void DeselectUnit()
        {
            if (SelectedUnitsAmount <= 0)
            {
                return;
            }

            if (SelectedUnitsAmount - UNIT_SELECT_STEPS <= 0)
            {
                // only if state changes from 1 -> 0
                _backgroundDefault.style.display = DisplayStyle.Flex;
                _backgroundSelected.style.display = DisplayStyle.None;
                _selectCounter.style.display = DisplayStyle.None;
            }

            SelectedUnitsAmount -= UNIT_SELECT_STEPS;
            _selectCounterText.text = $"x{SelectedUnitsAmount}";
        }

        public void ResetSelection()
        {
            SelectedUnitsAmount = 0;
            _backgroundDefault.SetEnabled(true);
            _backgroundSelected.SetEnabled(false);
            _selectCounter.SetEnabled(false);
        }
    }
}