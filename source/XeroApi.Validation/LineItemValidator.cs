﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XeroApi.Model;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using XeroApi.Validation.Helpers;

namespace XeroApi.Validation
{
    public class LineItemValidator : Validator<LineItem>
    {
        public LineItemValidator()
            : base(null, null)
        { }

        protected override void DoValidate(LineItem objectToValidate, object currentTarget, string key, ValidationResults validationResults)
        {
            if (objectToValidate.AccountCode.IsNullOrWhiteSpace())
            {
                validationResults.AddResult(new ValidationResult("No AccountCode Specified", currentTarget, key, "AccountCode", this));
            }

            if (objectToValidate.Description.IsNullOrWhiteSpace())
            {
                validationResults.AddResult(new ValidationResult("No Description Specified", currentTarget, key, "Description", this));
            }

            if (objectToValidate.LineAmount.HasValue)
            {
                if (objectToValidate.LineAmount == 0)
                {
                    validationResults.AddResult(new ValidationResult("LineAmount must be not equal to 0", currentTarget, key, "LineAmount", this));
                }
            }

            if (objectToValidate.UnitAmount.HasValue)
            {
                if (objectToValidate.UnitAmount == 0)
                {
                    validationResults.AddResult(new ValidationResult("UnitAmount must be not equal to 0", currentTarget, key, "UnitAmount", this));
                }

                if (objectToValidate.Quantity.HasValue)
                {
                    if (objectToValidate.Quantity.Value <= 0)
                    {
                        validationResults.AddResult(new ValidationResult("Quantity must be greater than 0", currentTarget, key, "Quantity", this));
                    }
                }

                if (!objectToValidate.Quantity.HasValue)
                {
                    validationResults.AddResult(new ValidationResult("Quantity must be specified if UnitAmount is specified", currentTarget, key, "Quantity", this));
                }
                else if (objectToValidate.LineAmount.HasValue)
                {
                    if (!(objectToValidate.UnitAmount.Value * objectToValidate.Quantity.Value).NearlyEqualTo(objectToValidate.LineAmount.Value))
                    {
                        validationResults.AddResult(new ValidationResult("LineAmount must be equal to Quantity * UnitAmount", currentTarget, key, "LineAmount", this));
                    }
                }
            }

            if (objectToValidate.Quantity.HasValue)
            {
                if (!objectToValidate.UnitAmount.HasValue)
                {
                    validationResults.AddResult(new ValidationResult("UnitAmount must be specified if Quantity is specified", currentTarget, key, "UnitAmount", this));
                }
            }

            if (objectToValidate.TaxAmount.HasValue)
            {
                if (Math.Sign(objectToValidate.TaxAmount.Value * objectToValidate.GetSubTotal()) == -1)
                {
                    validationResults.AddResult(new ValidationResult("TaxAmount must be same sign as LineAmount", currentTarget, key, "TaxAmount", this));
                }
                else
                {
                    if (Math.Abs(objectToValidate.TaxAmount.GetValueOrDefault()) > Math.Abs(objectToValidate.GetSubTotal()))
                    {
                        validationResults.AddResult(new ValidationResult("TaxAmount cannot be greater than the LineAmount", currentTarget, key, "TaxAmount", this));
                    }
                }
            }
        }

        protected override string DefaultMessageTemplate
        {
            get { return "The LineItem is invalid"; }
        }
    }
}
