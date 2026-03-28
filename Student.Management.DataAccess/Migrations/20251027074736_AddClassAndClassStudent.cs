using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student.Management.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddClassAndClassStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentEntity_Classes_ClassId",
                table: "StudentEntity");

            migrationBuilder.DropIndex(
                name: "IX_StudentEntity_ClassId",
                table: "StudentEntity");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "StudentEntity");

            migrationBuilder.CreateTable(
                name: "ClassStudents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    StudentEntityId = table.Column<int>(type: "int", nullable: false),
                    JoinedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassStudents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassStudents_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassStudents_StudentEntity_StudentEntityId",
                        column: x => x.StudentEntityId,
                        principalTable: "StudentEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassStudents_ClassId",
                table: "ClassStudents",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassStudents_StudentEntityId",
                table: "ClassStudents",
                column: "StudentEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassStudents");

            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "StudentEntity",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentEntity_ClassId",
                table: "StudentEntity",
                column: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentEntity_Classes_ClassId",
                table: "StudentEntity",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id");
        }
    }
}
