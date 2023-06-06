using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.CustomUI
{
    public class UnitCard : VisualElement
    {
        #region Boilerplate Component Code
        [UnityEngine.Scripting.Preserve]
        public new class UxmlFactory : UxmlFactory<UnitCard, UxmlTraits> { }

        //[UnityEngine.Scripting.Preserve]
        //public new class UxmlTraits : VisualElement.UxmlTraits
        //{
        //    private readonly UxmlBoolAttributeDescription startVisible = new UxmlBoolAttributeDescription { name = "start-visible", defaultValue = false };
        //    private readonly UxmlIntAttributeDescription fadeTime = new UxmlIntAttributeDescription { name = "fade-time", defaultValue = 30 };

        //    public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        //    {
        //        base.Init(ve, bag, cc);

        //        var item = ve as PopupPanel;
        //        var vis = startVisible.GetValueFromBag(bag, cc);
        //        item.FadeTime = fadeTime.GetValueFromBag(bag, cc);

        //        item.SetStartVisibility(vis);
        //    }
        //}
        #endregion

        public string Name;
        public string Description;
        public int Cost;
        public int SelectedUnitsAmount = 0;

        private readonly int MAX_UNITS_SELECTABLE = 10;
        private readonly string VIEW_ASSET_PATH = "Assets\\Level\\UI\\Additional UI Elements\\Scripts\\UnitCards\\UnitCard.uxml";

        private Label _nameLabel;
        private Label _descriptionLabel;
        private Label _costLabel;
        private VisualElement _image;
        private VisualElement _backgroundDefault;
        private VisualElement _backgroundSelected;
        private VisualElement _selectCounter;
        private Label _selectCounterText;
        private VisualElement _mainContainer;

        public override VisualElement contentContainer => _mainContainer;

        public UnitCard()
        {
            Init();

            Name = _nameLabel.text;
            Description = _descriptionLabel.text;
            bool isParsed = int.TryParse(_costLabel.text, out Cost);
        }

        public UnitCard(string name, string description, int cost)
        {
            Name = name;
            Description = description;
            Cost = cost;

            _nameLabel.text = name;
            _descriptionLabel.text = description;
            _costLabel.text = $"{cost}";

            Init();
        }

        private void Init()
        {
            // load view and set values to view
            VisualTreeAsset viewAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(VIEW_ASSET_PATH);

            viewAsset.CloneTree(this);

            _nameLabel = this.Q<Label>("unit-card__header__title");
            _descriptionLabel = this.Q<Label>("unit-card__body__description__text");
            _costLabel = this.Q<Label>("unit-card__header__cost__text");
            _image = this.Q<VisualElement>("unit-card__body__image").Children().ElementAt(0);
            _backgroundDefault = this.Q<VisualElement>("unit-card-bg--default");
            _backgroundDefault = this.Q<VisualElement>("unit-card-bg--selected");
            _selectCounter = this.Q<Label>("unit-card__select-counter");
            _selectCounterText = this.Q<Label>("unit-card__select-counter__text");
            _mainContainer = this.Q<VisualElement>("unit-card-container");

            _mainContainer.RegisterCallback<ClickEvent>(OnClick);
        }

        #region Events
        private void OnClick(ClickEvent e)
        {
            if (e.button == 0)
            {
                this.SelectUnit();
            }
            else
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
                _backgroundDefault.SetEnabled(false);
                _backgroundSelected.SetEnabled(true);
                _selectCounter.SetEnabled(true);
            }

            SelectedUnitsAmount++;
            _selectCounterText.text = $"{SelectedUnitsAmount}";
        }

        private void DeselectUnit()
        {
            if (SelectedUnitsAmount <= 0)
            {
                return;
            }

            if (SelectedUnitsAmount > 0)
            {
                // only if state changes from 1 -> 0
                _backgroundDefault.SetEnabled(true);
                _backgroundSelected.SetEnabled(false);
                _selectCounter.SetEnabled(false);
            }

            SelectedUnitsAmount--;
            _selectCounterText.text = $"{SelectedUnitsAmount}";
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