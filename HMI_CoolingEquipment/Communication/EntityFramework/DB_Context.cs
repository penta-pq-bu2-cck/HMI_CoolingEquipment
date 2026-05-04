using HMI_CoolingEquipment.Models;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace HMI_CoolingEquipment.Communication
{
    public class DB_Context : DbContext
    {
        public virtual DbSet<SETUPJOB> SETUPJOB { get; set; }
        public virtual DbSet<JOBDETAILS> JOBDETAILS { get; set; }
        public virtual DbSet<SETTINGS> SETTINGS { get; set; }
        public virtual DbSet<USERACCOUNT> USERACCOUNT { get; set; }
        public virtual DbSet<USERGROUPACCESS> USERGROUPACCESS { get; set; }
        public virtual DbSet<LOADPORT> LOADPORT { get; set; }
        public virtual DbSet<ALARMDEFINITION> ALARMDEFINITION { get; set; }
        public virtual DbSet<ALARMCURRENT> ALARMCURRENT { get; set; }
        public virtual DbSet<ALARMHISTORY> ALARMHISTORY { get; set; }
        public virtual DbSet<SETUPJOBHISTORY> SETUPJOBHISTORY { get; set; }
        public virtual DbSet<JOBDETAILSHISTORY> JOBDETAILSHISTORY { get; set; }
        public virtual DbSet<LOADPORTCONFIGURATION> LOADPORTCONFIGURATION { get; set; }
        public virtual DbSet<EVENTLOG> EVENTLOG { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#if !DEBUG
            optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["DB_Context"].ConnectionString);
            optionsBuilder.EnableSensitiveDataLogging(); 
#else
            optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["DB_Context_Debug"].ConnectionString);
            optionsBuilder.EnableSensitiveDataLogging();
#endif
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SETUPJOB>()
                .HasKey(e => new { e.JobID, e.LotNo });
            
            modelBuilder.Entity<JOBDETAILS>()
                .HasKey(e => new { e.JobID, e.LotNo, e.CarrierID });
            
            modelBuilder.Entity<SETTINGS>()
                .HasKey(e => new { e.Items, e.Page });
            
            modelBuilder.Entity<USERACCOUNT>()
                .HasKey(e => new { e.UserName });
            
            modelBuilder.Entity<USERGROUPACCESS>()
                .HasKey(e => new { e.UserGroup });
            
            modelBuilder.Entity<LOADPORT>()
                .HasKey(e => new { e.LoadPortID });
            
            modelBuilder.Entity<ALARMDEFINITION>()
                .HasKey(e => new { e.AlarmType, e.AlarmCode });
            
            modelBuilder.Entity<ALARMCURRENT>()
                .HasKey(e => new { e.No });

            modelBuilder.Entity<ALARMHISTORY>()
                .HasKey(e => new { e.No });

            modelBuilder.Entity<SETUPJOBHISTORY>()
                .HasKey(e => new { e.No });

            modelBuilder.Entity<JOBDETAILSHISTORY>()
                .HasKey(e => new { e.No });
            modelBuilder.Entity<LOADPORTCONFIGURATION>()
                .HasKey(e => new { e.LoadPortStatusKey });

            modelBuilder.Entity<EVENTLOG>()
                .HasKey(e => new { e.No });
            
        }
    }
}
