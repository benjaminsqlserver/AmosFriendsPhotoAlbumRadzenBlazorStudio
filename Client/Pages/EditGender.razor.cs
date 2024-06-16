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
    public partial class EditGender
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

        [Parameter]
        public int GenderID { get; set; }

        protected override async Task OnInitializedAsync()
        {
            gender = await ConDataService.GetGenderByGenderId(genderId:GenderID);
        }
        protected bool errorVisible;
        protected AmosFriendsPhotoAlbum.Server.Models.ConData.Gender gender;

        protected async Task FormSubmit()
        {
            try
            {
                var result = await ConDataService.UpdateGender(genderId:GenderID, gender);
                if (result.StatusCode == System.Net.HttpStatusCode.PreconditionFailed)
                {
                     hasChanges = true;
                     canEdit = false;
                     return;
                }
                DialogService.Close(gender);
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


        protected async Task ReloadButtonClick(MouseEventArgs args)
        {
            hasChanges = false;
            canEdit = true;

            gender = await ConDataService.GetGenderByGenderId(genderId:GenderID);
        }
    }
}