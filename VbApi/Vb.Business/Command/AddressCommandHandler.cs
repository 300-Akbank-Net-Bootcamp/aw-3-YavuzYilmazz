
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data;
using Vb.Data.Entity;
using Vb.Schema;

namespace Vb.Business.Command;

public class AddressCommandHandler :
    IRequestHandler<CreateAddressCommand, ApiResponse<AddressResponse>>,
    IRequestHandler<UpdateAddressCommand, ApiResponse>,
    IRequestHandler<DeleteAddressCommand, ApiResponse>
{
    private readonly VbDbContext dbContext;
    private readonly IMapper mapper;

    public AddressCommandHandler(VbDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<ApiResponse<AddressResponse>> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
    {

        var entity = mapper.Map<AddressRequest, Address>(request.Model);
        entity.Id = 0;
        dbContext.Entry(entity).State = EntityState.Added;

        await dbContext.SaveChangesAsync(cancellationToken);

        var mapped = mapper.Map<Address, AddressResponse>(entity);
        return new ApiResponse<AddressResponse>(mapped);
    }

    public async Task<ApiResponse> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        var fromDb = await dbContext.Set<Address>().Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (fromDb == null)
        {
            return new ApiResponse("Record not found");
        }

        fromDb.Address1 = request.Model.Address1;
        fromDb.Address2 = request.Model.Address2;
        fromDb.City = request.Model.City;
        fromDb.Country = request.Model.Country;
        fromDb.County = request.Model.County;
        fromDb.PostalCode = request.Model.PostalCode;

        dbContext.Entry(fromDb).State = EntityState.Modified;
        await dbContext.SaveChangesAsync(cancellationToken);

        return new ApiResponse();
    }

    public async Task<ApiResponse> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        var fromDb = await dbContext.Set<Address>().Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (fromDb == null)
        {
            return new ApiResponse("Record not found");
        }

        dbContext.Entry(fromDb).State = EntityState.Deleted;
        await dbContext.SaveChangesAsync(cancellationToken);

        return new ApiResponse();
    }
}