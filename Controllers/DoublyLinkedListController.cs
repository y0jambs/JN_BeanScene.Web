using BeanScene.Web.Models;
using BeanScene.Web.Models.DataStructures;
using Microsoft.AspNetCore.Mvc;

namespace BeanScene.Web.Controllers
{
    public class DoublyLinkedListController : Controller
    {
        // Shared list for all users (demo)
        private static readonly DoublyLinkedList _list = new DoublyLinkedList();

        public IActionResult Index()
        {
            var model = new DoublyLinkedListViewModel
            {
                Forward = _list.ToForwardList(),
                Backward = _list.ToBackwardList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult AddFirst(DoublyLinkedListViewModel model)
        {
            if (model.ValueToAdd.HasValue)
            {
                _list.AddFirst(model.ValueToAdd.Value);
                model.OperationMessage = $"Added {model.ValueToAdd.Value} to the beginning.";
            }

            model.Forward = _list.ToForwardList();
            model.Backward = _list.ToBackwardList();
            model.ValueToAdd = null;

            return View("Index", model);
        }

        [HttpPost]
        public IActionResult AddLast(DoublyLinkedListViewModel model)
        {
            if (model.ValueToAdd.HasValue)
            {
                _list.AddLast(model.ValueToAdd.Value);
                model.OperationMessage = $"Added {model.ValueToAdd.Value} to the end.";
            }

            model.Forward = _list.ToForwardList();
            model.Backward = _list.ToBackwardList();
            model.ValueToAdd = null;

            return View("Index", model);
        }

        [HttpPost]
        public IActionResult Delete(DoublyLinkedListViewModel model)
        {
            if (model.ValueToDelete.HasValue)
            {
                _list.Delete(model.ValueToDelete.Value);
                model.OperationMessage = $"Deleted first occurrence of {model.ValueToDelete.Value} (if it existed).";
            }

            model.Forward = _list.ToForwardList();
            model.Backward = _list.ToBackwardList();
            model.ValueToDelete = null;

            return View("Index", model);
        }
    }
}
