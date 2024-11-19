using System.Threading;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Account> Accounts { get; }
    DbSet<Order> Orders { get; }
    DbSet<Guest> Guests { get; }
    DbSet<DishSnapshot> DishSnapshots { get; }
    DbSet<Table> Tables { get; }
    DbSet<Dish> Dishes { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}