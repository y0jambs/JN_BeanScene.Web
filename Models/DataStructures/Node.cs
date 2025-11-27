namespace BeanScene.Web.Models.DataStructures
{
    public class Node
    {
        public int Data;
        public Node Prev;
        public Node Next;

        public Node(int data)
        {
            Data = data;
            Prev = null;
            Next = null;
        }
    }
}
