using System.Collections.Generic;

namespace BeanScene.Web.Models
{
    public class DoublyLinkedListViewModel
    {
        public List<int> Forward { get; set; } = new List<int>();
        public List<int> Backward { get; set; } = new List<int>();

        // For forms
        public int? ValueToAdd { get; set; }
        public int? ValueToDelete { get; set; }
        public string OperationMessage { get; set; }
    }
}
