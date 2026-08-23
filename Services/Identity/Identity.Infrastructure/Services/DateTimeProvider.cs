using System;
using System.Collections.Generic;
using System.Text;
using Identity.Application.Interfaces;

namespace Identity.Infrastructure.Services
{
    public sealed class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
