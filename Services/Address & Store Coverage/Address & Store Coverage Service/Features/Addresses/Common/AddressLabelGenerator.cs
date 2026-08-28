using Address___Store_Coverage_Service.Entities;
using Microsoft.EntityFrameworkCore;

namespace Address___Store_Coverage_Service.Features.Addresses.Common
{
    public static class AddressLabelGenerator
    {
        private const string Prefix = "Address";
        public static async Task<string> GenerateAsync(
            IQueryable<Address> addresses,
            Guid customerId,
            Guid? addressId,
            CancellationToken cancellationToken)
        {
            var siblings = await addresses
                .AsNoTracking()
                .Where(a => a.CustomerId == customerId && a.DeletedAt == null)
                .OrderBy(a => a.CreatedAt)
                .ThenBy(a => a.Id)
                .Select(a => new { a.Id, a.Label })
                .ToListAsync(cancellationToken);
            var position = addressId is null
                ? siblings.Count + 1
                : siblings.FindIndex(s => s.Id == addressId.Value) + 1;
            if (position <= 0) position = siblings.Count + 1;
            var taken = siblings
                .Where(s => s.Id != addressId && !string.IsNullOrWhiteSpace(s.Label))
                .Select(s => s.Label!)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            var n = position;
            while (taken.Contains($"{Prefix} {n}")) n++;
            return $"{Prefix} {n}";
        }
    }
}
