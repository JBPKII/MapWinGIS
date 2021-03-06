﻿/*******************************************************************
 * 文件名：GisToolboxGroups.cs
 * 描  述：提供Gis工具集合处理操作
 * ******************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapWinGIS.Interfaces;
using System.Windows.Forms;
using System.Collections;

namespace MapWinGIS.Controls.GisToolbox
{
    /// <summary>
    ///  提供访问组工具箱组的列表
    /// </summary>
    public class GisToolboxGroups : IGisToolboxGroups
    {
        // 在树形视图下面的节点列表
        private TreeNodeCollection m_nodes = null;

        /// <summary>
        /// 创建一个新的GisTools类的实例，公共构造函数必须不可用
        /// </summary>
        /// <param name="nodes">节点集合</param>
        internal GisToolboxGroups(TreeNodeCollection nodes)
        {
            if (nodes == null)
                throw new NullReferenceException();//节点集合为空，抛出异常
            m_nodes = nodes;
        }

        #region ICollection<IGisToolboxGroup> 成员
        /// <summary>
        /// 添加新的工具到组中
        /// </summary>
        /// <param name="item">需要添加的新的工具</param>
        public void Add(IGisToolboxGroup item)
        {
            if (this.Equals(item))//需要添加的工具与被添加的组相同，抛出异常
                throw new InvalidOperationException();

            GisToolboxGroup group = item as GisToolboxGroup;
            if (group == null)//需要添加的工具是空的，抛出异常
                throw new InvalidCastException();

            int i = 0;
            for (; i < m_nodes.Count; i++)
            {
                if (m_nodes[i].Tag as IGisToolboxGroup == null)//检查当前组中是否有空位
                    break;
            }

            TreeNode node = ((GisToolboxGroup)item).Node;//数据类型转换

            if (i < m_nodes.Count)
                //如果当前组中有空位，将工具添加到空位上
                m_nodes.Insert(i, node);
            else
                //如果当前组没有空位，将工具添加到当前组的末尾
                m_nodes.Add(node);
        }

        /// <summary>
        /// 清除所有组
        /// </summary>
        public void Clear()
        {
            for (int i = m_nodes.Count - 1; i >= 0; i--)//循环删除树节点
            {
                IGisToolboxGroup group = m_nodes[i].Tag as IGisToolboxGroup;
                if (group != null)
                    m_nodes.RemoveAt(i);
            }
        }

        /// <summary>
        /// 判断组列表中是否包含特定组
        /// </summary>
        /// <param name="item">特定组</param>
        /// <returns>包含返回true，不包含返回false</returns>
        public bool Contains(IGisToolboxGroup item)
        {
            if (item == null)
                return false;

            for (int i = 0; i < m_nodes.Count; i++)
            {
                IGisToolboxGroup group = m_nodes[i].Tag as IGisToolboxGroup;
                if (group == item)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 复制组列表到数组中
        /// </summary>
        /// <param name="array">存储组列表的数组</param>
        /// <param name="arrayIndex">数组下标</param>
        public void CopyTo(IGisToolboxGroup[] array, int arrayIndex)
        {
            if (object.ReferenceEquals(array, null))//空数组引用
                throw new ArgumentNullException("空数组引用");

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("下标超出范围");

            if (array.Rank > 1)//获取数组的维数
                throw new ArgumentException("该数组是多维数组");

            IEnumerable<TreeNode> nodes = m_nodes.Cast<TreeNode>().Where(node => (node.Tag as IGisToolboxGroup != null));//把组列表m_ndoes中的节点遍历并存入nodes

            foreach (TreeNode node in nodes)//循环访问集合nodes
            {
                array.SetValue(node.Tag as IGisToolboxGroup, arrayIndex);//把nodes中的数据存入数组array中
                arrayIndex++;
            }
        }

        /// <summary>
        /// 获取组列表中的数量
        /// </summary>
        public int Count
        {
            get
            {
                return m_nodes.Cast<TreeNode>().Where(node => (node.Tag as IGisToolboxGroup != null)).Count();
            }
        }

        /// <summary>
        /// 获取只读标记的集合
        /// </summary>
        public bool IsReadOnly
        {
            get { return m_nodes.IsReadOnly; }
        }

        /// <summary>
        /// 删除指定位置的组
        /// </summary>
        /// <param name="item">需要删除的工具</param>
        /// <returns>成功删除返回true，失败返回false</returns>
        public bool Remove(IGisToolboxGroup item)
        {
            foreach (TreeNode node in m_nodes)
            {
                if (node.Tag as IGisToolboxGroup == item)
                {
                    m_nodes.Remove(node);
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region IEnumerable 成员
        /// <summary>
        /// 获取类型化的枚举集合
        /// </summary>
        public IEnumerator<IGisToolboxGroup> GetEnumerator()
        {
            return new GroupEnumerator(m_nodes);
        }

        /// <summary>
        /// 获取非类型化的枚举集合
        /// </summary>
        System.Collections.IEnumerator IEnumerable.GetEnumerator()
        {
            return new GroupEnumerator(m_nodes);
        }
        #endregion

        #region Enumerator
        /// <summary>
        /// 枚举组集合
        /// </summary>
        private class GroupEnumerator : System.Collections.Generic.IEnumerator<IGisToolboxGroup>, System.Collections.IEnumerator
        {
            /// <summary>
            /// 树节点集合
            /// </summary>
            TreeNodeCollection m_nodes = null;

            /// <summary>
            /// 下标索引
            /// </summary>
            int m_index = -1;

            /// <summary>
            /// 创建一个新的组枚举类实例
            /// </summary>
            /// <param name="nodes">树节点集合</param>
            internal GroupEnumerator(TreeNodeCollection nodes)
            {
                if (nodes == null)
                    throw new NullReferenceException();
                m_nodes = nodes;
            }

            /// <summary>
            /// 获得集合中当前项
            /// </summary>
            object IEnumerator.Current
            {
                get { return m_nodes[m_index].Tag; }
            }

            /// <summary>
            /// 获得当前项
            /// </summary>
            public IGisToolboxGroup Current
            {
                get { return (IGisToolboxGroup)m_nodes[m_index].Tag; }
            }

            /// <summary>
            /// 移动到下一项
            /// </summary>
            /// <returns>超出集合范围返回false，成功移动到下一项返回true</returns>
            public bool MoveNext()
            {
                do
                {
                    m_index++;
                    if (m_index == m_nodes.Count)//下一项超出了集合的范围
                        return false;
                    if (m_nodes[m_index].Tag as GisToolboxGroup != null)//下一项为非空
                        return true;
                } while (true);
            }

            /// <summary>
            /// 复位，设置枚举器在集合开始位置
            /// </summary>
            public void Reset()
            {
                m_index = -1;
            }

            /// <summary>
            /// 处理项
            /// </summary>
            public void Dispose()
            {
                //不做任何事
            }
        }
        #endregion
    }
}
