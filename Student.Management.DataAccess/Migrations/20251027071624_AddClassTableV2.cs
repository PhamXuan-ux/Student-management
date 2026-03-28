using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student.Management.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddClassTableV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "StudentEntity",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Room = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ProgramId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Classes_Program_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "Program",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentEntity_ClassId",
                table: "StudentEntity",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_ProgramId",
                table: "Classes",
                column: "ProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentEntity_Classes_ClassId",
                table: "StudentEntity",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentEntity_Classes_ClassId",
                table: "StudentEntity");

            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropIndex(
                name: "IX_StudentEntity_ClassId",
                table: "StudentEntity");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "StudentEntity");
        }
    }
}
