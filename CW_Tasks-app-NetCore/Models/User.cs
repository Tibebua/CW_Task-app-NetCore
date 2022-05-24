using System;
using System.Collections.Generic;
using System.Linq;

namespace CW_Tasks_app_NetCore.Models
{
    public class User
    {
        //public User()
        //{
        //    Tasks = new HashSet<Task>();
        //}
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        //public ICollection<Task> Tasks { get; set; }
    }
}
