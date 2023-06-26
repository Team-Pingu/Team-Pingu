using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.CustomUI
{
    public class UnitCardPanel : VisualElement
    {
        #region Boilerplate Component Code
        [UnityEngine.Scripting.Preserve]
        public new class UxmlFactory : UxmlFactory<UnitCardPanel, UxmlTraits> { }
        #endregion

        public List<UnitCard> Cards { get; private set; }

        public enum UnitCardSize
        {
            s,
            m,
            l,
            xl
        }

        public static readonly int CARD_ROTATION_ANGLE_END = 8;
        public static readonly int CARD_ROTATION_ANGLE_START = -1;
        public static readonly int CARD_GAP_COLLAPSED = 180;
        public static readonly int CARD_GAP_EXPANDED = 0;
        public static readonly int CONTAINER_TRANSLATION = 200;
        private readonly string VIEW_ASSET_PATH = "Assets/Level/UI/Additional UI Elements/Scripts/UnitCardPanel/UnitCardPanel.uxml";
        private readonly UnitCardSize unitCardSize = UnitCardSize.m;

        private VisualElement _mainContainer;
        private VisualElement _cardContainer;
        private VisualElement _actionContainer;
        private Button _spawnButton;
        private Button _abortButton;

        // depending on whether one card or multiple selections are allowed
        public bool UseSingleSelectionOnly = true;
        public int SelectedUnits = 0;

        public override VisualElement contentContainer => _mainContainer;

        public UnitCardPanel()
        {
            Init();
            foreach (UnitCard unit in GetUnitCardsFromView())
            {
                this.AddUnitCard(unit, false);
            }
            ApplyUnitCardStyleFix();
        }

        public UnitCardPanel(UnitCard[] units)
        {
            Init();

            //Cards.AddRange(units);
            foreach (UnitCard unit in units)
            {
                this.AddUnitCard(unit);
            }
        }

        private void Init()
        {
            // load view and set values to view
            VisualTreeAsset viewAsset;
            var __viewAssetResource = new GameResource(VIEW_ASSET_PATH, null, GameResourceType.UI);
            viewAsset = __viewAssetResource.LoadRessource<VisualTreeAsset>();
            viewAsset.CloneTree(this);

            Cards = new List<UnitCard>();
            _mainContainer = this.Q<VisualElement>("unit-card-panel");
            _cardContainer = this.Q<VisualElement>("unit-card-panel__cards");
            _actionContainer = this.Q<VisualElement>("unit-card-panel__actions");
            _spawnButton = this.Q<Button>("unit-card-panel__actions__deploy");
            _abortButton = this.Q<Button>("unit-card-panel__actions__abort");

            _mainContainer.RegisterCallback<MouseOverEvent>(OnMouseOver);
            _mainContainer.RegisterCallback<MouseOutEvent>(OnMouseOut);

            //_spawnButton.RegisterCallback<ClickEvent>(OnSpawnButtonClicked);
            _abortButton.RegisterCallback<ClickEvent>(OnAbortButtonClicked);
        }

        #region Events
        private void OnSpawnButtonClicked(ClickEvent e)
        {
            //Debug.Log("Attached UnitCardPanel to Panel");
        }
        private void OnAbortButtonClicked(ClickEvent e)
        {
            DeselectAllUnits();
        }
        private void OnMouseOver(MouseOverEvent e)
        {
            //_mainContainer.transform.position = new Vector2(0, 200);
            _mainContainer.style.translate = new Translate(0, 0);
        }
        private void OnMouseOut(MouseOutEvent e)
        {
            //_mainContainer.transform.position = new Vector2(0, 0);
            _mainContainer.style.translate = new Translate(0, CONTAINER_TRANSLATION);
        }
        #endregion

        private List<UnitCard> GetUnitCardsFromView(bool ignoreDisabled = true)
        {
            List<UnitCard> result = new List<UnitCard>();
            foreach (var c in _cardContainer.Children())
            {
                if (c.style.display == DisplayStyle.None) continue;
                UnitCard card = c as UnitCard;
                result.Add(card);
            }
            return result;
        }

        public Vector2 GetUnitCardDimensions(UnitCardSize ucs)
        {
            switch (ucs)
            {
                case UnitCardSize.s:
                    return new Vector2(220, 350);
                case UnitCardSize.m:
                    return new Vector2(260, 380);
                case UnitCardSize.l:
                    return new Vector2(300, 420);
                case UnitCardSize.xl:
                    return new Vector2(350, 500);
                default:
                    return Vector2.zero;
            }
        }

        private List<VisualElement> GetUnitCardVisualElementsFromView(bool ignoreDisabled = true)
        {
            List<VisualElement> result = new List<VisualElement>();
            foreach (var c in _cardContainer.Children())
            {
                if (c.style.display == DisplayStyle.None) continue;
                result.Add(c);
            }
            return result;
        }

        private bool UpdateUnitCardAffordabilityState(int globalCurrencyAmount)
        {
            // loop all cards and update state
            return false;
        }

        private UnitCard[] GetSelectedUnitCards()
        {
            var selectedCards = new List<UnitCard>();
            foreach(UnitCard uc in Cards)
            {
                if (uc.SelectedUnitsAmount == 0) continue;
                selectedCards.AddRange(Enumerable.Repeat(uc, uc.SelectedUnitsAmount));
            }
            return selectedCards.ToArray();
        }

        public void SpawnSelectedUnits()
        {
            // spawn selected units on level grid
        }

        public void AddUnitCard(UnitCard uc, bool inflateAndApplyFix = true)
        {
            Vector2 cardDimensions = GetUnitCardDimensions(unitCardSize);
            uc.style.width = cardDimensions.x;
            uc.style.height = cardDimensions.y;
            uc.ParentUnitCardPanel = this;

            List<StylePropertyName> properties = new List<StylePropertyName>() { new StylePropertyName("margin") };
            uc.style.transitionProperty = new StyleList<StylePropertyName>(properties);
            List<TimeValue> durations = new List<TimeValue>() { new TimeValue(50f, TimeUnit.Millisecond) };
            uc.style.transitionDuration = new StyleList<TimeValue>(durations);
            List<EasingFunction> easingFunctions = new List<EasingFunction>() { new EasingFunction(EasingMode.Linear) };
            uc.style.transitionTimingFunction = new StyleList<EasingFunction>(easingFunctions);

            Cards.Add(uc);
            if (inflateAndApplyFix)
            {
                ApplyUnitCardStyleFix();
                _cardContainer.Add(uc);
            }
        }

        public void RemoveUnitCard()
        {
            // TODO: remove card here
            ApplyUnitCardStyleFix();
        }

        private void ApplyUnitCardStyleFix()
        {
            List<UnitCard> visibleCards = Cards;
            int cardRotationAbsoluteAngle = CARD_ROTATION_ANGLE_END - CARD_ROTATION_ANGLE_START;
            float cardRotationStep = cardRotationAbsoluteAngle / (float)visibleCards.Count;

            for (int i = 0; i < visibleCards.Count; i++)
            {
                var currentCard = visibleCards[i];
                bool isLastCard = i == visibleCards.Count - 1;
                if (!isLastCard)
                    currentCard.style.marginRight = -CARD_GAP_COLLAPSED;

                currentCard.style.rotate = new Rotate(Angle.Degrees(CARD_ROTATION_ANGLE_START + i * cardRotationStep));
                currentCard.RegisterCallback<MouseOverEvent>((e) =>
                {
                    if (isLastCard) return;
                    currentCard.style.marginRight = CARD_GAP_EXPANDED;
                });
                currentCard.RegisterCallback<MouseOutEvent>((e) =>
                {
                    if (isLastCard) return;
                    currentCard.style.marginRight = -CARD_GAP_COLLAPSED;
                });
            }
        }

        public void DeselectAllUnits()
        {
            SetSelectedUnits();
            foreach (UnitCard uc in Cards)
            {
                uc.ResetSelection();
            }
        }

        public void SetSelectedUnits(int diff = 1337)
        {
            if (diff == 1337) SelectedUnits = 0;
            else SelectedUnits += diff;

            // other actions
            // set abort button visibility
            if (SelectedUnits == 0) _abortButton.style.display = DisplayStyle.None;
            else _abortButton.style.display = DisplayStyle.Flex;
        }

        public new void Clear()
        {
            Cards.Clear();
            _cardContainer.Clear();
            SetSelectedUnits();
        }
    }
}