﻿using Microsoft.EntityFrameworkCore;
using TiendaApi_Users.Models;

namespace TiendaApi_Users.Data
{
    public class DataContext:DbContext
    {

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Users> Users { get; set; }
    }
}
