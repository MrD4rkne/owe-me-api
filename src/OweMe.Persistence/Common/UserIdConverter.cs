using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OweMe.Domain.Users;

namespace OweMe.Persistence.Common;

public class UserIdConverter(ConverterMappingHints? mappingHints = null)
    : ValueConverter<UserId, Guid>(ToProvider, FromProvider, mappingHints)
{
    private static readonly Expression<Func<UserId, Guid>> ToProvider = userId => userId.Id;
    private static readonly Expression<Func<Guid, UserId>> FromProvider = id => new UserId(id);
}