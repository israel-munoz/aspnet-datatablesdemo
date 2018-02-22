namespace DataTablesDemo.Controllers.api
{
    using DataTablesDemo.Models;
    using DataTablesDemo.Services;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web;
    using System.Web.Http;

    public class DataController : ApiController
    {
        private PeopleService peopleService;

        public DataController()
        {
            peopleService = new PeopleService();
        }

        [HttpPost]
        //[Route("api/data/{draw?}/{start?}/{end?}/{columns?}")]
        //public DataResponse GetData(int draw, int start, int length, [FromUri]DataTableColumn[] columns)
        [Route("api/data")]
        public HttpResponseMessage GetData(HttpRequestMessage request)
        {
            //System.Threading.Thread.Sleep(5000);

            string queryString = HttpUtility.UrlDecode(request.Content.ReadAsStringAsync().Result);
            var data = HttpUtility.ParseQueryString(queryString);
            int filteredCount;
            var requestData = DataTableRequest.Parse(data);
            var people = peopleService.GetPeople(
                requestData.Start,
                requestData.Length,
                requestData.Search.Value,
                requestData.GetFilters(),
                requestData.Order.ToDictionary(
                    o => o.Column,
                    o => o.Dir == DataTableOrderDirection.Asc ? 1 : -1),
                out filteredCount);
            var response = new DataTableResponse<Person>
            {
                Draw = requestData.Draw,
                RecordsTotal = peopleService.GetTotalPeople(),
                RecordsFiltered = filteredCount,
                DataItems = people
            };
            return request.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}
