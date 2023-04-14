using Microsoft.EntityFrameworkCore;

namespace MiniERP.SalesOrderService.Data
{
    public static class Migration
    {
        public static void ApplyMigration(IApplicationBuilder app)
        {
            using (IServiceScope scope = app.ApplicationServices.CreateScope())
            {
                if (scope.ServiceProvider.GetService<AppDbContext>() is AppDbContext context)
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}
