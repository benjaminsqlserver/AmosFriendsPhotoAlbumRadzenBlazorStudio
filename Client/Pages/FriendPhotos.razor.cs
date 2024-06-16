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
    public partial class FriendPhotos
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

        protected IEnumerable<AmosFriendsPhotoAlbum.Server.Models.ConData.FriendPhoto> friendPhotos;

        protected RadzenDataGrid<AmosFriendsPhotoAlbum.Server.Models.ConData.FriendPhoto> grid0;
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
                var result = await ConDataService.GetFriendPhotos(filter: $@"(contains(PhotoURL,""{search}"") or contains(PhotoDescription,""{search}"")) and {(string.IsNullOrEmpty(args.Filter)? "true" : args.Filter)}", expand: "Friend", orderby: $"{args.OrderBy}", top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null);
                friendPhotos = result.Value.AsODataEnumerable();
                count = result.Count;
            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load FriendPhotos" });
            }
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddFriendPhoto>("Add FriendPhoto", null);
            await grid0.Reload();
        }

        protected async Task EditRow(DataGridRowMouseEventArgs<AmosFriendsPhotoAlbum.Server.Models.ConData.FriendPhoto> args)
        {
            await DialogService.OpenAsync<EditFriendPhoto>("Edit FriendPhoto", new Dictionary<string, object> { {"PhotoID", args.Data.PhotoID} });
            await grid0.Reload();
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, AmosFriendsPhotoAlbum.Server.Models.ConData.FriendPhoto friendPhoto)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await ConDataService.DeleteFriendPhoto(photoId:friendPhoto.PhotoID);

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
                    Detail = $"Unable to delete FriendPhoto"
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await ConDataService.ExportFriendPhotosToCSV(new Query
{
    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
    OrderBy = $"{grid0.Query.OrderBy}",
    Expand = "Friend",
    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
}, "FriendPhotos");
            }

            if (args == null || args.Value == "xlsx")
            {
                await ConDataService.ExportFriendPhotosToExcel(new Query
{
    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
    OrderBy = $"{grid0.Query.OrderBy}",
    Expand = "Friend",
    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
}, "FriendPhotos");
            }
        }
    }
}