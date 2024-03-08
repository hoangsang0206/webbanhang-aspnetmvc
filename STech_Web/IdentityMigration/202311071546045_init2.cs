namespace STech_Web.IdentityMigration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "DateCreate", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "DateCreate", c => c.DateTime(nullable: true, defaultValue: DateTime.Now));
        }
    }
}
