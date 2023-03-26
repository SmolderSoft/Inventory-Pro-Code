using UnityEngine;

namespace Devdog.InventoryPro
{
    public abstract class ItemDropHandlerBase : ScriptableObject
    {
        public abstract GameObject DropItem(InventoryItemBase item);
        public abstract GameObject DropItem(InventoryItemBase item, Vector3 position, Quaternion rotation);
        public abstract Vector3? CalculateDropPosition(InventoryItemBase item);
    }
}
