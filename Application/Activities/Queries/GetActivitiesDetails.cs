using System;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Persistance;

namespace Application.Activities.Queries;

public class GetAcitivitesDetails
{
    public class Query : IRequest<Activity>
    {
        public required string Id { get; set; }

    }

    public class Handler(AppDbContext context) : IRequestHandler<Query, Activity>
    {
        public async Task<Activity> Handle(Query request, CancellationToken cancellationToken)
        {
            var activity = await context.Activities.FindAsync([request.Id], cancellationToken);

            if (activity == null) throw new Exception("Acitivity not found");

            return activity;
        }
    }
}
