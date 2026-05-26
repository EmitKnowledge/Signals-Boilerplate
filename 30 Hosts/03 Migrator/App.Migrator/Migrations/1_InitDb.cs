using App.Migrator.Base;
using SimpleMigrations;

namespace App.Migrator.Migrations
{
    [Migration(1, "Init")]
    public class InitDb : BaseMigration
    {
        public override void Up()
        {
        }

        public override void Down()
        {
        }
    }
}