using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AlbumsReviewRESTApi.Data.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Artists",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    StageName = table.Column<string>(maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    LastName = table.Column<string>(maxLength: 50, nullable: false),
                    DateOfBirth = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Albums",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Released = table.Column<DateTimeOffset>(nullable: false),
                    ArtistId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Albums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Albums_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SubmittedReview = table.Column<string>(nullable: false),
                    Rating = table.Column<float>(nullable: false),
                    Submitted = table.Column<DateTimeOffset>(nullable: false),
                    AlbumId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Albums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Artists",
                columns: new[] { "Id", "DateOfBirth", "FirstName", "LastName", "StageName" },
                values: new object[] { new Guid("05638ad6-f6b9-40f3-be72-bd220295d059"), new DateTimeOffset(new DateTime(1972, 8, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "Kaseem", "Ryan", "Ka" });

            migrationBuilder.InsertData(
                table: "Artists",
                columns: new[] { "Id", "DateOfBirth", "FirstName", "LastName", "StageName" },
                values: new object[] { new Guid("2f3e0481-33fe-4283-b4b4-72e866acf5e8"), new DateTimeOffset(new DateTime(1987, 6, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "Kendrick", "Lamar Duckworth", "Kendrick Lamar" });

            migrationBuilder.InsertData(
                table: "Artists",
                columns: new[] { "Id", "DateOfBirth", "FirstName", "LastName", "StageName" },
                values: new object[] { new Guid("efc6df62-7c32-4916-8920-b510c3e11907"), new DateTimeOffset(new DateTime(1981, 3, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)), "Daniel", "Dewan Sewell", "Danny Brown" });

            migrationBuilder.InsertData(
                table: "Albums",
                columns: new[] { "Id", "ArtistId", "Name", "Released" },
                values: new object[] { new Guid("bb5c8e98-548f-4766-b3fb-8c65a1b3a390"), new Guid("05638ad6-f6b9-40f3-be72-bd220295d059"), "Honor Killed The Samurai", new DateTimeOffset(new DateTime(2016, 8, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) });

            migrationBuilder.InsertData(
                table: "Albums",
                columns: new[] { "Id", "ArtistId", "Name", "Released" },
                values: new object[] { new Guid("00d676d5-fdf8-4f26-816f-b3775c1b1e1a"), new Guid("2f3e0481-33fe-4283-b4b4-72e866acf5e8"), "To Pimp a Butterfly", new DateTimeOffset(new DateTime(2015, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) });

            migrationBuilder.InsertData(
                table: "Albums",
                columns: new[] { "Id", "ArtistId", "Name", "Released" },
                values: new object[] { new Guid("4ab21a9f-2041-4847-befb-57711b73f8d1"), new Guid("efc6df62-7c32-4916-8920-b510c3e11907"), "Atrocity Exhibition", new DateTimeOffset(new DateTime(2016, 9, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "AlbumId", "Rating", "Submitted", "SubmittedReview" },
                values: new object[,]
                {
                    { new Guid("bc62a8ea-2c2e-4ec2-a2a0-b4c48b523873"), new Guid("bb5c8e98-548f-4766-b3fb-8c65a1b3a390"), 5f, new DateTimeOffset(new DateTime(2018, 10, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "A great Album" },
                    { new Guid("77534f47-9088-4207-97a1-e09d3d3b670a"), new Guid("bb5c8e98-548f-4766-b3fb-8c65a1b3a390"), 10f, new DateTimeOffset(new DateTime(2018, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "The best i have listened to" },
                    { new Guid("05de4a52-84ee-4bce-9921-9f1fc7912a69"), new Guid("00d676d5-fdf8-4f26-816f-b3775c1b1e1a"), 6f, new DateTimeOffset(new DateTime(2018, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 1, 0, 0, 0)), "A Lovely piece" },
                    { new Guid("a1806419-0ba2-40b8-8528-919499a98be7"), new Guid("00d676d5-fdf8-4f26-816f-b3775c1b1e1a"), 4f, new DateTimeOffset(new DateTime(2018, 8, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "LOVED IT!" },
                    { new Guid("ff6cd8e9-f886-4cc4-b9bf-0962609bfc47"), new Guid("4ab21a9f-2041-4847-befb-57711b73f8d1"), 9f, new DateTimeOffset(new DateTime(2018, 8, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "DANNY BROWN SPAT FIRE" },
                    { new Guid("597bd736-afa1-40a3-b72a-38a1de525a2f"), new Guid("4ab21a9f-2041-4847-befb-57711b73f8d1"), 8f, new DateTimeOffset(new DateTime(2018, 5, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)), "DANNY BROWN IS GOAT" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Albums_ArtistId",
                table: "Albums",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_AlbumId",
                table: "Reviews",
                column: "AlbumId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Albums");

            migrationBuilder.DropTable(
                name: "Artists");
        }
    }
}
