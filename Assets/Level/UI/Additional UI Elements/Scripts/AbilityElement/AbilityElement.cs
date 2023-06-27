using Game.CustomUI;
using System.Collections;
using System.Collections.Generic;
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

            _popupPanel = new PopupPanelCustom("something", "something", _mainContainer, PopupPositionAnchor.TopLeft);

            //_content.RegisterCallback<MouseOverEvent>(OnMouseOver);
            //_content.RegisterCallback<MouseOutEvent>(OnMouseOut);
            _mainContainer.RegisterCallback<ClickEvent>(MouseClick);
        }

        #region Events
        private void MouseClick(ClickEvent e)
        {
            Debug.Log("ClickEvent");
        }
        private void OnMouseOver(MouseOverEvent e)
        {
            Debug.Log("MouseOver");
            _popupPanel?.Show();
        }
        private void OnMouseOut(MouseOutEvent e)
        {
            Debug.Log("MouseOut");
            _popupPanel?.Hide();
        }
        #endregion
    }
}
