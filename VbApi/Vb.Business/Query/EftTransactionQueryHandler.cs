using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data;
using Vb.Data.Entity;
using Vb.Schema;


namespace Vb.Business.Query;
public class EftTransactionQueryHandler :
    IRequestHandler<GetAllEftTransactionsQuery, ApiResponse<List<EftTransactionResponse>>>,
    IRequestHandler<GetEftTransactionByIdQuery, ApiResponse<EftTransactionResponse>>,
    IRequestHandler<GetEftTransactionsByAccountIdQuery, ApiResponse<List<EftTransactionResponse>>>
{
    private readonly VbDbContext dbContext;
    private readonly IMapper mapper;

    public EftTransactionQueryHandler(VbDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<ApiResponse<List<EftTransactionResponse>>> Handle(GetAllEftTransactionsQuery request,
        CancellationToken cancellationToken)
    {
        var list = await dbContext.Set<EftTransaction>()
            .Include(x => x.Account)
            .ToListAsync(cancellationToken);

        var mappedList = mapper.Map<List<EftTransaction>, List<EftTransactionResponse>>(list);
        return new ApiResponse<List<EftTransactionResponse>>(mappedList);
    }

    public async Task<ApiResponse<EftTransactionResponse>> Handle(GetEftTransactionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await dbContext.Set<EftTransaction>()
            .Include(x => x.Account)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            return new ApiResponse<EftTransactionResponse>("Record not found");
        }

        var mapped = mapper.Map<EftTransaction, EftTransactionResponse>(entity);
        return new ApiResponse<EftTransactionResponse>(mapped);
    }

    public async Task<ApiResponse<List<EftTransactionResponse>>> Handle(GetEftTransactionsByAccountIdQuery request,
        CancellationToken cancellationToken)
    {
        var list = await dbContext.Set<EftTransaction>()
            .Include(x => x.Account)
            .Where(x => x.AccountId == request.AccountId)
            .ToListAsync(cancellationToken);

        var mappedList = mapper.Map<List<EftTransaction>, List<EftTransactionResponse>>(list);
        return new ApiResponse<List<EftTransactionResponse>>(mappedList);
    }
}