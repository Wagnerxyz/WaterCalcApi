using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using AutoMapper;
using Serilog;

namespace WengAn
{
    public static class Consts
    {
        public static string ProjectName { get; set; } = "BentleyAPI";
        public static IMapper Mapper { get; set; }
        public static string CurrentModelPath { get; set; } = string.Empty;
        public static IContainer Container { get; set; }
        public const string WenganDefaultModel = @"C:\BentleyModels\WengAn\WengAn20230412.wtg.sqlite";
        public const string HealthCheckApiPath = "/api/Home/Healthcheck";
        public static bool isWriteDatabase = false;
        public static ILogger Logger;

    }
}