using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateDoctorSchedules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DOCTOR_SCHEDULES",
                schema: "CLINICFLOW_APP",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DoctorId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    DayOfWeek = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    StartMinute = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    EndMinute = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    SlotDurationMinutes = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Status = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ConcurrencyToken = table.Column<byte[]>(type: "RAW(2000)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: true),
                    IsDeleted = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    DeletedBy = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOCTOR_SCHEDULES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DOCTOR_SCHEDULES_DOCTORS_DoctorId",
                        column: x => x.DoctorId,
                        principalSchema: "CLINICFLOW_APP",
                        principalTable: "DOCTORS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DOCTOR_SCHEDULES_DoctorId",
                schema: "CLINICFLOW_APP",
                table: "DOCTOR_SCHEDULES",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "UX_DOCTOR_SCHEDULES_SLOT",
                schema: "CLINICFLOW_APP",
                table: "DOCTOR_SCHEDULES",
                columns: new[] { "DoctorId", "DayOfWeek", "StartMinute", "EndMinute" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DOCTOR_SCHEDULES",
                schema: "CLINICFLOW_APP");
        }
    }
}
