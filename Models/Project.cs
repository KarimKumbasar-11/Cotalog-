using System;
using System.Collections.Generic;

namespace Cotalog.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public string? DetailedDescription { get; set; }
        public int AuthorId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public List<ProjectFile> Files { get; set; } = new();
    }
}