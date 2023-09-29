using Microsoft.EntityFrameworkCore.Migrations;

namespace GemSpaceIdentityServer.Data.Migrations.ApplicationUserDb
{
    public partial class AddActive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "idp",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "idp",
                table: "AspNetUsers");
        }
    }
}
