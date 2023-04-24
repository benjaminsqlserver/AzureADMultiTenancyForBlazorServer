using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using SimplifiedNorthwind.Data;

namespace SimplifiedNorthwind.Controllers
{
    public partial class ExportConDataController : ExportController
    {
        private readonly ConDataContext context;
        private readonly ConDataService service;

        public ExportConDataController(ConDataContext context, ConDataService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/ConData/customers/csv")]
        [HttpGet("/export/ConData/customers/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCustomersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetCustomers(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/customers/excel")]
        [HttpGet("/export/ConData/customers/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCustomersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetCustomers(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/orders/csv")]
        [HttpGet("/export/ConData/orders/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportOrdersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetOrders(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/orders/excel")]
        [HttpGet("/export/ConData/orders/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportOrdersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetOrders(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/orderitems/csv")]
        [HttpGet("/export/ConData/orderitems/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportOrderItemsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetOrderItems(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/orderitems/excel")]
        [HttpGet("/export/ConData/orderitems/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportOrderItemsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetOrderItems(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/products/csv")]
        [HttpGet("/export/ConData/products/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportProductsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetProducts(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/products/excel")]
        [HttpGet("/export/ConData/products/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportProductsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetProducts(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/solutionusers/csv")]
        [HttpGet("/export/ConData/solutionusers/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportSolutionUsersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetSolutionUsers(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/solutionusers/excel")]
        [HttpGet("/export/ConData/solutionusers/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportSolutionUsersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetSolutionUsers(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/suppliers/csv")]
        [HttpGet("/export/ConData/suppliers/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportSuppliersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetSuppliers(), Request.Query), fileName);
        }

        [HttpGet("/export/ConData/suppliers/excel")]
        [HttpGet("/export/ConData/suppliers/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportSuppliersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetSuppliers(), Request.Query), fileName);
        }
    }
}
