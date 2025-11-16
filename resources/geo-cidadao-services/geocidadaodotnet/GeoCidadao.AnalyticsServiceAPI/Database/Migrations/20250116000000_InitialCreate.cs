using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GeoCidadao.AnalyticsServiceAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "post_analytics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true),
                    LikesCount = table.Column<int>(type: "integer", nullable: false),
                    CommentsCount = table.Column<int>(type: "integer", nullable: false),
                    RelevanceScore = table.Column<double>(type: "double precision", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_analytics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "region_metrics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionIdentifier = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true),
                    TotalPosts = table.Column<int>(type: "integer", nullable: false),
                    PostsByCategory = table.Column<string>(type: "text", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MostFrequentCategory = table.Column<int>(type: "integer", nullable: true),
                    MostFrequentCategoryCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_region_metrics", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_post_analytics_Category",
                table: "post_analytics",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_post_analytics_City",
                table: "post_analytics",
                column: "City");

            migrationBuilder.CreateIndex(
                name: "IX_post_analytics_CreatedAt",
                table: "post_analytics",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_post_analytics_PostId",
                table: "post_analytics",
                column: "PostId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_post_analytics_State",
                table: "post_analytics",
                column: "State");

            migrationBuilder.CreateIndex(
                name: "IX_region_metrics_City",
                table: "region_metrics",
                column: "City");

            migrationBuilder.CreateIndex(
                name: "IX_region_metrics_RegionIdentifier",
                table: "region_metrics",
                column: "RegionIdentifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_region_metrics_State",
                table: "region_metrics",
                column: "State");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "post_analytics");

            migrationBuilder.DropTable(
                name: "region_metrics");
        }
    }
}
