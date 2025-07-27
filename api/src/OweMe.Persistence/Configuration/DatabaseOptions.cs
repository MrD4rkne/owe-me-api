namespace OweMe.Persistence.Configuration;

public class DatabaseOptions
{
    public const string SectionName = "Database";

    public string ConnectionString { get; set; } = string.Empty;
    public bool RunMigrations { get; set; }
}