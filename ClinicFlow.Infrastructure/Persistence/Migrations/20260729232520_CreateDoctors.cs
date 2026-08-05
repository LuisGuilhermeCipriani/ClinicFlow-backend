using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateDoctors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DOCTORS",
                schema: "CLINICFLOW_APP",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    CrmNumber = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    CrmState = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false),
                    SpecialtyId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(254)", maxLength: 254, nullable: false),
                    Phone = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
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
                    table.PrimaryKey("PK_DOCTORS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DOCTORS_SPECIALTIES_SpecialtyId",
                        column: x => x.SpecialtyId,
                        principalSchema: "CLINICFLOW_APP",
                        principalTable: "SPECIALTIES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DOCTORS_SpecialtyId",
                schema: "CLINICFLOW_APP",
                table: "DOCTORS",
                column: "SpecialtyId");

            migrationBuilder.CreateIndex(
                name: "UX_DOCTORS_CRM",
                schema: "CLINICFLOW_APP",
                table: "DOCTORS",
                columns: new[] { "CrmState", "CrmNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DOCTORS",
                schema: "CLINICFLOW_APP");
        }
    }
}
