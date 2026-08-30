namespace Catalog_Service.Entities
{
    public sealed class Promotion
    {
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public Guid? StoreId { get; private set; }
        public decimal DiscountPercent { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public bool IsActive { get; private set; }

        public Promotion(
            Guid productId,
            Guid? storeId,
            decimal discountPercent,
            DateTime startDate,
            DateTime endDate)
        {
            Id = Guid.NewGuid();
            ProductId = productId;
            StoreId = storeId;
            DiscountPercent = discountPercent;
            StartDate = startDate;
            EndDate = endDate;
            IsActive = true;
        }
    }
}
