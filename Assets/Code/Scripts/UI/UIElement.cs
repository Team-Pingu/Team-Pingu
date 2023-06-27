using Game.CustomUI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.CustomUI
{
    interface IUIElement
    {
        public string Name { get; }
        public string Description { get; }
        public int Cost { get; }
        public GameResource Resource { get; }
        public string VIEW_ASSET_PATH { get; }

        private void Init()
        {
            // load view and set values to view
            VisualTreeAsset viewAsset;
            var __viewAssetResource = new GameResource(VIEW_ASSET_PATH, null, GameResourceType.UI);
            viewAsset = __viewAssetResource.LoadRessource<VisualTreeAsset>();
            viewAsset.CloneTree();
        }

        private bool IsAffordable(int globalCurrencyAmount)
        {
            if (this.Cost <= globalCurrencyAmount) return true;
            return false;
        }

        public void Spawn();
    }
}
