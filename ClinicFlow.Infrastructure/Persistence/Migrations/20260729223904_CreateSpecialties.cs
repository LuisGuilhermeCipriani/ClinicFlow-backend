using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateSpecialties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "CLINICFLOW_APP");

            migrationBuilder.CreateTable(
                name: "SPECIALTIES",
                schema: "CLINICFLOW_APP",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ConcurrencyToken = table.Column<byte[]>(type: "RAW(2000)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: true),
                    IsDeleted = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    DeletedBy = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPECIALTIES", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "UX_SPECIALTIES_NAME",
                schema: "CLINICFLOW_APP",
                table: "SPECIALTIES",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SPECIALTIES",
                schema: "CLINICFLOW_APP");
        }
    }
}
