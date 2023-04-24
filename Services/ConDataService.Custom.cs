using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using SimplifiedNorthwind.Models.ConData;

namespace SimplifiedNorthwind
{
    public partial class ConDataService
    {
        public async Task<SolutionUser> GetSolutionUserByEmail(string name)
        {
            var items = Context.SolutionUsers
                             .AsNoTracking()
                             .Where(i => i.EmailAddress == name);


            var itemToReturn = items.FirstOrDefault();

            OnSolutionUserGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }
    }
}
