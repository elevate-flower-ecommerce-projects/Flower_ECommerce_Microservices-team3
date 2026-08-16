using Blocks.Contracts.Common;
using MediatR;

namespace Catalog_Service.Features.Home.GetSections;

public sealed record GetHomeSectionsQuery() : IRequest<Result<List<HomeSectionResponse>>>;
