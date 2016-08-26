using McpSmyrilLine.Hubs;
using McpSmyrilLine.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace McpSmyrilLine.api
{
    [Route("api/bugs/[action]")]
    public class BugsController : Controller
    {
        IBugsRepository _bugsRepository = new BugsRepository();
        private IHubContext _hub;

        public BugsController(IConnectionManager connectionManager)
        {
            _hub = connectionManager.GetHubContext<BugHub>();
        }

	    [HttpGet("~/api/bugs")]
        public IEnumerable<Bug> Get()
        {
            return _bugsRepository.GetBugs();
        }

        // Web API expects primitives coming from the request body to have no key value (e.g. '') - they should be encoded, then as '=value'
        //[HttpPost("~/api/bugs/backlog/{id}")]
        [HttpPost]
        public Bug BackLog(int id)
        {
            var bug = _bugsRepository.GetBugs().First(b => b.id == id);
            bug.state = "backlog";

            _hub.Clients.All.moved(bug);

            return bug;
        }

        //[HttpPost("~/api/bugs/working/{id}")]
        [HttpPost]
        public Bug Working(int id)
        {
            var bug = _bugsRepository.GetBugs().First(b => b.id == id);
            bug.state = "working";

            _hub.Clients.All.moved(bug);

            return bug;
        }

        [HttpPost]
        public Bug Done(int id)
        {
            var bug = _bugsRepository.GetBugs().First(b => b.id == id);
            bug.state = "done";

            _hub.Clients.All.moved(bug);

            return bug;
        }
    }
}