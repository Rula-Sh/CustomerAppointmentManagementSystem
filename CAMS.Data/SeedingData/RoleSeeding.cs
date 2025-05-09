//using DataAccessLayer.Models;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;

//namespace CAMS.Data.SeedingData;

//public class RoleSeeding
//{
//    public static void SeedRoles(ModelBuilder modelBuilder)
//    {
//        modelBuilder.Entity<Role>().HasData(
//            new Role
//            {
//                Id = 1,
//                Name = "Admin",
//                NormalizedName = "Admin".ToUpper(),
//                ConcurrencyStamp = "1",
//            },
//            new Role
//            {
//                Id = 2,
//                Name = "Employee",
//                NormalizedName = "Employee".ToUpper(),
//                ConcurrencyStamp = "2",
                
//            },
//            new Role
//            {
//                Id = 3,
//                Name = "Customer",
//                NormalizedName = "Customer".ToUpper(),
//                ConcurrencyStamp = "3"
//            }
//        );
//    }
//}
