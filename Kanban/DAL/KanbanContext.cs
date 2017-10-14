using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Kanban.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Kanban.DAL
{
    public class KanbanContext : IdentityDbContext
    {
        public KanbanContext() : base("KanbanDB")
        {
            Database.SetInitializer<KanbanContext>(new DropCreateDatabaseIfModelChanges<KanbanContext>());
        }

        public DbSet<Board> Boards { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Card> Cards { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}