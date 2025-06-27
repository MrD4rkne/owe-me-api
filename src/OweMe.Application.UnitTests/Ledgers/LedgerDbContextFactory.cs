using Microsoft.EntityFrameworkCore;
using Moq;
using OweMe.Domain.Ledgers;
using OweMe.Persistence.Ledgers;
using OweMe.Tests.Common;

namespace OweMe.Application.UnitTests.Ledgers;

public class LedgerDbContextMoq : PostgresTestBase
{
    private Mock<LedgerDbContext> _ledgerContextMock;
    
    private readonly TimeProvider _timeProvider;
    private readonly IUserContext _userContext;

    public static LedgerDbContextMoq Create(LedgerDbContextCreationOptions options)
    {
        if (options.TimeProvider is null)
        {
            options = options.WithTimeProvider(new Mock<TimeProvider>().Object);
        }
        
        if (options.UserContext is null)
        {
            options = options.WithUserContext(new Mock<IUserContext>().Object);
        }

        return new LedgerDbContextMoq(options.TimeProvider, options.UserContext);
    }
    
    private LedgerDbContextMoq(TimeProvider timeProvider,
        IUserContext userContext)
    {
        _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
    }
    
    public override async Task SetupAsync()
    {
        await base.SetupAsync();  
        
        var dbOptions = new DbContextOptionsBuilder<LedgerDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;
        
        _ledgerContextMock = new Mock<LedgerDbContext>(
            dbOptions,
            _timeProvider,
            _userContext
        )
        {
            CallBase = true
        };
        
        await _ledgerContextMock.Object.Database.EnsureCreatedAsync();
    }
    
    public ILedgerContext GetLedgerContext()
    {
        return _ledgerContextMock.Object;
    }

    public Mock<ILedgerContext> LedgerContextMock => _ledgerContextMock.As<ILedgerContext>();
    
    public readonly struct LedgerDbContextCreationOptions()
    {
        public TimeProvider? TimeProvider { get; init; } = null;
        public IUserContext? UserContext { get; init; } = null;

        public LedgerDbContextCreationOptions WithOptions(DbContextOptions<LedgerDbContext> options)
        {
            return new LedgerDbContextCreationOptions
            {
                TimeProvider = this.TimeProvider,
                UserContext = this.UserContext
            };
        }
    
        public LedgerDbContextCreationOptions WithTimeProvider(TimeProvider timeProvider)
        {
            return new LedgerDbContextCreationOptions
            {
                TimeProvider = timeProvider,
                UserContext = this.UserContext
            };
        }
    
        public LedgerDbContextCreationOptions WithUserContext(IUserContext userContext)
        {
            return new LedgerDbContextCreationOptions
            {
                TimeProvider = this.TimeProvider,
                UserContext = userContext
            };
        }
        
        public LedgerDbContextMoq Build()
        {
            return Create(this);
        }
    }
}