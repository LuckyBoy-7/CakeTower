using System;
using Lucky.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Lucky.Collections
{
    /// <summary>
    /// 一个格子只能存一个item, 并且一个item也只能在一个位置
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class GridManager<T> : ICloneable
    {
        [ShowInInspector] private DefaultDict<Vector2Int, T> gridPosToItem = new DefaultDict<Vector2Int, T>(() => default);
        [ShowInInspector] private DefaultDict<T, Vector2Int> itemToGridPos = new DefaultDict<T, Vector2Int>(() => default);
        private float cellWidth;
        private float cellHeight;
        private Func<T> defaultValueFunc;
        private Func<Vector2> pivotPosFunc;
        public Vector2 CellSize => new Vector2(cellWidth, cellHeight);
        public Vector2 HalfCellSize => CellSize / 2;

        /// 最左下角的位置, 或者把它想象成一个大网格就是原点的位置
        public Vector3 Origin => pivotPosFunc?.Invoke() ?? Vector2.zero;

        private Vector3 CenterOffset(bool center) => center ? HalfCellSize : Vector3.zero;

        public Vector2Int WorldPosToGridPos(Vector3 worldPos) => ((worldPos - Origin) / CellSize).FloorToVector2Int();
        public Vector3 GridPosToWorldPos(Vector2Int gridPos, bool center = true) => Origin + (Vector3)(gridPos * CellSize) + CenterOffset(center);
        public Vector3 GridPosToWorldPos(int x, int y, bool center = true) => GridPosToWorldPos(new Vector2Int(x, y), center);
        public Vector3 SnapWorldPosToGridPos(Vector3 position, bool center = true) => GridPosToWorldPos(WorldPosToGridPos(position)) + CenterOffset(center);
        public Vector2Int ItemToGridPos(T item) => itemToGridPos[item];


        public GridManager(float cellWidth, float cellHeight, Func<T> defaultValueFunc, Func<Vector2> pivotPosFunc = null)
        {
            this.cellWidth = cellWidth;
            this.cellHeight = cellHeight;
            this.defaultValueFunc = defaultValueFunc;
            this.pivotPosFunc = pivotPosFunc;
        }

        public void Reset()
        {
            gridPosToItem.Clear();
            itemToGridPos.Clear();
        }

        public void SetItem(T item, Vector2Int gridPos)
        {
            gridPosToItem[gridPos] = item;
            itemToGridPos[item] = gridPos;
        }

        public void DiscardItem(T item, Vector2Int gridPos)
        {
            gridPosToItem[gridPos] = defaultValueFunc();
            itemToGridPos[item] = Vector2Int.zero;
        }

        public T this[int x, int y]
        {
            get => gridPosToItem[new Vector2Int(x, y)];
            set => SetItem(value, new Vector2Int(x, y));
        }

        public T this[Vector2Int gridPosition]
        {
            get => gridPosToItem[gridPosition];
            set => SetItem(value, gridPosition);
        }

        public void Swap(Vector2Int pos0, Vector2Int pos1) => (this[pos0], this[pos1]) = (this[pos1], this[pos0]);

        #region Vector2

        public void SetItem(T item, Vector2 position)
        {
            gridPosToItem[WorldPosToGridPos(position)] = item;
            itemToGridPos[item] = WorldPosToGridPos(position);
        }

        public void DiscardItem(T item, Vector2 position)
        {
            gridPosToItem[WorldPosToGridPos(position)] = default;
            itemToGridPos[item] = Vector2Int.zero;
        }

        public T this[float x, float y]
        {
            get => gridPosToItem[WorldPosToGridPos(new Vector2(x, y))];
            set => SetItem(value, WorldPosToGridPos(new Vector2(x, y)));
        }

        public T this[Vector2 position]
        {
            get => gridPosToItem[WorldPosToGridPos(position)];
            set => SetItem(value, WorldPosToGridPos(position));
        }

        public void Swap(Vector2 pos0, Vector2 pos1) => (this[pos0], this[pos1]) = (this[pos1], this[pos0]);

        #endregion

        public object Clone()
        {
            GridManager<T> other = new GridManager<T>(cellWidth, cellHeight, defaultValueFunc, pivotPosFunc);
            other.gridPosToItem = (DefaultDict<Vector2Int, T>)gridPosToItem.Clone();
            other.itemToGridPos = (DefaultDict<T, Vector2Int>)itemToGridPos.Clone();
            return other;
        }
    }
}