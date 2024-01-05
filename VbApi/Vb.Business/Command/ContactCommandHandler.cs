using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data;
using Vb.Data.Entity;
using Vb.Schema;

namespace Vb.Business.Command;
public class ContactCommandHandler :
    IRequestHandler<CreateContactCommand, ApiResponse<ContactResponse>>,
    IRequestHandler<UpdateContactCommand, ApiResponse>,
    IRequestHandler<DeleteContactCommand, ApiResponse>
{
    private readonly VbDbContext _dbContext;
    private readonly IMapper _mapper;

    public ContactCommandHandler(VbDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ApiResponse<ContactResponse>> Handle(CreateContactCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<ContactRequest, Contact>(request.Model);

        _dbContext.Entry(entity).State = EntityState.Added;
        await _dbContext.SaveChangesAsync(cancellationToken);

        var mapped = _mapper.Map<Contact, ContactResponse>(entity);
        return new ApiResponse<ContactResponse>(mapped);
    }

    public async Task<ApiResponse> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
    {
        var fromDb = await _dbContext.Set<Contact>().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (fromDb == null)
        {
            return new ApiResponse("Record not found");
        }

        _dbContext.Entry(fromDb).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ApiResponse();
    }

    public async Task<ApiResponse> Handle(DeleteContactCommand request, CancellationToken cancellationToken)
    {
        var fromDb = await _dbContext.Set<Contact>().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (fromDb == null)
        {
            return new ApiResponse("Record not found");
        }

        _dbContext.Entry(fromDb).State = EntityState.Deleted;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ApiResponse();
    }
}

