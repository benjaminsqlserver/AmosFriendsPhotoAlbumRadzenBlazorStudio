using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace AmosFriendsPhotoAlbum.Client.Pages
{
    public partial class Friends
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        public ConDataService ConDataService { get; set; }

        protected IEnumerable<AmosFriendsPhotoAlbum.Server.Models.ConData.Friend> friends;

        protected RadzenDataGrid<AmosFriendsPhotoAlbum.Server.Models.ConData.Friend> grid0;
        protected int count;

        protected string search = "";

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            await grid0.Reload();
        }

        protected async Task Grid0LoadData(LoadDataArgs args)
        {
            try
            {
                var result = await ConDataService.GetFriends(filter: $@"(contains(FirstName,""{search}"") or contains(LastName,""{search}"") or contains(EmailAddress,""{search}"") or contains(PhoneNumber,""{search}"")) and {(string.IsNullOrEmpty(args.Filter)? "true" : args.Filter)}", expand: "Gender", orderby: $"{args.OrderBy}", top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null);
                friends = result.Value.AsODataEnumerable();
                count = result.Count;
            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load Friends" });
            }
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddFriend>("Add Friend", null);
            await grid0.Reload();
        }

        protected async Task EditRow(DataGridRowMouseEventArgs<AmosFriendsPhotoAlbum.Server.Models.ConData.Friend> args)
        {
            await DialogService.OpenAsync<EditFriend>("Edit Friend", new Dictionary<string, object> { {"FriendID", args.Data.FriendID} });
            await grid0.Reload();
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, AmosFriendsPhotoAlbum.Server.Models.ConData.Friend friend)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await ConDataService.DeleteFriend(friendId:friend.FriendID);

                    if (deleteResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete Friend"
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await ConDataService.ExportFriendsToCSV(new Query
{
    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
    OrderBy = $"{grid0.Query.OrderBy}",
    Expand = "Gender",
    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
}, "Friends");
            }

            if (args == null || args.Value == "xlsx")
            {
                await ConDataService.ExportFriendsToExcel(new Query
{
    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
    OrderBy = $"{grid0.Query.OrderBy}",
    Expand = "Gender",
    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
}, "Friends");
            }
        }
    }
}