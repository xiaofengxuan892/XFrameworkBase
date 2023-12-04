using System;
using System.Collections;
using System.Collections.Generic;

namespace XFrameworkBase.DataType
{
    public class XFrameworkLinkedList<T>
    {
        private readonly LinkedList<T> m_LinkedList;
        private readonly Queue<LinkedListNode<T>> m_CachedNodes;

        public XFrameworkLinkedList() {
            m_LinkedList = new LinkedList<T>();
            m_CachedNodes = new Queue<LinkedListNode<T>>();
        }
        
        #region 为方便集合内部元素的重复利用而提供的“private”方法
        //从“缓存队列m_CachedNodes”中获取可用的节点Node
        private LinkedListNode<T> SpawnNode(T value) {
            LinkedListNode<T> node = null;
            if (m_CachedNodes.Count > 0) {
                node = m_CachedNodes.Dequeue();
                node.Value = value;
            }
            else {
                node = new LinkedListNode<T>(value);
            }

            return node;
        }

        //将某个节点进行回收
        private void UnSpawnNode(LinkedListNode<T> node) {
            node.Value = default(T);
            m_CachedNodes.Enqueue(node);
        }
        
        #endregion
        
        #region 该集合提供给外部使用的public方法

        //将“指定value的新节点”添加到集合的不同位置
        public LinkedListNode<T> AddAfter(LinkedListNode<T> node, T value) {
            LinkedListNode<T> mNode = SpawnNode(value);
            m_LinkedList.AddAfter(node, mNode);
            return mNode;
        }
        
        public LinkedListNode<T> AddBefore(LinkedListNode<T> node, T value) {
            LinkedListNode<T> mNode = SpawnNode(value);
            m_LinkedList.AddBefore(node, mNode);
            return mNode;
        }

        public LinkedListNode<T> AddFirst(T value) {
            LinkedListNode<T> mNode = SpawnNode(value);
            m_LinkedList.AddFirst(mNode);
            return mNode;
        }

        public LinkedListNode<T> AddLast(T value) {
            LinkedListNode<T> mNode = SpawnNode(value);
            m_LinkedList.AddLast(mNode);
            return mNode;
        }
        
        //删除集合中的指定节点
        public bool Remove(T value) {
            LinkedListNode<T> mNode = m_LinkedList.Find(value);
            if (mNode == null) return false;

            m_LinkedList.Remove(mNode);
            UnSpawnNode(mNode);
            return true;
        }

        //移除链表的“首节点”，外部通常并不需要返回值，因此设置为“void”
        public void RemoveFirst() {
            LinkedListNode<T> mNode = m_LinkedList.First;
            if (mNode == null) return;
            
            m_LinkedList.RemoveFirst();
            UnSpawnNode(mNode);
        }

        public void RemoveLast() {
            LinkedListNode<T> mNode = m_LinkedList.Last;
            if(mNode == null) return;
            
            m_LinkedList.RemoveLast();
            UnSpawnNode(mNode);
        }
        
        //清除集合中的元素
        //注意：默认会将集合中所有的“LinkedListNode”缓存起来，方便再次向该集合添加元素
        public void Clear(bool keepCachedNodes = true) {
            if (!keepCachedNodes) {
                m_CachedNodes.Clear();
                m_LinkedList.Clear();
                return;
            }

            LinkedListNode<T> current = m_LinkedList.First;
            if (current != null) {
                UnSpawnNode(current);
                current = current.Next;
            }
            m_LinkedList.Clear();
        }
        
        #endregion

        #region 该集合提供给外部常用的一些属性

        //本集合的元素数量
        public int Count {
            get { return m_LinkedList.Count; }
        }

        //本集合的第一个T元素
        //注意：由于有可能返回“default(T)”，因此需要结合“属性Count”才能确定“该返回值的准确性”
        public T First {
            get { return m_LinkedList.First != null ? m_LinkedList.First.Value : default(T); }
        }

        //本集合的最后一个元素
        public T Last {
            get { return m_LinkedList.Last != null ? m_LinkedList.Last.Value : default(T); }
        }
        
        #endregion


    }
}