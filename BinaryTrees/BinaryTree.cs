using System;
using System.Collections;
using System.Collections.Generic;

namespace Generics.BinaryTrees
{
    public class BinaryTree<T> : IEnumerable<T> where T:IComparable<T>
    {
	    public BinaryTree<T> Parent { get; private set; }

        private BinaryTree<T> _left;
        private BinaryTree<T> _right;
        
        public BinaryTree<T> Left => _left ?? (_left = new BinaryTree<T>());
        public BinaryTree<T> Right => _right ?? (_right = new BinaryTree<T>());
        
        public T Value { get; private set; }
        public bool HasValue { get; private set; }

        public BinaryTree()
		{
			HasValue = false; 
			Parent = null;
			Value = default;
		}

        private static void CreateCell(T value, BinaryTree<T> currentNode, BinaryTree<T> parent)
        {
	        while (true)
	        {
		        if (!currentNode.HasValue)
		        {
			        currentNode.Value = value;
			        currentNode.Parent = parent;
			        currentNode.HasValue = true;
			        return;
		        }

		        var difference = value.CompareTo(currentNode.Value);
		        var currentNode1 = currentNode;
		        currentNode = difference == 1 ? currentNode.Right : currentNode.Left;
		        parent = currentNode1;
	        }
        }

        public void Add(T value)
        {
            if (!HasValue)
            {
                Value = value;
                HasValue = true;
            }
            else
            {
	            CreateCell(value, value.CompareTo(Value) == 1 ? Right : Left, this);
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator()
		{ 
			return new TreeCell<T>(this);
		}
	}

    public static class BinaryTree
    {
        public static IEnumerable<int> Create(params int[] items)
        {
            var binaryTree = new BinaryTree<int>();
            foreach(var item in items)
            {
                binaryTree.Add(item);
            }
            return binaryTree;
        }
    }
    
    internal class TreeCell<T> : IEnumerator<T> where T : IComparable<T>
	{
		private BinaryTree<T> OriginalTree { get; }
		private BinaryTree<T> CurrentCell { get; set; }
		object IEnumerator.Current => Current;
		public T Current => CurrentCell.Value;

		public TreeCell(BinaryTree<T> cell)
		{
			OriginalTree = cell;
			CurrentCell = null;
		}

		public bool MoveNext()
		{
			if (!OriginalTree.HasValue) 
				return false;

			if (CurrentCell == null)
				CurrentCell = GoToLeftEdge(OriginalTree);
			else
			{
				if (CurrentCell.Right.HasValue)
					CurrentCell = GoToLeftEdge(CurrentCell.Right);
				else
				{
					var childCellValue = CurrentCell.Value;

					while (CurrentCell.HasValue)
					{
						CurrentCell = CurrentCell.Parent;
						if (CurrentCell != null && Current != null)
						{ 
							var compare = Current.CompareTo(childCellValue); 
							if (compare < 0) continue;
						}
						break;
					}
				}
			}
			return CurrentCell != null;
		}

		public void Reset()
		{
			CurrentCell = new BinaryTree<T>();
		}

		private static BinaryTree<T> GoToLeftEdge(BinaryTree<T> treeCore)
		{
			var cell = treeCore;
			while (cell.Left.HasValue) 
				cell = cell.Left;
			return cell;
		}

		public void Dispose() { }
	}
}