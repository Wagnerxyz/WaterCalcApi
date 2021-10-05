using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiaoDongBay.Swagger
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SwaggerExcludeAttribute : Attribute
    {
    }
}