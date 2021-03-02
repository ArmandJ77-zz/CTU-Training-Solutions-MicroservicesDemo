using Microsoft.EntityFrameworkCore.Migrations;

namespace Products.Migrations
{
    public partial class SeedDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Name", "Qty" },
                values: new object[,]
                {
                    {"Alienware M15 R3", 5 },
                    {"Asus Zephyrus G14", 2 },
                    {"Razer Blade 17 Pro", 0 },
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
