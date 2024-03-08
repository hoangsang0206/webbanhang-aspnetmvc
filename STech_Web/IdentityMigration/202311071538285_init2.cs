namespace STech_Web.IdentityMigration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "DateCreate", c => c.DateTime(nullable: true, defaultValue: DateTime.Now));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "DateCreate");
        }
    }
}
