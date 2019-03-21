﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Humanizer;
using LexiconLMS.Data;
using LexiconLMS.Models;
using LexiconLMS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LexiconLMS.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class ActivityController : Controller
    {

        private readonly LexiconLMSContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public ActivityController(LexiconLMSContext context, IMapper mapper, UserManager<User> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
           
        }


        public async Task<IActionResult> Index()
        {
            var model = await _context.Activities.Include(a=>a.ActivityType).Include(a=>a.Module).Include(a => a.Module.Course).ToListAsync();
            return View(model);
        }

        // GET: Activity/Create
        public async Task<IActionResult> Create(int id)
        {

            if (!ModelState.IsValid)
            {
                return NotFound();
            }
            var module = await _context.Modules.Include(m=>m.Course).FirstOrDefaultAsync(m => m.Id == id);

            if (module is null)
            {
                return NotFound();
            }

            var model = new ActivityAddViewModel();

            model.ModuleId = module.Id;
            model.ModuleName =module.Name;
            model.Module = module;
            model.Course = module.Course;

            model.ParentStartDate = module.StartDate;
            model.ParentEndDate = module.EndDate;
            model.ModuleName = module.Name;

            var startTimeActivity = module.StartDate;
            model.StartDate = startTimeActivity;
           
            model.EndDate = startTimeActivity.AddDays(7);

            ViewData["ActivityTypeId"] = new SelectList(_context.Set<ActivityType>(), "Id", "Type");

            return View(model);
        }

        // POST: Activity/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Description, StartDate, EndDate, ModuleId, ModuleName, ActivityTypeId, ParentStartDate, ParentEndDate")] ActivityAddViewModel viewModel)
        {

            if (ModelState.IsValid)
            {
      
                Activityy activity = _mapper.Map<Activityy>(viewModel);
                activity.ActivityType = await _context.ActivityType.FirstOrDefaultAsync(at => at.Id == viewModel.ActivityTypeId);
                activity.Module = await _context.Modules.FirstOrDefaultAsync(m => m.Id == viewModel.ModuleId);
                await _context.Activities.AddAsync(activity);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                TempData["AlertMsg"] = "Activity added";
                return RedirectToAction("Details", "module", new { id = activity.Module.Id });
            }


            var module = await _context.Modules.Include(m => m.Course).FirstOrDefaultAsync(m => m.Id == viewModel.ModuleId);

            if (module is null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FirstOrDefaultAsync(a => a.Id == module.CourseId);
            if (course == null)
            {
                return NotFound();
            }

            viewModel.Course = course;
            viewModel.Module = module;
            viewModel.ModuleId = module.Id;
            viewModel.ModuleName = module.Name;

            ViewData["ActivityTypeId"] = new SelectList(_context.Set<ActivityType>(), "Id", "Type");
            return View(viewModel);

        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _context.Activities
                .Include(v => v.Module)
                .Include(v => v.ActivityType)
                .FirstOrDefaultAsync(m => m.Id== id);
            if (activity == null || activity.Module is null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FirstOrDefaultAsync(a => a.Id == activity.Module.CourseId);
            if (course == null)
            {
                return NotFound();
            }

            var model = new ActivityDetailsViewModel()
            {
                Id = activity.Id,
                ModuleName = activity.Module.Name,
                ModuleId = activity.ModuleId,
                Description = activity.Description,
                StartDate = activity.StartDate,
                EndDate = activity.EndDate,
                ActivityType = activity.ActivityType,
                Course = course,
                Module = activity.Module
            };

            var teachers = _userManager.GetUsersInRoleAsync("Teacher");
            teachers.Wait();
            var temp = teachers.Result;

            model.Documents = new List<DocumentListViewModel>();
            var documents = _context.ActivityDocument
                .Where(d => d.ActivityId == id)
                .Where(d => temp.Contains(d.User))
                .ToList();

            foreach (var doc in documents)
            {
                var newDoc = _mapper.Map<DocumentListViewModel>(doc);
                newDoc.Filezise = (doc.DocumentData.Length).Bytes().Humanize("#.#");
                model.Documents.Add(newDoc);
            }

            return View(model);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _context.Activities
                .Include(a => a.Module)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (activity == null || activity.Module is null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FirstOrDefaultAsync(a => a.Id == activity.Module.CourseId);
            if (course == null)
            {
                return NotFound();
            }

            var viewModel = _mapper.Map<ActivityAddViewModel>(activity);
            viewModel.Course = course;
            viewModel.ParentStartDate = activity.Module.StartDate;
            viewModel.ParentEndDate = activity.Module.EndDate;

            ViewData["ActivityTypeId"] = new SelectList(_context.Set<ActivityType>(), "Id", "Type", activity.ActivityTypeId);
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Description, StartDate, EndDate, ModuleId, ModuleName, ActivityTypeId, ParentStartDate, ParentEndDate")] ActivityAddViewModel AVM)
        {

            if (ModelState.IsValid)
            {
                var ActivityEntity = await _context.Activities.Include(a =>a.ActivityType).FirstOrDefaultAsync(a => a.Id == AVM.Id);

                ActivityEntity.Description = AVM.Description;
                ActivityEntity.StartDate = AVM.StartDate;
                ActivityEntity.EndDate = AVM.EndDate;
                //ActivityEntity.ModuleId = AVM.ModuleId;
                ActivityEntity.ActivityTypeId = AVM.ActivityTypeId;
                _context.Update(ActivityEntity);
                await _context.SaveChangesAsync();

                TempData["AlertMsg"] = "Saved changes";
                return RedirectToAction(nameof(Details), new { id = id });
            }

            var activity = await _context.Activities
                .Include(a => a.Module)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (activity == null || activity.Module is null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FirstOrDefaultAsync(a => a.Id == activity.Module.CourseId);
            if (course == null)
            {
                return NotFound();
            }

            AVM.Course = course;
            AVM.Module = activity.Module;
            AVM.ModuleId = activity.Module.Id;
            AVM.ModuleName = activity.Module.Name;

            ViewData["ActivityTypeId"] = new SelectList(_context.Set<ActivityType>(), "Id", "Type", AVM.ActivityTypeId);
            return View(AVM);
        }


        public async Task<IActionResult> Delete(int id)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }

            var activity = await _context.Activities.FirstOrDefaultAsync(a => a.Id == id);
            if (!(activity is null))
            {
                _context.Remove(activity);
                _context.SaveChanges();

                var module = await _context.Modules.FirstOrDefaultAsync(a => a.Id == activity.ModuleId);
                if (!(module is null))
                {
                    TempData["AlertMsg"] = "Activity deleted";
                    return RedirectToAction("Details", "module", new { id = module.Id });
                }
            }

            return NotFound();
        }
    }
}
