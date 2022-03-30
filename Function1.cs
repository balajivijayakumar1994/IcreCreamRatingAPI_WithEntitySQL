using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IcreCreamRatingAPI_WithEntitySQL
{

    public class Function1
    {
        private readonly IceCreamRatingContext _context;
        public Function1(IceCreamRatingContext context)
        {
            _context = context;
        }
        private static HttpClient httpClient = new HttpClient();

        [FunctionName("CreateRatings")]
        public async Task<IActionResult> CreateRating(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            IceCreamRatingContext dbContext = new IceCreamRatingContext();
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            IceCreamRating data = JsonConvert.DeserializeObject<IceCreamRating>(requestBody);

            var userAPI = "https://serverlessohapi.azurewebsites.net/api/GetUser?userId=" + data.userId;
            var userResponse = await httpClient.GetAsync(userAPI);

            if (userResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                return new BadRequestErrorMessageResult("Enter Valid userId.");
            }
            var productAPI = "https://serverlessohapi.azurewebsites.net/api/GetProduct?productId=" + data.productId;
            var productAPIResponse = httpClient.GetAsync(productAPI);

            if (productAPIResponse.Result.StatusCode == HttpStatusCode.BadRequest)
            {
                return new BadRequestErrorMessageResult("Enter Valid product Id carefully.");
            }


            if (data.rating < 0 || data.rating > 5)
            {
                return new BadRequestErrorMessageResult("Enter Valid Rating between 0 and 5.");
            }



            data.id = Guid.NewGuid().ToString();
            data.timestamp = DateTime.Now;
            var responseMessage = JsonConvert.SerializeObject(data);

            var entity = await _context.AddAsync(data);
            await _context.SaveChangesAsync();




            return new OkObjectResult(entity.Entity);
        }


        [FunctionName("GetRating")]
        public async Task<IActionResult> GetRatingByRatingId(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string id = req.Query["id"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            id = id ?? data?.id;

            if (string.IsNullOrEmpty(id))
            {
                return new BadRequestErrorMessageResult("Enter rating id");
            }

            var searchdata = _context.Rating.Where(a => a.id.Equals(id)).FirstOrDefault();

            if (searchdata == null)
                return new NotFoundObjectResult("NOT FOUND");
            else
                return new OkObjectResult(searchdata);
        }

        [FunctionName("GetRatings")]
        public async Task<IActionResult> GetRatingsbyUserId(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string userId = req.Query["userId"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            userId = userId ?? data?.productId;

            if (string.IsNullOrEmpty(userId))
            {
                return new BadRequestErrorMessageResult("Enter userId");
            }



            var searchresult = _context.Rating.Where(a => a.userId.Equals(userId)).ToList<IceCreamRating>();

            if (searchresult.Any())
                return new OkObjectResult(searchresult);
            else
                return new NotFoundObjectResult("NOT FOUND");

        }
    }
}
