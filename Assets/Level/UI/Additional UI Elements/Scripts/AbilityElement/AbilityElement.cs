using Code.Scripts;
using Game.CustomUI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

//abstract public class DeploymentElement
//{
//    public string Name;
//    public string Description;
//    public int Cost;

//    private Label _nameLabel;
//    private Label _descriptionLabel;
//    private Label _costLabel;

//    public abstract void Init();
//}

//interface IDeploymentElement
//{
//    void Init();
//    void Select();
//    void IsAffordable();
//}

namespace Game.CustomUI
{
    public class AbilityElement : VisualElement
    {
        #region Boilerplate Component Code
        [UnityEngine.Scripting.Preserve]
        public new class UxmlFactory : UxmlFactory<AbilityElement, UxmlTraits> { }
        #endregion

        public string Name;
        public string Description;
        public int Cost;

        private readonly string VIEW_ASSET_PATH = "Assets/Level/UI/Additional UI Elements/Scripts/AbilityElement/AbilityElement.uxml";

        private Label _nameLabel;
        private Label _descriptionLabel;
        private Label _costLabel;
        private VisualElement _mainContainer;
        private PopupPanelCustom _popupPanel;

        private Bank _bank;

        public override VisualElement contentContainer => _mainContainer;

        public AbilityElement()
        {
            Init();

            //Name = _nameLabel.text;
            //Description = _descriptionLabel.text;
            //bool isParsed = int.TryParse(_costLabel.text, out Cost);
            _costLabel.text = "300";
        }

        public AbilityElement(string name, string description, int cost)
        {
            Init();

            Name = name;
            Description = description;
            Cost = cost;

            //_nameLabel.text = name;
            //_descriptionLabel.text = description;
            _costLabel.text = $"{cost}";

            _popupPanel = new PopupPanelCustom(name, description, _mainContainer, "This is an instant Action");
        }

        private void Init()
        {
            // load view and set values to view
            VisualTreeAsset viewAsset;
            var __viewAssetResource = new GameResource(VIEW_ASSET_PATH, null, GameResourceType.UI);
            viewAsset = __viewAssetResource.LoadRessource<VisualTreeAsset>();
            viewAsset.CloneTree(this);

            //_nameLabel = this.Q<Label>("ability-element-popup__title");
            //_descriptionLabel = this.Q<Label>("ability-element-popup__description");
            _costLabel = this.Q<Label>("ability-element__cost");
            _mainContainer = this.Q<VisualElement>("ability-element-container");
            var _content = this.Q<VisualElement>("ability-element");

            _mainContainer.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            _mainContainer.RegisterCallback<MouseLeaveEvent>(OnMouseExit);
            _mainContainer.RegisterCallback<ClickEvent>(MouseClick);

            _bank = GameObject.Find("Player").GetComponent<Bank>();
            _bank.OnBalanceChanged += currentBalance => IsAffordable(currentBalance);
        }

        #region Events
        private void MouseClick(ClickEvent e)
        {
            Debug.Log("ClickEvent");
            if (IsAffordable(_bank.CurrentBalance))
            {
                Buy();
            }
        }

        private void OnMouseEnter(MouseEnterEvent e)
        {
            Debug.Log("ENTER");
            _popupPanel.SetScreenPos(this);
            _popupPanel.Show();
        }

        private void OnMouseExit(MouseLeaveEvent e)
        {
            Debug.Log("EXIT");
            _popupPanel.Hide();
        }
        #endregion

        private bool IsAffordable(int globalCurrencyAmount)
        {
            bool isAffordable = Cost <= globalCurrencyAmount;
            _costLabel.style.backgroundColor = new StyleColor(isAffordable ? new Color(0, 0, 0, 0) : Color.red);
            return isAffordable;
        }

        private void Buy()
        {
            _bank?.Withdraw(Cost);
        }

        private void Sell(int amount = 1)
        {
            _bank?.Deposit(Cost * amount);
        }
    }
}
