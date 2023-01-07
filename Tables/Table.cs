using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Generics.Tables
{
    public class Table<TRow, TColumn, TValue> : Hashtable
    {
        public readonly List<TRow> Rows = new List<TRow>();
        public readonly List<TColumn> Columns = new List<TColumn>();

        private readonly List<KeyValuePair<Tuple<TRow, TColumn>, TValue>>
            _cells = new List<KeyValuePair<Tuple<TRow, TColumn>, TValue>>();
        
        internal bool RowExists(TRow row) =>
            Rows.Contains(row) && Rows.Count != 0;

        internal bool ColumnExists(TColumn column) =>
            Columns.Contains(column) && Columns.Count != 0;

        public void AddRow(TRow row)
        {
            if (!RowExists(row))
                Rows.Add(row);
        }

        public void AddColumn(TColumn column)
        {
            if (!ColumnExists(column))
                Columns.Add(column);
        }
        
        public TValue this[TRow row, TColumn column]
        {
            get
            {
                return _cells.Where(keyValuePair => keyValuePair.Key.Item1.Equals(row) 
                && keyValuePair.Key.Item2.Equals(column))
                    .Select(keyValuePair => keyValuePair.Value)
                    .FirstOrDefault();
            }
            set
            {
                _cells.Add(new KeyValuePair<Tuple<TRow, TColumn>, TValue>
                    (new Tuple<TRow, TColumn>(row, column), value));
                AddRow(row);
                AddColumn(column);
            }
        }

        public Table<TRow, TColumn, TValue> Open => this;

        public CellValue<TRow, TColumn, TValue> Existed => new CellValue<TRow, TColumn, TValue>(this);
    }
    
    public class CellValue<TRow, TColumn, TValue>
    {
        private readonly Table<TRow, TColumn, TValue> _table;
       
        public CellValue(Table<TRow, TColumn, TValue> table)
        {
            _table = table;
        }
        
        private bool CheckExistence(TRow row, TColumn column)
        {
            return _table.RowExists(row) && _table.ColumnExists(column);
        }
        
        public TValue this[TRow row, TColumn column]
        {
            get
            {
                if (!CheckExistence(row, column))
                    throw new ArgumentException();
                return _table[row, column];
            }
            set
            {
                if (!CheckExistence(row, column))
                    throw new ArgumentException(); 
                _table[row, column] = value;
            }
        }
    }
}