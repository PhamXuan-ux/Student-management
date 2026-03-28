using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student.Management.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_Role_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Program_ProgramId",
                table: "Classes");

            migrationBuilder.DropForeignKey(
                name: "FK_Course_Program_ProgramId",
                table: "Course");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollment_StudentEntity_StudentEntityId",
                table: "Enrollment");

            migrationBuilder.DropForeignKey(
                name: "FK_EnrollmentDetail_Course_CourseId",
                table: "EnrollmentDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentEntity_User_ApplicationStudentId",
                table: "StudentEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClaim_User_UserId",
                table: "UserClaim");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLogin_User_UserId",
                table: "UserLogin");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_Role_RoleId",
                table: "UserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_User_UserId",
                table: "UserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserToken_User_UserId",
                table: "UserToken");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropIndex(
                name: "IX_StudentEntity_ApplicationStudentId",
                table: "StudentEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseDepartment",
                table: "CourseDepartment");

            migrationBuilder.DropIndex(
                name: "IX_CourseDepartment_CourseId",
                table: "CourseDepartment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClassStudents",
                table: "ClassStudents");

            migrationBuilder.DropIndex(
                name: "IX_ClassStudents_ClassId",
                table: "ClassStudents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserToken",
                table: "UserToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLogin",
                table: "UserLogin");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserClaim",
                table: "UserClaim");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Role",
                table: "Role");

            migrationBuilder.RenameTable(
                name: "UserToken",
                newName: "AspNetUserTokens");

            migrationBuilder.RenameTable(
                name: "UserRole",
                newName: "AspNetUserRoles");

            migrationBuilder.RenameTable(
                name: "UserLogin",
                newName: "AspNetUserLogins");

            migrationBuilder.RenameTable(
                name: "UserClaim",
                newName: "AspNetUserClaims");

            migrationBuilder.RenameTable(
                name: "Role",
                newName: "AspNetRoles");

            migrationBuilder.RenameIndex(
                name: "IX_UserRole_RoleId",
                table: "AspNetUserRoles",
                newName: "IX_AspNetUserRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_UserLogin_UserId",
                table: "AspNetUserLogins",
                newName: "IX_AspNetUserLogins_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserClaim_UserId",
                table: "AspNetUserClaims",
                newName: "IX_AspNetUserClaims_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "StudentProfile",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "StudentProfile",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "StudentProfile",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationStudentId",
                table: "StudentEntity",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "Enrollment",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "Enrollment",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<double>(
                name: "Grade",
                table: "ClassStudents",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "Classes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeacherId",
                table: "Classes",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseDepartment",
                table: "CourseDepartment",
                columns: new[] { "CourseId", "DepartmentId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClassStudents",
                table: "ClassStudents",
                columns: new[] { "ClassId", "StudentEntityId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserClaims",
                table: "AspNetUserClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Attendances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    StudentEntityId = table.Column<int>(type: "int", nullable: false),
                    AttendanceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RecordedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attendances_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attendances_StudentEntity_StudentEntityId",
                        column: x => x.StudentEntityId,
                        principalTable: "StudentEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Grades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    StudentEntityId = table.Column<int>(type: "int", nullable: false),
                    GradeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Score = table.Column<double>(type: "float", nullable: false),
                    MaxScore = table.Column<double>(type: "float", nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Grades_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Grades_StudentEntity_StudentEntityId",
                        column: x => x.StudentEntityId,
                        principalTable: "StudentEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StartTime = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EndTime = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Room = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schedules_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherProfile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherProfile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherProfile_AspNetUsers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentEntity_ApplicationStudentId",
                table: "StudentEntity",
                column: "ApplicationStudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_CourseId",
                table: "Classes",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_TeacherId",
                table: "Classes",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_ClassId",
                table: "Attendances",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_StudentEntityId",
                table: "Attendances",
                column: "StudentEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_ClassId",
                table: "Grades",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_StudentEntityId",
                table: "Grades",
                column: "StudentEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_ClassId",
                table: "Schedules",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherProfile_TeacherId",
                table: "TeacherProfile",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_AspNetUsers_TeacherId",
                table: "Classes",
                column: "TeacherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Course_CourseId",
                table: "Classes",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Program_ProgramId",
                table: "Classes",
                column: "ProgramId",
                principalTable: "Program",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Course_Program_ProgramId",
                table: "Course",
                column: "ProgramId",
                principalTable: "Program",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollment_StudentEntity_StudentEntityId",
                table: "Enrollment",
                column: "StudentEntityId",
                principalTable: "StudentEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EnrollmentDetail_Course_CourseId",
                table: "EnrollmentDetail",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentEntity_AspNetUsers_ApplicationStudentId",
                table: "StudentEntity",
                column: "ApplicationStudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_Classes_AspNetUsers_TeacherId",
                table: "Classes");

            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Course_CourseId",
                table: "Classes");

            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Program_ProgramId",
                table: "Classes");

            migrationBuilder.DropForeignKey(
                name: "FK_Course_Program_ProgramId",
                table: "Course");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollment_StudentEntity_StudentEntityId",
                table: "Enrollment");

            migrationBuilder.DropForeignKey(
                name: "FK_EnrollmentDetail_Course_CourseId",
                table: "EnrollmentDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentEntity_AspNetUsers_ApplicationStudentId",
                table: "StudentEntity");

            migrationBuilder.DropTable(
                name: "Attendances");

            migrationBuilder.DropTable(
                name: "Grades");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "TeacherProfile");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_StudentEntity_ApplicationStudentId",
                table: "StudentEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseDepartment",
                table: "CourseDepartment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClassStudents",
                table: "ClassStudents");

            migrationBuilder.DropIndex(
                name: "IX_Classes_CourseId",
                table: "Classes");

            migrationBuilder.DropIndex(
                name: "IX_Classes_TeacherId",
                table: "Classes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserClaims",
                table: "AspNetUserClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "StudentProfile");

            migrationBuilder.DropColumn(
                name: "Grade",
                table: "ClassStudents");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "Classes");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                newName: "UserToken");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                newName: "UserRole");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                newName: "UserLogin");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                newName: "UserClaim");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                newName: "Role");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "UserRole",
                newName: "IX_UserRole_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "UserLogin",
                newName: "IX_UserLogin_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "UserClaim",
                newName: "IX_UserClaim_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "StudentProfile",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "StudentProfile",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationStudentId",
                table: "StudentEntity",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "Enrollment",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedOn",
                table: "Enrollment",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseDepartment",
                table: "CourseDepartment",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClassStudents",
                table: "ClassStudents",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserToken",
                table: "UserToken",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLogin",
                table: "UserLogin",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserClaim",
                table: "UserClaim",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Role",
                table: "Role",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentEntity_ApplicationStudentId",
                table: "StudentEntity",
                column: "ApplicationStudentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseDepartment_CourseId",
                table: "CourseDepartment",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassStudents_ClassId",
                table: "ClassStudents",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "User",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "User",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_Role_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Program_ProgramId",
                table: "Classes",
                column: "ProgramId",
                principalTable: "Program",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Course_Program_ProgramId",
                table: "Course",
                column: "ProgramId",
                principalTable: "Program",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollment_StudentEntity_StudentEntityId",
                table: "Enrollment",
                column: "StudentEntityId",
                principalTable: "StudentEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EnrollmentDetail_Course_CourseId",
                table: "EnrollmentDetail",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentEntity_User_ApplicationStudentId",
                table: "StudentEntity",
                column: "ApplicationStudentId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClaim_User_UserId",
                table: "UserClaim",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogin_User_UserId",
                table: "UserLogin",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_Role_RoleId",
                table: "UserRole",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_User_UserId",
                table: "UserRole",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserToken_User_UserId",
                table: "UserToken",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
