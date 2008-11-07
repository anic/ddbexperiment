using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DistDBMS.UserInterface.SqlInput.GListBox
{
    public class GListBoxItem
    {
        private string m_Text;
        private int m_ImageIndex;
        private object m_Tag;
        // 属性 
        public string Text { 
            get { return m_Text; } 
            set { m_Text = value; } 
        } 

        public int ImageIndex { 
            get { return m_ImageIndex; } 
            set { m_ImageIndex = value; } 
        }


        public object Tag {
            get
            {
                return m_Tag;
            }
            set
            {
                m_Tag = value;
            }
        }

        int level;
        public int Level {
            get
            {
                return level;
            }
            set
            {
                if (value >= 0)
                    level = value;
                else
                    throw new Exception("Attribute Value can not be set less than 0");
            }
        }

        //构造函数 
        public GListBoxItem(string text, int index) {
            m_Text = text; 
            m_ImageIndex = index;
            level = 0;
        } 
        
        public GListBoxItem(string text) : this(text, -1) 
        { 

        } 

        public GListBoxItem() : this("") { } 
        
        public override string ToString() { return m_Text; }
    }
}
