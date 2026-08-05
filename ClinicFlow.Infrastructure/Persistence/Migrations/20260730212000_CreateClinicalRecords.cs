using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateClinicalRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CLINICAL_RECORDS",
                schema: "CLINICFLOW_APP",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    AppointmentId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    PatientId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    DoctorId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    ChiefComplaint = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    Diagnosis = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    Prescription = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true),
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
                    table.PrimaryKey("PK_CLINICAL_RECORDS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CLINICAL_RECORDS_APPOINTMENTS_AppointmentId",
                        column: x => x.AppointmentId,
                        principalSchema: "CLINICFLOW_APP",
                        principalTable: "APPOINTMENTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CLINICAL_RECORDS_DOCTORS_DoctorId",
                        column: x => x.DoctorId,
                        principalSchema: "CLINICFLOW_APP",
                        principalTable: "DOCTORS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CLINICAL_RECORDS_PATIENTS_PatientId",
                        column: x => x.PatientId,
                        principalSchema: "CLINICFLOW_APP",
                        principalTable: "PATIENTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "UX_CLINICAL_RECORDS_APPOINTMENT",
                schema: "CLINICFLOW_APP",
                table: "CLINICAL_RECORDS",
                column: "AppointmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CLINICAL_RECORDS_DOCTOR",
                schema: "CLINICFLOW_APP",
                table: "CLINICAL_RECORDS",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_CLINICAL_RECORDS_PATIENT",
                schema: "CLINICFLOW_APP",
                table: "CLINICAL_RECORDS",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CLINICAL_RECORDS",
                schema: "CLINICFLOW_APP");
        }
    }
}
