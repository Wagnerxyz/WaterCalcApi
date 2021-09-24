using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;

namespace LiaoDongBay
{
    public static class Consts
    {
        public static string ProjectName { get; set; } = "BentleyAPI";
        public static IMapper Mapper { get; set; }
        public static string CurrentModelPath { get; set; } = string.Empty;
    }
}