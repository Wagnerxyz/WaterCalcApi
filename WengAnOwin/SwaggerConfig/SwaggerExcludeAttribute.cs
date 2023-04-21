using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WengAnOwin.Swagger
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SwaggerExcludeAttribute : Attribute
    {
    }
}