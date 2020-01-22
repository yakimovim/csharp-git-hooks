using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListsService.Controllers
{
    public sealed class ListItem<T>
    {
        public ListItem(T value, string owner)
        {
            Value = value;
            Owner = owner;
        }

        public T Value { get; }
        public string Owner { get; }
    }

    public static class Lists
    {
        public static List<ListItem<int>> SqlVersions = new List<ListItem<int>>
        {
            new ListItem<int>(1, @"DOMAIN\Ivan"),
            new ListItem<int>(2, @"DOMAIN\Ivan"),
        };
        public static List<ListItem<int>> Constants = new List<ListItem<int>>
        {
            new ListItem<int>(1, @"DOMAIN\Ivan"),
            new ListItem<int>(2, @"DOMAIN\Ivan"),
            new ListItem<int>(3, @"DOMAIN\Ivan")
        };

        public static Dictionary<int, List<ListItem<int>>> AllLists = new Dictionary<int, List<ListItem<int>>>
        {
            {1, SqlVersions},
            {2, Constants},
        };
    }

    [Authorize]
    public class ListsController : Controller
    {
        [Route("/api/lists/{listId}")]
        [HttpGet]
        public IActionResult Index(int listId)
        {
            if (!Lists.AllLists.ContainsKey(listId))
                return NotFound();

            return Json(Lists.AllLists[listId]);
        }

        [Route("/api/lists/{listId}/ownerOf/{itemId}")]
        [HttpGet]
        public IActionResult GetOwner(int listId, int itemId)
        {
            if (!Lists.AllLists.ContainsKey(listId))
                return NotFound();

            var item = Lists.AllLists[listId].FirstOrDefault(li => li.Value == itemId);
            if(item == null)
                return NotFound();

            return Json(item.Owner);

        }
    }
}