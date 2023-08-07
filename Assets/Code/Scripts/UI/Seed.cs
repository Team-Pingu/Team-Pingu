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

            abilityElements.Add(new AbilityElement(
                "Blackout", 
                "Disable Towers for a short time.", 
                1200,
                new GameResource(
                    "Assets/Level/Prefabs/Abilities/BlackoutAbility.prefab", 
                    "BlackoutAbility", 
                    GameResourceType.Undefined)
                ));
            abilityElements[0].SetBackgroundImage("Assets/Level/UI/Art/Icons/Icon Pack 1/Icon19.png");
            //abilityElements.Add(new AbilityElement("Other", "Some other Description", 999));
            //abilityElements[1].SetBackgroundImage("Assets/Level/UI/Art/Icons/Icon Pack 1/Icon41.png");

            return abilityElements.ToArray();
        }

        public UnitCard[] GetUnitCards()
        {
            var unitCards = new List<UnitCard>();

            unitCards.Add(new UnitCard(
                "Aeather Shield Bearer",
                "(Magical, Defensive)\n Larger units carrying energy shields powered by aether crystals, providing protection from elemental damage to units within a specific radius",
                150,
                new GameResource("Assets/Level/Prefabs/Units/AetherShieldBearer.prefab", "AetherShieldBearer", GameResourceType.Minion)
            ));
            unitCards.Add(new UnitCard(
                "Clockwork Scout",
                "(Fast, Low Health)\n Small, nimble clockwork creatures, quickly navigating the battleground but susceptible to damage.",
                100,
                new GameResource("Assets/Level/Prefabs/Units/ClockworkScout.prefab", "ClockworkScout", GameResourceType.Minion)
            ));
            unitCards.Add(new UnitCard(
                "Gearhead Sapper",
                "(Can Destroy Towers)\n Specialized units trained in demolishing enemy defenses, capable of damaging or even destroying towers",
                250,
                new GameResource("Assets/Level/Prefabs/Units/GearheadSapper.prefab", "GearheadSapper", GameResourceType.Minion)
            ));
            unitCards.Add(new UnitCard(
                "Steam Powered Goliath",
                "(Slow, High Health)\n Massive, slow-moving units, difficult to take down but slower to navigate the map",
                200,
                new GameResource("Assets/Level/Prefabs/Units/SteamPoweredGoliath.prefab", "SteamPoweredGoliath", GameResourceType.Minion)
            ));

            return unitCards.ToArray();
        }

        public UpgradeElement[,] GetUpgradeElements()
        {
            // row 1
            var r11 = new UpgradeElement(
                "Movement Speed 1",
                "Increased Movement Speed by 50%",
                500,
                um => { um.UpdateMovementSpeedMultiplier(3 / 2f); },
                um => { um.UpdateMovementSpeedMultiplier(2 / 3f); },
                "I"
            );
            r11.SetBackgroundImage("Assets/Level/UI/Art/Icons/Icon Pack 2/Buffs/swiftness.png");
            var r12 = new UpgradeElement(
                "Movement Speed 2",
                "Increased Movement Speed by 75%",
                600,
                um => { um.UpdateMovementSpeedMultiplier(3.5f / 2f); },
                um => { um.UpdateMovementSpeedMultiplier(2 / 3.5f); },
                "II"
            );
            r12.SetBackgroundImage("Assets/Level/UI/Art/Icons/Icon Pack 2/Buffs/swiftness.png");
            var r13 = new UpgradeElement(
                "Movement Speed 3",
                "Increased Movement Speed by 150%",
                1000,
                um => { um.UpdateMovementSpeedMultiplier(5 / 2f); },
                um => { um.UpdateMovementSpeedMultiplier(2 / 5f); },
                "III"
            );
            r13.SetBackgroundImage("Assets/Level/UI/Art/Icons/Icon Pack 2/Buffs/swiftness.png");
            r11.SetChainedElements(null, r12);
            r12.SetChainedElements(r11, r13);
            r13.SetChainedElements(r12, null);

            // row 2
            var r21 = new UpgradeElement(
                "Health 1",
                "Increased Health by 50%",
                500,
                um => { um.UpdateHealthMultiplier(3 / 2f); },
                um => { um.UpdateHealthMultiplier(2 / 3f); },
                "I"
            );
            r21.SetBackgroundImage("Assets/Level/UI/Art/Icons/Icon Pack 2/Buffs/regeneration.png");
            var r22 = new UpgradeElement(
                "Health 2",
                "Increased Health by 75%",
                600,
                um => { um.UpdateHealthMultiplier(3.5f / 2f); },
                um => { um.UpdateHealthMultiplier(2 / 3.5f); },
                "II"
            );
            r22.SetBackgroundImage("Assets/Level/UI/Art/Icons/Icon Pack 2/Buffs/regeneration.png");
            var r23 = new UpgradeElement(
                "Health 3",
                "Increased Health by 150%",
                1000,
                um => { um.UpdateHealthMultiplier(5 / 2f); },
                um => { um.UpdateHealthMultiplier(2 / 5f); },
                "III"
            );
            r23.SetBackgroundImage("Assets/Level/UI/Art/Icons/Icon Pack 2/Buffs/regeneration.png");
            r21.SetChainedElements(null, r22);
            r22.SetChainedElements(r21, r23);
            r23.SetChainedElements(r22, null);

            // row 3
            var r31 = new UpgradeElement(
                "Shovel",
                "All Auto Minions are going to try to open up an alternative path along the route to the enemy's Core!",
                2000,
                um => { um.UpdateCanMinionsOpenAlternativePath(true); },
                um => { um.UpdateCanMinionsOpenAlternativePath(false); }
            );
            r31.SetBackgroundImage("Assets/Level/UI/Art/Icons/Icon Pack 2/Buffs/knockback_resistance.png");

            // row 4
            var r41 = new UpgradeElement(
                "Ressurection",
                "1 out of 10 Auto Minions will come back to life after death, with 50% remaining health", 
                1800
            );
            r41.SetBackgroundImage("Assets/Level/UI/Art/Icons/Icon Pack 2/Buffs/negative_status_resistance.png");

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

            abilityElements.Add(new AbilityElement(
                "Dynamite Explosion", 
                "Place a dynamite on the Path, which will explode after 5s, dealing area damage to enemy minions",
                1600,
                new GameResource("Assets/Level/Prefabs/Abilities/DynamiteAbility.prefab", "DynamiteAbility", GameResourceType.Undefined))
            );
            abilityElements[0].SetBackgroundImage("Assets/Level/UI/Art/Icons/Icon Pack 1/Icon13.png");
            abilityElements.Add(new AbilityElement(
                "Shock Field Trap", 
                "Place a trap along the path, which slows minions entering down for a short time and deals damage", 
                1400,
                new GameResource("Assets/Level/Prefabs/Abilities/ShockFieldTrapAbility.prefab", "ShockFieldTrapAbility", GameResourceType.Undefined))
            );
            abilityElements[1].SetBackgroundImage("Assets/Level/UI/Art/Icons/Icon Pack 1/Icon36.png");

            return abilityElements.ToArray();
        }

        public UnitCard[] GetUnitCards()
        {
            var unitCards = new List<UnitCard>();

            unitCards.Add(new UnitCard(
                "Bolt Thrower",
                "(Fast Attack Speed)\n Rapid-fire tower launching bolts at enemies. Effective against fast, low-health units.",
                200,
                new GameResource("Assets/Level/Prefabs/Towers/BoltThrower.prefab", "BoltThrower", GameResourceType.Minion)
            ));
            unitCards[0].SetBackgroundImage("Assets/Level/UI/Art/UI Images/bolt_thrower_tower.jpg");
            unitCards.Add(new UnitCard(
                "Thunder Coil",
                "(Elemental, AOE Damage)\n Unleashes powerful electric charges that deal area damage. Ideal for dealing with clusters of enemies.",
                300,
                new GameResource("Assets/Level/Prefabs/Towers/ThunderCoil.prefab", "ThunderCoil", GameResourceType.Minion)
            ));
            unitCards[1].SetBackgroundImage("Assets/Level/UI/Art/UI Images/thunder_coil_tower.jpg");
            unitCards.Add(new UnitCard(
                "Electric Fence Barricade",
                "(Deals Continuous Damage)\n Deals continuous damage to any enemy that touches it, great for slowing down and whittling away enemy health.",
                250,
                new GameResource("Assets/Level/Prefabs/Barricades/ElectricFence.prefab", "ElectricFence", GameResourceType.Barricade)
            ));
            unitCards[2].SetBackgroundImage("Assets/Level/UI/Art/UI Images/electric_fence_barricade.jpg");

            return unitCards.ToArray();
        }

        public UpgradeElement[,] GetUpgradeElements()
        {
            // row 1 (50%, 75%, 150%)
            var r11 = new UpgradeElement(
                "Damage 1", 
                "Increase Damage Tier 1, increases Damage by 50%", 
                500, 
                um => { um.UpdateAttackDamageMultiplier(3 / 2f); }, um => { um.UpdateAttackDamageMultiplier(2 / 3f); }, 
                "I"
            );
            r11.SetBackgroundImage("Assets/Level/UI/Art/Icons/Icon Pack 2/Buffs/attack_boost.png");
            var r12 = new UpgradeElement(
                "Damage 2",
                "Increase Damage Tier 2, increases Damage by 75%",
                600, 
                um => { um.UpdateAttackDamageMultiplier(3.5f / 2f); }, um => { um.UpdateAttackDamageMultiplier(2 / 3.5f); }, 
                "II"
            );
            r12.SetBackgroundImage("Assets/Level/UI/Art/Icons/Icon Pack 2/Buffs/attack_boost.png");
            var r13 = new UpgradeElement(
                "Damage 3",
                "Increase Damage Tier 2, increases Damage by 150%",
                1000, 
                um => { um.UpdateAttackDamageMultiplier(5 / 2f); }, um => { um.UpdateAttackDamageMultiplier(2 / 5f); }, 
                "III"
            );
            r13.SetBackgroundImage("Assets/Level/UI/Art/Icons/Icon Pack 2/Buffs/attack_boost.png");
            r11.SetChainedElements(null, r12);
            r12.SetChainedElements(r11, r13);
            r13.SetChainedElements(r12, null);

            // row 2 (50%, 75%, 150%)
            var r21 = new UpgradeElement(
                "Rate of Fire 1",
                "Rate of Fire Tier 1, increases Rate of Fire by 50%",
                500, 
                um => { um.UpdateAttackSpeedMultiplier(3 / 2f); }, um => { um.UpdateAttackSpeedMultiplier(2 / 3f); }, 
                "I"
            );
            r21.SetBackgroundImage("Assets/Level/UI/Art/Icons/Icon Pack 2/Buffs/exp_boost.png");
            var r22 = new UpgradeElement(
                "Rate of Fire 2",
                "Rate of Fire Tier 2, increases Rate of Fire by 75%",
                600, 
                um => { um.UpdateAttackSpeedMultiplier(3.5f / 2f); }, um => { um.UpdateAttackSpeedMultiplier(2 / 3.5f); }, 
                "II"
            );
            r22.SetBackgroundImage("Assets/Level/UI/Art/Icons/Icon Pack 2/Buffs/exp_boost.png");
            var r23 = new UpgradeElement(
                "Rate of Fire 3",
                "Rate of Fire Tier 3, increases Rate of Fire by 150%",
                1000, 
                um => { um.UpdateAttackSpeedMultiplier(5 / 2f); }, um => { um.UpdateAttackSpeedMultiplier(2 / 5f); }, 
                "III"
            );
            r23.SetBackgroundImage("Assets/Level/UI/Art/Icons/Icon Pack 2/Buffs/exp_boost.png");
            r21.SetChainedElements(null, r22);
            r22.SetChainedElements(r21, r23);
            r23.SetChainedElements(r22, null);

            // row 3 (50%, 75%, 150%)
            var r31 = new UpgradeElement(
                "Range 1",
                "Range Tier 1, increases Range by 50%", 
                500, 
                um => { um.UpdateAttackRangeMultiplier(3 / 2f); }, um => { um.UpdateAttackRangeMultiplier(2 / 3f); },
                "I"
            );
            r31.SetBackgroundImage("Assets/Level/UI/Art/Icons/Icon Pack 2/Buffs/element_boost.png");
            var r32 = new UpgradeElement(
                "Range 2",
                "Range Tier 2, increases Range by 75%",
                600, 
                um => { um.UpdateAttackRangeMultiplier(3.5f / 2f); }, um => { um.UpdateAttackRangeMultiplier(2 / 3.5f); }, 
                "II"
            );
            r32.SetBackgroundImage("Assets/Level/UI/Art/Icons/Icon Pack 2/Buffs/element_boost.png");
            var r33 = new UpgradeElement(
                "Range 3",
                "Range Tier 3, increases Range by 150%",
                1000, 
                um => { um.UpdateAttackRangeMultiplier(5 / 2f); }, um => { um.UpdateAttackRangeMultiplier(2 / 5f); }, 
                "III"
            );
            r33.SetBackgroundImage("Assets/Level/UI/Art/Icons/Icon Pack 2/Buffs/element_boost.png");
            r31.SetChainedElements(null, r32);
            r32.SetChainedElements(r31, r33);
            r33.SetChainedElements(r32, null);

            // row 4 (50%, 75%, 150%)
            var r41 = new UpgradeElement(
                "Area of Effect 1",
                "Area of Effect Tier 1, increases Area of Effect by 50%",
                500, um => { um.UpdateAttackAreaOfEffectMultiplier(3 / 2f); }, um => { um.UpdateAttackAreaOfEffectMultiplier(2 / 3f); }, 
                "I"
            );
            r41.SetBackgroundImage("Assets/Level/UI/Art/Icons/Icon Pack 2/Buffs/magic_amplification.png");
            var r42 = new UpgradeElement(
                "Area of Effect 2",
                "Area of Effect Tier 2, increases Area of Effect by 75%",
                600, 
                um => { um.UpdateAttackAreaOfEffectMultiplier(3.5f / 2f); }, um => { um.UpdateAttackAreaOfEffectMultiplier(2 / 3.5f); }, 
                "II"
            );
            r42.SetBackgroundImage("Assets/Level/UI/Art/Icons/Icon Pack 2/Buffs/magic_amplification.png");
            var r43 = new UpgradeElement(
                "Area of Effect 3",
                "Area of Effect Tier 3, increases Area of Effect by 150%",
                1000, 
                um => { um.UpdateAttackAreaOfEffectMultiplier(5 / 2f); }, um => { um.UpdateAttackAreaOfEffectMultiplier(2 / 5f); }, 
                "III"
            );
            r43.SetBackgroundImage("Assets/Level/UI/Art/Icons/Icon Pack 2/Buffs/magic_amplification.png");
            r41.SetChainedElements(null, r42);
            r42.SetChainedElements(r41, r43);
            r43.SetChainedElements(r42, null);

            // row 5
            var r51 = new UpgradeElement(
                "Ricochet", 
                "Tower projectiles riccochet off enemy units", 
                1600, 
                null
            );
            r51.SetBackgroundImage("Assets/Level/UI/Art/Icons/Icon Pack 2/Buffs/critical_boost.png");

            // row 6
            var r61 = new UpgradeElement(
                "Knockback", 
                "Enemy units are knocked back by tower projectiles", 
                1400, 
                um => { um.UpdateAttackKnockbackMultiplier(10); }, um => { um.UpdateAttackAreaOfEffectMultiplier(1 / 10f); }, 
                null
            );
            r61.SetBackgroundImage("Assets/Level/UI/Art/Icons/Icon Pack 2/Buffs/knockback_boost.png");

            // row 7
            var r71 = new UpgradeElement(
                "Bleed", 
                "Enemy units bleed on a tower projectile hit, dealing a little bit of damage per time", 
                1200, 
                null
            );
            r71.SetBackgroundImage("Assets/Level/UI/Art/Icons/Icon Pack 2/Debuffs/bleeding.png");

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
