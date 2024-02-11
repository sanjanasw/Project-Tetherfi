using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tetherfi.Data.Models;

namespace Tetherfi.Data.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasIndex(x => x.Email).IsUnique(false);
            builder.HasMany(x => x.RefreshTokens).WithOne().HasForeignKey(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}