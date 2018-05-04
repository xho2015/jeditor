/*
 * 由SharpDevelop创建。
 * 用户： Bob (XuYong Hou) houxuyong@hotmail.com
 * 日期: 2018/4/16
 * 时间: 22:13
 * 
 * 
 */
using System;

namespace Snow.X.Algorithm
{
    /// <summary>
    /// TRIE dictionary implementation.
    /// </summary>
    public class TrieDict
    {
        private TNode root = new TNode();
        
        /// <summary>
        /// <para>scan support partial matching<para/>
        /// <para> e.g. if given keyworkd is ("a ", 1), when try matching "a bcd", 1 will return instead of -1 (failure) </para>
        /// </summary>
        /// <param name="content"></param>
        /// <returns>-1: no matching; larger than -1 : matched</returns>
        public int Scan(string content) {
            if (string.IsNullOrEmpty(content))
                return -1;
            int tag = -1;
            char[] bArray = content.ToCharArray();
            TNode p = root;
            for (int i=0; i<bArray.Length; i++) {
                if (bArray[i] > 127) return tag;
                p = p.child[bArray[i]];
                if (p == null)  
                	return tag;
                else
                	tag = p.Tag;
            }
            return p.Tag;
        }
         

        public void Insert(string word, int tag) {
            if (string.IsNullOrEmpty(word))
                return;

            char[] charArray = word.ToCharArray();
            TNode cNode = root;

            for (int i=0; i <word.Length; i++)
            {
                //9 horizental tab is the smallest char
                int index = charArray[i];
                if (cNode.child[index] == null)
                {
                    var pNode = new TNode();
                    cNode.child[index] = pNode;
                }
                cNode = cNode.child[index];   
            }
            cNode.count = 1; //this is the tail node of current key word
            //HXY: assign the char array (keyword) to tail Node                   
            cNode.Tag = tag;
        }
        
        public int Allocated() {
            return TNode.allocated;
        }
    }
    
    class TNode {
        public int count {set; get;}
        public TNode[] child { set; get; }
        public int Tag  = -1;
        public static int allocated;
        public TNode()
        {
            count = 0;
            child = new TNode[127]; //don't support char > 127
            allocated+=child.Length;
        }
    }
}
