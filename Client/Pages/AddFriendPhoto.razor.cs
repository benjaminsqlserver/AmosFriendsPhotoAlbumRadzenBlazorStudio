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
    public partial class AddFriendPhoto
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

        protected override async Task OnInitializedAsync()
        {
            friendPhoto = new AmosFriendsPhotoAlbum.Server.Models.ConData.FriendPhoto();
        }
        protected bool errorVisible;
        protected AmosFriendsPhotoAlbum.Server.Models.ConData.FriendPhoto friendPhoto;

        protected IEnumerable<AmosFriendsPhotoAlbum.Server.Models.ConData.Friend> friendsForFriendID;


        protected int friendsForFriendIDCount;
        protected AmosFriendsPhotoAlbum.Server.Models.ConData.Friend friendsForFriendIDValue;
        protected async Task friendsForFriendIDLoadData(LoadDataArgs args)
        {
            try
            {
                var result = await ConDataService.GetFriends(top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null, filter: $"contains(FirstName, '{(!string.IsNullOrEmpty(args.Filter) ? args.Filter : "")}')", orderby: $"{args.OrderBy}");
                friendsForFriendID = result.Value.AsODataEnumerable();
                friendsForFriendIDCount = result.Count;

                if (!object.Equals(friendPhoto.FriendID, null))
                {
                    var valueResult = await ConDataService.GetFriends(filter: $"FriendID eq {friendPhoto.FriendID}");
                    var firstItem = valueResult.Value.FirstOrDefault();
                    if (firstItem != null)
                    {
                        friendsForFriendIDValue = firstItem;
                    }
                }

            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load Friend" });
            }
        }
        protected async Task FormSubmit()
        {
            try
            {
                var result = await ConDataService.CreateFriendPhoto(friendPhoto);
                DialogService.Close(friendPhoto);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }


        protected bool hasChanges = false;
        protected bool canEdit = true;
    }
}