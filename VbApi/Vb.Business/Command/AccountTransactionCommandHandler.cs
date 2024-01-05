using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data;
using Vb.Data.Entity;
using Vb.Schema;

namespace Vb.Business.Command;

public class AccountTransactionCommandHandler :
    IRequestHandler<CreateAccountTransactionCommand, ApiResponse<AccountTransactionResponse>>,
    IRequestHandler<UpdateAccountTransactionCommand, ApiResponse>,
    IRequestHandler<DeleteAccountTransactionCommand, ApiResponse>
{
    private readonly VbDbContext _dbContext;
    private readonly IMapper _mapper;

    public AccountTransactionCommandHandler(VbDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ApiResponse<AccountTransactionResponse>> Handle(CreateAccountTransactionCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<AccountTransactionRequest, AccountTransaction>(request.Model);

        _dbContext.Entry(entity).State = EntityState.Added;
        await _dbContext.SaveChangesAsync(cancellationToken);

        var mapped = _mapper.Map<AccountTransaction, AccountTransactionResponse>(entity);
        return new ApiResponse<AccountTransactionResponse>(mapped);
    }

    public async Task<ApiResponse> Handle(UpdateAccountTransactionCommand request, CancellationToken cancellationToken)
    {
        var fromDb = await _dbContext.Set<AccountTransaction>().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (fromDb == null)
        {
            return new ApiResponse("Record not found");
        }

        _dbContext.Entry(fromDb).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ApiResponse();
    }

    public async Task<ApiResponse> Handle(DeleteAccountTransactionCommand request, CancellationToken cancellationToken)
    {
        var fromDb = await _dbContext.Set<AccountTransaction>().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (fromDb == null)
        {
            return new ApiResponse("Record not found");
        }

        _dbContext.Entry(fromDb).State = EntityState.Deleted;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ApiResponse();
    }
}

