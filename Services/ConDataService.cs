using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Radzen;

using SimplifiedNorthwind.Data;
using SimplifiedNorthwind.Models.ConData;

namespace SimplifiedNorthwind
{
    public partial class ConDataService
    {
        ConDataContext Context
        {
           get
           {
             return this.context;
           }
        }

        private readonly ConDataContext context;
        private readonly NavigationManager navigationManager;

        public ConDataService(ConDataContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);


        public async Task ExportCustomersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/customers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/customers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportCustomersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/customers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/customers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnCustomersRead(ref IQueryable<SimplifiedNorthwind.Models.ConData.Customer> items);

        public async Task<IQueryable<SimplifiedNorthwind.Models.ConData.Customer>> GetCustomers(Query query = null)
        {
            var items = Context.Customers.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }

            OnCustomersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnCustomerGet(SimplifiedNorthwind.Models.ConData.Customer item);

        public async Task<SimplifiedNorthwind.Models.ConData.Customer> GetCustomerById(int id)
        {
            var items = Context.Customers
                              .AsNoTracking()
                              .Where(i => i.Id == id);

  
            var itemToReturn = items.FirstOrDefault();

            OnCustomerGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnCustomerCreated(SimplifiedNorthwind.Models.ConData.Customer item);
        partial void OnAfterCustomerCreated(SimplifiedNorthwind.Models.ConData.Customer item);

        public async Task<SimplifiedNorthwind.Models.ConData.Customer> CreateCustomer(SimplifiedNorthwind.Models.ConData.Customer customer)
        {
            OnCustomerCreated(customer);

            var existingItem = Context.Customers
                              .Where(i => i.Id == customer.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Customers.Add(customer);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(customer).State = EntityState.Detached;
                throw;
            }

            OnAfterCustomerCreated(customer);

            return customer;
        }

        public async Task<SimplifiedNorthwind.Models.ConData.Customer> CancelCustomerChanges(SimplifiedNorthwind.Models.ConData.Customer item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCustomerUpdated(SimplifiedNorthwind.Models.ConData.Customer item);
        partial void OnAfterCustomerUpdated(SimplifiedNorthwind.Models.ConData.Customer item);

        public async Task<SimplifiedNorthwind.Models.ConData.Customer> UpdateCustomer(int id, SimplifiedNorthwind.Models.ConData.Customer customer)
        {
            OnCustomerUpdated(customer);

            var itemToUpdate = Context.Customers
                              .Where(i => i.Id == customer.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(customer);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterCustomerUpdated(customer);

            return customer;
        }

        partial void OnCustomerDeleted(SimplifiedNorthwind.Models.ConData.Customer item);
        partial void OnAfterCustomerDeleted(SimplifiedNorthwind.Models.ConData.Customer item);

        public async Task<SimplifiedNorthwind.Models.ConData.Customer> DeleteCustomer(int id)
        {
            var itemToDelete = Context.Customers
                              .Where(i => i.Id == id)
                              .Include(i => i.Orders)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnCustomerDeleted(itemToDelete);


            Context.Customers.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterCustomerDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportOrdersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/orders/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/orders/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportOrdersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/orders/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/orders/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnOrdersRead(ref IQueryable<SimplifiedNorthwind.Models.ConData.Order> items);

        public async Task<IQueryable<SimplifiedNorthwind.Models.ConData.Order>> GetOrders(Query query = null)
        {
            var items = Context.Orders.AsQueryable();

            items = items.Include(i => i.Customer);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }

            OnOrdersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnOrderGet(SimplifiedNorthwind.Models.ConData.Order item);

        public async Task<SimplifiedNorthwind.Models.ConData.Order> GetOrderById(int id)
        {
            var items = Context.Orders
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Customer);
  
            var itemToReturn = items.FirstOrDefault();

            OnOrderGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnOrderCreated(SimplifiedNorthwind.Models.ConData.Order item);
        partial void OnAfterOrderCreated(SimplifiedNorthwind.Models.ConData.Order item);

        public async Task<SimplifiedNorthwind.Models.ConData.Order> CreateOrder(SimplifiedNorthwind.Models.ConData.Order order)
        {
            OnOrderCreated(order);

            var existingItem = Context.Orders
                              .Where(i => i.Id == order.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Orders.Add(order);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(order).State = EntityState.Detached;
                throw;
            }

            OnAfterOrderCreated(order);

            return order;
        }

        public async Task<SimplifiedNorthwind.Models.ConData.Order> CancelOrderChanges(SimplifiedNorthwind.Models.ConData.Order item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnOrderUpdated(SimplifiedNorthwind.Models.ConData.Order item);
        partial void OnAfterOrderUpdated(SimplifiedNorthwind.Models.ConData.Order item);

        public async Task<SimplifiedNorthwind.Models.ConData.Order> UpdateOrder(int id, SimplifiedNorthwind.Models.ConData.Order order)
        {
            OnOrderUpdated(order);

            var itemToUpdate = Context.Orders
                              .Where(i => i.Id == order.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(order);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterOrderUpdated(order);

            return order;
        }

        partial void OnOrderDeleted(SimplifiedNorthwind.Models.ConData.Order item);
        partial void OnAfterOrderDeleted(SimplifiedNorthwind.Models.ConData.Order item);

        public async Task<SimplifiedNorthwind.Models.ConData.Order> DeleteOrder(int id)
        {
            var itemToDelete = Context.Orders
                              .Where(i => i.Id == id)
                              .Include(i => i.OrderItems)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnOrderDeleted(itemToDelete);


            Context.Orders.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterOrderDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportOrderItemsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/orderitems/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/orderitems/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportOrderItemsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/orderitems/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/orderitems/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnOrderItemsRead(ref IQueryable<SimplifiedNorthwind.Models.ConData.OrderItem> items);

        public async Task<IQueryable<SimplifiedNorthwind.Models.ConData.OrderItem>> GetOrderItems(Query query = null)
        {
            var items = Context.OrderItems.AsQueryable();

            items = items.Include(i => i.Order);
            items = items.Include(i => i.Product);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }

            OnOrderItemsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnOrderItemGet(SimplifiedNorthwind.Models.ConData.OrderItem item);

        public async Task<SimplifiedNorthwind.Models.ConData.OrderItem> GetOrderItemById(int id)
        {
            var items = Context.OrderItems
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Order);
            items = items.Include(i => i.Product);
  
            var itemToReturn = items.FirstOrDefault();

            OnOrderItemGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnOrderItemCreated(SimplifiedNorthwind.Models.ConData.OrderItem item);
        partial void OnAfterOrderItemCreated(SimplifiedNorthwind.Models.ConData.OrderItem item);

        public async Task<SimplifiedNorthwind.Models.ConData.OrderItem> CreateOrderItem(SimplifiedNorthwind.Models.ConData.OrderItem orderitem)
        {
            OnOrderItemCreated(orderitem);

            var existingItem = Context.OrderItems
                              .Where(i => i.Id == orderitem.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.OrderItems.Add(orderitem);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(orderitem).State = EntityState.Detached;
                throw;
            }

            OnAfterOrderItemCreated(orderitem);

            return orderitem;
        }

        public async Task<SimplifiedNorthwind.Models.ConData.OrderItem> CancelOrderItemChanges(SimplifiedNorthwind.Models.ConData.OrderItem item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnOrderItemUpdated(SimplifiedNorthwind.Models.ConData.OrderItem item);
        partial void OnAfterOrderItemUpdated(SimplifiedNorthwind.Models.ConData.OrderItem item);

        public async Task<SimplifiedNorthwind.Models.ConData.OrderItem> UpdateOrderItem(int id, SimplifiedNorthwind.Models.ConData.OrderItem orderitem)
        {
            OnOrderItemUpdated(orderitem);

            var itemToUpdate = Context.OrderItems
                              .Where(i => i.Id == orderitem.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(orderitem);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterOrderItemUpdated(orderitem);

            return orderitem;
        }

        partial void OnOrderItemDeleted(SimplifiedNorthwind.Models.ConData.OrderItem item);
        partial void OnAfterOrderItemDeleted(SimplifiedNorthwind.Models.ConData.OrderItem item);

        public async Task<SimplifiedNorthwind.Models.ConData.OrderItem> DeleteOrderItem(int id)
        {
            var itemToDelete = Context.OrderItems
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnOrderItemDeleted(itemToDelete);


            Context.OrderItems.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterOrderItemDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportProductsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/products/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/products/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportProductsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/products/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/products/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnProductsRead(ref IQueryable<SimplifiedNorthwind.Models.ConData.Product> items);

        public async Task<IQueryable<SimplifiedNorthwind.Models.ConData.Product>> GetProducts(Query query = null)
        {
            var items = Context.Products.AsQueryable();

            items = items.Include(i => i.Supplier);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }

            OnProductsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnProductGet(SimplifiedNorthwind.Models.ConData.Product item);

        public async Task<SimplifiedNorthwind.Models.ConData.Product> GetProductById(int id)
        {
            var items = Context.Products
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Supplier);
  
            var itemToReturn = items.FirstOrDefault();

            OnProductGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnProductCreated(SimplifiedNorthwind.Models.ConData.Product item);
        partial void OnAfterProductCreated(SimplifiedNorthwind.Models.ConData.Product item);

        public async Task<SimplifiedNorthwind.Models.ConData.Product> CreateProduct(SimplifiedNorthwind.Models.ConData.Product product)
        {
            OnProductCreated(product);

            var existingItem = Context.Products
                              .Where(i => i.Id == product.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Products.Add(product);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(product).State = EntityState.Detached;
                throw;
            }

            OnAfterProductCreated(product);

            return product;
        }

        public async Task<SimplifiedNorthwind.Models.ConData.Product> CancelProductChanges(SimplifiedNorthwind.Models.ConData.Product item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnProductUpdated(SimplifiedNorthwind.Models.ConData.Product item);
        partial void OnAfterProductUpdated(SimplifiedNorthwind.Models.ConData.Product item);

        public async Task<SimplifiedNorthwind.Models.ConData.Product> UpdateProduct(int id, SimplifiedNorthwind.Models.ConData.Product product)
        {
            OnProductUpdated(product);

            var itemToUpdate = Context.Products
                              .Where(i => i.Id == product.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(product);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterProductUpdated(product);

            return product;
        }

        partial void OnProductDeleted(SimplifiedNorthwind.Models.ConData.Product item);
        partial void OnAfterProductDeleted(SimplifiedNorthwind.Models.ConData.Product item);

        public async Task<SimplifiedNorthwind.Models.ConData.Product> DeleteProduct(int id)
        {
            var itemToDelete = Context.Products
                              .Where(i => i.Id == id)
                              .Include(i => i.OrderItems)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnProductDeleted(itemToDelete);


            Context.Products.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterProductDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportSolutionUsersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/solutionusers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/solutionusers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportSolutionUsersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/solutionusers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/solutionusers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnSolutionUsersRead(ref IQueryable<SimplifiedNorthwind.Models.ConData.SolutionUser> items);

        public async Task<IQueryable<SimplifiedNorthwind.Models.ConData.SolutionUser>> GetSolutionUsers(Query query = null)
        {
            var items = Context.SolutionUsers.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }

            OnSolutionUsersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnSolutionUserGet(SimplifiedNorthwind.Models.ConData.SolutionUser item);

        public async Task<SimplifiedNorthwind.Models.ConData.SolutionUser> GetSolutionUserByUserId(long userid)
        {
            var items = Context.SolutionUsers
                              .AsNoTracking()
                              .Where(i => i.UserID == userid);

  
            var itemToReturn = items.FirstOrDefault();

            OnSolutionUserGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnSolutionUserCreated(SimplifiedNorthwind.Models.ConData.SolutionUser item);
        partial void OnAfterSolutionUserCreated(SimplifiedNorthwind.Models.ConData.SolutionUser item);

        public async Task<SimplifiedNorthwind.Models.ConData.SolutionUser> CreateSolutionUser(SimplifiedNorthwind.Models.ConData.SolutionUser solutionuser)
        {
            OnSolutionUserCreated(solutionuser);

            var existingItem = Context.SolutionUsers
                              .Where(i => i.UserID == solutionuser.UserID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.SolutionUsers.Add(solutionuser);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(solutionuser).State = EntityState.Detached;
                throw;
            }

            OnAfterSolutionUserCreated(solutionuser);

            return solutionuser;
        }

        public async Task<SimplifiedNorthwind.Models.ConData.SolutionUser> CancelSolutionUserChanges(SimplifiedNorthwind.Models.ConData.SolutionUser item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnSolutionUserUpdated(SimplifiedNorthwind.Models.ConData.SolutionUser item);
        partial void OnAfterSolutionUserUpdated(SimplifiedNorthwind.Models.ConData.SolutionUser item);

        public async Task<SimplifiedNorthwind.Models.ConData.SolutionUser> UpdateSolutionUser(long userid, SimplifiedNorthwind.Models.ConData.SolutionUser solutionuser)
        {
            OnSolutionUserUpdated(solutionuser);

            var itemToUpdate = Context.SolutionUsers
                              .Where(i => i.UserID == solutionuser.UserID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(solutionuser);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterSolutionUserUpdated(solutionuser);

            return solutionuser;
        }

        partial void OnSolutionUserDeleted(SimplifiedNorthwind.Models.ConData.SolutionUser item);
        partial void OnAfterSolutionUserDeleted(SimplifiedNorthwind.Models.ConData.SolutionUser item);

        public async Task<SimplifiedNorthwind.Models.ConData.SolutionUser> DeleteSolutionUser(long userid)
        {
            var itemToDelete = Context.SolutionUsers
                              .Where(i => i.UserID == userid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnSolutionUserDeleted(itemToDelete);


            Context.SolutionUsers.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterSolutionUserDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportSuppliersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/suppliers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/suppliers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportSuppliersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/suppliers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/suppliers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnSuppliersRead(ref IQueryable<SimplifiedNorthwind.Models.ConData.Supplier> items);

        public async Task<IQueryable<SimplifiedNorthwind.Models.ConData.Supplier>> GetSuppliers(Query query = null)
        {
            var items = Context.Suppliers.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }

            OnSuppliersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnSupplierGet(SimplifiedNorthwind.Models.ConData.Supplier item);

        public async Task<SimplifiedNorthwind.Models.ConData.Supplier> GetSupplierById(int id)
        {
            var items = Context.Suppliers
                              .AsNoTracking()
                              .Where(i => i.Id == id);

  
            var itemToReturn = items.FirstOrDefault();

            OnSupplierGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnSupplierCreated(SimplifiedNorthwind.Models.ConData.Supplier item);
        partial void OnAfterSupplierCreated(SimplifiedNorthwind.Models.ConData.Supplier item);

        public async Task<SimplifiedNorthwind.Models.ConData.Supplier> CreateSupplier(SimplifiedNorthwind.Models.ConData.Supplier supplier)
        {
            OnSupplierCreated(supplier);

            var existingItem = Context.Suppliers
                              .Where(i => i.Id == supplier.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Suppliers.Add(supplier);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(supplier).State = EntityState.Detached;
                throw;
            }

            OnAfterSupplierCreated(supplier);

            return supplier;
        }

        public async Task<SimplifiedNorthwind.Models.ConData.Supplier> CancelSupplierChanges(SimplifiedNorthwind.Models.ConData.Supplier item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnSupplierUpdated(SimplifiedNorthwind.Models.ConData.Supplier item);
        partial void OnAfterSupplierUpdated(SimplifiedNorthwind.Models.ConData.Supplier item);

        public async Task<SimplifiedNorthwind.Models.ConData.Supplier> UpdateSupplier(int id, SimplifiedNorthwind.Models.ConData.Supplier supplier)
        {
            OnSupplierUpdated(supplier);

            var itemToUpdate = Context.Suppliers
                              .Where(i => i.Id == supplier.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(supplier);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterSupplierUpdated(supplier);

            return supplier;
        }

        partial void OnSupplierDeleted(SimplifiedNorthwind.Models.ConData.Supplier item);
        partial void OnAfterSupplierDeleted(SimplifiedNorthwind.Models.ConData.Supplier item);

        public async Task<SimplifiedNorthwind.Models.ConData.Supplier> DeleteSupplier(int id)
        {
            var itemToDelete = Context.Suppliers
                              .Where(i => i.Id == id)
                              .Include(i => i.Products)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnSupplierDeleted(itemToDelete);


            Context.Suppliers.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterSupplierDeleted(itemToDelete);

            return itemToDelete;
        }

       
    }
}