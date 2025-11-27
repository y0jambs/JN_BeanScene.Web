using System.Collections.Generic;

namespace BeanScene.Web.Models.DataStructures
{
    public class DoublyLinkedList
    {
        private Node head;

        // Add node at the end
        public void AddLast(int data)
        {
            Node newNode = new Node(data);
            if (head == null)
            {
                head = newNode;
                return;
            }

            Node current = head;
            while (current.Next != null)
            {
                current = current.Next;
            }

            current.Next = newNode;
            newNode.Prev = current;
        }

        // Add node at the beginning
        public void AddFirst(int data)
        {
            Node newNode = new Node(data);
            if (head == null)
            {
                head = newNode;
                return;
            }

            newNode.Next = head;
            head.Prev = newNode;
            head = newNode;
        }

        // Delete a node by value
        public void Delete(int data)
        {
            Node current = head;
            while (current != null)
            {
                if (current.Data == data)
                {
                    if (current.Prev != null)
                        current.Prev.Next = current.Next;
                    else
                        head = current.Next; // deleting the head

                    if (current.Next != null)
                        current.Next.Prev = current.Prev;

                    return;
                }
                current = current.Next;
            }
        }

        // Convert to list (forward)
        public List<int> ToForwardList()
        {
            var result = new List<int>();
            Node current = head;
            while (current != null)
            {
                result.Add(current.Data);
                current = current.Next;
            }
            return result;
        }

        // Convert to list (backward)
        public List<int> ToBackwardList()
        {
            var result = new List<int>();
            if (head == null) return result;

            Node current = head;
            while (current.Next != null)
                current = current.Next;

            while (current != null)
            {
                result.Add(current.Data);
                current = current.Prev;
            }
            return result;
        }
    }
}
