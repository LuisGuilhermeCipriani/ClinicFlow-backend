using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicFlow.Infrastructure.Persistence.Migrations
{
    public partial class CreateAppointmentHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "APPOINTMENT_HISTORY",
                schema: "CLINICFLOW_APP",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    AppointmentId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    ChangeType = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    PreviousAppointmentDate = table.Column<DateTime>(type: "DATE", nullable: false),
                    PreviousStartMinute = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    PreviousEndMinute = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NewAppointmentDate = table.Column<DateTime>(type: "DATE", nullable: true),
                    NewStartMinute = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    NewEndMinute = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Reason = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_APPOINTMENT_HISTORY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_APPOINTMENT_HISTORY_APPOINTMENTS_AppointmentId",
                        column: x => x.AppointmentId,
                        principalSchema: "CLINICFLOW_APP",
                        principalTable: "APPOINTMENTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_APPOINTMENT_HISTORY_APPOINTMENT",
                schema: "CLINICFLOW_APP",
                table: "APPOINTMENT_HISTORY",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_APPOINTMENT_HISTORY_CREATED_AT",
                schema: "CLINICFLOW_APP",
                table: "APPOINTMENT_HISTORY",
                column: "CreatedAt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "APPOINTMENT_HISTORY",
                schema: "CLINICFLOW_APP");
        }
    }
}
