#ifndef _TREE_H_
#define _TREE_H_

namespace UnE
{
	template <class T>
	class Tree
	{
	public:
		class Node
		{
		public:
			Node();
			~Node();
			void SetData(const T& Data);
			T& GetData();
			Node* GetNext();
			Node* GetPrev();
			Node* GetParent();
			Node* GetChild();
			friend class Tree<T>;
			friend class EasyXML2;

		private:
			Node* next;
			Node* prev;
			Node* parent;
			Node* child;
			T data;
		};

	public:
		Tree();
		~Tree();
		Node* Insert(const T& data, Node* pNode = 0);
		Node* InsertBefore(const T& data, Node* pNode);
		Node* InsertAfter(const T& data, Node* pNode);
		// 자식 노드가 존재할 경우 자식 노드는 모두 삭제된다.
		void Remove(Node* pNode);
		void RemoveAll();
		// pNode 또는 그 하위 노드에서 data를 가진 노드가 존재하는지 검사한다.
		Node* Search(const T& Data, Node* pNode = 0);
		Node* root;
	};

	template <class T>
	Tree<T>::Node::Node()
	{
		next = prev = parent = child = 0;
	}

	template <class T>
	Tree<T>::Node::~Node()
	{
	}

	template <class T>
	void Tree<T>::Node::SetData(const T& Data)
	{
		data = Data;
	}

	template <class T>
	T& Tree<T>::Node::GetData()
	{
		return data;
	}

	template <class T>
	typename Tree<T>::Node* Tree<T>::Node::GetNext()
	{
		return next;
	}

	template <class T>
	typename Tree<T>::Node* Tree<T>::Node::GetPrev()
	{
		return prev;
	}

	template <class T>
	typename Tree<T>::Node* Tree<T>::Node::GetParent()
	{
		return parent;
	}

	template <class T>
	typename Tree<T>::Node* Tree<T>::Node::GetChild()
	{
		return child;
	}

	template <class T>
	Tree<T>::Tree()
	{
		root = 0;
	}

	template <class T>
	Tree<T>::~Tree() 
	{
		RemoveAll();
	}

	template <class T>
	typename Tree<T>::Node* Tree<T>::Insert(const T& data, typename Tree<T>::Node* pNode)
	{
		// Root Node에 삽입하라
		if (pNode == 0)
		{
			if (root != 0) return 0;
			else
			{
				Node* nod = new Node;
				nod->data = data;
				root = nod;
				return nod;
			}
		}

		Node* nod = new Node;
		nod->data = data;

		Node* pChild = pNode->child;

		if (pChild == 0)
		{
			pNode->child = nod;
			nod->parent	 = pNode;
		}
		else
		{
			Node* pPrev;

			while (pChild)
			{
				pPrev  = pChild;
				pChild = pChild->next;
			}

			pPrev->next = nod;
			nod->parent = pNode;
			nod->prev	= pPrev;
		}

		return nod;
	}

	template <class T>
	typename Tree<T>::Node* Tree<T>::InsertBefore(const T& data, typename Tree<T>::Node* pNode)
	{
		// Root Node 앞,뒤에는 삽입할 수 없다.
		if (pNode == root) return 0;

		Node* nod = new Node;
		nod->data = data;

		Node* pPrev = pNode->prev;

		if (pPrev == 0)
		{
			pNode->prev = nod;
			nod->next	= pNode;
			nod->parent = pNode->parent;
		
			if (nod->parent)
			{
				// nod가 제일 첫번째 child가 된다.
				nod->parent->child = nod;
			}
		}
		else
		{
			pPrev->next = nod;
			pNode->prev = nod;
			nod->next	= pNode;
			nod->prev	= pPrev;
			nod->parent = pNode->parent;
		}

		return nod;
	}

	template <class T>
	typename Tree<T>::Node* Tree<T>::InsertAfter(const T& data, typename Tree<T>::Node* pNode)
	{
		// Root Node 앞,뒤에는 삽입할 수 없다.
		if (pNode == root) return 0;

		Node* nod = new Node;
		nod->data = data;

		Node* pNext = pNode->next;

		if (pNext == 0)
		{
			pNode->next = nod;
			nod->prev	= pNode;
			nod->parent = pNode->parent;
		}
		else
		{
			pNext->prev = nod;
			pNode->next = nod;
			nod->next	= pNext;
			nod->prev	= pNode;
			nod->parent = pNode->parent;
		}

		return nod;
	}

	// 자식 노드가 존재할 경우 자식 노드는 모두 삭제된다.
	template <class T>
	void Tree<T>::Remove(typename Tree<T>::Node* pNode)
	{
		if (pNode == 0) return;

		Node* pParent = pNode->parent;
		Node* pNext	 = pNode->next;
		Node* pPrev	 = pNode->prev;
		Node* pChild	 = pNode->child;

		while (pChild)
		{
			Node* prevNode = pChild;
			pChild = pChild->next;

			prevNode->prev = 0;
			prevNode->next = 0;
			if (pChild) pChild->prev = 0;
		
			if (prevNode->child == 0) delete prevNode;
			else Remove(prevNode);
		}

		if (pNext)
		{
			pNext->prev = pPrev;
		}

		if (pPrev)
		{
			pPrev->next = pNext;
		}

		if (pParent)
		{
			if (pParent->child == pNode)
			{
				pParent->child = pNode->next;
			}
		}

		if (pNode == root) root = 0;
		delete pNode;
	}

	template <class T>
	void Tree<T>::RemoveAll()
	{
		Remove(root);
	}

	// pNode 또는 그 하위 노드에서 data를 가진 노드가 존재하는지 검사한다.
	template <class T>
	typename Tree<T>::Node* Tree<T>::Search(const T& data, typename Tree<T>::Node* pNode)
	{
		if (pNode == 0) 
		{
			if (root == 0) return 0;
			pNode = root;
		}

		Node* nod;

		if (pNode->data == data) return pNode;
		Node* pChild = pNode->child;

		while (pChild)
		{
			nod = Search(data,pChild);
			if (nod) return nod;
			pChild = pChild->next;
		}

		return 0;
	}
}

#endif
