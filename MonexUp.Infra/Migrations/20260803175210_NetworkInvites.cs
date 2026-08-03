using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DB.Infra.Migrations
{
    /// <inheritdoc />
    public partial class NetworkInvites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "invited_at",
                table: "monexup_user_networks",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "monexup_network_invites",
                columns: table => new
                {
                    invite_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    network_id = table.Column<long>(type: "bigint", nullable: false),
                    email = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    inviter_user_id = table.Column<long>(type: "bigint", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "(now() at time zone 'utc')"),
                    consumed_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    consumed_user_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("monexup_network_invites_pkey", x => x.invite_id);
                    table.ForeignKey(
                        name: "monexup_fk_network_invite_network",
                        column: x => x.network_id,
                        principalTable: "monexup_networks",
                        principalColumn: "network_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_monexup_network_invites_network_status",
                table: "monexup_network_invites",
                columns: new[] { "network_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ux_monexup_network_invites_pending",
                table: "monexup_network_invites",
                columns: new[] { "network_id", "email" },
                unique: true,
                filter: "status = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "monexup_network_invites");

            migrationBuilder.DropColumn(
                name: "invited_at",
                table: "monexup_user_networks");
        }
    }
}
