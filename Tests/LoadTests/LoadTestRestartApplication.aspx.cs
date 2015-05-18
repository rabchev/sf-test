using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Sitefinity.Data.OA;
using Telerik.Sitefinity.DynamicModules;
using Telerik.Sitefinity.DynamicModules.Builder;
using Telerik.Sitefinity.Modules.Events;
using Telerik.Sitefinity.Modules.Forms;
using Telerik.Sitefinity.Modules.GenericContent;
using Telerik.Sitefinity.Modules.Libraries;
using Telerik.Sitefinity.Modules.News;
using Telerik.Sitefinity.Multisite;
using Telerik.Sitefinity.Publishing;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Taxonomies.Model;
using Telerik.Sitefinity.Web;

namespace SitefinityWebApp.Tests.LoadTests
{
    /// <summary>
    /// Load test LoadTestRestartApplication
    /// </summary>
    public partial class LoadTestRestartApplication : System.Web.UI.Page
    {
        /// <summary>
        /// Page for reset restart load test
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the parameter</param>
        public void Page_Load(object sender, EventArgs e)
        {
            this.Server.ScriptTimeout = 600;
            var operation = Request.Params["operation"];
            switch (operation)
            {
                case "AppRestart":
                    SystemManager.RestartApplication(false);
                    break;
                case "ResetModel":
                    OpenAccessConnection.ResetModel();
                    break;
                case "AppRestartAndCleanModel":
                    OpenAccessConnection.CleanAll();
                    SystemManager.RestartApplication(false);
                    break;
                case "FullAppRestart":
                    SystemManager.RestartApplication(true);
                    string redirectUrl = this.Request.Url.AbsoluteUri.Split('?')[0];
                    this.Page.Response.Redirect(redirectUrl, true);
                    return;
                default:
                    // do nothing
                    break;
            }

            SystemManager.ClearCurrentTransactions();
            var nodes = SitefinitySiteMap.GetCurrentProvider().GetChildNodes(SitefinitySiteMap.GetCurrentProvider().RootNode);
            Telerik.Sitefinity.Security.UserManager.GetManager().GetUsers();
            RoleManager.GetManager().GetRoles();
            SecurityManager.GetManager().GetPermissions();
            ModuleBuilderManager.GetManager().Provider.GetDynamicModules();
            DynamicModuleManager.GetManager();
            TaxonomyManager.GetManager().GetTaxonomies<FlatTaxonomy>();
            NewsManager.GetManager().GetNewsItems();
            LibrariesManager.GetManager().GetAlbums();
            EventsManager.GetManager().GetEvents();
            ContentManager.GetManager().GetContent();
            MultisiteManager.GetManager().GetSites();
            FormsManager.GetManager().GetForms();
            PublishingManager.GetManager().GetPublishingPoints();
        }
    }
}