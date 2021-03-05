using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Timelogger.Entities;

namespace Timelogger
{
    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options) : base(options) { }

        public DbSet<Project> Projects { get; set; }

        public Project GetProjectByID(int projectId) => Projects.Where(p => p.Id == projectId).FirstOrDefault();
        public List<Project> GetProjectOrderedByDeadline() => Projects.OrderBy(p => p.Deadline).ToList();

        public string CompleteProject(int projectId)
        {
            var p = Projects.Where(p => p.Id == projectId).FirstOrDefault();
            if (p != null)
            {
                p.IsCompleted = true;
                return $"Project {p.Name} was closed!";
            }else
                return $"Project with ID: {projectId} does not exist!";
        }

        public string RegisterTime(int projectId, int minutes, string notes)
        {
            var p = Projects.Where(p => p.Id == projectId).FirstOrDefault();
            if (p == null)
                return $"Project with ID: {projectId} wasn't found";
            else
            {
                if (p.IsCompleted)
                    return $"Project {p.Name} is closed!{Environment.NewLine}You cannot register more time!";
                else
                {
                    AddTimeToProject(minutes, notes, p);
                    return $"{minutes} minutes where register to project {p.Name}";
                }
            }
        }

        private void AddTimeToProject(int minutes, string notes, Project p)
        {
            int logID = 1;
            if (p.Logs == null)
                p.Logs = new List<TimeLog>();
            else
            {
                var id = p.Logs.OrderByDescending(l => l.Id).FirstOrDefault().Id;
                logID = id + 1;
            }
            p.Logs.Add(
                new TimeLog
                {
                    Id = logID,
                    Note = notes,
                    Time = minutes
                });
            p.Minutes += new TimeSpan(0, minutes, 0); ;
            SaveChangesAsync();
        }
    }
}
