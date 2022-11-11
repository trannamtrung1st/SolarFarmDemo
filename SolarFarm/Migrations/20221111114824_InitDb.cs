using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolarFarm.Migrations
{
    public partial class InitDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SolarPanels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolarPanels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SolarPanelData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PanelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnergyGeneratedKwh = table.Column<int>(type: "int", nullable: false),
                    PowerGenerated = table.Column<int>(type: "int", nullable: false),
                    VoltageGenerated = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolarPanelData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolarPanelData_SolarPanels_PanelId",
                        column: x => x.PanelId,
                        principalTable: "SolarPanels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "SolarPanels",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("0278d12c-7568-4810-98ff-10bfc1359dae"), "Solar Panel 3" },
                    { new Guid("15e7f885-8e65-4392-958a-4c94b91eaaee"), "Solar Panel 9" },
                    { new Guid("29179f8a-52b3-48d1-8463-3b9e1ecc185d"), "Solar Panel 4" },
                    { new Guid("39237cbf-fad0-48fe-869f-ff4bb5702744"), "Solar Panel 8" },
                    { new Guid("5960384f-3a03-42e5-8269-9bbb165926d6"), "Solar Panel 2" },
                    { new Guid("63644c89-bb33-4e2d-9b8e-a7f8c8e54b7c"), "Solar Panel 7" },
                    { new Guid("71b016a0-5bab-4f61-bc65-6cf994a5ba23"), "Solar Panel 5" },
                    { new Guid("78256863-8f4d-436f-a0a9-95e85307953a"), "Solar Panel 10" },
                    { new Guid("9b32701e-ff66-48be-8da2-4036fabdafa1"), "Solar Panel 1" },
                    { new Guid("e8f6bd56-b8b2-4483-bceb-abc1f54d3b09"), "Solar Panel 6" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolarPanelData_PanelId",
                table: "SolarPanelData",
                column: "PanelId");

            migrationBuilder.Sql(@"
CREATE VIEW SolarPanelDataAggregate AS
SELECT sp.Name, spd.EnergyGeneratedKwh, spd.PowerGenerated, spd.VoltageGenerated
FROM SolarPanels sp
INNER JOIN (
    SELECT *, ROW_NUMBER() OVER(PARTITION BY PanelId ORDER BY Time DESC) AS RowNum 
    FROM SolarPanelData
) as spd ON sp.Id = spd.PanelId
WHERE spd.RowNum=1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolarPanelData");

            migrationBuilder.DropTable(
                name: "SolarPanels");
        }
    }
}
