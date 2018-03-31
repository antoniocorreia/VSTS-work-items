using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VSTS_work_items.Models;


namespace VSTS_work_items.Controllers
{
    public class HomeController : Controller
    {
        readonly string _uri;
        readonly string _personalAccessToken;
        readonly string _project;

        public HomeController()
        {
            _uri = "https://a2software.visualstudio.com";
            _personalAccessToken = "yourtoken";
        }

        public List<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> RunGetBugsQueryUsingClientLib()
        {
            Uri uri = new Uri(_uri);
            string personalAccessToken = _personalAccessToken;
            string project = _project;

            VssBasicCredential credentials = new VssBasicCredential("", _personalAccessToken);

            //create a wiql object and build our query
            Wiql wiql = new Wiql()
            {
                Query = "Select [State], [Title], [System.TeamProject], [System.WorkItemType], [System.IterationPath]" +
                        "From WorkItems " +
                        "Where [Work Item Type] in ('Task','Bug') " +
                        //"And [System.TeamProject] = '" + project + "' " +
                        "And [System.State] = 'To Do'  " +
                        "Order By [State] Asc, [Changed Date] Desc"
            };

            //create instance of work item tracking http client
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(uri, credentials))
            {
                //execute the query to get the list of work items in the results
                WorkItemQueryResult workItemQueryResult = workItemTrackingHttpClient.QueryByWiqlAsync(wiql).Result;

                //some error handling                
                if (workItemQueryResult.WorkItems.Count() != 0)
                {
                    //need to get the list of our work item ids and put them into an array
                    List<int> list = new List<int>();
                    foreach (var item in workItemQueryResult.WorkItems)
                    {
                        list.Add(item.Id);
                    }
                    int[] arr = list.ToArray();

                    //build a list of the fields we want to see
                    string[] fields = new string[6];
                    fields[0] = "System.Id";
                    fields[1] = "System.Title";
                    fields[2] = "System.State";
                    fields[3] = "System.TeamProject";
                    fields[4] = "System.WorkItemType";
                    //fields[5] = "System.AssignedTo";
                    fields[5] = "System.IterationPath";
                    

                    //get work items for the ids found in query
                    var workItems = workItemTrackingHttpClient.GetWorkItemsAsync(arr, fields, workItemQueryResult.AsOf).Result;

                    Console.WriteLine("Query Results: {0} items found", workItems.Count);

                    return workItems;
                }

                return null;
            }
        }

        public ActionResult Index()
        {
            var wi = RunGetBugsQueryUsingClientLib();
            List<WorkItemViewModel> model = new List<WorkItemViewModel>();
            foreach (var item in wi)
            {
                WorkItemViewModel i = new WorkItemViewModel();
                i.Id = int.Parse(item.Fields["System.Id"].ToString());
                i.Url = item.Url;
                i.Title = item.Fields["System.Title"].ToString();
                i.State = item.Fields["System.State"].ToString();
                i.Project = item.Fields["System.TeamProject"].ToString();
                i.Type = item.Fields["System.WorkItemType"].ToString();
                i.IterationPath = item.Fields["System.IterationPath"].ToString();
                
                model.Add(i);
            }
            return View(model);
        }

        
      
        
        public void GetReadOnlyWorkItemFields()
        {
            Uri uri = new Uri(_uri);
            VssBasicCredential credentials = new VssBasicCredential("", _personalAccessToken);
            WorkItemTrackingHttpClient workItemTrackingClient = new WorkItemTrackingHttpClient(uri,credentials);

            List<WorkItemField> result = workItemTrackingClient.GetFieldsAsync().Result;

            Console.WriteLine("Read only fields:");
            foreach (var workitemField in result)
            {
                Console.WriteLine(" * {0} ({1})", workitemField.Name, workitemField.ReferenceName);
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            Uri uri = new Uri(_uri);
            VssBasicCredential credentials = new VssBasicCredential("", _personalAccessToken);
            WorkItemTrackingHttpClient workItemTrackingClient = new WorkItemTrackingHttpClient(uri, credentials);

            List<WorkItemField> result = workItemTrackingClient.GetFieldsAsync().Result;
            List<DetailsViewModel> model = new List<DetailsViewModel>();
            Console.WriteLine("Read only fields:");
            foreach (var workitemField in result)
            {
                DetailsViewModel d = new DetailsViewModel();
                d.Name = workitemField.Name;
                d.ReferenceName = workitemField.ReferenceName;
                model.Add(d);
            }
            return View(model);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}