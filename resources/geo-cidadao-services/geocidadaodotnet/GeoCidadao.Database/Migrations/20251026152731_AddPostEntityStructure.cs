using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace GeoCidadao.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPostEntityStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS postgis");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:post_categories.post_category", "crime,vandalism,public_disorder,illegal_occupation,traffic_accident,damaged_signage,missing_signage,traffic_congestion,illegal_parking,lack_of_accessibility,road_hole,public_lighting,damaged_pavement,clogged_drain,damaged_bridge,damaged_sidewalk,illegal_construction,accumulated_garbage,irregular_waste_disposal,pollution,fire,deforestation,flooding,water_outage,power_outage,garbage_collection_failure,abandoned_animal,aggressive_animal,lack_of_sanitation,poor_public_space_maintenance,lack_of_social_programs,illegal_settlement,homeless_person,event_show,event_protest,event_fair,temporary_interdiction,large_gathering,lack_of_public_response,inefficient_inspection,abandoned_public_equipment")
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "post",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    category = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "post_location",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    position = table.Column<Point>(type: "geometry", nullable: false),
                    category = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_location", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "post_media",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    media_type = table.Column<string>(type: "text", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_media", x => x.id);
                    table.ForeignKey(
                        name: "FK_post_media_post_PostId",
                        column: x => x.PostId,
                        principalTable: "post",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_post_category",
                table: "post",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "IX_post_created_at",
                table: "post",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_post_updated_at",
                table: "post",
                column: "updated_at");

            migrationBuilder.CreateIndex(
                name: "IX_post_user_id",
                table: "post",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_post_location_category",
                table: "post_location",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "IX_post_location_created_at",
                table: "post_location",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_post_location_position",
                table: "post_location",
                column: "position");

            migrationBuilder.CreateIndex(
                name: "IX_post_location_post_id",
                table: "post_location",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_post_location_updated_at",
                table: "post_location",
                column: "updated_at");

            migrationBuilder.CreateIndex(
                name: "IX_post_media_created_at",
                table: "post_media",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_post_media_media_type",
                table: "post_media",
                column: "media_type");

            migrationBuilder.CreateIndex(
                name: "IX_post_media_order",
                table: "post_media",
                column: "order");

            migrationBuilder.CreateIndex(
                name: "IX_post_media_PostId",
                table: "post_media",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_post_media_updated_at",
                table: "post_media",
                column: "updated_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "post_location");

            migrationBuilder.DropTable(
                name: "post_media");

            migrationBuilder.DropTable(
                name: "post");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:Enum:post_categories.post_category", "crime,vandalism,public_disorder,illegal_occupation,traffic_accident,damaged_signage,missing_signage,traffic_congestion,illegal_parking,lack_of_accessibility,road_hole,public_lighting,damaged_pavement,clogged_drain,damaged_bridge,damaged_sidewalk,illegal_construction,accumulated_garbage,irregular_waste_disposal,pollution,fire,deforestation,flooding,water_outage,power_outage,garbage_collection_failure,abandoned_animal,aggressive_animal,lack_of_sanitation,poor_public_space_maintenance,lack_of_social_programs,illegal_settlement,homeless_person,event_show,event_protest,event_fair,temporary_interdiction,large_gathering,lack_of_public_response,inefficient_inspection,abandoned_public_equipment")
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");
        }
    }
}
