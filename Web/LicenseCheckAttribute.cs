using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Bentley.SelectServer.ManagedClient;
using Haestad.LicensingFacade;
using Haestad.ManagedLicensing;

namespace Web
{
    public class LicenseCheckAttribute : ActionFilterAttribute
    {
        private bool isServerModeLicense;

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            isServerModeLicense = Convert.ToBoolean(ConfigurationManager.AppSettings["ServerModeLicense"]);

            CheckLicense();
        }
        private void CheckLicense()
        {
            ManagedLicense m_managedLicense = new ManagedLicense((int)ProductId.Bentley_WaterGEMS, "WaterGEMS", "10.00.00.00");   // to be changed: Use OpenFlowAnalysisAPI product
            if (m_managedLicense.IsServerModeEnabled())
            {
                LicenseRunStatus managedStatus = m_managedLicense.StartServerLicense();
                if (managedStatus == LicenseRunStatus.Ok)
                {
                    // StartDesktopLicense()

                    LicenseRunStatusEnum status;
                    ProductRelease pr = new ProductRelease(ProductId.Bentley_WaterGEMS, "10.00.00.00");
                    var m_license = License.Default(pr, IntPtr.Zero, null);
                    if (m_license.Initialize())
                    {
                        status = m_license.StartDesktop();
                    }          
                }
            }
            else
            {
                LicenseRunStatusEnum status;
                ProductRelease pr = new ProductRelease(ProductId.Bentley_WaterGEMS, "10.00.00.00");
                License m_license = License.Default(pr, IntPtr.Zero, null);
                if (m_license.Initialize())
                {
                    status = m_license.StartDesktop();
                    if (status != LicenseRunStatusEnum.OK)
                    {
                        throw new LicenseClientException("License不正常");
                    }
                }
                m_license.Dispose();
            }
            m_managedLicense.StopLicense();

        }
    }
}