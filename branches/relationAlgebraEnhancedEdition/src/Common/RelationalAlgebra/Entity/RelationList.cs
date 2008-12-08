using System;
using System.Collections.Generic;
using System.Text;

namespace DistDBMS.Common.RelationalAlgebra.Entity
{
 /*   public class RelationList:ICollection<Relation>
    {
        List<Relation> innerList;

        Relation parent;

        public RelationList(Relation parent)
        {
            this.parent = parent;
            innerList = new List<Relation>();
        }

        public Relation this[int index]
        {
            get{return innerList[index];}
            set{innerList[index] = value;}
        }

        #region ICollection<Relation> Members

        public void Add(Relation item)
        {
            item.Parent = parent;
            innerList.Add(item);
        }

        public void Clear()
        {
            innerList.Clear();
        }

        public bool Contains(Relation item)
        {
            return innerList.Contains(item);
        }

        public void CopyTo(Relation[] array, int arrayIndex)
        {
            innerList.CopyTo(array,arrayIndex);
        }

        public int Count
        {
            get { return innerList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(Relation item)
        {
            bool result = innerList.Remove(item);
            if (result)
                item.Parent = null;

            return result;
        }

        #endregion

        #region IEnumerable<Relation> Members

        public IEnumerator<Relation> GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        #endregion
    }*/
}
