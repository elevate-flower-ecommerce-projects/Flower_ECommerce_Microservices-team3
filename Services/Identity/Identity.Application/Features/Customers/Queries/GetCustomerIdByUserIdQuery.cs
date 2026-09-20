using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.Customers.Queries
{
    public sealed record GetCustomerIdByUserIdQuery(Guid UserId) : IRequest<Guid?>;
}
