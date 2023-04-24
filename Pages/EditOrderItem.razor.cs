using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace SimplifiedNorthwind.Pages
{
    public partial class EditOrderItem
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
        public int Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            orderItem = await ConDataService.GetOrderItemById(Id);

            ordersForOrderId = await ConDataService.GetOrders();

            productsForProductId = await ConDataService.GetProducts();
        }
        protected bool errorVisible;
        protected SimplifiedNorthwind.Models.ConData.OrderItem orderItem;

        protected IEnumerable<SimplifiedNorthwind.Models.ConData.Order> ordersForOrderId;

        protected IEnumerable<SimplifiedNorthwind.Models.ConData.Product> productsForProductId;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                await ConDataService.UpdateOrderItem(Id, orderItem);
                DialogService.Close(orderItem);
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
    }
}