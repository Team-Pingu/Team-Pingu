using Game.CustomUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.CustomUI.Seed
{
    interface ISeed
    {
        public AbilityElement[] GetAbilityElements();
        public UnitCard[] GetUnitCards();
        public UpgradeElement[,] GetUpgradeElements();
        public void InflateUI(VisualElement rootVisualElement)
        {
            VisualElement upgradeElementContainer = rootVisualElement.Q<VisualElement>("game-upgrade-popup");
            ScrollView upgradeElementContent = upgradeElementContainer.Q<ScrollView>();
            VisualElement abilityElementContainer = rootVisualElement.Q<VisualElement>("player-controls__ability-bar");
            UnitCardPanel unitCardContainer = rootVisualElement.Q<VisualElement>("unit-card-panel") as UnitCardPanel;

            if (unitCardContainer == null || abilityElementContainer == null || upgradeElementContainer == null || upgradeElementContent == null)
            {
                throw new System.Exception("In order for the Seed to be initialized, the view containers cannot be null!");
            }

            // clear all children
            abilityElementContainer.Clear();
            unitCardContainer.Clear();
            upgradeElementContent.Clear();

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

            var upgradeElements = GetUpgradeElements();
            for (int i = 0; i < upgradeElements.GetLength(0); i++)
            {
                // row template
                VisualElement row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                row.style.justifyContent = Justify.SpaceAround;
                row.style.marginBottom = new StyleLength(10f);

                for (int j = 0; j < upgradeElements.GetLength(1); j++)
                {
                    UpgradeElement upgradeElement = upgradeElements[i, j];
                    row.Add(upgradeElement);
                }

                upgradeElementContent.Add(row);
            }
        }
    }

    #region Seeds
    public class AttackerSeed : ISeed
    {
        public AbilityElement[] GetAbilityElements()
        {
            var abilityElements = new List<AbilityElement>();

            abilityElements.Add(new AbilityElement("Blackout", "", 999));
            abilityElements.Add(new AbilityElement("Other", "", 599));

            return abilityElements.ToArray();
        }

        public UnitCard[] GetUnitCards()
        {
            var unitCards = new List<UnitCard>();

            unitCards.Add(new UnitCard(
                "Aeather Shield Bearer",
                "Beschreibung hier",
                100,
                new GameResource("Assets/Level/Prefabs/Units/AetherShieldBearer.prefab", null, GameResourceType.Minion)
            ));
            unitCards.Add(new UnitCard(
                "Clockwork Scout",
                "Beschreibung hier",
                250,
                new GameResource("Assets/Level/Prefabs/Units/ClockworkScout.prefab", null, GameResourceType.Minion)
            ));
            unitCards.Add(new UnitCard(
                "Gearhead Sapper",
                "Beschreibung hier",
                400,
                new GameResource("Assets/Level/Prefabs/Units/GearheadSapper.prefab", null, GameResourceType.Minion)
            ));
            unitCards.Add(new UnitCard(
                "Steam Powered Goliath",
                "Beschreibung hier",
                1000,
                new GameResource("Assets/Level/Prefabs/Units/SteamPoweredGoliath.prefab", null, GameResourceType.Minion)
            ));

            return unitCards.ToArray();
        }

        public UpgradeElement[,] GetUpgradeElements()
        {
            // row 1
            var r11 = new UpgradeElement(
                "Movement Speed 1",
                "Increased Movement Speed by 5%",
                200,
                um => { um.UpdateMovementSpeedMultiplier(3 / 2f); },
                um => { um.UpdateMovementSpeedMultiplier(2 / 3f); },
                "I"
            );
            var r12 = new UpgradeElement(
                "Movement Speed 2",
                "",
                200,
                um => { um.UpdateMovementSpeedMultiplier(3.5f / 2f); },
                um => { um.UpdateMovementSpeedMultiplier(2 / 3.5f); },
                "II"
            );
            var r13 = new UpgradeElement(
                "Movement Speed 3",
                "",
                200,
                um => { um.UpdateMovementSpeedMultiplier(5 / 2f); },
                um => { um.UpdateMovementSpeedMultiplier(2 / 5f); },
                "III"
            );

            // row 2
            var r21 = new UpgradeElement(
                "Health 1",
                "",
                200,
                um => { um.UpdateHealthMultiplier(3 / 2f); },
                um => { um.UpdateHealthMultiplier(2 / 3f); },
                "I"
            );
            var r22 = new UpgradeElement(
                "Health 2",
                "",
                200,
                um => { um.UpdateHealthMultiplier(3.5f / 2f); },
                um => { um.UpdateHealthMultiplier(2 / 3.5f); },
                "II"
            );
            var r23 = new UpgradeElement(
                "Health 3",
                "",
                200,
                um => { um.UpdateHealthMultiplier(5 / 2f); },
                um => { um.UpdateHealthMultiplier(2 / 5f); },
                "III"
            );

            // row 3
            var r31 = new UpgradeElement(
                "Shovel",
                "",
                200,
                um => { um.UpdateCanMinionsOpenAlternativePath(true); },
                um => { um.UpdateCanMinionsOpenAlternativePath(false); }
            );

            // row 4
            var r41 = new UpgradeElement("Ressurection", "", 200);

            var upgradeElements = new List<List<UpgradeElement>>();
            upgradeElements.Add(new List<UpgradeElement> { r11, r12, r13 });
            upgradeElements.Add(new List<UpgradeElement> { r21, r22, r23 });
            upgradeElements.Add(new List<UpgradeElement> { r31 });
            upgradeElements.Add(new List<UpgradeElement> { r41 });

            UpgradeElement[,] array = new UpgradeElement[upgradeElements.Count, upgradeElements[0].Count];
            for (int i = 0; i < upgradeElements.Count; i++)
            {
                for (int j = 0; j < upgradeElements[i].Count; j++)
                {
                    array[i, j] = upgradeElements[i][j];
                }
            }
            return array;
        }
    }

    public class DefenderSeed : ISeed
    {
        public AbilityElement[] GetAbilityElements()
        {
            var abilityElements = new List<AbilityElement>();

            abilityElements.Add(new AbilityElement("Dynamite Explosion", "", 300));
            abilityElements.Add(new AbilityElement("Shock Field Trap", "", 155));

            return abilityElements.ToArray();
        }

        public UnitCard[] GetUnitCards()
        {
            var unitCards = new List<UnitCard>();

            unitCards.Add(new UnitCard(
                "Bolt Thrower",
                "Beschreibung hier",
                100,
                new GameResource("Assets/Level/Prefabs/Towers/BoltThrower.prefab", null, GameResourceType.Minion)
            ));
            unitCards.Add(new UnitCard(
                "Thunder Coil",
                "Beschreibung hier",
                250,
                new GameResource("Assets/Level/Prefabs/Towers/ThunderCoil.prefab", null, GameResourceType.Minion)
            ));
            unitCards.Add(new UnitCard(
                "Electric Fence Barricade",
                "Beschreibung hier",
                400,
                new GameResource("Assets/Level/Prefabs/Barricades/ElectricFence.prefab", null, GameResourceType.Minion)
            ));

            return unitCards.ToArray();
        }

        public UpgradeElement[,] GetUpgradeElements()
        {
            // row 1 (50%, 75%, 150%)
            var r11 = new UpgradeElement("Damage 1", "", 200, um => { um.UpdateAttackDamageMultiplier(3 / 2f); }, um => { um.UpdateAttackDamageMultiplier(2 / 3f); }, "I");
            var r12 = new UpgradeElement("Damage 2", "", 200, um => { um.UpdateAttackDamageMultiplier(3.5f / 2f); }, um => { um.UpdateAttackDamageMultiplier(2 / 3.5f); }, "II");
            var r13 = new UpgradeElement("Damage 3", "", 200, um => { um.UpdateAttackDamageMultiplier(5 / 2f); }, um => { um.UpdateAttackDamageMultiplier(2 / 5f); }, "III");

            // row 2 (50%, 75%, 150%)
            var r21 = new UpgradeElement("Rate of Fire 1", "", 200, um => { um.UpdateAttackSpeedMultiplier(3 / 2f); }, um => { um.UpdateAttackSpeedMultiplier(2 / 3f); }, "I");
            var r22 = new UpgradeElement("Rate of Fire 2", "", 200, um => { um.UpdateAttackSpeedMultiplier(3.5f / 2f); }, um => { um.UpdateAttackSpeedMultiplier(2 / 3.5f); }, "II");
            var r23 = new UpgradeElement("Rate of Fire 3", "", 200, um => { um.UpdateAttackSpeedMultiplier(5 / 2f); }, um => { um.UpdateAttackSpeedMultiplier(2 / 5f); }, "III");

            // row 3 (50%, 75%, 150%)
            var r31 = new UpgradeElement("Range 1", "", 200, um => { um.UpdateAttackRangeMultiplier(3 / 2f); }, um => { um.UpdateAttackRangeMultiplier(2 / 3f); }, "I");
            var r32 = new UpgradeElement("Range 2", "", 200, um => { um.UpdateAttackRangeMultiplier(3.5f / 2f); }, um => { um.UpdateAttackRangeMultiplier(2 / 3.5f); }, "II");
            var r33 = new UpgradeElement("Range 3", "", 200, um => { um.UpdateAttackRangeMultiplier(5 / 2f); }, um => { um.UpdateAttackRangeMultiplier(2 / 5f); }, "III");

            // row 4 (50%, 75%, 150%)
            var r41 = new UpgradeElement("Area of Effect 1", "", 200, um => { um.UpdateAttackAreaOfEffectMultiplier(3 / 2f); }, um => { um.UpdateAttackAreaOfEffectMultiplier(2 / 3f); }, "I");
            var r42 = new UpgradeElement("Area of Effect 2", "", 200, um => { um.UpdateAttackAreaOfEffectMultiplier(3.5f / 2f); }, um => { um.UpdateAttackAreaOfEffectMultiplier(2 / 3.5f); }, "II");
            var r43 = new UpgradeElement("Area of Effect 3", "", 200, um => { um.UpdateAttackAreaOfEffectMultiplier(5 / 2f); }, um => { um.UpdateAttackAreaOfEffectMultiplier(2 / 5f); }, "III");

            // row 5
            var r51 = new UpgradeElement("Ricochet", "", 200, null);

            // row 6
            var r61 = new UpgradeElement("Knockback", "", 200, um => { um.UpdateAttackKnockbackMultiplier(10); }, um => { um.UpdateAttackAreaOfEffectMultiplier(1 / 10f); }, null);

            // row 7
            var r71 = new UpgradeElement("Bleed", "", 200, null);

            var upgradeElements = new List<List<UpgradeElement>>();
            upgradeElements.Add(new List<UpgradeElement> { r11, r12, r13 });
            upgradeElements.Add(new List<UpgradeElement> { r21, r22, r23 });
            upgradeElements.Add(new List<UpgradeElement> { r31, r32, r33 });
            upgradeElements.Add(new List<UpgradeElement> { r41, r42, r43 });
            upgradeElements.Add(new List<UpgradeElement> { r51 });
            upgradeElements.Add(new List<UpgradeElement> { r61 });
            upgradeElements.Add(new List<UpgradeElement> { r71 });

            UpgradeElement[,] array = new UpgradeElement[upgradeElements.Count, upgradeElements[0].Count];
            for (int i = 0; i < upgradeElements.Count; i++)
            {
                for (int j = 0; j < upgradeElements[i].Count; j++)
                {
                    array[i, j] = upgradeElements[i][j];
                }
            }
            return array;
        }
    }
    #endregion
}
