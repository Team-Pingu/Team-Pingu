using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Rendering.FilterWindow;

namespace Game.CustomUI
{
    public class UnitCardPanel : VisualElement
    {
        #region Boilerplate Component Code
        [UnityEngine.Scripting.Preserve]
        public new class UxmlFactory : UxmlFactory<UnitCardPanel, UxmlTraits> { }

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

        public List<UnitCard> Cards { get; private set; }

        public static readonly int MAX_CARD_ROTATION = 4;
        public static readonly int CARD_GAP_COLLAPSED = 200;
        public static readonly int CARD_GAP_EXPANDED = 0;
        public static readonly int CONTAINER_TRANSLATION = 250;
        private readonly string VIEW_ASSET_PATH = "Assets\\Level\\UI\\Additional UI Elements\\Scripts\\UnitCardPanel\\UnitCardPanel.uxml";

        private VisualElement _mainContainer;
        private VisualElement _cardContainer;
        private VisualElement _actionContainer;

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
            VisualTreeAsset viewAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(VIEW_ASSET_PATH);

            viewAsset.CloneTree(this);

            Cards = new List<UnitCard>();
            _mainContainer = this.Q<VisualElement>("unit-card-panel");
            _cardContainer = this.Q<VisualElement>("unit-card-panel__cards");
            _actionContainer = this.Q<VisualElement>("unit-card-panel__actions");

            _mainContainer.RegisterCallback<MouseOverEvent>(OnMouseOver);
            _mainContainer.RegisterCallback<MouseOutEvent>(OnMouseOut);

            //RegisterCallback<AttachToPanelEvent>(OnAttachedToPanel);
            //RegisterCallback<DetachFromPanelEvent>(OnDetachedFromPanel);
        }

        #region Events
        private void OnAttachedToPanel(AttachToPanelEvent e)
        {
            Debug.Log("Attached UnitCardPanel to Panel");
        }
        private void OnDetachedFromPanel(DetachFromPanelEvent e)
        {
            Debug.Log("Detached UnitCardPanel to Panel");
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

        public void SpawnSelectedUnits()
        {
            // spawn selected units on level grid
        }

        public void AddUnitCard(UnitCard uc, bool inflateAndApplyFix = true)
        {
            //uc.style.width = 300;
            //uc.style.height = 450;

            List<StylePropertyName> properties = new List<StylePropertyName>() { new StylePropertyName("margin") };
            uc.style.transitionProperty = new StyleList<StylePropertyName>(properties);
            List<TimeValue> durations = new List<TimeValue>() { new TimeValue(50f, TimeUnit.Millisecond) };
            uc.style.transitionDuration = new StyleList<TimeValue>(durations);
            List<EasingFunction> easingFunctions = new List<EasingFunction>() { new EasingFunction(EasingMode.Linear) };
            uc.style.transitionTimingFunction = new StyleList<EasingFunction>(easingFunctions);

            Cards.Add(uc);
            if(inflateAndApplyFix)
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
            float cardRotationStep = (MAX_CARD_ROTATION * 2) / (float)visibleCards.Count;
            var startAngle = -MAX_CARD_ROTATION;

            for (int i = 0; i < visibleCards.Count; i++)
            {
                var currentCard = visibleCards[i];
                bool isLastCard = i == visibleCards.Count - 1;
                if (!isLastCard)
                    currentCard.style.marginRight = -CARD_GAP_COLLAPSED;

                currentCard.style.rotate = new Rotate(Angle.Degrees(startAngle + i * cardRotationStep));
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

        private void DeselectAllUnits()
        {
            foreach (UnitCard uc in Cards)
            {
                uc.ResetSelection();
            }
        }
    }
}