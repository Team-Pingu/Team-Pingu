using Game.CustomUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.DebugUI;
using Label = UnityEngine.UIElements.Label;

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
        public string ExtraInfo;
        public Vector2 Position;
        public PopupPositionAnchor PopupPositionAnchor;

        private readonly float CONTAINER_PADDING = 10f;
        private readonly string VIEW_ASSET_PATH = "Assets/Level/UI/Additional UI Elements/Scripts/PopupPanelCustom/PopupPanelCustom.uxml";

        private Label _titleLabel;
        private Label _descriptionLabel;
        private Label _extraInfoLabel;
        private VisualElement _mainContainer;
        private VisualElement _popup;
        private VisualElement _root;

        public override VisualElement contentContainer => _mainContainer;

        public PopupPanelCustom()
        {
            Init();

            Title = _titleLabel.text;
            Description = _descriptionLabel.text;
            /*if (_mainContainer.style.left.keyword == StyleKeyword.Auto && _mainContainer.style.top.keyword == StyleKeyword.Auto)
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
            }*/
        }

        public PopupPanelCustom(string title, string description, VisualElement target, string extraInfo = null, PopupPositionAnchor popupPositionAnchor = PopupPositionAnchor.TopLeft)
        {
            Init();

            Title = title;
            Description = description;
            ExtraInfo = extraInfo;
            Position = PositionFromVisualElement(target);

            _titleLabel.text = title;
            _descriptionLabel.text = description;
            _extraInfoLabel.text = extraInfo;

            //Task.Delay(2000).ContinueWith(t =>
            //{
            //    SetScreenPos(target);
            //    Debug.Log("TESTT");
            //});
        }

        private Vector2 PositionFromVisualElement(VisualElement visualElement)
        {
            Rect _worldBound = visualElement.worldBound;
            return new Vector2(_worldBound.position.x + _worldBound.width, _worldBound.position.y + _worldBound.height * 0.5f);
        }

        public void SetScreenPos(VisualElement visualElement)
        {
            var pos = PositionFromVisualElement(visualElement);
            style.marginLeft = CONTAINER_PADDING;
            style.marginTop = -this.worldBound.height * 0.5f;
            style.left = new StyleLength(pos.x);
            style.top = new StyleLength(pos.y);
            style.right = new StyleLength(StyleKeyword.Auto);
            style.bottom = new StyleLength(StyleKeyword.Auto);
        }

        private void Init()
        {
            // load view and set values to view
            VisualTreeAsset viewAsset;
            var __viewAssetResource = new GameResource(VIEW_ASSET_PATH, null, GameResourceType.UI);
            viewAsset = __viewAssetResource.LoadRessource<VisualTreeAsset>();
            viewAsset.CloneTree(this);

            _root = GameObject.Find("UIDocument")?.GetComponent<UIDocument>()?.rootVisualElement;
            _titleLabel = this.Q<Label>("c-popup__title");
            _descriptionLabel = this.Q<Label>("c-popup__description");
            _extraInfoLabel = this.Q<Label>("c-popup__extra-info");
            _popup = this.Q<VisualElement>("c-popup");
            _mainContainer = this.Q<VisualElement>("c-popup-container");
            _root?.Add(this);

            pickingMode = PickingMode.Ignore;
            style.visibility = Visibility.Hidden;
            style.opacity = 0f;
            style.position = UnityEngine.UIElements.Position.Absolute;
        }

        public long FadeTime = 10;
        public long Delay = 0;
        private IVisualElementScheduledItem task;

        public void Hide()
        {
            if (FadeTime > 0.0f && resolvedStyle.visibility != Visibility.Hidden)
            {
                task?.Pause();
                task = schedule
                    .Execute(() =>
                    {
                        var o = Mathf.Clamp01(resolvedStyle.opacity - 0.1f);
                        style.opacity = o;
                        if (o <= 0.0f) style.visibility = Visibility.Hidden;
                    })
                    .Every(FadeTime) // ms						
                    .Until(() => resolvedStyle.opacity <= 0.0f);
            }
            else
            {
                style.visibility = Visibility.Hidden;
                style.opacity = 0f;
            }
        }

        public void Show()
        {
            if (FadeTime > 0.0f)
            {
                task?.Pause();
                style.visibility = Visibility.Visible;
                style.opacity = 0f;
                task = schedule
                    .Execute(() => style.opacity = Mathf.Clamp01(resolvedStyle.opacity + 0.1f))
                    .Every(FadeTime) // ms	
                    .Until(() => resolvedStyle.opacity >= 1.0f)
                    .StartingIn(Delay);
            }
            else
            {
                style.visibility = Visibility.Visible;
                style.opacity = 1f;
            }
        }

        public bool IsActive() => style.visibility == Visibility.Visible;
    }
}