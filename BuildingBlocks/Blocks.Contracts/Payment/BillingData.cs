using System;
using System.Collections.Generic;
using System.Text;

namespace Blocks.Contracts.Payment
{
    public sealed record BillingData(
        string FirstName,
        string LastName,
        string Email,
        string PhoneNumber,
        string Country,
        string City,
        string Street,
        string Building,
        string Floor,
        string Apartment
    );
}
