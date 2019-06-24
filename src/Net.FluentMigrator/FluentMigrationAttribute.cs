using FluentMigrator;

namespace Net.FluentMigrator
{
    public class FluentMigrationAttribute : MigrationAttribute
    {
        public string Author { get; }

        public FluentMigrationAttribute(int year, int month, int day, int hour, int minute, string author)
            : base(CalculateMigrationOrdinal(year, month, day, hour, minute))
        {
            Author = author;
        }

        static long CalculateMigrationOrdinal(int year, int month, int day, int hour, int minute)
        {
            return year * 100000000L + month * 1000000L + day * 10000L + hour * 100L + minute;
        }
    }
}
