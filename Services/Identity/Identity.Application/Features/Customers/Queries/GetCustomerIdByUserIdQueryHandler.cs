using Blocks.Contracts.Interfaces;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Features.Customers.Queries
{
    public sealed class GetCustomerIdByUserIdQueryHandler(
    IGenericRepository<Customer> customerRepository)
    : IRequestHandler<GetCustomerIdByUserIdQuery, Guid?>
    {
        public async Task<Guid?> Handle(
       GetCustomerIdByUserIdQuery request,
       CancellationToken cancellationToken)
        {
            return await customerRepository.GetQueryable()
                .AsNoTracking()
                .Where(c => c.UserId == request.UserId && c.DeletedAt == null)
                .Select(c => (Guid?)c.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
