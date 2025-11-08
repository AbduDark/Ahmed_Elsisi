using Microsoft.EntityFrameworkCore;
using LineManagementSystem.Models;

namespace LineManagementSystem.Services;

public class DatabaseContext : DbContext
{
    public DbSet<LineGroup> LineGroups { get; set; }
    public DbSet<PhoneLine> PhoneLines { get; set; }
    public DbSet<Alert> Alerts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=linemanagement.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LineGroup>()
            .HasMany(g => g.Lines)
            .WithOne(l => l.Group)
            .HasForeignKey(l => l.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Alert>()
            .HasOne(a => a.Group)
            .WithMany()
            .HasForeignKey(a => a.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void EnsureCreated()
    {
        Database.EnsureCreated();
        
        // Manual migrations
        try
        {
            using var connection = Database.GetDbConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            
            // Check if LineSystem column exists in PhoneLines
            command.CommandText = "PRAGMA table_info(PhoneLines)";
            using var reader = command.ExecuteReader();
            bool lineSystemExists = false;
            while (reader.Read())
            {
                if (reader.GetString(1) == "LineSystem")
                {
                    lineSystemExists = true;
                    break;
                }
            }
            reader.Close();
            
            // Add LineSystem column if it doesn't exist
            if (!lineSystemExists)
            {
                command.CommandText = "ALTER TABLE PhoneLines ADD COLUMN LineSystem TEXT NULL";
                command.ExecuteNonQuery();
            }
            
            // Check if MaxLines column exists in LineGroups and remove it
            command.CommandText = "PRAGMA table_info(LineGroups)";
            using var reader2 = command.ExecuteReader();
            bool maxLinesExists = false;
            while (reader2.Read())
            {
                if (reader2.GetString(1) == "MaxLines")
                {
                    maxLinesExists = true;
                    break;
                }
            }
            reader2.Close();
            
            // Remove MaxLines column if it exists (SQLite doesn't support DROP COLUMN directly)
            if (maxLinesExists)
            {
                // Disable foreign keys temporarily to allow table rebuild
                command.CommandText = "PRAGMA foreign_keys = OFF";
                command.ExecuteNonQuery();
                
                // Begin transaction
                command.CommandText = "BEGIN TRANSACTION";
                command.ExecuteNonQuery();
                
                try
                {
                    // Create new table without MaxLines
                    command.CommandText = @"
                        CREATE TABLE LineGroups_new (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Name TEXT NOT NULL,
                            Provider INTEGER NOT NULL,
                            Status INTEGER NOT NULL,
                            RequiresCashWallet INTEGER NOT NULL,
                            LastRenewedOn TEXT,
                            NextRenewalDue TEXT,
                            AssignedToEmployee TEXT,
                            AssignedCustomer TEXT,
                            ExpectedHandoverDate TEXT,
                            IsHandedOver INTEGER NOT NULL,
                            ActualHandoverDate TEXT,
                            AdditionalDetails TEXT,
                            IsBusinessGroup INTEGER NOT NULL,
                            CreatedAt TEXT NOT NULL,
                            UpdatedAt TEXT NOT NULL
                        )";
                    command.ExecuteNonQuery();
                    
                    // Copy data from old table to new table
                    command.CommandText = @"
                        INSERT INTO LineGroups_new 
                        SELECT Id, Name, Provider, Status, RequiresCashWallet, LastRenewedOn, 
                               NextRenewalDue, AssignedToEmployee, AssignedCustomer, ExpectedHandoverDate,
                               IsHandedOver, ActualHandoverDate, AdditionalDetails, IsBusinessGroup, 
                               CreatedAt, UpdatedAt
                        FROM LineGroups";
                    command.ExecuteNonQuery();
                    
                    // Drop old table
                    command.CommandText = "DROP TABLE LineGroups";
                    command.ExecuteNonQuery();
                    
                    // Rename new table
                    command.CommandText = "ALTER TABLE LineGroups_new RENAME TO LineGroups";
                    command.ExecuteNonQuery();
                    
                    // Commit transaction
                    command.CommandText = "COMMIT";
                    command.ExecuteNonQuery();
                }
                catch
                {
                    // Rollback on error
                    command.CommandText = "ROLLBACK";
                    command.ExecuteNonQuery();
                    throw;
                }
                finally
                {
                    // Re-enable foreign keys
                    command.CommandText = "PRAGMA foreign_keys = ON";
                    command.ExecuteNonQuery();
                }
            }
        }
        catch
        {
            // If migration fails, ignore (table might not exist yet)
        }
    }
}
