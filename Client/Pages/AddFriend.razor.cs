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
    public partial class AddFriend
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
            friend = new AmosFriendsPhotoAlbum.Server.Models.ConData.Friend();
        }
        protected bool errorVisible;
        protected AmosFriendsPhotoAlbum.Server.Models.ConData.Friend friend;

        protected IEnumerable<AmosFriendsPhotoAlbum.Server.Models.ConData.Gender> gendersForGenderID;


        protected int gendersForGenderIDCount;
        protected AmosFriendsPhotoAlbum.Server.Models.ConData.Gender gendersForGenderIDValue;
        protected async Task gendersForGenderIDLoadData(LoadDataArgs args)
        {
            try
            {
                var result = await ConDataService.GetGenders(top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null, filter: $"contains(GenderName, '{(!string.IsNullOrEmpty(args.Filter) ? args.Filter : "")}')", orderby: $"{args.OrderBy}");
                gendersForGenderID = result.Value.AsODataEnumerable();
                gendersForGenderIDCount = result.Count;

                if (!object.Equals(friend.GenderID, null))
                {
                    var valueResult = await ConDataService.GetGenders(filter: $"GenderID eq {friend.GenderID}");
                    var firstItem = valueResult.Value.FirstOrDefault();
                    if (firstItem != null)
                    {
                        gendersForGenderIDValue = firstItem;
                    }
                }

            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage(){ Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to load Gender" });
            }
        }
        protected async Task FormSubmit()
        {
            try
            {
                var result = await ConDataService.CreateFriend(friend);
                DialogService.Close(friend);
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