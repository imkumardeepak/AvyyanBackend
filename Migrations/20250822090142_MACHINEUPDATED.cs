using Microsoft.EntityFrameworkCore.Migrations;
using System.Text;

#nullable disable

namespace AvyyanBackend.Migrations
{
	/// <inheritdoc />
	public partial class MACHINEUPDATED : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			var sqlBuilder = new StringBuilder();

			// SQL for Rpm column alteration
			sqlBuilder.AppendLine("ALTER TABLE \"MachineManagers\"");
			sqlBuilder.AppendLine("ALTER COLUMN \"Rpm\" TYPE numeric(18,5),");
			sqlBuilder.AppendLine("ALTER COLUMN \"Rpm\" SET NOT NULL;");
			migrationBuilder.Sql(sqlBuilder.ToString());
			sqlBuilder.Clear();

			// SQL for Constat column alteration
			sqlBuilder.AppendLine("ALTER TABLE \"MachineManagers\"");
			sqlBuilder.AppendLine("ALTER COLUMN \"Constat\" TYPE numeric(18,5) USING \"Constat\"::numeric(18,5),");
			sqlBuilder.AppendLine("ALTER COLUMN \"Constat\" DROP NOT NULL;");
			migrationBuilder.Sql(sqlBuilder.ToString());
			sqlBuilder.Clear();

			// SQL for adding Efficiency column
			sqlBuilder.AppendLine("ALTER TABLE \"MachineManagers\"");
			sqlBuilder.AppendLine("ADD COLUMN \"Efficiency\" numeric(18,5) NOT NULL DEFAULT 0;");
			migrationBuilder.Sql(sqlBuilder.ToString());
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			var sqlBuilder = new StringBuilder();

			// SQL for dropping Efficiency column
			sqlBuilder.AppendLine("ALTER TABLE \"MachineManagers\"");
			sqlBuilder.AppendLine("DROP COLUMN \"Efficiency\";");
			migrationBuilder.Sql(sqlBuilder.ToString());
			sqlBuilder.Clear();

			// SQL for Rpm column alteration (rollback)
			sqlBuilder.AppendLine("ALTER TABLE \"MachineManagers\"");
			sqlBuilder.AppendLine("ALTER COLUMN \"Rpm\" TYPE numeric(18,2),");
			sqlBuilder.AppendLine("ALTER COLUMN \"Rpm\" SET NOT NULL;");
			migrationBuilder.Sql(sqlBuilder.ToString());
			sqlBuilder.Clear();

			// SQL for Constat column alteration (rollback)
			sqlBuilder.AppendLine("ALTER TABLE \"MachineManagers\"");
			sqlBuilder.AppendLine("ALTER COLUMN \"Constat\" TYPE character varying(100) USING \"Constat\"::character varying(100),");
			sqlBuilder.AppendLine("ALTER COLUMN \"Constat\" DROP NOT NULL;");
			migrationBuilder.Sql(sqlBuilder.ToString());
		}
	}
}