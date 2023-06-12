using Game.CustomUI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

namespace Game.CustomUI
{
    public enum PopupPositionAnchor
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
    };


    public class PopupPanelCustom : VisualElement
    {
        #region Boilerplate Component Code
        [UnityEngine.Scripting.Preserve]
        public new class UxmlFactory : UxmlFactory<PopupPanelCustom, UxmlTraits> { }
        #endregion

        public string Title;
        public string Description;
        public Vector2 Position;
        public PopupPositionAnchor PopupPositionAnchor;

        private readonly float CONTAINER_PADDING = 10f;
        private readonly string VIEW_ASSET_PATH = "Assets\\Level\\UI\\Additional UI Elements\\Scripts\\PopupPanelCustom\\PopupPanelCustom.uxml";

        private Label _titleLabel;
        private Label _descriptionLabel;
        private VisualElement _mainContainer;
        private VisualElement _popup;
        private VisualElement _root;

        public override VisualElement contentContainer => _mainContainer;

        public PopupPanelCustom()
        {
            Init();

            Title = _titleLabel.text;
            Description = _descriptionLabel.text;
            if (_mainContainer.style.left.keyword == StyleKeyword.Auto && _mainContainer.style.top.keyword == StyleKeyword.Auto)
            {
                PopupPositionAnchor = PopupPositionAnchor.TopLeft;
                Position = new Vector2(_mainContainer.style.left.value.value, _mainContainer.style.top.value.value);
            }
            else if (_mainContainer.style.right.keyword == StyleKeyword.Auto && _mainContainer.style.top.keyword == StyleKeyword.Auto)
            {
                PopupPositionAnchor = PopupPositionAnchor.TopRight;
                Position = new Vector2(_mainContainer.style.right.value.value, _mainContainer.style.top.value.value);
            }
            else if (_mainContainer.style.right.keyword == StyleKeyword.Auto && _mainContainer.style.bottom.keyword == StyleKeyword.Auto)
            {
                PopupPositionAnchor = PopupPositionAnchor.BottomRight;
                Position = new Vector2(_mainContainer.style.right.value.value, _mainContainer.style.bottom.value.value);
            }
            else if (_mainContainer.style.left.keyword == StyleKeyword.Auto && _mainContainer.style.bottom.keyword == StyleKeyword.Auto)
            {
                PopupPositionAnchor = PopupPositionAnchor.BottomLeft;
                Position = new Vector2(_mainContainer.style.left.value.value, _mainContainer.style.bottom.value.value);
            }
        }

        public PopupPanelCustom(string title, string description, VisualElement parent, PopupPositionAnchor popupPositionAnchor)
        {
            Init();

            Title = title;
            Description = description;
            Position = PositionFromVisualElement(parent);

            _titleLabel.text = title;
            _descriptionLabel.text = description;

            this.style.position = UnityEngine.UIElements.Position.Absolute;

            if (popupPositionAnchor == PopupPositionAnchor.TopLeft)
            {
                this.style.top = new StyleLength(Position.y);
                this.style.left = new StyleLength(Position.x);
                this.style.right = new StyleLength(StyleKeyword.Auto);
                this.style.bottom = new StyleLength(StyleKeyword.Auto);

                _mainContainer.style.paddingLeft = new StyleLength(CONTAINER_PADDING);
                _mainContainer.style.paddingTop = new StyleLength(CONTAINER_PADDING);
            }
            else if (popupPositionAnchor == PopupPositionAnchor.TopRight)
            {
                this.style.top = new StyleLength(Position.y);
                this.style.left = new StyleLength(StyleKeyword.Auto);
                this.style.right = new StyleLength(Position.x);
                this.style.bottom = new StyleLength(StyleKeyword.Auto);

                _mainContainer.style.paddingRight = new StyleLength(CONTAINER_PADDING);
                _mainContainer.style.paddingTop = new StyleLength(CONTAINER_PADDING);
            }
            else if (popupPositionAnchor == PopupPositionAnchor.BottomLeft)
            {
                this.style.top = new StyleLength(StyleKeyword.Auto);
                this.style.left = new StyleLength(Position.x);
                this.style.right = new StyleLength(StyleKeyword.Auto);
                this.style.bottom = new StyleLength(Position.y);

                _mainContainer.style.paddingLeft = new StyleLength(CONTAINER_PADDING);
                _mainContainer.style.paddingBottom = new StyleLength(CONTAINER_PADDING);
            }
            else
            {
                this.style.top = new StyleLength(StyleKeyword.Auto);
                this.style.left = new StyleLength(StyleKeyword.Auto);
                this.style.right = new StyleLength(Position.x);
                this.style.bottom = new StyleLength(Position.y);

                _mainContainer.style.paddingRight = new StyleLength(CONTAINER_PADDING);
                _mainContainer.style.paddingBottom = new StyleLength(CONTAINER_PADDING);
            }
        }

        private Vector3 PositionFromVisualElement(VisualElement visualElement)
        {
            return visualElement.transform.position;
            //// Get the RectTransform of the VisualElement
            //RectTransform rectTransform = (RectTransform)visualElement.transform;

            //// Get the position of the VisualElement in world space
            //Vector3 worldPosition = rectTransform.TransformPoint(new Vector3(rectTransform.rect.center.x, rectTransform.rect.center.y, 0));

            //// Get the screen coordinates of the VisualElement
            //Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

            //return screenPosition;
        }

        private VisualElement GetRootParentElement()
        {
            VisualElement parent = this;
            while (parent != null)
            {
                parent = parent.parent;
            }
            return parent;
        }

        private void Init()
        {
            // load view and set values to view
            VisualTreeAsset viewAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(VIEW_ASSET_PATH);

            viewAsset.CloneTree(this);

            _titleLabel = this.Q<Label>("c-popup__title");
            _descriptionLabel = this.Q<Label>("c-popup__description");
            _popup = this.Q<VisualElement>("c-popup");
            _mainContainer = this.Q<VisualElement>("c-popup-container");

            _root = GetRootParentElement();
        }

        public void Hide()
        {
            this.style.display = DisplayStyle.None;
        }

        public void Show()
        {
            this.style.display = DisplayStyle.Flex;
        }

        public bool IsActive() => this.style.display == DisplayStyle.Flex;
    }
}