using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Security.Principal;

namespace GitHooks
{
    class ListItemOwnerChecker
    {
        public static string GetListItemOwner(int listId, int itemId)
        {
            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true
            };

            var client = new HttpClient(handler);

            var response = client.GetAsync($"https://localhost:44389/api/lists/{listId}/ownerOf/{itemId}")
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            var owner = response.Content
                .ReadAsStringAsync()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            return JsonConvert.DeserializeObject<string>(owner);
        }

        public static bool DoesCurrentUserOwnListItem(int listId, int itemId)
        {
            var owner = GetListItemOwner(listId, itemId);

            if (owner == null)
            {
                Console.WriteLine($"There is no item '{itemId}' in the list '{listId}' registered on the lists service.");
                return false;
            }

            if (owner != WindowsIdentity.GetCurrent().Name)
            {
                Console.WriteLine($"Item '{itemId}' in the list '{listId}' registered by '{owner}' and you are '{WindowsIdentity.GetCurrent().Name}'.");
                return false;
            }

            return true;
        }
    }
}
