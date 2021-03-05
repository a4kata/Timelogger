using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Timelogger.Api.Module;
using Timelogger.Entities;

namespace Timelogger.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProjectsController : Controller
	{
		private readonly ApiContext _context;

		public ProjectsController(ApiContext context)
		{
			_context = context;
		}

        #region GETs
		[HttpGet]
		[Route("GetProjectById")]
		public IActionResult GetProjectById([Required] int projectId) => Ok(_context.GetProjectByID(projectId));

		[HttpGet]
		[Route("GetProjects_OrderByDeadline")]
		public IActionResult GetProjects_OrderByDeadline() => Ok(_context.GetProjectOrderedByDeadline());

		[HttpGet]
		[Route("GetAllProjects")]
		public IActionResult GetAllProjects() => Ok(_context.Projects);
		#endregion

		#region PUTs
		//[HttpPut]
		//[Route("CreateProject")]
		//public string Create(Project pro) => "Hello Back!";
		#endregion

		#region POSTs
		[HttpPost]
		[Route("RegisterTime")]
		public string RegisterTime(RegisterTimeRequest rtr) => _context.RegisterTime(rtr.ProjectID, rtr.Minutes, rtr.Notes);

		[HttpPost]
		[Route("CompleteProject")]
		public string CompleteProject([Required] int projectId) => _context.CompleteProject(projectId);
		#endregion
	}
}
