using Game.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class UnitCardPanel : VisualElement
{
    #region Boilerplate Component Code
    [UnityEngine.Scripting.Preserve]
    public new class UxmlFactory : UxmlFactory<UnitCard, UxmlTraits> { }

    public UnitCardPanel() { }

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

    private readonly int MAX_CARD_ROTATION = 4;
    private readonly string VIEW_ASSET_PATH = "Assets\\Level\\UI\\Additional UI Elements\\Scripts\\UnitCardPanel\\UnitCardPanel.uxml";

    private VisualElement _mainContainer;
    private VisualElement _cardContainer;
    private VisualElement _actionContainer;

    public override VisualElement contentContainer => _mainContainer;

    public UnitCardPanel(UnitCard[] units)
    {
        //Cards.AddRange(units);
        foreach (var unit in units)
        {
            this.AddUnitCard(unit);
        }

        // load view and set values to view
        VisualTreeAsset viewAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(VIEW_ASSET_PATH);

        viewAsset.CloneTree(this);

        _mainContainer = this.Q<VisualElement>("unit-card-panel");
        _cardContainer = this.Q<VisualElement>("unit-card-panel__cards");
        _actionContainer = this.Q<VisualElement>("unit-card-panel__actions");

        _mainContainer.RegisterCallback<MouseOverEvent>(OnMouseOver);
        _mainContainer.RegisterCallback<MouseOutEvent>(OnMouseOut);
    }

    #region Events
    private void OnMouseOver(MouseOverEvent e)
    {
        //_mainContainer.transform.position = new Vector2(0, 200);
        _mainContainer.style.translate = new Translate(0, 200);
    }
    private void OnMouseOut(MouseOutEvent e)
    {
        //_mainContainer.transform.position = new Vector2(0, 0);
        _mainContainer.style.translate = new Translate(0, 0);
    }
    #endregion

    private bool UpdateUnitCardAffordabilityState(int globalCurrencyAmount)
    {
        // loop all cards and update state
        return false;
    }

    public void SpawnSelectedUnits()
    {
        // spawn selected units on level grid
    }

    public void AddUnitCard(UnitCard uc)
    {
        uc.style.width = 450;
        uc.style.height = 300;
        Cards.Add(uc);
        ApplyUnitCardStyleFix();
        _cardContainer.Add(uc);
    }

    public void RemoveUnitCard()
    {
        // TODO: remove card here
        ApplyUnitCardStyleFix();
    }

    private void ApplyUnitCardStyleFix()
    {
        var cardRotationStep = MAX_CARD_ROTATION / Cards.Count;
        var startAngle = -MAX_CARD_ROTATION;

        for (int i = 0; i < Cards.Count; i++)
        {
            if (i != Cards.Count - 1)
                Cards[i].style.marginRight = -200;

            Cards[i].style.rotate = new Rotate(Angle.Degrees(startAngle + i * cardRotationStep));
        }
    }

    private void DeselectAllUnits()
    {
        foreach(UnitCard uc in Cards)
        {
            uc.ResetSelection();
        }
    }
}
