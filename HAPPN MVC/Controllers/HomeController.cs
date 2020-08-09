﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HAPPAN_MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using HAPPAN_MVC.Data;
using HAPPAN_MVC.Models.VMModels;

namespace HAPPAN_MVC.Controllers
{
   [Authorize]
    public class HomeController : Controller
    {
        private HAPPANDBContext db;

        

        public HomeController(HAPPANDBContext hAPPAN_MVC_AuthDBContext)
        {
            this.db = hAPPAN_MVC_AuthDBContext;
        }


        // GET: Project
        public IActionResult Index()
        {
            var projectRepo = db.Projects.ToList();
            //var result = projectRepo.Select(s => new VMProjects
            //{
            //    ProjectId = s.ProjectId,
            //    ProjectName = s.ProjectName,
            //    Status = s.Status,
            //    ProjectProgress = (db.Tasks.Where(w=>w.ProjectId == s.ProjectId).Count() * 100) - (db.Tasks.Where(w => w.ProjectId == s.ProjectId).Select(l => l.PercentageOfProject).Sum()),
            //    Task = db.Tasks.Where(w => w.ProjectId == s.ProjectId).Select(ss => new VMTask
            //    {
            //        TaskId = ss.TaskId,
            //        TaskName = ss.TaskName,
            //        Progress = ss.PercentageOfProject
            //    }).ToList()
            //}).ToList();


            var taskRepo = db.Tasks.ToList();
            var result = new List<VMProjects>();
            foreach (var s in projectRepo)
            {
                var d = new VMProjects();
                d.ProjectId = s.ProjectId;
                d.ProjectName = s.ProjectName;
                d.Status = s.Status;
                d.ProjectProgress = ((taskRepo.Where(w => w.ProjectId == s.ProjectId).Select(l => l.PercentageOfProject).Sum() * 100) / (taskRepo.Where(w => w.ProjectId == s.ProjectId).Count() * 100));
                d.Task = taskRepo.Where(w => w.ProjectId == s.ProjectId).Select(ss => new VMTask
                {
                    TaskId = ss.TaskId,
                    TaskName = ss.TaskName,
                    Progress = ss.PercentageOfProject
                }).ToList();
                result.Add(d);
            }

            return View(result);
        }

        // GET: Project/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var project = await db.Projects.FindAsync(id);
            db.Projects.Remove(project);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
