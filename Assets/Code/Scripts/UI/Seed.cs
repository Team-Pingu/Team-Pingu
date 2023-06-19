using Game.CustomUI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.CustomUI.Seed
{
    interface ISeed
    {
        public AbilityElement[] GetAbilityElements();
        public UnitCard[] GetUnitCards();
        public UpgradeElement[] GetUpgradeElements();
        public void InflateUI(VisualElement rootVisualElement)
        {
            //VisualElement upgradeElementContainer = rootVisualElement.Q<VisualElement>("");
            VisualElement abilityElementContainer = rootVisualElement.Q<VisualElement>("player-controls__ability-bar");
            UnitCardPanel unitCardContainer = rootVisualElement.Q<VisualElement>("unit-card-panel") as UnitCardPanel;

            if (unitCardContainer == null || abilityElementContainer == null)
            {
                throw new System.Exception("In order for the Seed to be initialized, the view containers cannot be null!");
            }

            // clear all children
            abilityElementContainer.Clear();
            unitCardContainer.Clear();

            // inflate containers with content
            var unitCards = GetUnitCards();
            foreach (var card in unitCards)
            {
                unitCardContainer.AddUnitCard(card);
            }

            var abilityElements = GetAbilityElements();
            foreach (var abilityElement in abilityElements)
            {
                abilityElement.style.marginBottom = new StyleLength(10);
                abilityElementContainer.Add(abilityElement);
            }
        }
    }

    #region Seeds
    public class AttackerSeed : ISeed
    {
        public AbilityElement[] GetAbilityElements()
        {
            var abilityElements = new List<AbilityElement>();
            //abilityElements.Add(new AbilityElement());
            return abilityElements.ToArray();
        }

        public UnitCard[] GetUnitCards()
        {
            var unitCards = new List<UnitCard>();
            //unitCards.Add(new UnitCard());
            return unitCards.ToArray();
        }

        public UpgradeElement[] GetUpgradeElements()
        {
            var upgradeElements = new List<UpgradeElement>();
            //upgradeElements.Add(new UpgradeElement());
            return upgradeElements.ToArray();
        }
    }

    public class DefenderSeed : ISeed
    {
        public AbilityElement[] GetAbilityElements()
        {
            var abilityElements = new List<AbilityElement>();
            //abilityElements.Add(new AbilityElement());
            return abilityElements.ToArray();
        }

        public UnitCard[] GetUnitCards()
        {
            var unitCards = new List<UnitCard>();
            //unitCards.Add(new UnitCard());
            return unitCards.ToArray();
        }

        public UpgradeElement[] GetUpgradeElements()
        {
            var upgradeElements = new List<UpgradeElement>();
            //upgradeElements.Add(new UpgradeElement());
            return upgradeElements.ToArray();
        }
    }
    #endregion
}
