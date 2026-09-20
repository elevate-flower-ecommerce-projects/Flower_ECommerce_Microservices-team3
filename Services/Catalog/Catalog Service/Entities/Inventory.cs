namespace Catalog_Service.Entities
{
    public sealed class Inventory
    {
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public Guid StoreId { get; private set; }
        public int Quantity { get; private set; }

        private Inventory()
        {
        }

        public Inventory(
            Guid productId,
            Guid storeId,
            int quantity)
        {
            Id = Guid.NewGuid();
            ProductId = productId;
            StoreId = storeId;
            Quantity = quantity;
        }

        public void Increase(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity));

            Quantity += quantity;
        }

        public void Decrease(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity));

            if (quantity > Quantity)
                throw new InvalidOperationException("Insufficient stock.");

            Quantity -= quantity;
        }
    }
}
