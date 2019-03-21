﻿using LexiconLMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.ViewModels
{
    public class CourseDetailsViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Course name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Course starts")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Course ends")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Teacher")]
        public string TeacherEmail { get; set; }

        [Display(Name = "Students")]
        public IEnumerable<User> Students { get; set; }

        public List<ModuleAddViewModel> Modules { get; set; }

        public List<DocumentListViewModel> Documents { get; set; }
    }
}
