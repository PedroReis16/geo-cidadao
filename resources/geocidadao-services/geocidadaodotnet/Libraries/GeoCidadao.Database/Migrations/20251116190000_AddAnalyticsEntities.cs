using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeoCidadao.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalyticsEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create problem_event table for analytics
            migrationBuilder.CreateTable(
                name: "problem_event",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    category = table.Column<int>(type: "integer", nullable: false),
                    region = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    city = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    state = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    latitude = table.Column<double>(type: "double precision", nullable: true),
                    longitude = table.Column<double>(type: "double precision", nullable: true),
                    event_timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    likes_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    comments_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    relevance_score = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_problem_event", x => x.id);
                });

            // Create indexes for common analytics queries
            migrationBuilder.CreateIndex(
                name: "IX_problem_event_post_id",
                table: "problem_event",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_problem_event_category",
                table: "problem_event",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "IX_problem_event_event_timestamp",
                table: "problem_event",
                column: "event_timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_problem_event_city",
                table: "problem_event",
                column: "city");

            migrationBuilder.CreateIndex(
                name: "IX_problem_event_state",
                table: "problem_event",
                column: "state");

            migrationBuilder.CreateIndex(
                name: "IX_problem_event_relevance_score",
                table: "problem_event",
                column: "relevance_score");

            migrationBuilder.CreateIndex(
                name: "IX_problem_event_created_at",
                table: "problem_event",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_problem_event_updated_at",
                table: "problem_event",
                column: "updated_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "problem_event");
        }
    }
}
