﻿using LexiconLMS.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models
{
    public class EndDateNotLaterThanParentEndDate : ValidationAttribute, IClientModelValidator
    {
        public EndDateNotLaterThanParentEndDate()
        {
        }

        protected override ValidationResult IsValid(
            object value, ValidationContext validationContext)
        {
            if(validationContext.ObjectInstance is IDateInterval child && validationContext.ObjectInstance is IParentDateInterval parent)
            {
                if (parent.ParentEndDate.CompareTo(child.EndDate) < 0)
                {
                    return new ValidationResult(string.Format(ErrorMessage, child.EndDate.ToString(Common.DateFormat), parent.ParentEndDate.ToString(Common.DateFormat)));
                }                
            }
            return ValidationResult.Success;
        }

        void IClientModelValidator.AddValidation(ClientModelValidationContext context)
        {
        }
    }
}

