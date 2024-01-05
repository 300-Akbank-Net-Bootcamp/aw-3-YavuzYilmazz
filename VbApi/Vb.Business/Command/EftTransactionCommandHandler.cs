using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data;
using Vb.Data.Entity;
using Vb.Schema;

namespace Vb.Business.Command;
public class EftTransactionCommandHandler :
    IRequestHandler<CreateEftTransactionCommand, ApiResponse<EftTransactionResponse>>,
    IRequestHandler<UpdateEftTransactionCommand, ApiResponse>,
    IRequestHandler<DeleteEftTransactionCommand, ApiResponse>
{
    private readonly VbDbContext _dbContext;
    private readonly IMapper _mapper;

    public EftTransactionCommandHandler(VbDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ApiResponse<EftTransactionResponse>> Handle(CreateEftTransactionCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<EftTransactionRequest, EftTransaction>(request.Model);

        _dbContext.Entry(entity).State = EntityState.Added;
        await _dbContext.SaveChangesAsync(cancellationToken);

        var mapped = _mapper.Map<EftTransaction, EftTransactionResponse>(entity);
        return new ApiResponse<EftTransactionResponse>(mapped);
    }

    public async Task<ApiResponse> Handle(UpdateEftTransactionCommand request, CancellationToken cancellationToken)
    {
        var fromDb = await _dbContext.Set<EftTransaction>().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (fromDb == null)
        {
            return new ApiResponse("Record not found");
        }


        _dbContext.Entry(fromDb).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ApiResponse();
    }

    public async Task<ApiResponse> Handle(DeleteEftTransactionCommand request, CancellationToken cancellationToken)
    {
        var fromDb = await _dbContext.Set<EftTransaction>().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (fromDb == null)
        {
            return new ApiResponse("Record not found");
        }

        _dbContext.Entry(fromDb).State = EntityState.Deleted;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ApiResponse();
    }
}

