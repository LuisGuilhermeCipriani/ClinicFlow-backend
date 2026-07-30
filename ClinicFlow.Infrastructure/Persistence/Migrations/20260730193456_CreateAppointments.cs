using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "APPOINTMENTS",
                schema: "CLINICFLOW_APP",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DoctorId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    PatientId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "DATE", nullable: false),
                    StartMinute = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    EndMinute = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DurationMinutes = table.Column<int>(type: "NUMBER(10)", nullable: false),
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
                    table.PrimaryKey("PK_APPOINTMENTS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_APPOINTMENTS_DOCTORS_DoctorId",
                        column: x => x.DoctorId,
                        principalSchema: "CLINICFLOW_APP",
                        principalTable: "DOCTORS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_APPOINTMENTS_PATIENTS_PatientId",
                        column: x => x.PatientId,
                        principalSchema: "CLINICFLOW_APP",
                        principalTable: "PATIENTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_APPOINTMENTS_DOCTOR",
                schema: "CLINICFLOW_APP",
                table: "APPOINTMENTS",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_APPOINTMENTS_PATIENT",
                schema: "CLINICFLOW_APP",
                table: "APPOINTMENTS",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "UX_APPOINTMENTS_SLOT",
                schema: "CLINICFLOW_APP",
                table: "APPOINTMENTS",
                columns: new[] { "DoctorId", "AppointmentDate", "StartMinute", "EndMinute" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "APPOINTMENTS",
                schema: "CLINICFLOW_APP");
        }
    }
}
